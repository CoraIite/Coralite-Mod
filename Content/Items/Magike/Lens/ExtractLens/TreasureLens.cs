using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.ExtractLens
{
    public class TreasureLens() : MagikeApparatusItem(TileType<TreasureLensTile>(), Item.sellPrice(silver: 5)
            , ItemRarityID.LightRed, AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Skarn>(12)
                .AddIngredient(ItemID.GoldDust, 3)
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class TreasureLensTile() : BaseLensTile
        (Color.Gold, DustID.GoldCoin)
    {
        public override int DropItemType => ItemType<TreasureLens>();

                public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                BrilliantLevel.ID,
                ];
        }
    }

    public class TreasureLensTileEntity : BaseCostItemProducerTileEntity<TreasureLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new TreasureLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new TreasureLensSender();

        public override MagikeCostItemProducer GetStartProducer()
            => new TreasureProducer();

        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };
    }

    public class TreasureLensContainer : UpgradeableContainer<TreasureLensTile>
    {
    }

    public class TreasureLensSender : UpgradeableLinerSender<TreasureLensTile>
    {
    }

    public class TreasureProducer : MagikeCostItemProducer, IUpgradeable,IUpgradeLoadable
    {
        public override string GetCanProduceText => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemWithValue);

        public override MagikeSystem.UITextID NameText => MagikeSystem.UITextID.TresureLensName;

        public int TileType => TileType<TreasureLensTile>();

        public override bool CanConsumeItem(Item item)
            => item.value > 99;

        public override int GetMagikeAmount(Item item)
            => Math.Clamp(item.value / 100, 1, 100);

        #region 升级部分

        public override void Initialize()
        {
            InitializeLevel();
        }

        public void InitializeLevel()
        {
            ProductionDelayBase = -1;
        }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            ProductionDelayBase = MagikeSystem.GetLevelData4Time(incomeLevel, name + nameof(ProductionDelayBase));

            Timer = ProductionDelay;
        }

        #endregion
    }
}
