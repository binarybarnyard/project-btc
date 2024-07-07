using System;
using System.Threading.Tasks;
using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public class DoubleJump : IPowerUp
    {
        public static string InputMapName { get; set; } = "jump";
        public static string Name { get; set; } = "Jump";
        private AudioStreamPlayer2D soundPlayer { get; set; }
        private AnimatedSprite2D animatedSprite { get; set; }
        private CharacterBody2D body { get; set; }
        public const float JumpVelocity = -400.0f;
        private bool canDoubleJump { get; set; } = true;
        private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        public DoubleJump(CharacterBody2D body, AnimatedSprite2D sprite)
        {
            var audio = GD.Load<AudioStream>("res://Assets/Audio/Sounds/woosh.wav");
            soundPlayer = new AudioStreamPlayer2D { Stream = audio };

            // Add SoundPlayer to the body node
            if (body.HasNode("SoundPlayer"))
            {
                body.GetNode<AudioStreamPlayer2D>("SoundPlayer").QueueFree(); // Remove existing sound player to avoid duplicates
            }
            this.body = body;
            animatedSprite = sprite;

            // Add soundPlayer to the body node
            body.AddChild(soundPlayer);
        }

        public async Task Execute()
        {
            if (canDoubleJump && !body.IsOnFloor() && body.Velocity.Y != 0)
            {
                GD.Print("Inside DoubleJump");
                body.Velocity = new Vector2(body.Velocity.X, JumpVelocity); // Set the jump velocity directly
                animatedSprite.Stop();
                animatedSprite.Play("jump");
                soundPlayer.Play();
                canDoubleJump = false;
            }
        }

        public void Update(double delta)
        {
            if (body.IsOnFloor())
            {
                canDoubleJump = true;
            }
        }
    }
}