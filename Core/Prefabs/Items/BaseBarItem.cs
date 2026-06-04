using Terraria;
using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseBarItem<TTile>(int value, int rare, string texturePath, bool pathHasName = false) : BaseMaterial(Item.CommonMaxStack, value, rare, texturePath, pathHasName) where TTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<TTile>());
        }
    }
}
