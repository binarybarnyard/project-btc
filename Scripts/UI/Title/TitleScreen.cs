using Godot;
using System;

public partial class TitleScreen : Control
{
	private Button startButton;
	private Button exitButton;
	private AudioStreamPlayer musicPlayer;

	public override void _Ready()
	{
		// Initialize nodes
		startButton = GetNode<Button>("VBoxContainer/NewGameButton");
		exitButton = GetNode<Button>("VBoxContainer/ExitButton");
		musicPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		// Play background music
		musicPlayer.Play();
		// Connect button signals
		startButton.Connect("pressed", new Callable(this, nameof(OnNewGamePressed)));
		exitButton.Connect("pressed", new Callable(this, nameof(OnExitPressed)));
	}

	private void OnNewGamePressed()
	{
		// Replace "MainScene" with the path to your main game scene
		GetTree().ChangeSceneToFile("res://Scenes/MainScene.tscn");
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
