using Godot;
using System;

public partial class DebugHud : Control
{
	private Label _fpsLabel;
	private Label _playerPositionLabel;
	private Label _debugLabel;
	private Timer _updateTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("DebugHUD ready");

		_fpsLabel = GetNode<Label>("CanvasLayer/VBoxContainer/FpsLabel");
		_playerPositionLabel = GetNode<Label>("CanvasLayer/VBoxContainer/PlayerPositionLabel");
		_debugLabel = GetNode<Label>("CanvasLayer/VBoxContainer/DebugLabel");

		// Create and configure the timer
		_updateTimer = new Timer
		{
			WaitTime = 0.1f,
			OneShot = false,
			Autostart = true
		};

		AddChild(_updateTimer);  // Add the timer to the scene tree before starting it
		_updateTimer.Connect("timeout", new Callable(this, nameof(OnUpdateDebugInfo)));
		_updateTimer.Start();

		Hide();  // Hide the HUD initially
	}

	private void OnUpdateDebugInfo()
	{
		// Try to find the player node
		Player player = GetTree().Root.GetNode<Player>("MainScene/Player");
		if (player == null)
		{
			GD.PrintErr("Player node not found");
			_fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond().ToString()}";
			_playerPositionLabel.Text = "Player Position: N/A";
			_debugLabel.Text = "Player Velocity: N/A";
			return;
		}

		// Update debug information
		_fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond().ToString()}";
		_playerPositionLabel.Text = $"Player Position: {player.Position.ToString()}";
		_debugLabel.Text = $"Player Velocity: {player.Velocity.ToString()}";
	}
}
