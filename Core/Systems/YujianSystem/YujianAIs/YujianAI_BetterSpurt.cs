using System.IO.Enumeration;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Coralite.Content.Items.YujianHulu;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class YujianAI_BetterSpurt : YujianAI
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
        public YujianAI_BetterSpurt(int readyTime, int spurtTime, int restTime,int distanceToKeep,float slowdownFactor)
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
                float factor = (yujianProj.Timer - firstPhaseTime) / (float)(StartTime - firstPhaseTime);

                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetVector = targetCenter - Projectile.Center;
                Vector2 targetDirection = targetVector.SafeNormalize(Vector2.Zero);
                float targetAngle = targetDirection.ToRotation();

                Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle + 1.57f, 0.04f);
                float length = targetVector.Length();

                if (length > distanceToKeep + 20)
                {
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * 2) / 21f;
                }
                else if (length < distanceToKeep - 20)
                {
                    Projectile.velocity = (Projectile.velocity * 20f + targetDirection * -2) / 21f;
                }
                else
                {
                    Projectile.velocity *= slowdownFactor;
                }

                Projectile.velocity += targetDirection.RotatedBy(1.57f) * factor * 0.2f;

                return;
            }

            if (yujianProj.Timer == firstPhaseTime)     //转为突刺阶段
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                int spurtTime = firstPhaseTime - SecondPhaseTime;
                float speed = (Vector2.Distance(targetCenter, Projectile.Center) + distanceToKeep * 0.3f) / spurtTime;
                Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * speed;

                Projectile.rotation = (targetCenter - Projectile.Center).ToRotation() + 1.57f;

                Projectile.tileCollide = false;

                yujianProj.InitTrailCaches();

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * (Projectile.extraUpdates + 1), ModContent.ProjectileType<SpurtProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner, spurtTime/ (Projectile.extraUpdates + 1));
                }
            }

            if (yujianProj.Timer > SecondPhaseTime)     //突刺阶段
            {
                yujianProj.UpdateCaches();
                return;
            }

            if (yujianProj.Timer==SecondPhaseTime)
            {
                Projectile.tileCollide = yujianProj.TileCollide;
            }

            //后摇休息阶段
            Projectile.velocity *= slowdownFactor;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if ( yujianProj.Timer > firstPhaseTime)
                return;

            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.6f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 3)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + i * 4;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }
    }
}