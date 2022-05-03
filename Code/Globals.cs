using Godot;
using System;

public class Globals : Node2D
{
	public static int CurrentLevel = 0;
	public static string[] LevelNames = { "Intro.tscn", "Earth.tscn", "Water.tscn", "Air.tscn", "Fire.tscn" };
	public static int[] ParTimes = { 60 * 60, 60 * 60, 120 * 60, 90 * 60, 180 * 60 };
	public static int Score = 0;
	public static int Lives = 4;
	public static int LivesGiven = 0;
	public static int Gems = 0;
	public static int GemScale
	{
		get => Gems <  7 ? 100 :
			   Gems < 14 ? 200 :
			   Gems < 21 ? 300 :
			   Gems < 24 ? 400 :
						   600;
	}
	public static int RollSpeed = 10;
	public static int Timer = 0;
	public static bool TimerActive = false;
	public static int TimerPauseFrames = 120;
}
