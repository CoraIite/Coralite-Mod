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
        ( Color.Purple, DustID.PurpleTorch)
    {
        public override int DropItemType => ItemType<MoonlightLens>();
        public override MagikeTileEntity GetEntityInstance() => GetInstance<MoonlightLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.BloodJade,
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
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.MagicCrystal:
                    MagikeMaxBase = 9;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.BloodJade:
                    MagikeMaxBase = 562;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class MoonlightLensSender : UpgradeableLinerSender
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
                case MagikeApparatusLevel.MagicCrystal:
                    UnitDeliveryBase = 3;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.BloodJade:
                    UnitDeliveryBase = 150;
                    SendDelayBase = 8;
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

        public override int Throughput => base.Throughput + (Main.bloodMoon ? 7 : 0);

        public override bool CheckDayTime()
        {
            return !Main.dayTime && !Main.raining && Main.moonType != (int)MoonPhase.Empty;
        }

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
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Hallow:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
                case MagikeApparatusLevel.HolyLight:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}