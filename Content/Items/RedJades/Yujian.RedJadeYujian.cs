using Coralite.Content.Items.YujianHulu;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeYujian : BaseYujian
    {
        public RedJadeYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 11, 1f, AssetDirectory.RedJadeItems) { }

        public override int ProjType => ModContent.ProjectileType<RedJadeYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeYujianProj : BaseYujianProj
    {
        public RedJadeYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(70,18,25,180,0.93f),
                 new YujianAI_Slash(startTime: 76,
                    slashWidth: 55,
                    slashTime: 36,
                    startAngle: -1.6f,
                    totalAngle: 1.84f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 4.5f,
                    Coralite.Instance.NoSmootherInstance),
                new YujianAI_Slash(startTime: 76,
                    slashWidth: 60,
                    slashTime: 36,
                    startAngle: -1.4f,
                    totalAngle: 1.7f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 3f,
                    Coralite.Instance.NoSmootherInstance),
            },
            new YujianAI_RedDash(75, 35, 30, 180, 0.96f),
            PowerfulAttackCost: 150,
            attackLength: 430,
            width: 30, height: 60,
            new Color(43, 43, 51), Coralite.RedJadeRed,
            trailCacheLength: 24,
            texturePath: AssetDirectory.RedJadeItems,
            yujianAIsRandom: new int[]
            {
                4,
                2,
                1
            }
            )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(5))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class YujianAI_RedDash : YujianAI
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
        public YujianAI_RedDash(int readyTime, int spurtTime, int restTime, int distanceToKeep, float slowdownFactor)
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
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * 2) / 21f;
                else if (length < distanceToKeep - 20)
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * -2) / 21f;
                else
                    Projectile.velocity *= slowdownFactor;

                Projectile.velocity += targetDirection.RotatedBy(1.57f) * factor * 0.2f;

                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + distanceToKeep * 0.6f) / spurtTime;
                Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * speed;
                Projectile.rotation = (targetCenter - Projectile.Center).ToRotation() + 1.57f;
                Projectile.tileCollide = false;

                yujianProj.InitTrailCaches();

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<SpurtProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, spurtTime / (Projectile.extraUpdates + 1), 32);

                Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.RedJadeRed, 0.8f);
                SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();
                Projectile.rotation += 0.4f;
                if (yujianProj.Timer % 10 == 0 && Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero,
                        ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                return;
            }

            if (yujianProj.Timer == SecondPhaseTime)
            {
                Projectile.tileCollide = yujianProj.TileCollide;
            }

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
            Projectile.rotation += 0.4f * (yujianProj.Timer / SecondPhaseTime);
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if (yujianProj.Timer > firstPhaseTime)
                return;

            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.6f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 2)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 60 + i * 2;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }

    }
}
