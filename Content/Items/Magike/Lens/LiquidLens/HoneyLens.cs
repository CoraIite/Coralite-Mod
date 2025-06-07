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
                    MagikeMaxBase = 178;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.CrystallineMagike:
                    MagikeMaxBase = 500;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.Feather:
                    MagikeMaxBase = 1000;
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
            ConnectLengthBase = 6 * 16;

            switch (incomeLevel)
            {
                default:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Beeswax:
                    UnitDeliveryBase = 80;
                    SendDelayBase = 4 * 60 + 30;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 200;
                    SendDelayBase = 4 * 60;
                    break;
                case MALevel.Feather:
                    UnitDeliveryBase = 350;
                    SendDelayBase = 3 * 60 + 30;
                    break;
            }

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
                    ProductionDelayBase = 4 * 60 + 30;
                    ThroughputBase = 16;
                    break;
                case MALevel.CrystallineMagike:
                    ProductionDelayBase = 4 * 60;
                    ThroughputBase = 40;
                    break;
                case MALevel.Feather:
                    ProductionDelayBase = 3 * 60 + 30;
                    ThroughputBase = 70;
                    break;
            }

            Timer = ProductionDelayBase;
        }
    }
}