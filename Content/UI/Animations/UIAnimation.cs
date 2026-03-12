using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public abstract class UIAnimation : UIElement
    {
        private int Timer;
        private int MaxTime;
        private bool Pause = true;

        /// <summary>
        /// 仅在初始化时使用，用于记录时间轴
        /// </summary>
        public int TempTimer;

        private List<UIAnimationComponent> _components;
        /// <summary>
        /// 存储所有组件
        /// </summary>
        public List<UIAnimationComponent> Components
        {
            get
            {
                _components ??= [];
                return _components;
            }
        }

        /// <summary>
        /// 在当前时间上添加一个图片绘制动画
        /// </summary>
        /// <param name="texPath"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationTexture CreateTextureCurrent(string texPath,Vector2 center)
        {
            var element = new UIAnimationTexture(texPath, center);
            element.StartTime = TempTimer;
            Components.Add(element);
            Append(element);
            return element;
        }

        #region 更新


        public override void Update(GameTime gameTime)
        {



            foreach (var element in Components)
                element.UpdateAnimation(Timer);
        }

        public override void Recalculate()
        {
            Pause = true;
            base.Recalculate();
        }

        #endregion

        #region 绘制

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            foreach (var element in Components)
                element.DrawAnimation(spriteBatch, Timer);
        }

        #endregion
    }
}
