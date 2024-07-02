using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
    // Movement
    private Vector2 _velocity = new();
    private string _lastHorizontalInput = "right"; // To store the last horizontal input direction
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;
    
    // Powers
    public bool _canDoubleJump = false;
    public bool _isDashing = false;
    public bool _canAirDash = true;
    public const float DashVelocity = 500.0f;
    public float _dashTime = 0.2f; // Time in seconds

    // Environment
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // Animation
    private AnimatedSprite2D _animatedSprite;
    private AnimationPlayer _animationPlayer;


    public override void _Ready()
    {
        Console.WriteLine("_Ready");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
    }

    private void HandleControls(double delta)
    {
        HandleJump();
        HandleDash();
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
        if (IsOnFloor())
        {
            // Recharge Double Jump while on floor
            _canDoubleJump = true;
        }

        // Listen for Jump Event
        if (Input.IsActionJustPressed("jump"))
        {
            
            if (IsOnFloor())
            {
                _velocity.Y = JumpVelocity;
                _animatedSprite.Play("jump");
            }
            // Double Jump (restart jump animation)
            else if (_canDoubleJump)
            {
                _velocity.Y = JumpVelocity;
                _canDoubleJump = false;
                _animatedSprite.Stop();
                _animatedSprite.Play("jump");
            }
        }
    }

    private async void HandleDash()
    {
        // Must be on the floor or have air dash enabled
        if (Input.IsActionJustPressed("dash") && !_isDashing && (IsOnFloor() || _canAirDash))
        {
            _isDashing = true;
            Vector2 dashDistance = new Vector2(DashVelocity * _dashTime, 0);

            if (_lastHorizontalInput == "left")
            {
                _animatedSprite.Stop();
                _animatedSprite.Play("jump");
                await StartDash(-dashDistance);
            }
            else if (_lastHorizontalInput == "right")
            {
                _animatedSprite.Stop();
                _animatedSprite.Play("jump");
                await StartDash(dashDistance);
            }
        }
    }

    private async Task StartDash(Vector2 dashDistance)
    {
        float elapsedTime = 0;
        Vector2 dashVelocity = dashDistance / _dashTime;

        while (elapsedTime < _dashTime)
        {
            Vector2 motion = dashVelocity * (float)GetProcessDeltaTime();
            KinematicCollision2D collision = MoveAndCollide(motion);

            if (collision != null)
            {
                // Stop dashing if a collision occurs
                break;
            }

            elapsedTime += (float)GetProcessDeltaTime();
            await ToSignal(GetTree(), "process_frame");
        }

        _isDashing = false;
    }

    private void HandleGravity(double delta)
    {
        // Apply gravity only if the player is not on the floor and not dashing
        if (!IsOnFloor() && !_isDashing)
        {
            _velocity.Y += gravity * (float)delta;
        }
    }

    private void UpdateAnimation()
    {
        // Update the animation based on the player's state
        if (IsOnFloor())
        {
            if (_isDashing)
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
