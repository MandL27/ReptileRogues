using Godot;
using System;

public class Player : Entity
{
	// Declare member variables here.
	AnimatedSprite PlayerSprite;
	Node2D TonguePivot;
	AnimatedSprite TongueSprite;
	ColorRect FadeRect;
	Vector2 SpawnPos = Vector2.Zero;
	Vector2 InitialPos = Vector2.Zero;
	Vector2 Direction = Vector2.Zero;
	Vector2 Heading = Vector2.Up;
	Vector2[] Dirs = new Vector2[] { Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right };
	Action BufferAction = Action.None;
	bool BufferReady = true;
	Action Action = Action.None;
	int Frames = 0;
	int MaxTongueLength = 3;
	bool Invisible = false;
	int InvisFrames = 0;
	int PauseFrames = 120;
	bool OverWater = false;
	bool OverGoal = false;
	PackedScene SCScene;
	bool FadingIn = false;
	int FadeFrames = 0;
	bool Held = false;
	uint[] FadeColors = new uint[] { 0x000000FF, 0x080808FF, 0x101010FF, 0x181818FF, 0x202020FF, 0x282828FF, 0x303030FF, 0x383838FF, 0x404040FF, 0x484848FF, 0x505050FF, 0x585858FF, 0x606060FF, 0x686868FF, 0x707070FF, 0x787878FF, 0x808080FF, 0x888888FF, 0x909090FF, 0x989898FF, 0xA0A0A0FF, 0xA8A8A8FF, 0xB0B0B0FF, 0xB8B8B8FF, 0xC0C0C0FF, 0xC8C8C8FF, 0xD0D0D0FF, 0xD8D8D8FF, 0xE0E0E0FF, 0xE8E8E8FF, 0xF0F0F0FF, 0xF8F8F8FF };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		PlayerSprite = GetNode<AnimatedSprite>("PlayerSprite");
		TonguePivot = PlayerSprite.GetNode<Node2D>("TonguePivot");
		TongueSprite = TonguePivot.GetNode<AnimatedSprite>("TongueSprite");
		FadeRect = GetNode<Camera2D>("Camera2D").GetNode<Node2D>("HUD").GetNode<ColorRect>("ColorRect");
		SpawnPos = GlobalPosition;
		SCScene = GD.Load<PackedScene>("res://UI/StageComplete.tscn");
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if (FadeFrames > 0)
		{
			FadeFrames--;
			int i = FadeFrames;
			if (!FadingIn) i = 30 - i;
			FadeRect.Color = new Color((int)FadeColors[i]);
			if (PauseFrames > 180 && FadeFrames == 0) System.Environment.Exit(0);
		}
		if (PauseFrames > 0)
		{
			PauseFrames--;
			Action = Action.None;
			BufferAction = Action.None;
			InitialPos = Vector2.Zero;
			if (PauseFrames == 121 && !OverGoal)
			{
				FadeOut();
			}
			if (PauseFrames == 60 && !OverGoal)
			{
				GlobalPosition = SpawnPos;
			}
			if (PauseFrames == 31 && !OverGoal)
			{
				FadeIn();
			}
		}
		else if (PauseFrames == 0 && Globals.TimerActive == false && OverGoal)
		{
			System.Environment.Exit(0);
		}
		else
		{
			if (InvisFrames == 0 && Invisible)
			{
				Invisible = false;
				InvisFrames = 20 * 60;
				PlayerSprite.Modulate = new Godot.Color(1, 1, 1, 1);
			}
			if (InvisFrames > 0)
				InvisFrames--;

			BufferAction = ParseInputDelta();
			if (Frames == 0)
			{
				// reset tongue
				TonguePivot.Visible = false;
				TongueSprite.Playing = false;

				BufferAction = ParseInput();
				Action = Action.None;
				if (OverWater)
				{
					PauseFrames = 180;
					Globals.Lives--;
					if (Globals.Lives < 0)
					{
						Globals.TimerActive = false;
						Globals.TimerPauseFrames = int.MaxValue;
					}
				}
				if (OverGoal)
				{
					PauseFrames = 240;
					Globals.TimerActive = false;
					Globals.TimerPauseFrames = int.MaxValue;
					Node2D node = SCScene.Instance<Node2D>();
					AddChild(node);
				}
				switch (BufferAction)
				{
					case Action.StepUp:
					case Action.StepLeft:
					case Action.StepDown:
					case Action.StepRight:
						InitialPos = Position;
						Direction = Dirs[(int)BufferAction - 1];
						Heading = Direction;
						if (!IsAdjacentTileSolid(Direction) && !(Held && IsAdjacentTileSolid(Direction, 2)))
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
					case Action.CoilTongue:
						TonguePivot.Rotation = Heading.Angle();
						MaxTongueLength = 0;
						for (int i = 1; i <= 3; i++)
						{
							if (IsAdjacentTileSolid(Heading * i))
								break;
							MaxTongueLength++;
						}
						TongueSprite.Frame = 0;
						TonguePivot.Visible = true;

						Action = Action.CoilTongue;
						Frames = 30;
						break;
					case Action.CoilInvis:
						if (InvisFrames == 0 && !Invisible)
						{
							Invisible = true;
							InvisFrames = 600;
							PlayerSprite.Modulate = new Godot.Color(1, 1, 1, 0.5f);
						}
						break;
				}
				BufferAction = Action.None;
				Held = false;
			}
			if (Action != Action.CoilTongue)
				MaxTongueLength = 3;

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
				case Action.CoilTongue:
					switch (MaxTongueLength)
					{
						case 0:
							if (Frames >= 27)
							{
								TongueSprite.Frame = 0;
								SetTongueCollision(0);
							}
							else
							{
								Frames = 0;
							}
							break;
						case 1:
							if (Frames >= 27)
							{
								TongueSprite.Frame = 0;
								SetTongueCollision(0);
							}
							else if (Frames >= 24)
							{
								TongueSprite.Frame = 1;
								SetTongueCollision(1);
							}
							else if (Frames >= 21)
							{
								TongueSprite.Frame = 9;
								SetTongueCollision(0);
							}
							else
							{
								Frames = 0;
							}
							break;
						case 2:
							if (Frames >= 27)
							{
								TongueSprite.Frame = 0;
								SetTongueCollision(0);
							}
							else if (Frames >= 24)
							{
								TongueSprite.Frame = 1;
								SetTongueCollision(1);
							}
							else if (Frames >= 21)
							{
								TongueSprite.Frame = 2;
								SetTongueCollision(2);
							}
							else if (Frames >= 18)
							{
								TongueSprite.Frame = 7;
							}
							else if (Frames >= 15)
							{
								TongueSprite.Frame = 8;
								SetTongueCollision(1);
							}
							else if (Frames >= 12)
							{
								TongueSprite.Frame = 9;
								SetTongueCollision(0);
							}
							else
							{
								Frames = 0;
							}
							break;
						case 3:
							if (Frames >= 27)
							{
								TongueSprite.Frame = 0;
								SetTongueCollision(0);
							}
							else if (Frames >= 24)
							{
								TongueSprite.Frame = 1;
								SetTongueCollision(1);
							}
							else if (Frames >= 21)
							{
								TongueSprite.Frame = 2;
								SetTongueCollision(2);
							}
							else if (Frames >= 18)
							{
								TongueSprite.Frame = 3;
								SetTongueCollision(3);
							}
							else if (Frames >= 15)
							{
								TongueSprite.Frame = 4;
							}
							else if (Frames >= 12)
							{
								TongueSprite.Frame = 5;
							}
							else if (Frames >= 9)
							{
								TongueSprite.Frame = 6;
							}
							else if (Frames >= 6)
							{
								TongueSprite.Frame = 7;
								SetTongueCollision(2);
							}
							else if (Frames >= 3)
							{
								TongueSprite.Frame = 8;
								SetTongueCollision(1);
							}
							else if (Frames >= 0)
							{
								TongueSprite.Frame = 9;
								SetTongueCollision(0);
							}
							break;
					}
					break;
				case Action.CoilTether:
					Position = InitialPos.LinearInterpolate(InitialPos + Direction * 24, ((6 * Direction.Length()) - Frames) / (6 * Direction.Length()));
					break;
			}

			string anim = "";
			switch (((int)Mathf.Rad2Deg(Heading.Angle()) + 360) % 360)
			{
				case 0:
					anim = "Right";
					break;
				case 90:
					anim = "Down";
					break;
				case 180:
					anim = "Left";
					break;
				case 270:
					anim = "Up";
					break;
			}
			switch (Action)
			{
				case Action.StepUp:
				case Action.StepDown:
				case Action.StepLeft:
				case Action.StepRight:
					anim += "Step";
					break;
				case Action.LongJump:
					anim += "Jump";
					break;
				default:
					anim += "Idle";
					break;
			}
			PlayerSprite.Animation = anim;
		}
	}

	private void SetTongueCollision(int length)
	{
		for (int i = 1; i <= 3; i++)
		{
			TonguePivot.GetNode<Area2D>($"TongueBody{i}").GetNode<CollisionShape2D>("CollisionShape2D").Disabled = i > length;
		}
	}


	private Action ParseInput()
	{
		Action ret = Action.None;
		if (Input.IsActionPressed("jump"))
			ret = Action.LongJump;
		else if (Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			ret = Action.StepUp;
		else if (!Input.IsActionPressed("ui_up") && Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			ret = Action.StepLeft;
		else if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_right"))
			ret = Action.StepDown;
		else if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_down") && Input.IsActionPressed("ui_right"))
			ret = Action.StepRight;
		else if (Input.IsActionPressed("action1"))
			ret = Action.CoilTongue;
		else if (Input.IsActionPressed("action2"))
			ret = Action.CoilInvis;
		Held = ret != BufferAction;
		if (ret == Action.None) return BufferAction;
		return ret;
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
		if (Input.IsActionJustPressed("action1"))
			return Action.CoilTongue;
		if (Input.IsActionJustPressed("action2"))
			return Action.CoilInvis;
		return BufferAction;
	}

	private void FadeOut()
	{
		FadingIn = false;
		FadeFrames = 31;
		if (Globals.Lives < 0) PauseFrames = int.MaxValue;
	}

	private void FadeIn()
	{
		FadingIn = true;
		FadeFrames = 31;
	}

	private bool IsAdjacentTileSolid(Vector2 direction, uint mask)
	{
		return GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition, GlobalPosition + (direction * 72), null, mask, false, true).Count > 0;
	}

	private bool IsAdjacentTileSolid(Vector2 direction)
	{
		return IsAdjacentTileSolid(direction, 1);
	}

	private void OnBodyEntered(object area)
	{
		Area2D a = (Area2D)area;
		switch (a.CollisionLayer)
		{
			case 2: // water
				OverWater = true;
				break;
			case 8: // enemy
				if (PauseFrames == 0)
				{
					PauseFrames = 180;
					Globals.Lives--;
					if (Globals.Lives < 0)
					{
						Globals.TimerActive = false;
						Globals.TimerPauseFrames = int.MaxValue;
					}
				}
				break;
			case 16: // enemy line of sight
				if (!Invisible)
					((Enemy)(a.GetParent().GetParent())).ChaseTarget = this;
				break;
			case 32: // pickup
				a.GetParent().QueueFree();
				if (a.GetParent().Name.Contains("Gem"))
				{
					Globals.Score += Globals.GemScale;
					Globals.Gems++;
				}
				break;
			case 256: // checkpoint
				SpawnPos = a.GlobalPosition;
				a.GetParent<Checkpoint>().Activate();
				break;
			case 512: // goal
				OverGoal = true;
				break;
			case 1024: // switch
				a.GetParent<Switch>().Activate();
				break;
		}
	}

	private void OnBodyExited(object area)
	{
		Area2D a = (Area2D)area;
		switch (a.CollisionLayer)
		{
			case 2: // water
				OverWater = false;
				break;
			case 512: // goal
				OverGoal = false;
				break;
		}
	}

	private async void OnTongueEntered(object area, int length)
	{
		Area2D a = (Area2D)area;
		switch (a.CollisionLayer)
		{
			case 32: // pickup
				a.GetParent().QueueFree();
				if (a.GetParent().Name.Contains("Gem"))
				{
					Globals.Score += Globals.GemScale;
					Globals.Gems++;
				}
				break;
			case 128: // pole
				if (MaxTongueLength >= length)
				{
					Direction = Heading * length;
					Action = Action.CoilTether;
					InitialPos = Position;
					Frames = 6 * length;
					MaxTongueLength = 0;
					TongueSprite.Playing = true;
					CallDeferred("SetTongueCollision", 0);
				}
				break;
			case 1024: // switch
				a.GetParent<Switch>().Activate();
				break;
		}
	}

	private void OnTongue1Entered(object area)
	{
		OnTongueEntered(area, 1);
	}

	private void OnTongue2Entered(object area)
	{
		OnTongueEntered(area, 2);
	}

	private void OnTongue3Entered(object area)
	{
		OnTongueEntered(area, 3);
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
