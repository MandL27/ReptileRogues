using Godot;
using System;

public class Checkpoint : Entity
{
	// Declare member variables here.
	AnimatedSprite FlagSprite;
	int RaiseFrames = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		FlagSprite = GetNode<AnimatedSprite>("Flag");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		if (RaiseFrames > 0)
		{
			RaiseFrames--;
		}
		if (RaiseFrames == 0)
		{
			FlagSprite.Animation = "On";
		}
	}

	public void Activate()
	{
		Area.GetNode<CollisionShape2D>("Collider").SetDeferred("disabled", true);
		FlagSprite.Animation = "OnStart";
		FlagSprite.Playing = true;
		RaiseFrames = 18;
	}
}
