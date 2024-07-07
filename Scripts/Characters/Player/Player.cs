using Godot;
using System;

public partial class Player : StoryCharacter
{
	// Sub-classes
	private Controls _controls;

	// Animation
	private AnimatedSprite2D _animatedSprite;
	private AnimationPlayer _animationPlayer;
	

	public override void _Ready()
	{
		// StoryCharacter init
		base._Ready();
		Console.WriteLine("_Ready");

		// Character movement init
		_controls = new Controls(this); 

		// Animation init
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		// StoryCharacter Gravity
		base._PhysicsProcess(delta); 

		// Controls Class
		_controls.HandleControls(delta);
		_velocity.X = _controls._velocity.X;
		_velocity.Y += _controls._velocity.Y;

		// Animation
		UpdateAnimation();

		MoveAndSlide();
	}

	private void UpdateAnimation()
	{
		// Update the animation based on the player's state
		if (IsOnFloor())
		{
			if (Velocity.X == 0)
			{
				_animatedSprite.Play("idle");
			}
			else
			{
				_animatedSprite.Play("walk");
			}
		}

		// Flip the sprite based on the last horizontal input direction
		if (_controls.LastHorizontalInput == "left")
		{
			_animatedSprite.FlipH = true;
		}
		else if (_controls.LastHorizontalInput == "right")
		{
			_animatedSprite.FlipH = false;
		}
	}
}
