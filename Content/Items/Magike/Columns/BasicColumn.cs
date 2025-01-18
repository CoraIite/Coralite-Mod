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
    public class BasicColumn() : MagikeApparatusItem(TileType<BasicColumnTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeColumns)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicColumnTile() : BaseColumnTile
        (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<BasicColumn>();

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

    public class BasicColumnTileEntity : BaseSenderTileEntity<BasicColumnTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicColumnTileSender();
    }

    public class BasicColumnTileContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 720,
                MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle => 3600,
                MALevel.CrystallineMagike => 12000,
                MALevel.Soul
                or MALevel.Feather => 60000,
                MALevel.SplendorMagicore => 270000,
                _ => 0,
            };
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase / 2;
            //LimitAntiMagikeAmount();
        }
    }

    public class BasicColumnTileSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 6 * 16;
            SendDelayBase = 60 * 10;

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
                    UnitDeliveryBase = 60;
                    break;
                case MALevel.Crimson:
                case MALevel.Corruption:
                case MALevel.Icicle:
                    UnitDeliveryBase = 300;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 500;
                    break;
                case MALevel.Soul:
                case MALevel.Feather:
                    UnitDeliveryBase = 2500;
                    break;
                case MALevel.SplendorMagicore:
                    UnitDeliveryBase = 9000;
                    break;
            }

            RecheckConnect();
        }
    }
}
