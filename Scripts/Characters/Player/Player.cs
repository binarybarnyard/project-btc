using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private AnimatedSprite2D _animatedSprite;
    private AnimationPlayer _animationPlayer;
    private Vector2 _velocity = new();
    private string _lastHorizontalInput = ""; // To store the last horizontal input direction

    public override void _Ready()
    {
        Console.WriteLine("_Ready");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        Console.WriteLine("_PhysicsProcess");
        HandleMovement(delta);
        Console.WriteLine("_HandleMovement Passed");
        UpdateAnimation();
        Console.WriteLine("UpdateAnimation Passed");
    }

    private void HandleMovement(double delta)
    {
        // Apply gravity if the player is not on the floor
        if (!IsOnFloor())
            _velocity.Y += gravity * (float)delta;

        // Handle jumping
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            _velocity.Y = JumpVelocity;

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
            _velocity.X = Mathf.MoveToward(_velocity.X, 0, Speed * (float)delta);
        }

        // Apply the velocity to the player
        Velocity = _velocity;
        MoveAndSlide();
    }

    private void UpdateAnimation()
    {
        // Update the animation based on the player's state
        if (!IsOnFloor())
        {
            _animatedSprite.Play("jump");
        }
        else if (_velocity.X == 0)
        {
            _animatedSprite.Play("idle");
        }
        else
        {
            _animatedSprite.Play("walk");
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
