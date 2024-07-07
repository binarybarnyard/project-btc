using Godot;
using System;

public partial class Controls : CharacterBody2D
{
    private Player _player;

    // Movement
	public Vector2 _velocity = new();
	public string LastHorizontalInput = "right"; // To store the last horizontal input direction
	public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    // Constructor
    public Controls(Player player)
    {
        _player = player;
    }

	public void HandleControls(double delta)
	{
        // Reset velocity each frame
        _velocity = Vector2.Zero;

		// HandleDash();
		HandleMovement(delta);
    	HandleJump();
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
				LastHorizontalInput = "left";
			else if (direction.X > 0)
				LastHorizontalInput = "right";
		}
		else
		{
			_velocity.X = Mathf.MoveToward(_velocity.X, 0, Speed * (float)delta * 10);
		}
	}

	private void HandleJump()
	{    
		// Listen for Jump Event
		if (Input.IsActionJustPressed("jump") && _player.IsOnFloor())
		{
            _velocity.Y = JumpVelocity;
		}
	}
}
