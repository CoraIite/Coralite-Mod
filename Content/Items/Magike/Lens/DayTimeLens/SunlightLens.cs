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

namespace Coralite.Content.Items.Magike.Lens.DayTimeLens
{
    public class SunlightLens() : MagikeApparatusItem(TileType<SunlightLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.Sunflower)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SunlightLensTile() : BaseLensTile
        (Color.SaddleBrown, DustID.HallowedTorch)
    {
        public override int DropItemType => ItemType<SunlightLens>();

                public override List<ushort> GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Glistent,
                MALevel.Hallow,
                MALevel.HolyLight,
            ];
        }
    }

    public class SunlightLensTileEntity : BaseActiveProducerTileEntity<SunlightLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new SunlightLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new SunlightLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new SunlightProducer();
    }

    public class SunlightLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Glistent:
                    MagikeMaxBase = 100;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Hallow:
                    MagikeMaxBase = 1000;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.HolyLight:
                    MagikeMaxBase = 1928;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class SunlightLensSender : UpgradeableLinerSender
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
                case MALevel.Glistent:
                    UnitDeliveryBase = 50;
                    SendDelayBase = 5 * 60;
                    break;
                case MALevel.Hallow:
                    UnitDeliveryBase = 400;
                    SendDelayBase = 4 * 60;
                    break;
                case MALevel.HolyLight:
                    UnitDeliveryBase = 675;
                    SendDelayBase = 3 * 60 + 30;
                    break;
            }

            RecheckConnect();
        }
    }

    public class SunlightProducer : UpgradeableProducerByTime
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.SunlightLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.SunlightCondition;

        public override bool CheckDayTime()
        {
            return Main.dayTime && !Main.raining && !Main.eclipse;
        }

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = -1;
                    ThroughputBase = 0;
                    break;
                case MALevel.Glistent:
                    ProductionDelayBase = 5 * 60;
                    ThroughputBase = 10;
                    break;
                case MALevel.Hallow:
                    ProductionDelayBase = 4 * 60;
                    ThroughputBase = 80;
                    break;
                case MALevel.HolyLight:
                    ProductionDelayBase = 3 * 60 + 30;
                    ThroughputBase = 135;
                    break;
            }

            Timer = ProductionDelayBase;
        }
    }
}
