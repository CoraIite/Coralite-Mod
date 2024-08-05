using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberHammer : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AshWoodHammer);
            Item.hammer = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.t_Slime, Alpha: 150, newColor: Color.Blue, Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
