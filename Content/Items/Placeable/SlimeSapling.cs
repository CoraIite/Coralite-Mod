﻿using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Placeable
{
    public class SlimeSapling : ModItem
    {
        public override string Texture => AssetDirectory.Placeable + Name;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.noMelee = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.createTile = ModContent.TileType<Tiles.Trees.SlimeSapling>();
        }
    }
}
