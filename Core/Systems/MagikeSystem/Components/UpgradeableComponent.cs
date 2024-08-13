using Coralite.Core.Systems.MagikeSystem.TileEntities;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class UpgradeableLinerSender : MagikeLinerSender, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
        {
            int tileType = (Entity as MagikeTileEntity).TileType;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            if (!keyValuePairs.ContainsKey(incomeLevel))
                return false;

            return true;
        }
    }

    public class UpgradeableContainer : MagikeContainer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
        {
            int tileType = (Entity as MagikeTileEntity).TileType;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            if (!keyValuePairs.ContainsKey(incomeLevel))
                return false;

            return true;
        }
    }

    public class UpgradeableActiveProducer : MagikeActiveProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
        {
            int tileType = (Entity as MagikeTileEntity).TileType;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            if (!keyValuePairs.ContainsKey(incomeLevel))
                return false;

            return true;
        }
    }

    public class UpgradeableExtractProducer : MagikeExtractProducer, IUpgradeable
    {
        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
        {
            int tileType = (Entity as MagikeTileEntity).TileType;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(tileType, out var keyValuePairs))
                return false;

            if (!keyValuePairs.ContainsKey(incomeLevel))
                return false;

            return true;
        }
    }
}
