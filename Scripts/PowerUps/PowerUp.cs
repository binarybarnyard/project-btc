using System.Threading.Tasks;

namespace ProjectBTC.Scripts.PowerUps
{
    public interface IPowerUp
    {
        public static string InputMapName { get; set; }
        public static string Name { get; set; }
        public Task Execute();
    }
}