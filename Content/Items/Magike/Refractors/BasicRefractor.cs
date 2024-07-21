using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Components;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicRefractor : BaseMagikePlaceableItem
    {
        public BasicRefractor() : base(TileType<CrystalRefractorTile>(), Item.sellPrice(0, 0, 5, 0)
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
    }

    public class BasicRefractorTileEntity : BaseRefractorTileEntity<BasicRefractorTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicRefractorContainer();

        public override MagikeLinerSender GetStartSender()
        {
            return null;
        }
    }

    public class BasicRefractorContainer : UpgradeableContainer
    {
        public override void Initialize()
        {
            MagikeMaxBase = 0;
        }

        public override bool CanUpgrade(MagikeApparatusLevel incomeLevel)
        {
            return incomeLevel switch
            {
                MagikeApparatusLevel.MagicCrystal
                or MagikeApparatusLevel.Crimson
                or MagikeApparatusLevel.Corruption
                or MagikeApparatusLevel.Icicle
                or MagikeApparatusLevel.CrystallineMagike
                or MagikeApparatusLevel.Feather
                or MagikeApparatusLevel.SplendorMagicore => true,
                _ => false,
            };
        }

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                case MagikeApparatusLevel.None:
                    break;
                case MagikeApparatusLevel.MagicCrystal:

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
