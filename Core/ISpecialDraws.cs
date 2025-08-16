using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core
{
    /*
     * 这里包含了各类特殊绘制的接口，继承这些接口可以实现特殊的绘制
     * 大部分目前仅支持弹幕和NPC
     */

    public interface IDrawAdditive
    {
        /// <summary>
        /// 用<see cref="BlendState.Additive"/>来绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawAdditive(SpriteBatch spriteBatch);
    }

    public interface IPostDrawAdditive
    {
        /// <summary>
        /// 用<see cref="BlendState.Additive"/>来绘制，但是在绘制Non层后进行
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawAdditive(SpriteBatch spriteBatch);
    }

    public interface IDrawNonPremultiplied
    {
        /// <summary>
        /// 用<see cref="BlendState.NonPremultiplied"/>来绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawNonPremultiplied(SpriteBatch spriteBatch);
    }

    public interface IDrawWarp
    {
        /// <summary>
        /// 绘制扭曲，目前可以在<see cref="Terraria.Projectile"/>和<see cref="Terraria.NPC"/>中使用<br></br>
        /// 绘制时颜色 R代表方向，0~<see cref="MathHelper.TwoPi"/>，G代表扭曲强度，0.25f为正常强度，最高1
        /// </summary>
        void DrawWarp();
    }

    public interface IDrawPrimitive
    {
        /// <summary>
        /// 绘制顶点
        /// </summary>
        void DrawPrimitives();
    }

    public interface IDrawColorReverse
    {
        void DrawColorReverse(SpriteBatch spriteBatch);
    }

}
