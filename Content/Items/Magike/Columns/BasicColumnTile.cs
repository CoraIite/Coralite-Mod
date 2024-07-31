using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicColumnTile : BaseMagikePlaceableItem
    {
        public BasicColumnTile() : base(TileType<BasicColumnTileTile>(), Item.sellPrice(0, 0, 5, 0)
            , RarityType<MagicCrystalRarity>(), 25, AssetDirectory.MagikeColumns)
        { }

        public override int MagikeMax => 1;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicColumnTileTile() : BaseColumnTile
        (1, 2, Coralite.Instance.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<BasicColumnTile>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicColumnTileTileEntity>();

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
                //MagikeApparatusLevel.Feather,
                MagikeApparatusLevel.SplendorMagicore
                ];
        }
    }

    public class BasicColumnTileTileEntity : BaseSenderTileEntity<BasicColumnTileTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicColumnTileSender();
    }

    public class BasicColumnTileContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MagikeApparatusLevel.MagicCrystal => 720,
                MagikeApparatusLevel.Crimson
                or MagikeApparatusLevel.Corruption
                or MagikeApparatusLevel.Icicle => 3600,
                MagikeApparatusLevel.CrystallineMagike => 12000,
                MagikeApparatusLevel.Soul
                or MagikeApparatusLevel.Feather => 60000,
                MagikeApparatusLevel.SplendorMagicore => 270000,
                _ => 0,
            };
            LimitMagikeAmount();
        }
    }

    public class BasicColumnTileSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MaxConnectBase = 1;

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
                    UnitDeliveryBase = 60;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.Crimson:
                case MagikeApparatusLevel.Corruption:
                case MagikeApparatusLevel.Icicle:
                    UnitDeliveryBase = 300;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    UnitDeliveryBase = 500;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.Soul:
                case MagikeApparatusLevel.Feather:
                    UnitDeliveryBase = 2500;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    UnitDeliveryBase = 9000;
                    SendDelayBase = 60 * 10;
                    ConnectLengthBase = 15 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
