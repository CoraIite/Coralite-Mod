using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class WoodenBowl : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 3;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<WoodenBowlProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 14;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(8, 3);
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 0, 50));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class WoodenBowlProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + "WoodenBowl";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 18;
        }

        public override void InitFields()
        {
            MaxChannelTime = 40;
            MaxFlyTime = 9;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.WoodFurniture, Helper.NextVec2Dir(0.3f, 1f));
        }

        public override void PostDrawSpecial(SpriteEffects effect, Color lightColor)
        {
            if (Catch == 1)
                return;

            float rot = Projectile.rotation - MathHelper.PiOver2;

            if (State == AIStates.Held)
                rot = Projectile.rotation - Owner.direction * MathHelper.PiOver2;

            DrawFairyItem(Projectile.Center + rot.ToRotationVector2() * 8 - Main.screenPosition
                , lightColor, effect);
        }
    }
}
