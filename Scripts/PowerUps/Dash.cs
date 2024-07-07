using System.Threading.Tasks;
using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public partial class Dash : IPowerUp
    {
        public static string InputMapName { get; set; } = "dash";
        public static string Name { get; set; } = "Dash";
        private bool IsDashing = false;
        private const float DashVelocity = 500.0f;
        private const float DashTime = 0.2f; // Time in seconds
        private AudioStreamPlayer2D soundPlayer { get; set; }
        private AnimatedSprite2D animatedSprite { get; set; }
        private CharacterBody2D body { get; set; }

        public Dash(CharacterBody2D body, AnimatedSprite2D sprite)
        {
            var audio = GD.Load<AudioStream>("res://Assets/Audio/Sounds/power_up.wav");
            soundPlayer = new AudioStreamPlayer2D { Stream = audio };
            this.body = body;
            animatedSprite = sprite;

            // Add soundPlayer to the body node
            body.AddChild(soundPlayer);
        }

        public async Task Execute()
        {
            GD.Print("Dash Bang");
            // Must have dash enabled and either on the floor or air dash enabled
            if (!IsDashing && (body.IsOnFloor() || body.IsOnWall()))
            {
                IsDashing = true;
                soundPlayer.Play();

                Vector2 dashVelocity = new Vector2(DashVelocity, 0);
                float horizontalInput = Input.GetAxis("move_left", "move_right");

                if (horizontalInput < 0)
                {
                    dashVelocity.X = -DashVelocity;
                }
                else
                {
                    dashVelocity.X = DashVelocity;
                }

                animatedSprite.Stop();
                animatedSprite.Play("jump");

                await StartDash(dashVelocity);
            }
        }

        private async Task StartDash(Vector2 dashVelocity)
        {
            float elapsedTime = 0;

            while (elapsedTime < DashTime)
            {
                body.Velocity = dashVelocity;

                // Wait for the next frame
                elapsedTime += (float)body.GetProcessDeltaTime();
                body.MoveAndSlide();
                await body.ToSignal(body.GetTree(), "process_frame");
            }

            // Reset the velocity after the dash
            //body.Velocity = Vector2.Zero;
            IsDashing = false;
        }
    }
}
