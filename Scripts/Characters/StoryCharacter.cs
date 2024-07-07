using Godot;
using System;

public partial class StoryCharacter : CharacterBody2D
{
    // Movement
	public Vector2 _velocity = new();

    // Environment
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public const float TerminalVelocity = 600.0f;


    public override void _PhysicsProcess(double delta)
	{
		Console.WriteLine("_PhysicsProcess");
		HandleGravity(delta);
		Console.WriteLine("HandleGravity Passed");

        // Update character velocity
        Velocity = _velocity;

        // Call MoveAndSlide within Subclass
	}

    public void HandleGravity(double delta)
	{
		// Apply gravity only if the player is not on the floor
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
}