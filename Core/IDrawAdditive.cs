using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core
{
    public interface IDrawAdditive
    {
        /// <summary>
        /// 用addtive来绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawAdditive(SpriteBatch spriteBatch);
    }
}
