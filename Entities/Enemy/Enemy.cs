using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
	LinkedList<Vector2> ReturnQueue = new LinkedList<Vector2>();
	Vector2 TopLeft;
	Vector2 BottomRight;
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
		TopLeft = new Vector2(left - 3, top - 3);
		BottomRight = new Vector2(right + 3, bottom + 3);
		// If the path forms a complete loop, the starting cell
		// is written at the front and back. This makes the below
		// check for open-circuit paths work.
		if (PathCells[0] != PathCells[PathCells.Count - 1])
		{
			GD.PushWarning($"{Name}: Enemy path is not a closed circuit!");
		}
		Heading = PathCells[NextCell] - PathCells[CurrCell];
		SightPivot.Rotation = Heading.Angle();
		GD.Print($"{Name}: ({TopLeft}, {BottomRight})");
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
					LinkedList<Vector2> vecs = FindAStarPath(((Player)ChaseTarget).GetTilePos());
					Heading = vecs.ElementAt(1) - GetTilePos();
					if (!IsInRange(GetTilePos() + Heading))
					{
						ChaseTarget = null;
						Heading *= -1;
						ReturnQueue = FindAStarPath(PathCells[CurrCell]);
						Heading = ReturnQueue.ElementAt(1) - GetTilePos();
					}
				}
				else if (GetTilePos() == PathCells[NextCell])
				{
					CurrCell++;
					if (CurrCell == PathCells.Count) CurrCell = 0;
					NextCell++;
					if (NextCell == PathCells.Count) NextCell = 0;
					Heading = PathCells[NextCell] - PathCells[CurrCell];
				}
				else if (ReturnQueue.Count > 1)
				{
					ReturnQueue.RemoveFirst();
					ReturnQueue = FindAStarPath(PathCells[CurrCell]);
					Heading = ReturnQueue.ElementAt(1) - GetTilePos();
				}
				SightPivot.Rotation = Heading.Angle();
			}
			Position += Heading;
		}
		// TODO: homing
	}

	bool IsInRange(Vector2 point)
	{
		return point.x >= TopLeft.x && point.y >= TopLeft.y && point.x <= BottomRight.x && point.y <= BottomRight.y;
	}

	LinkedList<Vector2> FindAStarPath(Vector2 target)
	{
		Dictionary<Vector2, LinkedList<Vector2>> visited = new Dictionary<Vector2, LinkedList<Vector2>>();
		PriorityQueue<Vector2> queue = new PriorityQueue<Vector2>();
		queue.Push(GetTilePos(), 0);
		visited[GetTilePos()] = new LinkedList<Vector2>();
		while (!queue.IsEmpty())
		{
			Vector2 node = queue.Pop();
			foreach (Vector2 dir in new Vector2[] { Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right })
			{
				// TODO: occlude on walls and water(?)
				Vector2 next = node + dir;
				LinkedList<Vector2> list = new LinkedList<Vector2>(visited[node]);
				list.AddLast(node);
				if (next == target)
				{
					list.AddLast(target);
					return list;
				}
				int priority = 3 + (int)Mathf.Abs(target.x - next.x) + (int)Mathf.Abs(target.y - next.y);
				// discourage turning to reduce enemy diagonals
				Vector2 head;
				if (visited[node].Count == 0)
				{
					head = Heading;
				}
				else
				{
					head = node - visited[node].Last.Value;
				}
				if (dir == Heading * -1)
				{
					priority += 4;
				}
				else if (dir != Heading)
				{
					priority += 1;
				}
				queue.Push(next, priority);
				visited[next] = list;
			}
		}
		return null;
	}
}

enum EnemyType
{
	Normal,
	Chase,
	Homing
}
