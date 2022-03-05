using Godot;
using System;

public class Lives : Node2D
{
	// Declare member variables here.
	AnimatedSprite[] Digits = new AnimatedSprite[2];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Digits[0] = GetNode<AnimatedSprite>("Life1");
		Digits[1] = GetNode<AnimatedSprite>("Life2");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		string lifeStr = Globals.Lives.ToString().PadLeft(2, '0');
		if (Globals.Lives < 0)
			lifeStr = "00";
		for (int i = 0; i < 2; i++)
		{
				Digits[i].Animation = lifeStr.Substring(i, 1);
		}
	}
}
