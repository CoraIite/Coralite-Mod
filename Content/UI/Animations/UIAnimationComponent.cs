using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public abstract class UIAnimationComponent:UIElement
    {
        public int StartTime;
        public int EndTime;

        /// <summary>
        /// 根据当前时间轴设置结束时间
        /// </summary>
        /// <param name="ani"></param>
        public void SetEnd(UIAnimation ani)
        {
            EndTime = ani.TempTimer;
        }

        public virtual void UpdateAnimation(int timer)
        {

        }

        public virtual void DrawAnimation(SpriteBatch spriteBatch,int timer)
        {

        }
    }
}
