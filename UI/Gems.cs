using Godot;
using System;

public class Gems : Node2D
{
	// Declare member variables here.
	AnimatedSprite[] Digits = new AnimatedSprite[2];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Digits[0] = GetNode<AnimatedSprite>("Gm1");
		Digits[1] = GetNode<AnimatedSprite>("Gm2");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		string gemStr = Globals.Gems.ToString().PadLeft(2, '0');
		for (int i = 0; i < 2; i++)
		{
				Digits[i].Animation = gemStr.Substring(i, 1);
		}
	}
}
