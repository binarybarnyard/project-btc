using Godot;
using ProjectBTC.Scripts.PowerUps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
		collectedPowers.Add( Dash.InputMapName, new Dash(this, _animatedSprite));
	}

	public override void _PhysicsProcess(double delta)
	{
		Console.WriteLine("_PhysicsProcess");
		HandleControls(delta);
		Console.WriteLine("_HandleControls Passed");
		UpdateAnimation();
		Console.WriteLine("UpdateAnimation Passed");
		HandleGravity(delta);
		Console.WriteLine("HandleGravity Passed");

		if (!IsOnFloor())
		{
			_leftFloor = true;
		}
	}

	public override void _Input(InputEvent @event)
	{
		foreach (string action in collectedPowers.Keys)
		{
			if (@event.IsAction(action))
			{
				collectedPowers[action].Execute();
			}
		}
	}

	private void HandleControls(double delta)
	{
		HandleJump();
		HandleMovement(delta);

		// Apply the velocity to the player
		Velocity = _velocity;
		MoveAndSlide();
	}

	private void HandleMovement(double delta)
	{
		// Get the current movement direction from input
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
			// Normalize only if both horizontal and vertical inputs are non-zero
			if (direction.X != 0 && direction.Y != 0)
			{
				direction = direction.Normalized();
				// Add 1/2 speed to account for multi-directional input slowdown while grounded
				_velocity.X = direction.X * (Speed + Speed / 2);
			}
			else
			{
				// Apply the speed
				_velocity.X = direction.X * Speed;
			}

			// Store the last horizontal input direction for flipping the sprite
			if (direction.X < 0)
				_lastHorizontalInput = "left";
			else if (direction.X > 0)
				_lastHorizontalInput = "right";
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, Speed * (float)delta * 10);
		}
	}

	private void HandleJump()
	{

		if (IsOnFloor() && DoubleJumpEnabled)
		{
			// Recharge Double Jump while on floor
			CanDoubleJump = true;
		}

		if (IsOnFloor() && _leftFloor)
		{
			// Play landing sound if you weren't on the floor but now are
			GetNode<AudioStreamPlayer2D>("Land").Play();
			_leftFloor = false;
		}

		// Listen for Jump Event
		if (Input.IsActionJustPressed("jump"))
		{
			if (IsOnFloor())
			{
				_velocity.Y = JumpVelocity;
				_animatedSprite.Play("jump");
				GetNode<AudioStreamPlayer2D>("Jump").Play();
			}
			// Double Jump (restart jump animation)
			else if (CanDoubleJump)
			{
				_velocity.Y = JumpVelocity;
				CanDoubleJump = false;
				_animatedSprite.Stop();
				_animatedSprite.Play("jump");
				GetNode<AudioStreamPlayer2D>("Jump").Play();
			}
		}
	}

	private void HandleGravity(double delta)
	{
		// Apply gravity only if the player is not on the floor and not dashing
		// Max fall speed cannot go past TerminalVelocity
		// TODO: Walk animation plays when walking off ledge
		if (!IsOnFloor())
		{
			_velocity.Y += gravity * (float)delta;
			_velocity.Y = Mathf.Min(_velocity.Y, TerminalVelocity);
		}

		// Reset and apply gravity if headache on ceiling 
		if (IsOnCeiling())
		{
			_velocity.Y = 0;
			_velocity.Y += gravity * (float)delta;
		}
	}

	private void UpdateAnimation()
	{
		// Update the animation based on the player's state
		if (IsOnFloor())
		{
			// if (IsDashing)
			// {
			// 	_animatedSprite.Play("jump");
			// }
			// else
			if (_velocity.X == 0)
			{
				_animatedSprite.Play("idle");
			}
			else
			{
				_animatedSprite.Play("walk");
			}
		}

		// Flip the sprite based on the last horizontal input direction
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
