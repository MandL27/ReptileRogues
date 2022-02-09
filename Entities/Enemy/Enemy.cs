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
	Rect2 ChaseRange;
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
		ChaseRange = new Rect2(left, top, right - left, bottom - top);
		// If the path forms a complete loop, the starting cell
		// is written twice. This makes the below check for
		// open-circuit paths work.
		if (PathCells[0] != PathCells[PathCells.Count - 1])
		{
			GD.PushWarning($"{Name}: Enemy path is not a closed circuit!");
		}
		SightPivot.Rotation = (PathCells[NextCell] - PathCells[CurrCell]).Angle();
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		SpeedRem += Speed;
		while (SpeedRem >= 1)
		{
			SpeedRem--;
			if (GetTilePos() == PathCells[NextCell] && IsGridAligned())
			{
				CurrCell++;
				if (CurrCell == PathCells.Count) CurrCell = 0;
				NextCell++;
				if (NextCell == PathCells.Count) NextCell = 0;
				SightPivot.Rotation = (PathCells[NextCell] - PathCells[CurrCell]).Angle();
			}
			Position += PathCells[NextCell] - PathCells[CurrCell];
		}
		// TODO: chase deviating from beat
		// TODO: homing
	}
}

enum EnemyType
{
	Normal,
	Chase,
	Homing
}
