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
        if (!IsOnFloor())
            _velocity.Y += gravity * (float)delta;

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            _velocity.Y = JumpVelocity;

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (direction != Vector2.Zero)
        {
            _velocity.X = direction.X * Speed;
        }
        else
        {
            _velocity.X = Mathf.MoveToward(_velocity.X, 0, Speed * (float)delta);
        }

        Velocity = _velocity;
        MoveAndSlide();
    }

    private void UpdateAnimation()
    {
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
    }
}