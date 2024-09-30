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
        ( Color.SaddleBrown, DustID.HallowedTorch)
    {
        public override int DropItemType => ItemType<SunlightLens>();
        public override MagikeTileEntity GetEntityInstance() => GetInstance<SunlightLensTileEntity>();

        public override MALevel[] GetAllLevels()
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
                    AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Glistent:
                    MagikeMaxBase = 36;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Hallow:
                    MagikeMaxBase = 618;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.HolyLight:
                    MagikeMaxBase = 835;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class SunlightLensSender : UpgradeableLinerSender
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
                case MALevel.Glistent:
                    UnitDeliveryBase = 12;
                    SendDelayBase = 10;
                    break;
                case MALevel.Hallow:
                    UnitDeliveryBase = 165;
                    SendDelayBase = 8;
                    break;
                case MALevel.HolyLight:
                    UnitDeliveryBase = 195;
                    SendDelayBase = 7;
                    break;
            }

            SendDelayBase *= 60;
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
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MALevel.Glistent:
                    ProductionDelayBase = 10;
                    ThroughputBase = 4;
                    break;
                case MALevel.Hallow:
                    ProductionDelayBase = 8;
                    ThroughputBase = 55;
                    break;
                case MALevel.HolyLight:
                    ProductionDelayBase = 7;
                    ThroughputBase = 65;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
