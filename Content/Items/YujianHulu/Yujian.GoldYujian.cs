using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class GoldYujian : BaseYujian
    {
        public GoldYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 18, 0), 10, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<GoldYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.Trails + "LiteSlash";

        public GoldYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(80,20,35,180,0.95f),
                new YujianAI_PreciseSlash(startTime: 100,
                    slashWidth: 80,
                    slashTime: 70,
                    startAngle: -2f,
                    totalAngle: 3f,
                    turnSpeed: 2.2f,
                    roughlyVelocity: 0.9f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    Coralite.Instance.HeavySmootherInstance),
            },
            new YujianAI_GlodenSpurt(50, 12, 25, 180, 0.92f),
            PowerfulAttackCost: 150,
            attackLength: 390,
            width: 30, height: 58,
            Color.Gold, Color.Gold,
            trailCacheLength: 18
            )
        { }
    }

    public class YujianAI_GlodenSpurt : YujianAI
    {
        /// <summary> 大于此时间时为准备阶段 </summary>
        public readonly int firstPhaseTime;
        /// <summary> 大于此时间时为突刺阶段，小于此时间为休息阶段 </summary>
        public readonly int SecondPhaseTime;
        public readonly int distanceToKeep;
        public readonly float slowdownFactor;

        public Vector2 targetCenter;
        public float targetDir;

        /// <summary>
        /// 只需要输入对应的3个时间，内部将自动计算timer
        /// </summary>
        /// <param name="readyTime">准备时间</param>
        /// <param name="spurtTime">突刺时间</param>
        /// <param name="restTime">休息时间</param>
        public YujianAI_GlodenSpurt(int readyTime, int spurtTime, int restTime, int distanceToKeep, float slowdownFactor)
        {
            StartTime = readyTime + spurtTime + readyTime;
            firstPhaseTime = spurtTime + restTime;
            SecondPhaseTime = restTime;
            this.distanceToKeep = distanceToKeep;
            this.slowdownFactor = slowdownFactor;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            if (yujianProj.Timer > firstPhaseTime)  //准备阶段
            {
                //从1到0
                float factor = (yujianProj.Timer - firstPhaseTime) / (StartTime - firstPhaseTime);

                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetVector = targetCenter - Projectile.Center;
                Vector2 targetDirection = targetVector.SafeNormalize(Vector2.Zero);
                float targetAngle = targetDirection.ToRotation();

                Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle + 1.57f, 0.04f);
                float length = targetVector.Length();

                if (length > distanceToKeep + 20)
                    Projectile.velocity = ((Projectile.velocity * 20f) + (targetDirection * 2)) / 21f;
                else if (length < distanceToKeep - 20)
                    Projectile.velocity = ((Projectile.velocity * 20f) + (targetDirection * -2)) / 21f;
                else
                    Projectile.velocity *= slowdownFactor;

                Projectile.velocity += targetDirection.RotatedBy(1.57f) * factor * 0.2f;

                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + (distanceToKeep * 0.3f)) / spurtTime;
                Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * speed;

                Projectile.rotation = (targetCenter - Projectile.Center).ToRotation() + 1.57f;

                Projectile.tileCollide = false;

                yujianProj.InitTrailCaches();

                if (Projectile.IsOwnedByLocalPlayer())
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<GlodenSpurtProj>(),
                        Projectile.damage * 2, Projectile.knockBack, Projectile.owner, spurtTime / (Projectile.extraUpdates + 1), 32);
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();
                return;
            }

            if (yujianProj.Timer == SecondPhaseTime)
                Projectile.tileCollide = yujianProj.TileCollide;

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if (yujianProj.Timer > firstPhaseTime)
                return;

            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.6f + (yujianProj.trailCacheLength * 0.015f);

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 3)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + (i * 4);
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - (i * 0.015f), SpriteEffects.None, 0f);
            }
        }
    }

    public class GlodenSpurtProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float maxTime => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.localAI[1];
        public Vector2 center;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        private bool span;
        public void Initialize()
        {
            center = Projectile.Center;
        }

        public override void AI()
        {
            if (span)
            {
                Initialize();
                span = true;
            }
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localNPCHitCooldown = (int)maxTime + 12;
                Projectile.timeLeft = (int)maxTime + 12;
                Projectile.localAI[0] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            }

            if (Projectile.timeLeft < 6)
                Alpha -= 1 / 6f;
            else if (Projectile.timeLeft < 12)
                Alpha += 1 / 6f;
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Particle particle = PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2CircularEdge(16, 16) + (i * Projectile.velocity), Vector2.Zero, CoraliteContent.ParticleType<HorizontalStar>(), Color.Gold, Main.rand.NextFloat(0.1f, 0.15f));
                    particle.Rotation = 1.57f;
                }
            }

        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft > 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 12)
                return false;

            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, Projectile.Center, Width, ref a);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 12)
                return false;

            Texture2D mainTex = TextureAssets.Extra[98].Value;
            Vector2 scale = new(Width / mainTex.Width, (Projectile.Center - center).Length() / mainTex.Height);

            Main.spriteBatch.Draw(mainTex, center - Projectile.velocity - Main.screenPosition, null, Color.Gold * Alpha, Projectile.rotation + 3.141f, new Vector2(mainTex.Width / 2, 0), scale, SpriteEffects.None, 0);

            return false;
        }

    }
}
