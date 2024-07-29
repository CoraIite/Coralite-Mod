﻿using Coralite.Content.Items.Glistent;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniDemoniteColumn : MagikeChargeableItem
    {
        public MiniDemoniteColumn() : base(200, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniCrystalColumn>()
                .AddIngredient<GlistentBar>(2)
                .AddIngredient(ItemID.ShadowScale, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

