using Coralite.Content.Items.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class WindSpeedArrows() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 5))
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddIngredient(ItemID.SoulofSight)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.06f;

            float speed = player.velocity.Length();
            //Main.NewText(speed);

            speed = Math.Clamp(speed, 0, 20);

            player.GetDamage(DamageClass.Ranged) += speed * 0.01f;
        }
    }
}
