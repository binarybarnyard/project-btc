using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public class Dash : IPowerUp
    {
        public string InputMapName { get; set; } = "dash";
        public string Name { get; set; } = "Dash";

        public void Execute()
        {
            GD.Print("Dash Bang");
        }
    }
}