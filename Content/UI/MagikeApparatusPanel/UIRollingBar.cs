using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class UIRollingBar:UIElement
    {
        public Action<int> SetIndex { get; set; }
        public Func<int> GetIndex { get; set; }

        private float _indexForVisual;
        private float _indexForVisualOld;
        private int _timerForVisual;

        public UIRollingBar(Action<int> setIndex, Func<int> getIndex)
        {
            SetIndex = setIndex;
            GetIndex = getIndex;
        }

        private void SetValues(int changeCount)
        {
            int currentValue=GetIndex();
            currentValue += changeCount;
            currentValue = Math.Clamp(currentValue,0,Elements.Count-1);

            SetIndex(currentValue);
            _timerForVisual = 20;
            _indexForVisualOld = _indexForVisual;
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            //改变外界的变量
            if (evt.ScrollWheelValue > 0)
                SetValues(1);
            else
                SetValues(-1);

            base.ScrollWheel(evt);
        }

        public override void Update(GameTime gameTime)
        {
            if (_timerForVisual>0)
            {
                _timerForVisual--;
                _indexForVisual = Helper.Lerp(GetIndex(), _indexForVisualOld, _timerForVisual / 20f);
            }

            base.Update(gameTime);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (GetIndex == null)
                return;

            int index = GetIndex();

            //共计绘制5个
            for (int i = index - 2; i < index + 2; i++)
            {
                if (!Elements.IndexInRange(i))
                    continue;

            }
        }
    }

    public class UIAlphaDrawElement:UIElement
    {
        protected sealed override void DrawSelf(SpriteBatch spriteBatch) { }
    
        protected virtual void DrawSelfAlpha (SpriteBatch spriteBatch,float alpha) { }
    }
}
