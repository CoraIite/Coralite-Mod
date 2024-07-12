﻿using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class BasaltChest : BaseChestItem
    {
        public BasaltChest() : base(Item.sellPrice(0, 0, 0, 10), ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<BasaltChestTile>(), AssetDirectory.MagikeSeries1Item)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient<Basalt>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
