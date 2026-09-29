using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    public class FightingRoar : ModProjectile
    {
        public override string Texture => AssetDirectory.EoC+Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.width =  Projectile.height = 16;
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
            List<ColoredVertex> Vertex= CoraliteSystem.Vertexes;

            int Max = 40;
            for (int a = 0; a < 4; a++)
            {
                float ARot = MathHelper.TwoPi / 4f * a;
                for (int i = 0; i <= Max; i++)
                {
                    Vector2 Vec = Vector2.UnitX.RotatedBy(Projectile.rotation + ARot + MathHelper.PiOver2 / Max * i) * Projectile.ai[1];
                    Vector2 Pos = Projectile.Center + Vec - Main.screenPosition;
                    Vector2 Rot = Vec.SafeNormalize(Vector2.UnitX) * Projectile.ai[1] * 1.5f;
                    Color color = Projectile.GetAlpha(Color.Gray);
                    color *= 0.75f;
                    Vertex.Add(new ColoredVertex(Pos + Vec + Rot, new Vector3(i / (float)Max, 0f, 1f), color));
                    Vertex.Add(new ColoredVertex(Pos + Vec - Rot, new Vector3(i / (float)Max, 1f, 1f), color));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            
            if (Vertex.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex.ToArray(), 0, Vertex.Count - 2);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]==0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.velocity *= 0f;
                Projectile.netUpdate = true;
                Projectile.localAI[0] = 1;
            }

            Projectile.ai[1] += 3f;
            Projectile.ai[1] *= 1.05f;
            if (Projectile.timeLeft < 30f)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha > 255) Projectile.Kill();
            }
        }
    }
}
