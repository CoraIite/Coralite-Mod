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
    public class LavaLens() : MagikeApparatusItem(TileType<LavaLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.LavaBucket)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LavaLensTile() : BaseLensTile
        (Color.Red, DustID.Lava)
    {
        public override int DropItemType => ItemType<LavaLens>();
        public override MagikeTileEntity GetEntityInstance() => GetInstance<LavaLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Hellstone,
                MagikeApparatusLevel.EternalFlame,
            ];
        }
    }

    public class LavaLensTileEntity : BaseActiveProducerTileEntity<LavaLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new LavaLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new LavaLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new LavaProducer();
    }

    public class LavaLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.Hellstone:
                    MagikeMaxBase = 250;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.EternalFlame:
                    MagikeMaxBase = 900;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class LavaLensSender : UpgradeableLinerSender
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
                case MagikeApparatusLevel.Hellstone:
                    UnitDeliveryBase = 75;
                    SendDelayBase = 9;
                    break;
                case MagikeApparatusLevel.EternalFlame:
                    UnitDeliveryBase = 240;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class LavaProducer : UpgradeableProducerByLiquid
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.LavaLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.LavaCondition;

        public override int LiquidType => LiquidID.Lava;

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Hellstone:
                    ProductionDelayBase = 9;
                    ThroughputBase = 25;
                    break;
                case MagikeApparatusLevel.EternalFlame:
                    ProductionDelayBase = 8;
                    ThroughputBase = 80;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}