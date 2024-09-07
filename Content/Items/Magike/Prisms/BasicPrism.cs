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

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicPrism() : MagikeApparatusItem(TileType<BasicPrismTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicPrismTile() : BasePrismTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns, 8)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicPrism>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicPrismTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.Glistent,
                MagikeApparatusLevel.Crimson,
                MagikeApparatusLevel.Corruption,
                MagikeApparatusLevel.Icicle,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Hallow,
                MagikeApparatusLevel.HolyLight,
                MagikeApparatusLevel.SplendorMagicore
                ];
        }
    }

    public class BasicPrismTileEntity : BaseSenderTileEntity<BasicPrismTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicPrismContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicPrismSender();
    }

    public class BasicPrismContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MagikeApparatusLevel.MagicCrystal => 60,
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

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class BasicPrismSender : UpgradeableLinerSender
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
                    UnitDeliveryBase = 15;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 8 * 16;
                    break;
                case MagikeApparatusLevel.Glistent:
                    MaxConnectBase = 3;
                    UnitDeliveryBase = 50;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 8 * 16;
                    break;
                case MagikeApparatusLevel.Crimson:
                case MagikeApparatusLevel.Corruption:
                case MagikeApparatusLevel.Icicle:
                    MaxConnectBase = 3;
                    UnitDeliveryBase = 80;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 8 * 16;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    MaxConnectBase = 3;
                    UnitDeliveryBase = 200;
                    SendDelayBase = 60 * 8;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.Hallow:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 600;
                    SendDelayBase = 60 * 8;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.HolyLight:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 1000;
                    SendDelayBase = 60 * 8;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 1500;
                    SendDelayBase = 60 * 6;
                    ConnectLengthBase = 12 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
