using Godot;
using System;

public partial class MainScene : Node2D
{
	private PackedScene _hudScene;
	private DebugHud _hudInstance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("MainScene Ready");

		_hudScene = GD.Load<PackedScene>("res://Scenes/UI/DebugHud.tscn");
		if (_hudScene == null)
		{
			GD.PrintErr("Failed to load Debug HUD scene");
			return;
		}

		_hudInstance = (DebugHud)_hudScene.Instantiate();
		if (_hudInstance == null)
		{
			GD.PrintErr("Failed to instance DebugHud");
			return;
		}

		AddChild(_hudInstance);
		_hudInstance.Show();
	}

	public void ShowHud()
	{
		_hudInstance.Show();
	}
}
