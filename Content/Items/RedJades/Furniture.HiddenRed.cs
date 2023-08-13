using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class HiddenRed : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HiddenRedTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }
    }

    public class HiddenRedTile : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            this.PaintingPrefab(2, 3, Coralite.Instance.RedJadeRed, DustID.GemRuby);
        }

    }
}
