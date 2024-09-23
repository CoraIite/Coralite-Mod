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
        public override MagikeTileEntity GetEntityInstance() => GetInstance<HoneyLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Beeswax,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Feather,
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
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.Beeswax:
                    MagikeMaxBase = 9;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    MagikeMaxBase = 562;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MagikeApparatusLevel.Feather:
                    MagikeMaxBase = 562;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class HoneyLensSender : UpgradeableLinerSender
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
                case MagikeApparatusLevel.Beeswax:
                    UnitDeliveryBase = 3;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    UnitDeliveryBase = 150;
                    SendDelayBase = 8;
                    break;
                case MagikeApparatusLevel.Feather:
                    UnitDeliveryBase = 150;
                    SendDelayBase = 8;
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

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Beeswax:
                    ProductionDelayBase = 10;
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
                case MagikeApparatusLevel.Feather:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}