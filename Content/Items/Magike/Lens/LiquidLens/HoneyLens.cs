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
    public class HoneyLens() : MagikeApparatusItem(TileType<HoneyLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.HoneyBucket)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HoneyLensTile() : BaseLensTile
        (Color.Honeydew, DustID.Honey)
    {
        public override int DropItemType => ItemType<HoneyLens>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Beeswax,
                MALevel.CrystallineMagike,
                MALevel.Feather,
            ];
        }
    }

    public class HoneyLensTileEntity : BaseActiveProducerTileEntity<HoneyLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new HoneyLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new HoneyLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new HoneyProducer();
    }

    public class HoneyLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Beeswax:
                    MagikeMaxBase = 320;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.CrystallineMagike:
                    MagikeMaxBase = 675;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.Feather:
                    MagikeMaxBase = 1800;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class HoneyLensSender : UpgradeableLinerSender
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
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Beeswax:
                    UnitDeliveryBase = 96;
                    SendDelayBase = 9;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 180;
                    SendDelayBase = 8;
                    break;
                case MALevel.Feather:
                    UnitDeliveryBase = 420;
                    SendDelayBase = 7;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class HoneyProducer : UpgradeableProducerByLiquid
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.HoneyLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.HoneyCondition;

        public override int LiquidType => LiquidID.Honey;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = -1;
                    ThroughputBase = 0;
                    break;
                case MALevel.Beeswax:
                    ProductionDelayBase = 9;
                    ThroughputBase = 32;
                    break;
                case MALevel.CrystallineMagike:
                    ProductionDelayBase = 8;
                    ThroughputBase = 60;
                    break;
                case MALevel.Feather:
                    ProductionDelayBase = 7;
                    ThroughputBase = 140;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}