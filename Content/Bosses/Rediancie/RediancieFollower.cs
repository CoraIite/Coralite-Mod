using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie
{
    /// <summary>
    /// 赤玉灵的随从
    /// 纯用于视觉效果以及记录坐标用
    /// </summary>
    public class RediancieFollower
    {
        public Vector2 center;
        public float rotation;
        public float scale;
        public bool drawBehind;

        public RediancieFollower(Vector2 center)
        {
            this.center = center;
        }
        
        public static Asset<Texture2D> mainTex;

        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(mainTex.Value, center-Main.screenPosition, null, drawColor, rotation, mainTex.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}