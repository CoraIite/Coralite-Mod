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

namespace Coralite.Content.Items.Magike.Pedestal
{
    public class BasicPedestal() : MagikeApparatusItem(TileType<BasicPedestalTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikePedestals)
    {
        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicPedestalTile() : BasePedestalTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikePedestalTiles + Name;
        public override int DropItemType => ItemType<BasicPedestal>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicPedestalTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.Glistent,
                MagikeApparatusLevel.Crimson,
                MagikeApparatusLevel.Corruption,
                MagikeApparatusLevel.Icicle,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Hallow,
                MagikeApparatusLevel.Soul,
                MagikeApparatusLevel.Feather,
                MagikeApparatusLevel.HolyLight,
                MagikeApparatusLevel.SplendorMagicore,
                MagikeApparatusLevel.Hellstone,
                MagikeApparatusLevel.EternalFlame,
                MagikeApparatusLevel.Quicksand,
                MagikeApparatusLevel.Forbidden,
                MagikeApparatusLevel.Eiderdown,
                MagikeApparatusLevel.Flight,
                MagikeApparatusLevel.Seashore,
                MagikeApparatusLevel.Pelagic,
                MagikeApparatusLevel.RedJade,
            ];
        }
    }

    public class BasicPedestalTileEntity : Pedestal<BasicPedestalTile>
    {
        public override ItemContainer GetStartItemContainer()
            => new UpgradeableItemContainer()
            {
                CapacityBase = 1
            };
    }
}
