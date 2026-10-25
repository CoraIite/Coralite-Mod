using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>由外向内收缩的吸收环，ai[0..2] 分别传入 0~255 的 RGB 颜色值。</summary>
    public class AbsorbProj : ModProjectile
    {
        public override string Texture => AssetDirectory.EoC + "FightingRoar";

        public ref float R => ref Projectile.ai[0];
        public ref float G => ref Projectile.ai[1];
        public ref float B => ref Projectile.ai[2];

        private const float InitialRadius = 600f;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90;
            Projectile.alpha = 0;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> vertices = CoraliteSystem.Vertexes;
            Color color = Projectile.GetAlpha(new Color((int)R, (int)G, (int)B)) * 0.75f;

            const int max = 40;
            for (int a = 0; a < 4; a++)
            {
                float arcRotation = MathHelper.TwoPi / 4f * a;
                for (int i = 0; i <= max; i++)
                {
                    Vector2 offset = Vector2.UnitX.RotatedBy(Projectile.rotation + arcRotation + MathHelper.PiOver2 / max * i) * Projectile.localAI[1];
                    Vector2 position = Projectile.Center + offset - Main.screenPosition;
                    Vector2 width = offset.SafeNormalize(Vector2.UnitX) * Projectile.localAI[1] * 1.5f;
                    // 翻转 V 坐标，让贴图方向与向内吸收的运动一致。
                    vertices.Add(new ColoredVertex(position + offset + width, new Vector3(i / (float)max, 1f, 1f), color));
                    vertices.Add(new ColoredVertex(position + offset - width, new Vector3(i / (float)max, 0f, 1f), color));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;

            if (vertices.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = InitialRadius;
                Projectile.netUpdate = true;
                Projectile.localAI[0] = 1;
            }

            // 反向执行原吼叫环的半径增长，从外圈收缩到中心。
            Projectile.localAI[1] /= 1.05f;
            Projectile.localAI[1] -= 3f;
            if (Projectile.localAI[1] <= 0f)
            {
                Projectile.localAI[1] = 0f;
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha > 255)
                    Projectile.Kill();
            }
        }
    }
}
