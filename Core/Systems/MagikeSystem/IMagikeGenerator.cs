namespace Coralite.Core.Systems.MagikeSystem
{
    public interface IMagikeGeneratorItem
    {
        public int HowManyToGenerate { get; }
        public string GenerateDelay { get; }
    }
}
