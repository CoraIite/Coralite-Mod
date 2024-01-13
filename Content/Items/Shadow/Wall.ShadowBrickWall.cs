using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowBrickWall:ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.ShadowCastle.ShadowBrickWall>());
        }
    }
}
