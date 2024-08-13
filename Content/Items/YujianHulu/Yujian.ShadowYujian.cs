using Coralite.Content.Items.Shadow;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class ShadowYujian : BaseYujian
    {
        public ShadowYujian() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20, 0), 12, 1.5f) { }

        public override int ProjType => ModContent.ProjectileType<ShadowYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowCrystal>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShadowYujianProj : BaseYujianProj
    {
        public ShadowYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(60, 16, 30, 180, 0.95f),
                 new YujianAI_ShadowSlash(),
            },
            new YujianAI_ShadowSummon(),
            PowerfulAttackCost: 100,
            attackLength: 440,
            width: 30, height: 60,
            Color.MediumPurple, Color.Purple,
            trailCacheLength: 18,
            yujianAIsRandom: new int[]
            {
                5,2
            }
            )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            //target.StrikeNPC(damage, );
            //target.StrikeNPC(damage, 0, target.direction);
            //TODO: 把它的3连击加回来
            //TODO: 为多人模式适配，添加联机信息发送
            Owner.addDPS(damage * 2);
        }
    }

    public class YujianAI_ShadowSlash : YujianAI_BaseSlash
    {
        private bool canSlash = false;
        protected bool doubleSlash = true;
        private Trail trail;

        public YujianAI_ShadowSlash() : base(90, 55, 40, -2f, 4f, 2.3f, 0.8f, 1f, 1f, Coralite.Instance.NoSmootherInstance) { }

        public void Reset()
        {
            StartTime = 80;

            SlashTime = 70;
            StartAngle = 2f;

            halfShortAxis = 1f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public void Init()
        {
            StartTime = 85;

            SlashTime = 35;
            StartAngle = -2f;

            halfShortAxis = 1f;
            halfLongAxis = 2f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //先尝试接近NPC，距离小于一定值后开始斩击
            if (canSlash)
            {
                //斩击AI
                int time = StartTime - (int)yujianProj.Timer;
                if (time < 3)
                    yujianProj.InitTrailCaches();

                if (time < SlashTime)
                {
                    Slash(Projectile, time);
                    return;
                }

                if (time == SlashTime)
                {
                    if (doubleSlash)
                    {
                        Reset();
                        doubleSlash = false;
                        yujianProj.Timer = StartTime;
                        startElliptical = Helper.EllipticalEase(StartAngle, halfShortAxis, halfLongAxis);
                        //重设中心点以及角度和拖尾数组
                        Vector2 slashCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                        Projectile.rotation = targetRotation + StartAngle;
                        Projectile.Center = slashCenter + Projectile.rotation.ToRotationVector2() * SlashWidth;
                        Projectile.rotation += 1.57f;

                        yujianProj.InitTrailCaches();
                        trail?.SetVertical(StartAngle < 0);      //开始角度为正时设为false

                        return;
                    }

                    canDamage = false;
                    AfterSlash(Projectile);
                }

                return;
            }

            TryGetClosed2Target(yujianProj, out float distance, out _);

            if (distance < SlashWidth * 2 && distance > SlashWidth * 1.8f)      //瞬移并生成2次粒子
            {
                canSlash = true;
                canDamage = true;
                SpawnShadowDust(Projectile);

                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.Center = targetCenter + targetDir * distance;
                Projectile.rotation = targetDir.ToRotation() + 1.57f;

                Vector2 slashCenter = Projectile.Center - (Projectile.rotation - 1.57f).ToRotationVector2() * SlashWidth;
                Projectile.localAI[0] = slashCenter.X;
                Projectile.localAI[1] = slashCenter.Y;
                targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                targetRotation = targetDir.ToRotation();

                SpawnShadowDust(Projectile);

                yujianProj.InitTrailCaches();
                trail?.SetVertical(StartAngle < 0);      //开始角度为正时设为false
            }

        }

        private void SpawnShadowDust(Projectile Projectile)
        {
            float r = 0f;
            for (int i = 0; i < 14; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, r.ToRotationVector2() * Main.rand.NextFloat(1.6f, 2.5f), default, default, Main.rand.NextFloat(1.4f, 2f));
                dust.noGravity = true;
                r += 0.45f;
            }

            Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<HorizontalStar>(), Color.Purple, 0.2f);
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            yujianProj.Projectile.velocity += (yujianProj.Projectile.rotation - 1.57f).ToRotationVector2() * 0.02f;
            Init();
            doubleSlash = true;
            canDamage = false;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, yujianProj.Projectile.oldPos.Length, new NoTip(), factor => yujianProj.Projectile.height / 2,
            factor =>
            {
                return Color.Lerp(yujianProj.color1, yujianProj.color2, factor.X) * 0.8f;
            }, flipVertical: StartAngle < 0);

            trail.Positions = yujianProj.Projectile.oldPos;
            return canSlash;
        }

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || time < yujianProj.trailCacheLength || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            trail?.Render(effect);
        }

    }

    public class YujianAI_ShadowSummon : YujianAI
    {
        public YujianAI_ShadowSummon()
        {
            StartTime = 70;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile projectile = yujianProj.Projectile;
            if (yujianProj.Timer > 50)
            {
                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetVector = targetCenter - projectile.Center;
                Vector2 targetDirection = targetVector.SafeNormalize(Vector2.Zero);
                float targetAngle = targetDirection.ToRotation();

                projectile.rotation = projectile.rotation.AngleTowards(targetAngle + 1.57f, 0.18f);
                float length = targetVector.Length();

                if (length > 220)
                    projectile.velocity = (projectile.velocity * 20f + targetDirection * 2) / 21f;
                else if (length < 180)
                    projectile.velocity = (projectile.velocity * 20f + targetDirection * -2) / 21f;
                else
                    projectile.velocity *= 0.97f;

                projectile.velocity += targetDirection.RotatedBy(1.57f) * 0.2f;
            }
            else if (yujianProj.Timer == 50)
            {
                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                float rot = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center + (rot + i * 6.282f / 3).ToRotationVector2() * 64, Vector2.Zero,
                            ModContent.ProjectileType<ShadowYujianSummons>(), projectile.damage * 2, 0f, projectile.owner, targetCenter.X, targetCenter.Y);
                }

                for (int i = 0; i < 16; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Shadowflame, (i * 6.282f / 16).ToRotationVector2() * Main.rand.NextFloat(5f, 8f));
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, projectile.Center);
            }
            else
                projectile.velocity *= 0.93f;
        }
    }

    /// <summary>
    /// ai0和ai1分别存储目标位置的X和Y
    /// </summary>
    public class ShadowYujianSummons : ModProjectile
    {
        public override string Texture => AssetDirectory.YujianHulu + "ShadowYujian";

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Granite, (i * 6.282f / 16).ToRotationVector2() * Main.rand.NextFloat(5f, 8f));
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Vector2 targetCenter = new(Projectile.ai[0], Projectile.ai[1]);
            if (Timer == 0)
                Projectile.velocity = -(targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * 1.5f;
            else if (Timer < 15)
                Projectile.rotation += 0.25f;
            else if (Timer == 15)
            {
                Vector2 targetDir = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One);
                Vector2 truelyCenter = targetCenter + targetDir * 150;
                Projectile.velocity = targetDir * (truelyCenter - Projectile.Center).Length() / 15;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, -targetDir.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)));
                    dust.noGravity = true;
                }
            }
            else if (Timer > 30)
                Projectile.Kill();

            if (Timer % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.Granite);
                dust.noGravity = true;
            }

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(0.5f, 0.6f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f, Projectile.rotation - 1 / 57f, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
