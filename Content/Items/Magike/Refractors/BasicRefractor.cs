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

                public override List<ushort> GetAllLevels()
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
                MALevel.MagicCrystal => 120,
                MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle => 600,
                MALevel.CrystallineMagike => 2160,
                MALevel.Soul
                or MALevel.Feather => 8960,
                MALevel.SplendorMagicore => 36000,
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
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    UnitDeliveryBase = 18;
                    SendDelayBase = 60 * 3;
                    ConnectLengthBase = 20 * 16;
                    break;
                case MALevel.Crimson:
                case MALevel.Corruption:
                case MALevel.Icicle:
                    UnitDeliveryBase = 72;
                    SendDelayBase = 60 * 3;
                    ConnectLengthBase = 20 * 16;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 144;
                    SendDelayBase = 60 * 2;
                    ConnectLengthBase = 20 * 16;
                    break;
                case MALevel.Soul:
                case MALevel.Feather:
                    UnitDeliveryBase = 512;
                    SendDelayBase = 60 * 2;
                    ConnectLengthBase = 20 * 16;
                    break;
                case MALevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase = 60;
                    ConnectLengthBase = 25 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
