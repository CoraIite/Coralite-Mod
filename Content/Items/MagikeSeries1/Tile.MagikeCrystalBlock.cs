﻿using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalBlock : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagicCrystalBlockTile>());
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<MagicCrystal>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();
        }
    }
}
