using System.Threading.Tasks;
using Godot;

namespace ProjectBTC.Scripts.PowerUps
{
    public class DoubleJump : IPowerUp
    {
        public static string InputMapName { get; set; } = "jump";
        public static string Name { get; set; } = "Double Jump";

        public async Task Execute()
        {
            GD.Print("Double Jump Bang");
        }
    }
}