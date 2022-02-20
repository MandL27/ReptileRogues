using Godot;
using System;

public class Player : Entity
{
	// Declare member variables here.
	AnimatedSprite PlayerSprite;
	Vector2 InitialPos = Vector2.Zero;
	Vector2 Direction = Vector2.Zero;
	Vector2 Heading = Vector2.Up;
	Vector2[] Dirs = new Vector2[] { Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right };
	Action BufferAction = Action.None;
	bool BufferReady = true;
	Action Action = Action.None;
	int Frames = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		PlayerSprite = GetNode<AnimatedSprite>("PlayerSprite");
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		BufferAction = ParseInputDelta();

		if (Frames == 0)
		{
			BufferAction = ParseInput();
			Action = Action.None;
			switch (BufferAction)
			{
				case Action.StepUp:
				case Action.StepLeft:
				case Action.StepDown:
				case Action.StepRight:
					InitialPos = Position;
					Direction = Dirs[(int)BufferAction - 1];
					Heading = Direction;
					if (!IsAdjacentTileSolid(Direction))
					{
						Action = BufferAction;
						Frames = 10;
					}
					break;
				case Action.LongJump:
					InitialPos = Position;
					if (!IsAdjacentTileSolid(Heading * 2))
					{
						Direction = Heading;
						Action = Action.LongJump;
						Frames = 32;
					}
					else if (!IsAdjacentTileSolid(Heading))
					{
						Direction = Heading / 2;
						Action = Action.LongJump;
						Frames = 32;
					}
					break;
				case Action.TurnLeft:
				case Action.TurnRight:
					Direction = Heading;
					Action = BufferAction;
					Frames = 6;
					break;
			}
			BufferAction = Action.None;
		}

		Frames--;
		switch (Action)
		{
			case Action.None:
				Frames = 0;
				break;
			case Action.StepUp:
			case Action.StepLeft:
			case Action.StepDown:
			case Action.StepRight:
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
		switch (((int)Mathf.Rad2Deg(Heading.Angle()) + 360) % 360)
		{
			case 0:
				PlayerSprite.Animation = "RightIdle";
				break;
			case 90:
				PlayerSprite.Animation = "DownIdle";
				break;
			case 180:
				PlayerSprite.Animation = "LeftIdle";
				break;
			case 270:
				PlayerSprite.Animation = "UpIdle";
				break;
		}
	}

	private Action ParseInput()
	{
		if (Input.IsActionPressed("jump"))
			return Action.LongJump;
		if (Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepUp;
		if (!Input.IsActionPressed("ui_up") && Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepLeft;
		if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepDown;
		if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && Input.IsActionPressed("ui_right"))
			return Action.StepRight;
		return BufferAction;
	}

	private Action ParseInputDelta()
	{
		if (Input.IsActionJustPressed("jump"))
			return Action.LongJump;
		if (Input.IsActionJustPressed("turn_left") && !Input.IsActionJustPressed("turn_right"))
			return Action.TurnLeft;
		if (!Input.IsActionJustPressed("turn_left") && Input.IsActionJustPressed("turn_right"))
			return Action.TurnRight;
		if (Input.IsActionJustPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepUp;
		if (!Input.IsActionPressed("ui_up") && Input.IsActionJustPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepLeft;
		if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && Input.IsActionJustPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			return Action.StepDown;
		if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && Input.IsActionJustPressed("ui_right"))
			return Action.StepRight;
		return BufferAction;
	}

	private bool IsAdjacentTileSolid(Vector2 direction)
	{
		return GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition, GlobalPosition + (direction * 72), null, 1, false, true).Count > 0;
	}

	private void OnBodyEntered(object area)
	{
		Area2D a = (Area2D)area;
		switch (a.CollisionLayer)
		{
			case 8: // enemy
					// TODO: kill player
				GD.Print("Enemy touched");
				break;
			case 16: // enemy line of sight
				((Enemy)(a.GetParent().GetParent())).ChaseTarget = this;
				GD.Print($"Spotted by {a.GetParent().GetParent().Name}");
				break;
			case 32: // pickup
				a.GetParent().QueueFree();
				if (a.GetParent().Name.Contains("Gem"))
				{
					Globals.Score += 100;
					Globals.Gems++;
				}
				break;
		}
	}
}

enum Action
{
	None,
	StepUp,
	StepLeft,
	StepDown,
	StepRight,
	LongJump,
	TurnLeft,
	TurnRight,
	CoilTongue,
	CoilTether,
	CoilInvis,
	WatcherPush,
	WatcherBite
}


