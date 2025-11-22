namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 无限
    /// </summary>
    public class InfinityLevel : MagikeLevel
    {
        public override bool Available => false;

        public override float MagikeCostValue => float.MaxValue;
    }
}
