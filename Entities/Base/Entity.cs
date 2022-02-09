using Godot;
using System;

public class Entity : Node2D
{
	// Declare member variables here.
	protected Area2D Area;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area = GetNode<Area2D>("Area2D");
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{

	}

	protected Vector2 GetTilePos()
	{
		return new Vector2((int)Position.x / 24, (int)Position.y / 24);
	}

	protected bool IsGridAligned()
	{
		return Position.x % 24 == 0 && Position.y % 24 == 0;
	}
}
