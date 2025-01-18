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
    public class BasicRefractor() : MagikeApparatusItem(TileType<BasicRefractorTile>(), Item.sellPrice(silver: 5)
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

    public class BasicRefractorTile() : BaseRefractorTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicRefractor>();

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Crimson,
                MALevel.Corruption,
                MALevel.Icicle,
                MALevel.CrystallineMagike,
                MALevel.Soul,
                MALevel.Feather,
                MALevel.SplendorMagicore
                ];
        }
    }

    public class BasicRefractorTileEntity : BaseSenderTileEntity<BasicRefractorTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicRefractorContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicRefractorSender();
    }

    public class BasicRefractorContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 60,
                MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle => 300,
                MALevel.CrystallineMagike => 1800,
                MALevel.Soul
                or MALevel.Feather => 7500,
                MALevel.SplendorMagicore => 18000,
                _ => 0,
            };
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase;
            //LimitAntiMagikeAmount();
        }
    }

    public class BasicRefractorSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;

            switch (incomeLevel)
            {
                default:
                case MALevel.None:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    UnitDeliveryBase = 10;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.Crimson:
                case MALevel.Corruption:
                case MALevel.Icicle:
                    UnitDeliveryBase = 50;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 120;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MALevel.Soul:
                case MALevel.Feather:
                    UnitDeliveryBase = 500;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MALevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
