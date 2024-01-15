using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Evevts.ShadowCastle
{
    public class BlackHoleTrials : ModSystem
    {
        public static bool DownedBlackHoleTrails;

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("DownedBlackHoleTrails", DownedBlackHoleTrails);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            DownedBlackHoleTrails = tag.Get<bool>("DownedBlackHoleTrails");
        }
    }

    public class BlackHoleMainProj : ModProjectile, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "BlackBall";

        ref float Timer => ref Projectile.ai[0];
        ref float State => ref Projectile.ai[1];

        public float scale;
        public float alpha;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180000;
            Projectile.width = Projectile.height = 64;
        }

        public override void AI()
        {
            Projectile.rotation += 0.08f;

            switch (State)
            {
                default:
                case 0://刚刚生成
                    {
                        if (Timer < 10)
                        {
                            scale += 0.5f / 10f;
                            alpha += 1 / 10f;
                            break;
                        }

                        if (Timer > 60)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://缓慢吸收，在周围出现黑星弹幕
                    {

                    }
                    break;
                case 2://加速，生成环绕的弹幕
                    {

                    }
                    break;
                case 3://冲刺，冲刺，冲！更加多的弹幕
                    {

                    }
                    break;
                case 4://失败
                    {

                    }
                    break;

            }
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D blackBallTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(blackBallTex, pos, null, Color.White * alpha, 0, blackBallTex.Size() / 2, scale*0.9f, 0, 0);

            Texture2D flowTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleEvents + "BlackHoleFlow").Value;

            Color c = Color.DarkOrange * alpha;
            c.A = 0;
            Color c2 = Color.DarkRed * alpha;
            c2.A = 0;
            Main.spriteBatch.Draw(flowTex, pos, null, c2 * 0.3f, Projectile.rotation * 2.2f, flowTex.Size() / 2, scale * 1f, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.8f, Projectile.rotation + 0.8f, flowTex.Size() / 2, scale * 0.88f, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.9f, Projectile.rotation * 2, flowTex.Size() / 2, scale * 0.93f, 0, 0);

            return false;
        }

        public void DrawWarp()
        {
            Texture2D warpTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "CircleWarp").Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;

                  Main.spriteBatch.Draw(warpTex, pos, null, Color.White * alpha, 1.57f, warpTex.Size() / 2, scale+0.5f, 0, 0);
        }
    }

    public class BlackStarProj:ModProjectile,IDrawPrimitive
    {
        private Trail trail;

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["Flow"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(TextureAssets.Extra[189].Value);

            trail?.Render(effect);

        }
    }
}
