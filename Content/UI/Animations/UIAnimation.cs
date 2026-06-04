using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Localization;
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
        /// 初始化阶段调用，拖动时间轴
        /// </summary>
        /// <param name="passTime"></param>
        public void Init_LetTimePass(int passTime)
            => TempTimer += passTime;

        /// <summary>
        /// 设置时间轴终点，一般初始化最后调用
        /// </summary>
        public void Init_EndTime()
            => MaxTime = TempTimer;

        #region 添加组件

        /// <summary>
        /// 在当前时间上添加一个图片绘制动画
        /// </summary>
        /// <param name="texPath"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationTexture CreateTextureCurrent(string texPath, Vector2 center)
        {
            var element = new UIAnimationTexture(texPath, center);
            element.StartTime = TempTimer;
            Components.Add(element);
            Append(element);
            return element;
        }

        /// <summary>
        /// 在当前时间上添加一个文字动画
        /// </summary>
        /// <param name="texPath"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationText CreateTextCurrent(LocalizedText text, Vector2 center)
        {
            var element = new UIAnimationText(text, center);
            element.StartTime = TempTimer;
            Components.Add(element);
            Append(element);
            return element;
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            if (!Pause)
                Timer++;

            Timer = Math.Clamp(Timer, 0, MaxTime);

            foreach (var element in Components)
                element.UpdateAnimationInner(Timer);
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
                element.DrawAnimationInner(spriteBatch, Timer);
        }

        #endregion
    }
}
