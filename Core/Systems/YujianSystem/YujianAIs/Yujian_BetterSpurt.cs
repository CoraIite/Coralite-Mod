using System.IO.Compression;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class Yujian_BetterSpurt : YujianAI
    {
        /// <summary> 大于此时间时为准备阶段 </summary>
        public readonly int firstPhaseTime;
        /// <summary> 大于此时间时为突刺阶段，小于此时间为休息阶段 </summary>
        public readonly int SecondPhaseTime;

        public Vector2 targetCenter;

        public Yujian_BetterSpurt(int readyTime, int spurtTime, int restTime)
        {
            StartTime = readyTime + spurtTime + readyTime;
            firstPhaseTime = spurtTime + restTime;
            SecondPhaseTime = restTime;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            if (yujianProj.Timer > firstPhaseTime)
            {
                Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
                Vector2 targetDirection = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
                float targetAngle = targetDirection.ToRotation();

                Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle + 1.57f, 0.001f);

                return;
            }

            if (yujianProj.Timer > firstPhaseTime)
            {
                targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
            }

            if (yujianProj.Timer > SecondPhaseTime)
            {
                return;
            }


        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if (yujianProj.Timer < StartTime || yujianProj.Timer > SecondPhaseTime)
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
                int a = 20 + i * 6;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }
    }
}