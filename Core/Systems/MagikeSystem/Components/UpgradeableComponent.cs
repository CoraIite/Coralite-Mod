using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Helpers;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class UpgradeableLinerSender : MagikeLinerSender, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableContainer : MagikeContainer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableActiveProducer : MagikeActiveProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableExtractProducer : MagikeExtractProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public abstract class UpgradeableCostItemProducer : MagikeCostItemProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public abstract class UpgradeableProducerByBiome : ProducerByBiome, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public abstract class UpgradeableProducerByTime : ProducerByTime, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public abstract class UpgradeableProducerByLiquid : ProducerByLiquid, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableItemContainer : ItemContainer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }

    public class UpgradeableCharger : Charger, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);
    }
}
