using Godot;
using System;

public class Player : Entity
{
	// Declare member variables here.
	AnimatedSprite PlayerSprite;
	Node2D TonguePivot;
	AnimatedSprite TongueSprite;
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


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		PlayerSprite = GetNode<AnimatedSprite>("PlayerSprite");
		TonguePivot = PlayerSprite.GetNode<Node2D>("TonguePivot");
		TongueSprite = TonguePivot.GetNode<AnimatedSprite>("TongueSprite");
		SpawnPos = GlobalPosition;
	}

	// Called every tick. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if (PauseFrames > 0)
		{
			PauseFrames--;
			if (PauseFrames == 60)
			{
				GlobalPosition = SpawnPos;
			}
		}
		else if (OverWater && Action == Action.None)
		{
			PauseFrames = 180;
			Globals.Lives--;
			if (Globals.Lives < 0)
			{
				Globals.TimerActive = false;
				Globals.TimerPauseFrames = int.MaxValue;
				PauseFrames = int.MaxValue;
			}
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
		if (Input.IsActionPressed("action1"))
			return Action.CoilTongue;
		if (Input.IsActionPressed("action2"))
			return Action.CoilInvis;
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
		if (Input.IsActionJustPressed("action1"))
			return Action.CoilTongue;
		if (Input.IsActionJustPressed("action2"))
			return Action.CoilInvis;
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
						PauseFrames = int.MaxValue;
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
		}
	}

	private void OnTongueEntered(object area, int length)
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
