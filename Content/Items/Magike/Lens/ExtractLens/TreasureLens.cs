using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System;
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

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.CrystallineMagike,
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

    public class TreasureLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.CrystallineMagike => 1125,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class TreasureLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 4 * 16;
            switch (incomeLevel)
            {
                default:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 300;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class TreasureProducer : MagikeCostItemProducer, IUpgradeable
    {
        public override string GetCanProduceText => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemWithValue);

        public override MagikeSystem.UITextID NameText => MagikeSystem.UITextID.TresureLensName;

        public override bool CanConsumeItem(Item item)
            => item.value > 99;

        public override int GetMagikeAmount(Item item)
            => Math.Clamp(item.value / 100, 1, 100);

        #region 升级部分

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public void Upgrade(MALevel incomeLevel)
        {
            ProductionDelayBase = incomeLevel switch
            {
                MALevel.CrystallineMagike => 8,
                _ => 1_0000_0000 / 60,//随便填个大数
            } * 60;

            Timer = ProductionDelayBase;
        }

        #endregion
    }
}
