using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationText(LocalizedText text, Vector2 center, int maxWidth = -1) : UIAnimationComponent(center)
    {
        /// <summary>
        /// 指着的位置
        /// </summary>
        private Vector2? pointerPos = null;
        private Vector2 scale = Vector2.One;
        private Color textColor = Color.White;
        private Color lineColor = Color.White;
        private float lineWidth = 2f;

        public override void RecalculateOthers()
        {
            this.SetSize(Helper.GetStringSize(text.Value, scale, maxWidth));
        }

        public UIAnimationText SetTextColor(Color c)
        {
            textColor = c;
            return this;
        }

        public UIAnimationText SetLineColor(Color c)
        {
            lineColor = c;
            return this;
        }

        /// <summary>
        /// 设置指向的点，与中心位置之间是相对位置
        /// </summary>
        /// <param name="pointerPos"></param>
        /// <returns></returns>
        public UIAnimationText SetPointerPos(Vector2 pointerPos)
        {
            this.pointerPos = pointerPos;
            return this;
        }

        /// <summary>
        /// 设置线条粗细
        /// </summary>
        /// <param name="lineWidth"></param>
        /// <returns></returns>
        public UIAnimationText SetLineWidth(float lineWidth)
        {
            this.lineWidth = lineWidth;
            return this;
        }

        public override void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {
            if (timer < StartTime || timer > EndTime)
                return;

            Helper.DrawText(spriteBatch, text.Value, maxWidth, center, new Vector2(0.5f, 1), scale, new Color(50, 50, 50) * fadeFactor, textColor * fadeFactor, out Vector2 size, true);

            Texture2D magicPix = CoraliteAssets.Misc.White32x32.Value;

            //绘制一条线
            spriteBatch.Draw(magicPix, center + new Vector2(0, 4), null, lineColor, Rotation, Vector2.One / 2, new Vector2(fadeFactor * size.X / magicPix.Width, lineWidth / magicPix.Height), 0, 0);

            if (pointerPos.HasValue)//绘制指向的点连线
            {
                Vector2 pPos = pointerPos.Value;
                float length = pPos.Length();

                spriteBatch.Draw(magicPix, center + new Vector2(0, 4), null, lineColor, pPos.ToRotation(), Vector2.One / 2, new Vector2(fadeFactor * length / magicPix.Width, lineWidth / magicPix.Height), 0, 0);
            }
        }
    }
}
