using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class GlazeBulwark : BaseFlyingShieldItem<GlazeBulwarkGuard>
    {
        public GlazeBulwark() : base(Item.sellPrice(0, 0, 80), ItemRarityID.Green, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<GlazeBulwarkProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 13.5f;
            Item.damage = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlassShield>()
                .AddIngredient(ItemID.Hellstone, 5)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }

    public class GlazeBulwarkProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GlazeBulwark";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 38;
        }

        public override void SetOtherValues()
        {
            flyingTime = 19;
            backTime = 6;
            backSpeed = 13.5f;
            trailCachesLength = 6;
        }
    }

    public class GlazeBulwarkGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GlazeBulwark";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.scale = 1.1f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 4;
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileFromThis<GlazeBulwarkEXProj>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(4, 8), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Main.rand.Next(4));
            }
        }
    }

    public class GlazeBulwarkEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GlazeBulwarkProj";

        float alpha = 1;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = (int)Projectile.ai[0];
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60)
            {
                alpha -= 1 / 60f;
            }

            if (Projectile.velocity.Y < 8)
            {
                Projectile.velocity.Y += 0.25f;
            }

            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += Projectile.velocity.X / 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Glass, Helpers.Helper.NextVec2Dir(1, 4));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(4, 1, Projectile.frame, 0);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor * alpha, Projectile.rotation, frameBox.Size() / 2,
                Projectile.scale, 0, 0);

            return false;
        }
    }
}
