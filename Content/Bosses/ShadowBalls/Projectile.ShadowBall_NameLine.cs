using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBall_NameLine : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + "ShadowBallNameLine";

        float RectScale;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = GetPreTime()+60;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        public static int GetPreTime()
            => SpawnProj.ShadowBallAppearTime + SpawnProj.ShadowProjSpawnTime + SpawnProj.LockAppearTime + SpawnProj.RoarTime;

        public override void AI()
        {
            if (Timer <= SpawnProj.ShadowBallAppearTime)
            {
                RectScale = Timer / SpawnProj.ShadowBallAppearTime;
            }
            else if (Timer > GetPreTime())
            {
                RectScale = 1 - ((Timer - GetPreTime()) / 60f);
            }
            else
            {
                if (Timer % 8 == 0)
                {
                    var p = PRTLoader.NewParticle<CrossLightParticle>(Main.screenPosition + new Vector2(Main.screenWidth / 2 + Main.rand.Next(-150, 150), Main.rand.Next(130,255)), Helper.NextVec2Dir(0.3f,0.6f), Coralite.ShadowPurple, Main.rand.NextFloat(0.5f, 0.9f));

                    p.Rotation = MathHelper.PiOver4;
                    p.maxTime = Main.rand.Next(16, 32);
                }
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D mainTex = Projectile.GetTextureValue();

            var pos = Main.ScreenSize.ToVector2()/2;
            var origin = mainTex.Size() / 2;

            Vector2 size = new(RectScale * RectScale * 380, Main.screenHeight);

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            spriteBatch.End();
            Rectangle scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch, pos, size), spriteBatch.GraphicsDevice.ScissorRectangle);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
            spriteBatch.GraphicsDevice.RasterizerState = ShadowBall.OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, ShadowBall.OverflowHiddenRasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            Utils.DrawBorderStringBig(spriteBatch, ModContent.GetInstance<ShadowBall>().DisplayName.Value
                ,  new Vector2(Main.screenWidth/2, 130), Coralite.ShadowPurple, 1f, 0.5f);

            var barPos = new Vector2(Main.screenWidth/2,225);
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(mainTex, barPos + ((i * MathHelper.PiOver4).ToRotationVector2() * 2), null
                    , Color.Purple * 0.5f, 0f, origin, 1.7f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, barPos, null, Color.White, 0f, origin, 1.7f, SpriteEffects.None, 0f);

            rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = -1; k < 2; k += 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Helper.DrawPrettyLine(1, 0, new Vector2(Main.screenWidth / 2 + k * RectScale * RectScale * 380/2, (130 + 255) / 2), Coralite.ShadowPurple with { A = 0 } * 0.6f, Coralite.ShadowPurple * 0.7f, RectScale, 0, 0.2f, 0.7f, 1, MathHelper.PiOver2, 4-i*0.7f, new Vector2(2, 1));
                }
            }

            return false;
        }

        public Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Vector2 center, Vector2 frameBox)
        {
            float width = frameBox.X;
            float height = frameBox.Y;

            Vector2 position = center + new Vector2(-width / 2, -height / 2);
            Vector2 size = new(width, height);
            position = Vector2.Transform(position, Main.Transform);
            //size = Vector2.Transform(size, Main.Transform);
            size *= Main.GameZoomTarget;

            Rectangle rectangle = new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            int screenWidth = Main.screenWidth;
            int screenHeight = Main.screenHeight;
            rectangle.X = Utils.Clamp(rectangle.X, 0, screenWidth);
            rectangle.Y = Utils.Clamp(rectangle.Y, 0, screenHeight);
            rectangle.Width = Utils.Clamp(rectangle.Width, 0, screenWidth - rectangle.X);
            rectangle.Height = Utils.Clamp(rectangle.Height, 0, screenHeight - rectangle.Y);
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            int num3 = Utils.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
            int num4 = Utils.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
            int num5 = Utils.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
            int num6 = Utils.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);
            return new Rectangle(num3, num4, num5 - num3, num6 - num4);
        }
    }
}
