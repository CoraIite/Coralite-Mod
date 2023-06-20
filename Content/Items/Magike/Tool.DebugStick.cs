using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class DebugStick : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 20;
            Item.useTurn = true;

        }

        public override bool CanUseItem(Player player)
        {
            Point point = Main.MouseWorld.ToTileCoordinates();
            if (MagikeHelper.TryGetEntity(point.X, point.Y, out MagikeContainer magikeContainer))
                magikeContainer.Charge(magikeContainer.magikeMax);

            return true;
        }
    }
}
