using Godot;
using Godot.Collections;
using System;

public class Switch : Entity
{
	// Declare member variables here.
	[Export] Node2D DoorNode;
	AnimatedSprite SwitchSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		SwitchSprite = GetNode<AnimatedSprite>("SwitchSprite");
	}

	// Called every tick. 'delta' is the elapsed time since the previous tick.
	public override void _PhysicsProcess(float delta)
	{
		
	}

	public void Activate()
	{
		Area.QueueFree();
		SwitchSprite.Playing = true;
		//DoorNode.QueueFree();
		GetNode<Node2D>("Door").QueueFree();
	}
}
