using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void DrawTrail(GraphicsDevice device, Action draw
            , BlendState blendState = null, SamplerState samplerState = null, RasterizerState rasterizerState = null)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            BlendState originalBlendState = Main.graphics.GraphicsDevice.BlendState;
            SamplerState originalSamplerState = Main.graphics.GraphicsDevice.SamplerStates[0];

            device.BlendState = blendState ?? originalBlendState;
            device.SamplerStates[0] = samplerState ?? originalSamplerState;
            device.RasterizerState = rasterizerState ?? originalState;

            draw();

            device.RasterizerState = originalState;
            device.BlendState = originalBlendState;
            device.SamplerStates[0] = originalSamplerState;
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// 快速绘制
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="pos"></param>
        /// <param name="selfColor"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public static void QuickCenteredDraw(this Texture2D tex, SpriteBatch spriteBatch, Vector2 pos, Color? selfColor = null, float rotation = 0, float scale = 1, SpriteEffects effect = SpriteEffects.None)
        {
            spriteBatch.Draw(tex, pos, null, selfColor ?? Color.White, rotation, tex.Size() / 2, scale, effect, 0);
        }

        public static void QuickBottomDraw(this Texture2D tex, SpriteBatch spriteBatch, Vector2 pos, Color? selfColor = null, float rotation = 0, float scale = 1)
        {
            spriteBatch.Draw(tex, pos, null, selfColor ?? Color.White, rotation, new Vector2(tex.Width / 2, tex.Height), scale, 0, 0);
        }

        /// <summary>
        /// 快速绘制
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="pos"></param>
        /// <param name="selfColor"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public static void QuickCenteredDraw(this Texture2D tex, SpriteBatch spriteBatch, Rectangle frame, Vector2 pos, Color? selfColor = null, float rotation = 0, float scale = 1, SpriteEffects effect = SpriteEffects.None)
        {
            var frameBox = tex.Frame(frame.Width, frame.Height, frame.X, frame.Y);
            spriteBatch.Draw(tex, pos, frameBox, selfColor ?? Color.White, rotation, frameBox.Size() / 2, scale, effect, 0);
        }

        public static void QuickCenteredDraw(this Texture2D tex, SpriteBatch spriteBatch, Rectangle frame, Vector2 pos, SpriteEffects effect, Color? selfColor = null, float rotation = 0, float scale = 1)
        {
            var frameBox = tex.Frame(frame.Width, frame.Height, frame.X, frame.Y);
            spriteBatch.Draw(tex, pos, frameBox, selfColor ?? Color.White, rotation, frameBox.Size() / 2, scale, effect, 0);
        }
    }
}
