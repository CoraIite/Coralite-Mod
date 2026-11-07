using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarsGuider : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Lasers + "MultLines";

        private const float ArcHalfAngle = 0.7f;

        public ref float Style => ref Projectile.ai[0];
        public ref float Distance => ref Projectile.ai[1];

        public ref float LineWidth => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float LinePercent => ref Projectile.localAI[2];

        public float alpha = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Style == 0f)
                Projectile.rotation = (Owner.Center - Projectile.Center).ToRotation();

            Timer++;
            if (Timer < 30)
            {
                LinePercent = Helper.SqrtEase(Timer / 30f);
            }
            else if (Timer < 30 + 80)
            {
                LinePercent = 1;
            }
            else if (Timer < 30 + 80 + 30)
            {
                alpha = 1 - Helper.SqrtEase((Timer - 30 - 80) / 30f);
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Style == 0f && LinePercent > 1)
                DrawArc();

            return false;
        }

        private void DrawArc()
        {
            const int pointCounts = 60;

            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            Texture2D mainTex = Projectile.GetTextureValue();

            float per = 2f * (pointCounts + 1) / mainTex.Width;
            float centerAngle = (Owner.Center - Projectile.Center).ToRotation();
            float halfAngle = ArcHalfAngle * MathHelper.Clamp(LinePercent, 0f, 1f);
            float radius = Distance;

            for (int i = 0; i <= pointCounts; i++)
            {
                float factor = i / (float)pointCounts;
                float angle = centerAngle - halfAngle + (halfAngle * 2f * factor);
                Vector2 center = Projectile.Center + angle.ToRotationVector2() * radius;
                Vector2 tangent = (angle + MathHelper.PiOver2).ToRotationVector2();
                float f = MathF.Sin(factor * MathHelper.Pi);
                Vector2 width = tangent * LineWidth;
                Color color = Color.White * (0.5f * f * alpha);
                float u = (1f - factor) * per;

                bars.Add(new(center + width, color, new Vector2(u, 0f)));
                bars.Add(new(center - width, color, new Vector2(u, 1f)));
            }

            Effect effect = ShaderLoader.GetShader("ShadowLaser");
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1.5f + Projectile.whoAmI * 0.2143f);
            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(mainTex);
            effect.Parameters["gradientTexture"].SetValue(CoraliteAssets.Misc.White32x32.Value);
            effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
            effect.Parameters["warptTexture"].SetValue(CoraliteAssets.Distortion.Twist.Value);
            effect.Parameters["cOff"].SetValue(0.05f);

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                    bars.ToArray(), 0, bars.Count - 2);
            }
        }
    }
}