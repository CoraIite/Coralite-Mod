using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class SpikeGelBall : GelBall
    {
        public override void OnKill(int timeLeft)
        {
            //生成粒子
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //生成一些尖刺弹幕
                int howMany = Helper.ScaleValueForDiffMode(3, 4, 5, 6);
                for (int i = 0; i < howMany; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height),
                        -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(10, 14), ModContent.ProjectileType<GelSpike>(), Projectile.damage, 0, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            float exRot = Projectile.whoAmI * MathHelper.PiOver2;

            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;

            Vector2 scale = Scale;

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Color color = new Color(50, 152 + (int)(100 * factor), 225);
            color *= Projectile.localAI[0] * 0.75f;

            //绘制影子拖尾
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 8; i += 2)
                DrawGelBall(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition
                    , color * (0.3f - (i * 0.03f)), Projectile.oldRot[i], Projectile.oldRot[i] + exRot + i * MathHelper.PiOver2, scale, false, highlightUseRot: true);

            //绘制自己
            DrawGelBall(mainTex, pos, lightColor * Projectile.localAI[0]
                , Projectile.rotation, exRot + Projectile.rotation, scale, true, true, color, highlightUseRot: true);

            return false;
        }
    }
}
