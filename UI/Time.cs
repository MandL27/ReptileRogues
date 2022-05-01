using Godot;
using System;

public class Time : Node2D
{
	// Declare member variables here.
	AnimatedSprite[] Digits = new AnimatedSprite[7];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Digits[0] = GetNode<AnimatedSprite>("Min1");
		Digits[1] = GetNode<AnimatedSprite>("Colon");
		Digits[2] = GetNode<AnimatedSprite>("Sec1");
		Digits[3] = GetNode<AnimatedSprite>("Sec2");
		Digits[4] = GetNode<AnimatedSprite>("Dot");
		Digits[5] = GetNode<AnimatedSprite>("Ms1");
		Digits[6] = GetNode<AnimatedSprite>("Ms2");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		if (Globals.TimerActive)
		{
			Globals.Timer++;
			TimeSpan span = new TimeSpan(0, 0, 0, 0, Globals.Timer * 50 / 3);
			string timeStr = span.ToString(@"m\:ss\.ff");
			if (span.TotalMinutes >= 10)
			{
				timeStr = "9:59.99";
			}
			for (int i = 0; i < 7; i++)
			{
				Digits[i].Animation = timeStr.Substring(i, 1);
			}
		}
		else
		{
			Globals.TimerPauseFrames--;
			if (Globals.TimerPauseFrames < 0)
			{
				Globals.TimerActive = true;
			}
		}
	}
}
