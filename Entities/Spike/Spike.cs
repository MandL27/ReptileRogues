using Godot;
using System;

public class Spike : Entity
{
	// Declare member variables here.
	[Export] int DownFrames = 90;
	[Export] int UpFrames = 30;
	int StateFrames = 0;
	bool IsUp = false;
	AnimatedSprite SpikeSprite;
	CollisionShape2D Shape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		StateFrames = DownFrames;
		SpikeSprite = GetNode<AnimatedSprite>("SpikeSprite");
		SpikeSprite.Animation = "OffStart";
		SpikeSprite.Playing = true;
		Shape = Area.GetNode<CollisionShape2D>("Collider");
			Shape.Disabled = true;
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		StateFrames--;
		if (!IsUp && DownFrames - StateFrames == 8)
		{
			SpikeSprite.Animation = "Off";
			GD.Print("Down finish");
		}
		else if (!IsUp && StateFrames == 30)
		{
			SpikeSprite.Animation = "OnStart";
			GD.Print("Up start");
		}
		else if (!IsUp && StateFrames == 0)
		{
			SpikeSprite.Animation = "On";
			StateFrames = UpFrames;
			Shape.Disabled = false;
			IsUp = true;
			GD.Print("Up finish");
		}
		else if (IsUp && StateFrames == 0)
		{
			SpikeSprite.Animation = "OffStart";
			StateFrames = DownFrames;
			Shape.Disabled = true;
			IsUp = false;
			GD.Print("Down start");
		}
	}
}
