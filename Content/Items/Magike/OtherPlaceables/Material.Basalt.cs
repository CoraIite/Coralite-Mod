﻿using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class Basalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BasaltTile>());
        }
    }
}
