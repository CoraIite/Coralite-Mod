using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.GlobalItems;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IceAxe : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BloodLustCluster);
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleScale>()
                .AddIngredient<IcicleBreath>()
                .AddTile(TileID.IceMachine)
                .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int index = Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.FrostStaff);
            Main.dust[index].noGravity = true;
        }
    }
}
