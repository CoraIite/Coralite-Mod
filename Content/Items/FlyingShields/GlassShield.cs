using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class GlassShield : BaseFlyingShieldItem<GlassShieldGuard>
    {
        public GlassShield() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.White, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 26;
            Item.shoot = ModContent.ProjectileType<GlassShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12;
            Item.damage = 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Glass, 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GlassShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GlassShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 26;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 4;
            backSpeed = 12;
            trailCachesLength = 6;
        }

        public void BrokenSound()
            => Helper.PlayPitched(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center, volume: 0.2f);

        public void Broken(Vector2 dir)
        {
            BrokenSound();

            int damage = (int)(Projectile.damage * 0.5f);
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileFromThis<GlassShatter>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(4, 8), damage, Projectile.knockBack);
            }

            Projectile.Kill();

            int count = Main.rand.Next(3, 6);
            for (int i = 0; i < count; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Glass, Helper.NextVec2Dir(1, 4));
            }
        }

        public override void TurnToBack()
        {
            Broken(Projectile.velocity.SafeNormalize(Vector2.Zero));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Broken(Projectile.velocity.SafeNormalize(Vector2.Zero));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Broken(-oldVelocity.SafeNormalize(Vector2.Zero));

            return true;
        }
    }

    public class GlassShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GlassShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 26;
            Projectile.height = 30;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            int damage = (int)(Projectile.damage * 0.75f);
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileFromThis<GlassShatter>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(4, 8), damage, Projectile.knockBack);
            }
        }
    }

    public class GlassShatter : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;
        
        float alpha = 1;
        private bool span = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 60+20;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[2]==1)
            {
                return false;
            }
            return null;
        }

        public void Initialize()
        {
            Projectile.frame = Main.rand.Next(4);
        }

        public override void AI()
        {
            if (span)
            {
                Initialize();
                span = false;
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60)
            {
                alpha -= 1 / 20f;
            }

            if (Projectile.shimmerWet)
            {
                if (Projectile.velocity.Y > -12)
                    Projectile.velocity.Y -= 0.9f;
            }
            else if (Projectile.velocity.Y < 8)
                Projectile.velocity.Y += 0.25f;

            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += Projectile.velocity.X / 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 1;
            if (Projectile.localAI[0] < 60)
            {
                Projectile.localAI[0] = 60;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var box = new Rectangle(Projectile.frame, 0, 4, 1);
            Projectile.QuickFrameDraw(box, lightColor * alpha, 0);
            Projectile.QuickFrameDraw(box, Color.White * 0.4f * alpha, 0);

            return false;
        }
    }
}
