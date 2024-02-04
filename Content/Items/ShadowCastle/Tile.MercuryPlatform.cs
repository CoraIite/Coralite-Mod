﻿using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowCastle
{
    public class MercuryPlatform : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MercuryPlatformTile>());
        }
    }
}