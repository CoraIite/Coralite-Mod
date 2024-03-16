using Coralite.Core;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowBrickWall : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.ShadowCastle.ShadowBrickWall>());
        }
    }
}
