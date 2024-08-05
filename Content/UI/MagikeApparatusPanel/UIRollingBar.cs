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
        private int _timerForVisual;

        public UIRollingBar(Action<int> setIndex, Func<int> getIndex)
        {
            SetIndex = setIndex;
            GetIndex = getIndex;
        }

        public void LimitValue()
        { 
            
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            //改变外界的变量

            base.ScrollWheel(evt);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (GetIndex == null)
                return;

            int index = GetIndex();
            int count = Elements.Count;
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
