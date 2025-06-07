using Coralite.Content.UI.UILib;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
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
                    if (bookPanel.CurrentDrawingPage == 0)
                        return;

                    bookPanel.PreviousPage();
                    Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);

                    break;
                case ArrowType.Right:
                    if (bookPanel.CurrentDrawingPage >= bookPanel.Pages.Count - 1)
                        return;

                    bookPanel.NextPage();
                    Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);

                    break;
                default:
                    break;
            }
            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Center();
            switch (arrowType)
            {
                case ArrowType.Left:
                    if (bookPanel.CurrentDrawingPage == 0)
                        return;
                    if (IsMouseHovering)
                        pos.X -= MathF.Sin((int)Main.timeForVisualEffects * 0.1f) * 10;
                    break;
                case ArrowType.Right:
                    if (bookPanel.CurrentDrawingPage >= bookPanel.Pages.Count - 2)
                        return;
                    if (IsMouseHovering)
                        pos.X += MathF.Sin((int)Main.timeForVisualEffects * 0.1f) * 10;
                    break;
                default:
                    break;
            }

            float alpha = 0.5f;

            if (IsMouseHovering)
                alpha = 1;

            spriteBatch.Draw(ArrowTex.Value, pos, null, Color.White * alpha, 0, ArrowTex.Size() / 2, 1, 0, 0);
        }
    }
}
