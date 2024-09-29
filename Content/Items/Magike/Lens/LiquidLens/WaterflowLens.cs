using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.LiquidLens
{
    public class WaterflowLens() : MagikeApparatusItem(TileType<WaterflowLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.WaterBucket)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class WaterflowLensTile() : BaseLensTile
        (Color.Red, DustID.Water)
    {
        public override int DropItemType => ItemType<WaterflowLens>();
        public override MagikeTileEntity GetEntityInstance() => GetInstance<WaterflowLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Glistent,
                MagikeApparatusLevel.Pelagic,
            ];
        }
    }

    public class WaterflowLensTileEntity : BaseActiveProducerTileEntity<WaterflowLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new WaterflowLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new WaterflowLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new WaterflowProducer();
    }

    public class WaterflowLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.Glistent:
                    MagikeMaxBase = 72;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.Pelagic:
                    MagikeMaxBase = 732;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class WaterflowLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
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
                case MagikeApparatusLevel.Glistent:
                    UnitDeliveryBase = 24;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.Pelagic:
                    UnitDeliveryBase = 195;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class WaterflowProducer : UpgradeableProducerByLiquid
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.WaterflowLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.WaterflowCondition;

        public override int LiquidType => LiquidID.Water;

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Glistent:
                    ProductionDelayBase = 10;
                    ThroughputBase = 8;
                    break;
                case MagikeApparatusLevel.Pelagic:
                    ProductionDelayBase = 8;
                    ThroughputBase = 65;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}