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

        public override MagikeTP GetEntityInstance() => GetInstance<BasicPedestalTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.Crimson,
                MALevel.Corruption,
                MALevel.Icicle,
                MALevel.CrystallineMagike,
                MALevel.Hallow,
                MALevel.Soul,
                MALevel.Feather,
                MALevel.HolyLight,
                MALevel.SplendorMagicore,
                MALevel.Hellstone,
                MALevel.EternalFlame,
                MALevel.Quicksand,
                MALevel.Forbidden,
                MALevel.Eiderdown,
                MALevel.Flight,
                MALevel.Seashore,
                MALevel.Pelagic,
                MALevel.RedJade,
            ];
        }
    }

    public class BasicPedestalTileEntity : Pedestal<BasicPedestalTile>
    {
        public override ItemContainer GetStartItemContainer() => new UpgradeableItemContainer() { CapacityBase = 1 };
    }
}
