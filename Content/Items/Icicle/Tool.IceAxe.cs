using Coralite.Content.Items.GlobalItems;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IceAxe : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

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
