using Terraria;

namespace Coralite.Core.Prefabs.Items
{
    public class BaseBannerItem(int value, int rare, int createTile, string texturePath=AssetDirectory.Banner, bool pathHasName = false) : BasePlaceableItem(value, rare, createTile, texturePath, pathHasName)
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
