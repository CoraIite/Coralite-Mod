using Coralite.Core.Systems.CoraliteActorComponent;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    /// <summary>
    /// 偏振滤镜，用于升级魔能仪器
    /// </summary>
    public abstract class PolarizedFilter : MagikeFilter
    {
        public abstract MagikeApparatusLevel UpgradeLevel { get; }

        public override void ChangeComponentValues(Component component)
        {
            if (component is not IUpgradeable upgrageableComponent)
                return;

            upgrageableComponent.Upgrade(UpgradeLevel);
        }

        public override void Update(IEntity entity)
        {

        }
    }
}
