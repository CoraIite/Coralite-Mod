namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class UpgradeableLinerSender : MagikeLinerSender, IUpgradeable
    {
        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel) => false;
    }

    public class UpgradeableContainer : MagikeContainer, IUpgradeable
    {
        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel) => false;
    }

    public class UpgradeableActiveProducer : MagikeActiveProducer, IUpgradeable
    {
        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel) => false;
    }
}
