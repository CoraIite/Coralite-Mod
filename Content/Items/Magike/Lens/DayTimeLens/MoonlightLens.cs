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
using Terraria.Enums;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.DayTimeLens
{
    public class MoonlightLens() : MagikeApparatusItem(TileType<MoonlightLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class MoonlightLensTile() : BaseLensTile
        (Color.Purple, DustID.PurpleTorch)
    {
        public override int DropItemType => ItemType<MoonlightLens>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.BloodJade,
            ];
        }
    }

    public class MoonlightLensTileEntity : BaseActiveProducerTileEntity<MoonlightLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new MoonlightLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new MoonlightLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new MoonlightProducer();
    }

    public class MoonlightLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    MagikeMaxBase = 20;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.BloodJade:
                    MagikeMaxBase = 1000;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class MoonlightLensSender : UpgradeableLinerSender
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
                case MALevel.MagicCrystal:
                    UnitDeliveryBase = 10;
                    SendDelayBase = 5;
                    break;
                case MALevel.BloodJade:
                    UnitDeliveryBase = 400;
                    SendDelayBase = 4;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class MoonlightProducer : UpgradeableProducerByTime
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.MoonlightLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.MoonlightCondition;

        public override int Throughput
        {
            get
            {
                int i = base.Throughput;
                if (Main.bloodMoon)
                    i = (int)(i * 2f);
                if (Main.moonType == (int)MoonPhase.Full)
                    i = (int)(i * 2f);

                return i;
            }
        }

        public override bool CheckDayTime()
        {
            return !Main.dayTime && !Main.raining && Main.moonType != (int)MoonPhase.Empty;
        }

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = -1;
                    ThroughputBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    ProductionDelayBase = 5;
                    ThroughputBase = 2;
                    break;
                case MALevel.BloodJade:
                    ProductionDelayBase = 4;
                    ThroughputBase = 80;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}