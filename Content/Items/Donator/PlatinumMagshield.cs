using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Donator
{
    public class PlatinumMagshield : BaseFlyingShieldItem<PlatinumMagshieldGuard>, IMagikeCraftable
    {
        public PlatinumMagshield() : base(Item.sellPrice(0, 0, 40), ModContent.RarityType<MagicCrystalRarity>(), AssetDirectory.Donator)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<PlatinumMagshieldProj>();
            Item.knockBack = 3;
            Item.shootSpeed = 11f;
            Item.damage = 20;
            Item.GetMagikeItem().MagikeMax = 500;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.PlatinumOre, ModContent.ItemType<PlatinumMagshield>(), MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 3, 60 * 3)
                , 20)
                .AddIngredient<MagicCrystal>()
                .Register();
        }
    }

    public class PlatinumMagshieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.Donator + "PlatinumMagshield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
        }

        public override void SetOtherValues()
        {
            flyingTime = 22;
            backTime = 4;
            backSpeed = 12;
            trailCachesLength = 5;
            trailWidth = 24 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Helper.PlayPitched(CoraliteSoundID.Metal_NPCHit4, Projectile.Center, pitch: 0f);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6), DustID.CrystalSerpent_Pink
                    , Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 5), Scale: 1f);
                d.noGravity = true;
            }
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6), DustID.CrystalSerpent_Pink
                    , Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 5), Scale: 1f);
                d.noGravity = true;
            }
        }

        public override Color GetColor(float factor)
        {
            return Color.Silver * factor;
        }
    }

    public class PlatinumMagshieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.3f;
            distanceAdder = 3;
            parryTime = 12;
        }

        public override void OnParry()
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 15);
                    Owner.immune = true;
                }

                if (cp.parryTime < 250)
                    cp.parryTime += 80;
            }

            Helper.PlayPitched("Misc/ShieldGuard", 0.3f, 0f, Projectile.Center);

            float projSpeed = 4;
            float particleBaseSpeed = 0.5f;
            float particleSpeedAdder = 0.4f;
            int damage = 1;

            if (MagikeHelper.TryCosumeMagike(3, Item, Owner) || Owner.CheckMana(80, true, true))
            {
                projSpeed = 12;
                particleBaseSpeed = 3;
                particleSpeedAdder = 1;
                damage = 5;

                Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Projectile.Center, pitch: -0.5f);
                Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, Projectile.Center, pitch: 0.7f);
            }

            LightCiecleParticle.Spawn(Projectile.Center, Coralite.MagicCrystalPink * 0.5f, 0.2f, Projectile.rotation
                , new Vector2(0.25f + Main.rand.NextFloat(-0.1f, 0.1f), 0.6f), new Color(108, 19, 58, 0));

            float rot = Projectile.rotation + Main.rand.NextFloat(-0.6f, 0.6f);

            Color c = Coralite.MagicCrystalPink;
            for (int i = 0; i < 8; i++)
            {
                float angle = 0.45f - i * 0.4f / 8;
                rot = Main.rand.NextFloat(-angle, angle);

                Color color = i == 7 ? c : Main.rand.NextFromList(c, new Color(211, 103, 156), Color.Pink);
                MagikeFlowLine.Spawn(Projectile.Center, (Projectile.rotation + rot).ToRotationVector2() * (particleBaseSpeed + i * particleSpeedAdder)
                    , Main.rand.Next(1, 3), Main.rand.Next(14, 25), Main.rand.Next(8, 16), Main.rand.NextFloat(0.4f, 0.6f)
                    , color);
            }

            for (int i = -1; i < 2; i++)
            {
                rot = i * 0.4f + Main.rand.NextFloat(-0.2f, 0.2f);
                LightShotParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(8, 8), Color.DeepPink, rot + Projectile.rotation
                    , new Vector2(Main.rand.NextFloat(0.4f, 0.7f) * Helper.EllipticalEase(rot, 1, 0.4f)
                    , 0.03f));
            }

            if (Projectile.IsOwnedByLocalPlayer())
                Projectile.NewProjectileFromThis<PlatinumMagshieldParry>(Projectile.Center, Projectile.rotation.ToRotationVector2() * projSpeed
                    , Projectile.damage * damage, 6);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(2, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(2, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class PlatinumMagshieldParry : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), ModContent.DustType<PixelPoint>()
                    , Projectile.velocity * Main.rand.NextFloat(0.9f, 1.6f), newColor: Coralite.MagicCrystalPink, Scale: Main.rand.NextFloat(1, 2f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class MagikeFlowLine : TrailParticle
    {
        public override string Texture => AssetDirectory.Blank;

        private static BasicEffect effect;
        private int liveTime;
        private int trailCount;
        private float rotate;
        private int recordDir;
        private int RotateTimer;

        public MagikeFlowLine()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetProperty()
        {
        }

        public override void AI()
        {
            if (Opacity < 0)
            {
                Color *= 0.88f;
                Velocity *= 0.85f;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 p = Vector2.Lerp(Position, oldPositions[^1], i / 3f);
                    Dust d = Dust.NewDustPerfect(p, DustID.PinkTorch, Vector2.Zero, 150, Color, 0.75f);
                    d.noGravity = true;
                }
                RotateTimer--;
                if (RotateTimer == 0)
                {
                    if (recordDir == 0)
                    {
                        RotateTimer = Main.rand.Next(3, 5);
                        recordDir = Main.rand.NextFromList(-1, 1);

                        Velocity = Velocity.RotatedBy(recordDir * rotate);
                    }
                    else
                    {
                        RotateTimer = Main.rand.Next(6, 8);
                        Velocity = Velocity.RotatedBy(-recordDir * rotate);

                        recordDir = 0;
                    }
                }
            }

            UpdatePositionCache(trailCount);
            trail.TrailPositions = oldPositions;

            if (Opacity < -60 || Color.A < 10)
                active = false;

            Opacity -= 1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public override void DrawPrimitive()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.DrawTrail(effect);
        }

        public static void Spawn(Vector2 center, Vector2 velocity, float trailWidth, int liveTime, int trailCount, float rotate, Color color = default)
        {
            if (VaultUtils.isServer)
                return;

            MagikeFlowLine particle = PRTLoader.NewParticle<MagikeFlowLine>(center, velocity, color, 1f);
            if (particle != null)
            {
                particle.Opacity = liveTime;
                particle.InitializePositionCache(trailCount);
                particle.trail = new Trail(Main.instance.GraphicsDevice, trailCount, new ArrowheadTrailGenerator(trailWidth * 2), factor => trailWidth, factor =>
                {
                    return Color.Lerp(new Color(0, 0, 0, 0), particle.Color, factor.X);
                });

                particle.liveTime = liveTime;
                particle.trailCount = trailCount;
                particle.rotate = rotate;
                particle.RotateTimer = Main.rand.Next(6, 8);
            }
        }
    }
}
