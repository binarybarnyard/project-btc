using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public class AirDash : IPowerUp
    {
        public string InputMapName { get; set; } = "jump";
        public string Name { get; set; } = "Double Jump";

        public void Execute()
        {
            GD.Print("Double Jump Bang");
        }

    }
}