using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Helpers;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class UpgradeableLinerSender<T> : MagikeLinerSender, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableContainer<T> : MagikeContainer, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableActiveProducer<T> : MagikeActiveProducer, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableExtractProducer<T> : MagikeExtractProducer, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableCostItemProducer<T> : MagikeCostItemProducer, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableProducerByBiome<T> : ProducerByBiome, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableProducerByTime<T> : ProducerByTime, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableProducerByLiquid<T> : ProducerByLiquid, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableItemContainer<T> : ItemContainer, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }

    public abstract class UpgradeableCharger<T> : Charger, IUpgradeable, IUpgradeLoadable
        where T : ModTile
    {
        public int TileType => ModContent.TileType<T>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public virtual void Upgrade(ushort incomeLevel) { }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public abstract void InitializeLevel();
    }
}
