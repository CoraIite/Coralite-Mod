using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class SpikeGelBall : GelBall
    {
        public override void SetSpecialBonus()
        {
            if (((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.SpilkeBallBonus_P1_2))
            {
                SpBonus = 1;
            }
            else if (((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.SpilkeBallBonus_P2_2))
            {
                SpBonus = 2;
            }
        }

        public override void SpecialAIBonus()
        {
            if (Projectile.localAI[0] < 1)
                return;

            Projectile.localAI[0]++;

            if (SpBonus == 1)
            {
                Helper.ChaseGradually(Projectile, Owner.MountedCenter, 6, 20, 21);

                if (Projectile.localAI[0] > 120 && Vector2.Distance(Owner.MountedCenter, Projectile.Center) < 600)
                    Projectile.Kill();
            }
            else if (SpBonus == 2)
            {
                if (Projectile.localAI[0] % 60 == 0)
                    SpawnSpikes();
            }
        }

        public override void OnKill(int timeLeft)
        {
            //生成粒子
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }

            //生成一些尖刺弹幕
            SpawnSpikes();
        }

        private void SpawnSpikes()
        {
            int howMany = Helper.ScaleValueForDiffMode(3, 4, 4, 5);
            for (int i = 0; i < howMany; i++)
            {
                Projectile.NewProjectileFromThis<GelSpike>(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height),
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(10, 14), Projectile.damage, 0);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var pos = Projectile.Center - Main.screenPosition;

            float exRot = Projectile.whoAmI * MathHelper.PiOver2;
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Color color = new Color(50, 152 + (int)(100 * factor), 225);
            float light = Projectile.localAI[0];
            if (light>1)
            {
                light = 1;
            }
            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;
            if (SpBonus == 1)
            {
                color = Color.Red;
                lightColor = new Color(255, 150, 150);
            }
            else if (SpBonus == 2)
            {
                color = new Color(255, 120, 255);
                lightColor = new Color(225, 150, 255);
            }

            Vector2 scale = Scale;

            color *= light * 0.75f;

            //绘制影子拖尾
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 8; i += 2)
                DrawGelBall(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition
                    , color * (0.3f - (i * 0.03f)), Projectile.oldRot[i], Projectile.oldRot[i] + exRot + i * MathHelper.PiOver2, scale, false, highlightUseRot: true);

            //绘制自己
            DrawGelBall(mainTex, pos, lightColor * light
                , Projectile.rotation, exRot + Projectile.rotation, scale, true, true, color, highlightUseRot: true);

            return false;
        }
    }
}
