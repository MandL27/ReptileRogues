using Godot;
using System;

public class SCText : AnimatedSprite
{
	// Declare member variables here.
	Vector2 FinalPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FinalPos = Position;
		Position = new Vector2(-161, Position.y);
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		if (Position.x < FinalPos.x)
			Translate(Vector2.Right * 3);
	}
}
