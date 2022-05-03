using Godot;
using System;

public class Title : Node2D
{
	// Declare member variables here.
	ColorRect Rect;
	bool Fading = false;
	int FadeFrames = 0;
	uint[] FadeColors = new uint[] { 0x000000FF, 0x080808FF, 0x101010FF, 0x181818FF, 0x202020FF, 0x282828FF, 0x303030FF, 0x383838FF, 0x404040FF, 0x484848FF, 0x505050FF, 0x585858FF, 0x606060FF, 0x686868FF, 0x707070FF, 0x787878FF, 0x808080FF, 0x888888FF, 0x909090FF, 0x989898FF, 0xA0A0A0FF, 0xA8A8A8FF, 0xB0B0B0FF, 0xB8B8B8FF, 0xC0C0C0FF, 0xC8C8C8FF, 0xD0D0D0FF, 0xD8D8D8FF, 0xE0E0E0FF, 0xE8E8E8FF, 0xF0F0F0FF, 0xF8F8F8FF };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Rect = GetNode<ColorRect>("ColorRect");
		Globals.Score = 0;
		Globals.Timer = 0;
		Globals.Lives = 4;
		Globals.LivesGiven = 0;
		Globals.CurrentLevel = 0;
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		if (!Fading && Input.IsPhysicalKeyPressed((int)KeyList.Enter))
		{
			Fading = true;
		}
		if (Fading)
		{
			if (FadeFrames == 32)
			{
				GetTree().ChangeScene("Intro.tscn");
			}
			else
			{
				Rect.Color = new Color((int)FadeColors[FadeFrames]);
				FadeFrames++;
			}
		}
	}
}
