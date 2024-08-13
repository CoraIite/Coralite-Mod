using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.ExtractLens
{
    public class BasicExtractLens() : MagikeApparatusItem(TileType<BasicExtractLensTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicExtractLensTile() : BaseLensTile
        (2, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns, 8)
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;
        public override int DropItemType => ItemType<BasicExtractLens>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicExtractLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.Crimson,
                MagikeApparatusLevel.Corruption,
                MagikeApparatusLevel.Icicle,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Soul,
                MagikeApparatusLevel.Feather,
                MagikeApparatusLevel.SplendorMagicore
                ];
        }
    }

    public class BasicExtractLensTileEntity : BaseCostItemProducerTileEntity<BasicExtractLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicExtractLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicExtractLensSender();

        public override MagikeCostItemProducer GetStartProducer()
            => new BasicExtractProducer();

        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };
    }

    public class BasicExtractLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MagikeApparatusLevel.MagicCrystal => 90,
                MagikeApparatusLevel.Glistent => 300,
                MagikeApparatusLevel.Crimson
                or MagikeApparatusLevel.Corruption
                or MagikeApparatusLevel.Icicle => 480,
                MagikeApparatusLevel.CrystallineMagike => 2250,
                MagikeApparatusLevel.Hallow => 9000,
                MagikeApparatusLevel.HolyLight => 15000,
                MagikeApparatusLevel.SplendorMagicore => 35000,
                _ => 0,
            };
            LimitMagikeAmount();
        }
    }

    public class BasicExtractLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MaxConnectBase = 2;

            switch (incomeLevel)
            {
                default:
                case MagikeApparatusLevel.None:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MagikeApparatusLevel.MagicCrystal:
                    UnitDeliveryBase = 10;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.Glistent:
                    UnitDeliveryBase = 50;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.Crimson:
                case MagikeApparatusLevel.Corruption:
                case MagikeApparatusLevel.Icicle:
                    UnitDeliveryBase = 50;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    UnitDeliveryBase = 120;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.Hallow:
                    UnitDeliveryBase = 500;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.HolyLight:
                    UnitDeliveryBase = 500;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
            }

            RecheckConnect();
        }
    }

    public class BasicExtractProducer : UpgradeableExtractProducer
    {

    }
}
