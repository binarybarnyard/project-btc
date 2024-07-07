namespace ProjectBTC.Scripts.PowerUps
{
    public interface IPowerUp
    {
        public string InputMapName { get; set; }
        public string Name { get; set; }
        public void Execute();
    }
}