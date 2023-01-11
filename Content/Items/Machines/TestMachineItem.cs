using Coralite.Content.Tiles.Machines;
using Coralite.Core;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Machines
{
    public class TestMachineItem:ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;
        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.width = 16;
            Item.height = 16;
            Item.placeStyle = 0;
            Item.maxStack = 1;
            Item.rare = 10;
            Item.createTile = ModContent.TileType<testMachine>();
        }
    }
}
