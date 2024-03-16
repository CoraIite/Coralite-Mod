using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class ANewBirthOfIce : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ANewBirthOfIceTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }
    }

    public class ANewBirthOfIceTile : ModTile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            this.PaintingPrefab(3, 2, Coralite.Instance.IcicleCyan, DustID.Frost);
        }
    }
}
