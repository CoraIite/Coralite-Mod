using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria.ModLoader;
using Terraria;
using Coralite.Content.Raritys;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class MagicCrystalBlock : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagicCrystalBlockTile>());
            Item.rare = ModContent.RarityType<MagikeCrystalRarity>();
        }

    }
}
