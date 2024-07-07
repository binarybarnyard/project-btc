using Godot;
using System;

public partial class PickupArea2D : Area2D
{

    // States
    public bool Collected = false;

    // Environment
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public float TerminalVelocity = 100f;
    private Vector2 velocity = Vector2.Zero;
    private RayCast2D floorRayCast;

    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
        floorRayCast = GetNode<RayCast2D>("FloorRayCast"); // Initialize RayCast2D
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleGravity(delta);
        Disappear(delta);
    }

    private void OnBodyEntered(Node body)
    {
        // If collected by player, apply pickup effect
        if (body is Player player && !Collected)
        {
            ApplyPickupEffect(body as Player);
            GetNode<AudioStreamPlayer2D>("Coin").Play();
            Collected = true;
        }
    }

    private void HandleGravity(double delta)
    {
        // Apply gravity if the item is not collected and not on the floor
        if (!Collected && !floorRayCast.IsColliding())
        {
            // Update velocity using gravity or terminal velocity, whichever is lower
            velocity.Y += gravity * (float)delta;
            velocity.Y = Mathf.Min(velocity.Y, TerminalVelocity);

            // Apply velocity to position
            Position += velocity * (float)delta;
        }
        else if (floorRayCast.IsColliding())
        {
            // Stop vertical movement when on the floor
            velocity.Y = 0; 
        }
    }

    private void Disappear(double delta)
    {
        // Rotate and shrink the item when collected
        if (Collected)
        {
            Rotation += (Mathf.Pi * 3) * (float)delta;
            Scale = new Vector2(Scale.X - (float)delta * 2, Scale.Y - (float)delta * 2);

            // Remove the item when it's fully shrunk
            if (Scale.X <= 0)
            {
                QueueFree();
            }
        }
    }

    protected virtual void ApplyPickupEffect(Player player)
    {
    }
}
