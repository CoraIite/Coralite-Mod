using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class WindrangerQuiver() : BaseAccessory(ItemRarityID.Yellow, Item.sellPrice(0, 8))
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WindSpeedArrows>()
                .AddIngredient(ItemID.MagicQuiver)
                .AddIngredient(ItemID.CrossNecklace)
                .AddIngredient(ItemID.Ectoplasm, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.longInvince = true;
            player.magicQuiver = true;
            player.moveSpeed += 0.08f;

            float speed = player.velocity.Length();
            //Main.NewText(speed);

            speed = Math.Clamp(speed, 0, 25);

            player.arrowDamage += speed * 0.012f;
        }
    }
}
