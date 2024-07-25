using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicRefractor : BaseMagikePlaceableItem
    {
        public BasicRefractor() : base(TileType<BasicRefractorTile>(), Item.sellPrice(0, 0, 5, 0)
            , RarityType<MagicCrystalRarity>(), 25, AssetDirectory.MagikeRefractors)
        { }

        public override int MagikeMax => 1;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicRefractorTile() : BaseRefractorTile<BasicRefractorTileEntity>
        (1, 2, Coralite.Instance.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            MagikeSystem.RegisterApparatusLevel(Type,
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.Crimson,
                MagikeApparatusLevel.Corruption,
                MagikeApparatusLevel.Icicle,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Feather,
                MagikeApparatusLevel.SplendorMagicore
                );
        }
    }

    public class BasicRefractorTileEntity : BaseRefractorTileEntity<BasicRefractorTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicRefractorContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicRefractorSender();
    }

    public class BasicRefractorContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                case MagikeApparatusLevel.None:
                    MagikeMaxBase = 20;
                    break;
                case MagikeApparatusLevel.MagicCrystal:
                    MagikeMaxBase = 50;
                    break;
                case MagikeApparatusLevel.Crimson:
                    break;
                case MagikeApparatusLevel.Corruption:
                    break;
                case MagikeApparatusLevel.Icicle:
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    break;
                case MagikeApparatusLevel.Feather:
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    break;
                default:
                    break;
            }
        }
    }

    public class BasicRefractorSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MaxConnectBase = 1;

            switch (incomeLevel)
            {
                case MagikeApparatusLevel.None:
                    //MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.MagicCrystal:
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.Crimson:
                    break;
                case MagikeApparatusLevel.Corruption:
                    break;
                case MagikeApparatusLevel.Icicle:
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    break;
                case MagikeApparatusLevel.Feather:
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    break;
                default:
                    break;
            }
        }
    }
}
