using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBall_NameLine : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + "ShadowBallNameLine";

        float RectScale;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 260;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void AI()
        {
            int timer = 260 - Projectile.timeLeft;

            if (timer < 60)
            {
                RectScale = timer / 60f;
            }

            else if (timer > 200)
            {
                RectScale =1- (timer - 240) / 60f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 screenPosition = Main.screenPosition;

            var pos = Main.LocalPlayer.Center - screenPosition;
            var origin = mainTex.Size() / 2;

            Vector2 size = new Vector2(RectScale * 380, Main.screenHeight);

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            spriteBatch.End();
            Rectangle scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch, pos, size), spriteBatch.GraphicsDevice.ScissorRectangle);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
            spriteBatch.GraphicsDevice.RasterizerState = ShadowBall.OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, ShadowBall.OverflowHiddenRasterizerState, null,Main.GameViewMatrix.TransformationMatrix);

            Utils.DrawBorderStringBig(spriteBatch, ModContent.GetInstance<ShadowBall>().DisplayName.Value
                , Main.LocalPlayer.Center - new Vector2(0, 325) - screenPosition, new Color(180, 80, 255), 0.9f, 0.5f);

            var barPos = Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition;
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(mainTex, barPos + (i * MathHelper.PiOver4).ToRotationVector2() * 2, null
                    , Color.Purple*0.5f, 0f, origin, 1.7f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, barPos, null, Color.White, 0f, origin, 1.7f, SpriteEffects.None, 0f);

            rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null);

            return false;
        }

        public Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Vector2 center, Vector2 frameBox)
        {
            float width = RectScale * frameBox.X;
            float height = frameBox.Y;

            Vector2 position = center + new Vector2(-width / 2, -height / 2);
            Vector2 size = new Vector2(width, height);
            position = Vector2.Transform(position, Main.Transform);
            //size = Vector2.Transform(size, Main.Transform);
            size *= Main.GameZoomTarget;

            Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
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
