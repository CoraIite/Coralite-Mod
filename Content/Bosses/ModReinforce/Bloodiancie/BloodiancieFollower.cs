using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    /// <summary>
    /// 赤玉灵的随从
    /// 纯用于视觉效果以及记录坐标用
    /// </summary>
    public class BloodiancieFollower
    {
        public Vector2 center;
        public float rotation;
        public float scale;
        public bool drawBehind;
        /// <summary>
        /// 用于区分层数
        /// </summary>
        public float lengthOffset;
        private int textureType;

        public BloodiancieFollower(Vector2 center)
        {
            this.center = center;
            lengthOffset = Main.rand.NextFloat(0.6f,1f);
            textureType = Main.rand.Next(2);
        }

        public static Asset<Texture2D> tex1;
        public static Asset<Texture2D> tex2;


        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D mainTex = textureType switch
            {
                0 => tex1.Value,
                _ => tex2.Value
            };
            spriteBatch.Draw(mainTex, center - Main.screenPosition, null, drawColor, rotation, mainTex.Size() / 2, scale, SpriteEffects.None, 0);
        }

        public void DrawInBCL(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D mainTex = textureType switch
            {
                0 => tex1.Value,
                _ => tex2.Value
            };
            spriteBatch.Draw(mainTex, center, null, drawColor, rotation, mainTex.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}