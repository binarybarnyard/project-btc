using System.Threading.Tasks;
using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public class AirDash : IPowerUp
    {
        public static string InputMapName { get; set; } = "dash";
        public static string Name { get; set; } = "Air Dash";

        public async Task Execute()
        {
            GD.Print("Air Dash Bang");
        }

        public void Update(double delta)
        {
            throw new System.NotImplementedException();
        }

    }
}