using Godot;
using System;

public class Score : Node2D
{
	// Declare member variables here.
	int ScoreRoll = 0;
	AnimatedSprite[] Digits = new AnimatedSprite[6];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Digits[0] = GetNode<AnimatedSprite>("Col1");
		Digits[1] = GetNode<AnimatedSprite>("Col2");
		Digits[2] = GetNode<AnimatedSprite>("Col3");
		Digits[3] = GetNode<AnimatedSprite>("Col4");
		Digits[4] = GetNode<AnimatedSprite>("Col5");
		Digits[5] = GetNode<AnimatedSprite>("Col6");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		bool updated = false;
		if (ScoreRoll < Globals.Score)
		{
			ScoreRoll += Globals.RollSpeed;
			updated = true;
		}
		if (ScoreRoll > Globals.Score)
		{
			ScoreRoll = Globals.Score;
			updated = true;
		}
		if (updated)
		{
			string scoreStr = Globals.Score.ToString();
			for (int i = 0; i < 6; i++)
			{
				Digits[i].Animation = scoreStr.Substring(i, 1);
			}
		}
	}
}
