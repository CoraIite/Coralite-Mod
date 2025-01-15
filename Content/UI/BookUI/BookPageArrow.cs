﻿using Coralite.Content.UI.UILib;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.BookUI
{
    public class BookPageArrow : UIElement
    {
        private UI_BookPanel bookPanel;

        public ATex ArrowTex;

        public ArrowType arrowType;

        public enum ArrowType
        {
            Left,
            Right   
        }

        public BookPageArrow(UI_BookPanel bookPanel, ATex arrowTex, ArrowType arrowType)
        {
            this.bookPanel = bookPanel;
            ArrowTex = arrowTex;
            this.arrowType = arrowType;

            this.SetSize(ArrowTex.Size());
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            switch (arrowType)
            {
                case ArrowType.Left:
                    if (bookPanel.currentDrawingPage == 0)
                        return;

                    bookPanel.PreviousPage();
                    break;
                case ArrowType.Right:
                    if (bookPanel.currentDrawingPage >= bookPanel.Pages.Count - 1)
                        return;

                    bookPanel.NextPage();

                    break;
                default:
                    break;
            }
            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            switch (arrowType)
            {
                case ArrowType.Left:
                    if (bookPanel.currentDrawingPage == 0)
                        return;
                    break;
                case ArrowType.Right:
                    if (bookPanel.currentDrawingPage >= bookPanel.Pages.Count - 1)
                        return;
                    break;
                default:
                    break;
            }

            float alpha = 0.5f;

            if (IsMouseHovering)
                alpha = 1;

            spriteBatch.Draw(ArrowTex.Value, GetDimensions().Center(), null,Color.White * alpha,0,ArrowTex.Size()/2,1,0,0);
        }
    }
}