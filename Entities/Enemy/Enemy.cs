using Godot;
using System;
using System.Collections.Generic;

public class Enemy : Entity
{
	// Declare member variables here.
	[Export] Vector2[] PathNodes = new Vector2[2];
	[Export] float Speed = 1;
	float SpeedRem = 0;
	int CurrCell = 0;
	int NextCell = 1;
	List<Vector2> PathCells;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PathCells = new List<Vector2>();
		Vector2 pos = GetTilePos();
		PathCells.Add(pos);
		foreach (Vector2 node in PathNodes)
		{
			for (int i = 0; i < node.Length(); i++)
			{
				pos += node.Normalized();
				PathCells.Add(pos);
			}
		}
		// If the path forms a complete loop, the starting cell
		// is written twice. This makes the below check for
		// open-circuit paths work.
		if (PathCells[0] != PathCells[PathCells.Count - 1])
		{
			GD.PushWarning($"{Name}: Enemy path is not a closed circuit!");
		}
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
			}
			Position += PathCells[NextCell] - PathCells[CurrCell];
		}
		// TODO: chase
		// TODO: homing
	}
}

enum EnemyType
{
	Normal,
	Chase,
	Homing
}
