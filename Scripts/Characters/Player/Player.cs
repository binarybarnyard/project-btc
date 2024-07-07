using Godot;
using ProjectBTC.Scripts.PowerUps;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D
{
	// Movement
	private Vector2 _velocity = new();
	private string _lastHorizontalInput = "right"; // To store the last horizontal input direction
	private bool _leftFloor = true;
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// Powers
	public bool DoubleJumpEnabled = false;
	public bool CanDoubleJump = false;
	public bool AirDashEnabled = false;

	// Environment
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public const float TerminalVelocity = 600.0f;

	// Animation
	private AnimatedSprite2D _animatedSprite;
	private AnimationPlayer _animationPlayer;
	private Dictionary<string, IPowerUp> collectedPowers = new Dictionary<string, IPowerUp>();

	public override void _Ready()
	{
		Console.WriteLine("_Ready");
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collectedPowers.Add(DoubleJump.InputMapName, new DoubleJump(this, _animatedSprite));
		collectedPowers.Add(Dash.InputMapName, new Dash(this, _animatedSprite));
	}

	public override void _PhysicsProcess(double delta)
	{
		Console.WriteLine("PhysicsProcess");
		UpdateAnimation();
		Console.WriteLine("UpdateAnimation Passed");
		HandleGravity(delta);
		Console.WriteLine("HandleGravity Passed");
		HandleControls(delta);
		Console.WriteLine("HandleControls Passed");
		PowerUpdate(delta); // Run update on any powers added in case they need to reset based on the player's position
		if (!IsOnFloor())
		{
			_leftFloor = true;
		}

		MoveAndSlide();
	}

	private void PowerUpdate(double delta) 
	{
		collectedPowers.Keys.ToList().ForEach(k => collectedPowers[k].Update(delta));
	}

	private void HandleControls(double delta)
	{
		HandleJump();
		HandleMovement(delta);
		foreach (string action in collectedPowers.Keys)
		{
			if (Input.IsActionJustPressed(action))
			{
				collectedPowers[action].Execute();
			}
		}
	}

	private void HandleMovement(double delta)
	{
		// Get the current movement direction from input
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
			if (direction.X != 0 && direction.Y != 0)
			{
				direction = direction.Normalized();
				_velocity.X = direction.X * (Speed + Speed / 2);
			}
			else
			{
				_velocity.X = direction.X * Speed;
			}

			if (direction.X < 0)
				_lastHorizontalInput = "left";
			else if (direction.X > 0)
				_lastHorizontalInput = "right";
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, Speed * (float)delta * 10);
		}

		// Update Velocity only with horizontal movement
		Velocity = new Vector2(_velocity.X, Velocity.Y);
	}

	private void HandleJump()
	{
		if (IsOnFloor())
		{
			CanDoubleJump = true;
		}

		if (IsOnFloor() && _leftFloor)
		{
			GetNode<AudioStreamPlayer2D>("Land").Play();
			_leftFloor = false;
		}

		// Listen for Jump Event
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			_velocity.Y = JumpVelocity;
			_animatedSprite.Play("jump");
			GetNode<AudioStreamPlayer2D>("Jump").Play();
			// Update Velocity with jump
			Velocity = new Vector2(Velocity.X, _velocity.Y);
		}
	}

	private void HandleGravity(double delta)
	{
		if (!IsOnFloor())
		{
			_velocity.Y += gravity * (float)delta;
			_velocity.Y = Mathf.Min(_velocity.Y, TerminalVelocity);
		}

		if (IsOnCeiling())
		{
			_velocity.Y = 0;
			_velocity.Y += gravity * (float)delta;
		}

		// Update Velocity only with vertical movement
		Velocity = new Vector2(Velocity.X, _velocity.Y);
	}

	private void UpdateAnimation()
	{
		if (IsOnFloor())
		{
			if (_velocity.X == 0)
			{
				_animatedSprite.Play("idle");
			}
			else
			{
				_animatedSprite.Play("walk");
			}
		}

		if (_lastHorizontalInput == "left")
		{
			_animatedSprite.FlipH = true;
		}
		else if (_lastHorizontalInput == "right")
		{
			_animatedSprite.FlipH = false;
		}
	}
}
