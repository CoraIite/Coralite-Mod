using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.DashBowChapter;
using Coralite.Content.Items.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class WindSpeedArrows() : BaseAccessory(ItemRarityID.Pink, Item.sellPrice(0, 5)),IConsultableItem
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<DashBowKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<DashBowPage1>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddIngredient(ItemID.SoulofSight)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
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

            player.arrowDamage += speed * 0.01f;
        }
    }
}
