using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Glove
{
    public class StrokeGlove : BaseGloveItem
    {
        public override int CatchPower => 3;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<StrokeGloveProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(12, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 30));
            Item.UseSound = CoraliteSoundID.Swing_Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 2)
                .AddIngredient(ItemID.TinBar, 5)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Leather, 2)
                .AddIngredient(ItemID.CopperBar, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class StrokeGloveProj() : BaseGloveProj(1.4f)
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + "StrokeGlove";
        public override void SetOtherDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 54;
            DistanceController = (-15, 26);
            OffsetAngle = 0.2f;
            MaxTime = 21;
            BackTimeMax = 15;
            smoother = Coralite.Instance.BezierEaseSmoother;
        }
    }
}
