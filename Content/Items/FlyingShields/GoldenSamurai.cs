using Coralite.Content.Items.Materials;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class GoldenSamurai : BaseFlyingShieldItem<GoldenSamuraiGuard>
    {
        public GoldenSamurai() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Orange, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<GoldenSamuraiProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 32;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RemainsOfSamurai>()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<RemainsOfSamurai>()
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldenSamuraiProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GoldenSamurai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 20;
            backTime = 6;
            backSpeed = 14;
            trailCachesLength = 6;
            trailWidth = 20 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(238, 202, 158) * factor;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(State != (int)FlyingShieldStates.JustHited&& State != (int)FlyingShieldStates.Backing)
                SpawnGoldStrike(target, hit, damageDone);

            base.OnHitNPC(target, hit, damageDone);
        }

        public void SpawnGoldStrike(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float dir = (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFromList(-0.6f, 0.5f, 0.3f, -0.15f);

            Vector2 direction = dir.ToRotationVector2();

            float length1 = Main.rand.Next(16 * 8, 16 * 12);
            int damage1 = (int)(Projectile.damage * 0.6f);

            Projectile.NewProjectileFromThis<GoldenSamuraiStrike>(target.Center - direction * length1, direction * length1 * 2 / (6 + 4), damage1, Projectile.knockBack * 0.3f, 4, 0.5f);

            dir += Main.rand.NextFromList(-0.6f, 0.5f);
            direction = dir.ToRotationVector2();

            length1 = Main.rand.Next(16 * 4, 16 * 8);
            damage1 = (int)(Projectile.damage * 0.3f);
            Projectile.NewProjectileFromThis<GoldenSamuraiStrike>(target.Center - direction * length1, direction * length1 * 2 / (6 + 8), damage1, Projectile.knockBack * 0.3f, 8, 0.2f);
        }
    }

    public class GoldenSamuraiGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GoldenSamurai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 54;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.1f;
        }
    }

    /// <summary>
    /// ai0传入移动时间，ai1传入宽度
    /// </summary>
    public class GoldenSamuraiStrike:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float MaxTime => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float Alpha => ref Projectile.localAI[1];
        public Vector2 center;
        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.DamageType = DamageClass.Melee;
        }

        public void Initialize()
        {
            center = Projectile.Center;
            Timer = (int)MaxTime + 12;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }

            if (Timer < 6)
            {
                Alpha -= 1 / 6f;
                Width *= 0.8f;
            }
            else
            {
                if (Alpha < 1)
                {
                    Alpha += 1 / 6f;
                    if (Alpha > 1)
                    {
                        Alpha = 1;
                    }
                }

                if (Timer > 8)
                {
                    if (Main.rand.NextBool(4))
                    {
                        Particle particle = PRTLoader.NewParticle<HorizontalStar>(Projectile.Center + Main.rand.NextVector2CircularEdge(8, 8), Projectile.velocity * Main.rand.NextFloat(-0.05f, 0.05f), Color.Gold, Main.rand.NextFloat(0.1f, 0.1f));
                        particle.Rotation = 1.57f;
                    }

                    Projectile.SpawnTrailDust(DustID.GoldCoin, Main.rand.NextFloat(-0.2f, 0.2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                }
            }

            Timer--;
            if (Timer<0)
            {
                Projectile.Kill();
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return Timer >= 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer < 6)
                return false;

            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, Projectile.Center, Width*Projectile.width, ref a);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer > 12)
                return false;

            Texture2D mainTex = CoraliteAssets.Sparkle.ShotLineSPA.Value;
            float length = (Projectile.Center - center).Length();
            Vector2 scale = new(length / mainTex.Width, Width);

            Vector2 position = (Projectile.Center + center) / 2 - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, position, null, Color.Gold * Alpha, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

            mainTex = CoraliteAssets.Trail.Meteor.Value;
            scale = new(length / mainTex.Width, Width*2.5f);
            origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, position, null, (Color.LightGoldenrodYellow * Alpha) with { A = 0 }, Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0);

            return false;
        }
    }
}
