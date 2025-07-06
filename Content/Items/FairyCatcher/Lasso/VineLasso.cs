using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Lasso
{
    public class VineLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<VineLassoSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(8, 3);
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 20));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.VineRope, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class VineLassoSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "VineLassoCatcher";

        public override float GetShootRandAngle => Main.rand.NextFloat(-0.2f, 0.2f);

        public override void SetSwingProperty()
        {
            base.SetSwingProperty();
            DrawOriginOffsetX = 8;
        }

        public override Color GetStringColor(Vector2 pos)
        {
            Color c = Color.Green;
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            return c;
        }
    }
}
