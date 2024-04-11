using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Items.FlyingShields
{
    public class VampireEarl : BaseFlyingShieldItem<VampireEarlGuard>
    {
        public VampireEarl() : base(Item.sellPrice(0, 15), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<VampireEarlProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 20/2;
            Item.damage = 110;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VampiresFang>()
                .AddIngredient(ItemID.LunarBar, 4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class VampireEarlProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "VampireEarl";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 1;
        }

        public override void SetOtherValues()
        {
            flyingTime = 30 * 2;
            backTime = 22 * 2;
            backSpeed = 30 / 2;
            trailCachesLength = 16;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
            Projectile.SpawnTrailDust(14f, DustID.CrimsonTorch, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(1, 1.5f));

            if (Timer > flyingTime * 0.3f && Timer % (flyingTime / 3) == 0)
            {
                //射流星
                //Projectile.NewProjectileFromThis<VampireEarlMeteor>(Projectile.Center
                //    , (Projectile.extraUpdates + 1) * Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(0.8f, 1.2f),
                //    (int)(Projectile.damage * 0.75f), Projectile.knockBack);
            }
        }

        public override void OnBackDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
            Projectile.SpawnTrailDust(14f, DustID.CrimsonTorch, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(1f, 1.5f));
        }

        public override Color GetColor(float factor)
        {
            return Color.Red * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Color c = Color.Red;
            c.A = 0;

            for (int i = trailCachesLength - 1; i > 10; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                c * 0.5f * ((trailCachesLength-i) * 1f / (trailCachesLength-10)), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale * ( i * 1f/trailCachesLength), 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.3f, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale * 1.15f, 0, 0);
        }
    }

    public class VampireEarlGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "VampireEarl";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 62;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.35f;
            scalePercent = 1.7f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            //int num4 = Projectile.NewProjectileFromThis(Owner.Center, Vector2.Zero, 608, Projectile.damage, 15f);
            //Main.projectile[num4].netUpdate = true;
            //Main.projectile[num4].Kill();
        }
    }
}
