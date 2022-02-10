using Godot;
using System;
using System.Collections.Generic;

public class Enemy : Entity
{
	// Declare member variables here.
	[Export] Vector2[] PathVecs = new Vector2[2];
	[Export] float Speed = 1;
	[Export] EnemyType Type = EnemyType.Normal;
	float SpeedRem = 0;
	int CurrCell = 0;
	int NextCell = 1;
	List<Vector2> PathCells;
	public Node2D ChaseTarget = null;
	Rect2 ChaseRange;
	Vector2 Heading = Vector2.Zero;
	Node2D SightPivot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SightPivot = GetNode<Node2D>("Pivot");
		PathCells = new List<Vector2>();
		Vector2 pos = GetTilePos();
		PathCells.Add(pos);
		float top = pos.y, left = pos.x, bottom = pos.y, right = pos.x;
		foreach (Vector2 node in PathVecs)
		{
			for (int i = 0; i < node.Length(); i++)
			{
				pos += node.Normalized();
				PathCells.Add(pos);
				if (top > pos.y) top = pos.y;
				if (left > pos.x) left = pos.x;
				if (bottom < pos.y) bottom = pos.y;
				if (right < pos.x) right = pos.x;
			}
		}
		// No, I don't understand why these numbers make it work either.
		ChaseRange = new Rect2(left - 3, top - 3, right + 7 - left, bottom + 6 - top);
		// If the path forms a complete loop, the starting cell
		// is written twice. This makes the below check for
		// open-circuit paths work.
		if (PathCells[0] != PathCells[PathCells.Count - 1])
		{
			GD.PushWarning($"{Name}: Enemy path is not a closed circuit!");
		}
		Heading = PathCells[NextCell] - PathCells[CurrCell];
		SightPivot.Rotation = Heading.Angle();
		GD.Print($"{Name}: {ChaseRange}");
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		SpeedRem += Speed;
		if (ChaseTarget != null)
		{
			SpeedRem += Speed / 2; // 1.5x speed in chase mode
		}
		while (SpeedRem >= 1)
		{
			SpeedRem--;
			if (IsGridAligned())
			{

				if (ChaseTarget != null)
				{
					// TODO: replace with actual A*
					// (will do when enemies need to pathfind in a not-vacuum)
					Vector2 dir = ChaseTarget.Position - Position;
					if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
						dir.y = 0;
					else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
						dir.x = 0;
					else
						dir = Heading;
					Heading = dir.Normalized();
					if (!ChaseRange.HasPoint(GetTilePos() + Heading))
					{
						ChaseTarget = null;
						Heading *= -1;
					}
				}
				else if (GetTilePos() == PathCells[NextCell])
				{
					CurrCell++;
					if (CurrCell == PathCells.Count) CurrCell = 0;
					NextCell++;
					if (NextCell == PathCells.Count) NextCell = 0;
					Heading = PathCells[NextCell] - PathCells[CurrCell];
					if (!ChaseRange.HasPoint(GetTilePos() + Heading))
					{
						GD.Print($"{Name}: bounding box excludes {GetTilePos() + Heading}!");
					}
				}
				// TODO: proper pathfinding back to the path
				else if (PathCells.Contains(GetTilePos()))
				{
					CurrCell = PathCells.IndexOf(GetTilePos());
					NextCell = CurrCell + 1;
					if (NextCell == PathCells.Count) NextCell = 0;
					Heading = PathCells[NextCell] - PathCells[CurrCell];
				}
				SightPivot.Rotation = Heading.Angle();
			}
			Position += Heading;
		}
		// TODO: homing
	}
}

enum EnemyType
{
	Normal,
	Chase,
	Homing
}
