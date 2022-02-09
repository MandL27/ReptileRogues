using Godot;
using System;

public class Player : Entity
{
	// Declare member variables here.
	Vector2 InitialPos = Vector2.Zero;
	Vector2 Direction = Vector2.Zero;
	Vector2 Heading = Vector2.Up;
	Vector2 Buffer = Vector2.Zero;
	bool BufferReady = true;
	Action Action = Action.None;
	int Frames = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if (Frames == 0) BufferReady = true;
		Vector2 buff = Vector2.Zero;
		buff.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		buff.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
		if (buff != Vector2.Zero && BufferReady)
		{
			Buffer = buff;
		}
		else if (!BufferReady && buff == Vector2.Zero)
		{
			Buffer = Vector2.Zero;
			BufferReady = true;
		}
		if (Frames == 0)
		{
			Action = Action.None;
			bool axial = false;
			Direction.x = Buffer.x;
			Direction.y = Buffer.y;
			if (Mathf.Abs(Direction.x) >= Mathf.Abs(Direction.y) * 2 && Mathf.Abs(Direction.x) > 0)
			{
				Direction.y = 0;
				axial = true;
			}
			if (Mathf.Abs(Direction.y) >= Mathf.Abs(Direction.x) * 2 && Mathf.Abs(Direction.y) > 0)
			{
				Direction.x = 0;
				axial = true;
			}
			if (axial)
			{
				BufferReady = false;
				Buffer = Vector2.Zero;
				InitialPos = Position;
				// TODO: collision
				Direction = Direction.Normalized();
				Heading = Direction.Normalized();
				Action = Action.Step;
				Frames = 10;
			}
			else if (Input.IsActionPressed("jump"))
			{
				InitialPos = Position;
				// TODO: collision
				Direction = Heading;
				Action = Action.LongJump;
				Frames = 32;
			}
			else if (Input.GetActionStrength("turn_left") > Input.GetActionStrength("turn_right"))
			{
				Direction = Heading;
				Action = Action.TurnLeft;
				Frames = 6;
			}
			else if (Input.GetActionStrength("turn_left") < Input.GetActionStrength("turn_right"))
			{
				Direction = Heading;
				Action = Action.TurnRight;
				Frames = 6;
			}
			// TODO: abilities
		}
		Frames--;
		switch (Action)
		{
			case Action.None:
				Frames = 0;
				break;
			case Action.Step:
				Position = InitialPos.LinearInterpolate(InitialPos + Direction * 24, (10 - Frames) / 10f);
				break;
			case Action.LongJump:
				Position = InitialPos.LinearInterpolate(InitialPos + Direction * 48, (32 - Frames) / 32f);
				break;
			case Action.TurnLeft:
				if (Frames == 0)
				{
					Heading.x = Direction.y;
					Heading.y = -Direction.x;
				}
				break;
			case Action.TurnRight:
				if (Frames == 0)
				{
					Heading.x = -Direction.y;
					Heading.y = Direction.x;
				}
				break;
		}
	}
}

enum Action
{
	None,
	Step,
	LongJump,
	TurnLeft,
	TurnRight,
	CoilTongue,
	CoilTether,
	CoilInvis,
	WatcherPush,
	WatcherBite
}
