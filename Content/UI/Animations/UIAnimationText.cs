using Coralite.Content.CoraliteNotes;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationText(LocalizedText text, Vector2 origCenter, float maxWidth = -1) : UIAnimationComponent(origCenter)
    {
        /// <summary>
        /// 指着的位置
        /// </summary>
        private Vector2? pointerPos = null;
        /// <summary>
        /// 指向线条的在原本线条连接点的X位置偏移
        /// </summary>
        private float pointerBasePosPercent;

        /// <summary>
        /// 位置移动的关键帧，会根据fadeTime来进行缩放
        /// </summary>
        public List<(int, Vector2)> PointerKeyFrameInit { get; set; } = null;
        protected (int, Vector2)[] PointerKeyFrame { get; private set; } = null;
        protected ISmoother PointerSmoother { get; private set; }

        private Vector2 scale = Vector2.One;
        private Color textColor = Color.White;
        private Color textBackColor = new Color(151,127,117);
        private Color lineColor = Color.White;
        private Color lineBackColor = new Color(40, 40, 40, 255);
        private float lineWidth = 2f;

        private readonly Vector2 origCenter = origCenter;

        public override void RecalculateOthers()
        {
            Vector2 size = Helper.GetStringSize(text.Value, scale, maxWidth);
            this.SetSize(size);
        }

        public override void SetEnd(UIAnimation ani)
        {
            base.SetEnd(ani);
            if (PointerKeyFrameInit != null)
            {
                if (PointerKeyFrameInit.Count > 0)
                    PointerKeyFrameInit.Insert(0, (StartTime, PointerKeyFrameInit[0].Item2));
                PointerKeyFrame = [.. PointerKeyFrameInit];
            }

            PointerKeyFrameInit = null;

            PointerSmoother ??= Coralite.Instance.NoSmootherInstance;
        }

        public override void UpdateAnimation(int timer)
        {
            if (PointerKeyFrame == null)
                return;

            for (int i = 0; i < PointerKeyFrame.Length; i++)
            {
                int currTime = PointerKeyFrame[i].Item1;
                if (timer < currTime)//在当前范围内
                {
                    if (i == 0)//第一帧，直接从初始时间开始
                    {
                        if (currTime == StartTime)//和起始帧重合，为了避免除以0所以单独设置一下
                        {
                            pointerPos = PointerKeyFrame[i].Item2;
                            return;
                        }

                        float f = (timer - StartTime) / (float)(currTime - StartTime);
                        pointerPos = Vector2.Lerp(Vector2.Zero, PointerKeyFrame[i].Item2, PointerSmoother.Smoother(f));

                        return;
                    }

                    int PrevTime = PointerKeyFrame[i - 1].Item1;
                    float f2 = (timer - PrevTime) / (float)(currTime - PrevTime);
                    pointerPos = Vector2.Lerp(PointerKeyFrame[i - 1].Item2, PointerKeyFrame[i].Item2, PointerSmoother.Smoother(f2));

                    return;
                }

                if (i == PointerKeyFrame.Length - 1)
                {
                    pointerPos = PointerKeyFrame[i].Item2;
                }
            }
        }

        /// <summary>
        /// 添加位置偏移关键帧，注意不要重复添加到同一帧
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="addTime"></param>
        /// <returns></returns>
        public UIAnimationComponent AddPointerOffsetKeyFrame(UIAnimation anim, int addTime, Vector2 posOffset)
        {
            PointerKeyFrameInit ??= [];
            PointerKeyFrameInit.Add((anim.TempTimer + addTime, posOffset));
            return this;
        }

        /// <summary>
        /// 设置文字颜色（有时候没用）
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationText SetPointerSmoother(ISmoother smoother)
        {
            PointerSmoother = smoother;
            return this;
        }

        /// <summary>
        /// 设置文字颜色（有时候没用）
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationText SetTextColor(Color c)
        {
            textColor = c;
            return this;
        }

        /// <summary>
        /// 设置文字背景颜色，默认为珊瑚笔记的棕色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationText SetTextBackColor(Color c)
        {
            textBackColor = c;
            return this;
        }

        /// <summary>
        /// 设置线条本身颜色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationText SetLineColor(Color c)
        {
            lineColor = c;
            return this;
        }

        /// <summary>
        /// 设置线条背景颜色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationText SetLineBackColor(Color c)
        {
            lineBackColor = c;
            return this;
        }

        ///// <summary>
        ///// 设置指向的点，与中心位置之间是相对位置
        ///// </summary>
        ///// <param name="pointerPos"></param>
        ///// <returns></returns>
        //public UIAnimationText SetPointerPos(Vector2 pointerPos)
        //{
        //    this.pointerPos = pointerPos;
        //    return this;
        //}

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

            Vector2 size = Helper.GetStringSize(text.Value, scale, maxWidth);
            size.X *= 1.25f;
            size.Y *= 1.25f;

            Texture2D backTex = CoraliteNoteSystem.NewTextBarBack.Value;

            spriteBatch.Draw(backTex, center + new Vector2(0, -4), null, textBackColor*fadeFactor, 0, backTex.Size() / 2, size / backTex.Size(), 0, 0);

            Helper.DrawText(spriteBatch, text.Value, maxWidth, center + new Vector2(0, 4), new Vector2(0.5f, 0.5f), scale, new Color(50, 50, 50) * fadeFactor, textColor * fadeFactor, out size, true);

            DrawLines(spriteBatch, center, fadeFactor, size);

            //Helper.DrawDebugFrame(this, spriteBatch);
        }

        private void DrawLines(SpriteBatch spriteBatch, Vector2 center, float fadeFactor, Vector2 size)
        {
            Texture2D magicPix = CoraliteAssets.Misc.White32x32.Value;

            //绘制一条线

            Vector2 horizontalLinePos = center + new Vector2(-size.X / 2, size.Y / 2);
            Vector2 horizontalLineScale = new(fadeFactor * size.X / magicPix.Width, lineWidth / magicPix.Height);
            Vector2 origin = new(0, magicPix.Height / 2);

            for (int i = 0; i < 4; i++)
                spriteBatch.Draw(magicPix, horizontalLinePos + (i * MathHelper.PiOver2).ToRotationVector2() * lineWidth, null, lineBackColor, Rotation, origin, horizontalLineScale, 0, 0);

            if (pointerPos.HasValue)//绘制指向的点连线
            {
                Vector2 pPos = pointerPos.Value;
                Vector2 originCenter = GetDimensions().Center() ;

                pPos += (originCenter - origCenter);

                if (pPos.X > center.X + size.X / 2)
                    pointerBasePosPercent = Helper.Lerp(pointerBasePosPercent, 1, 0.2f);
                else if (pPos.X < center.X - size.X / 2)
                    pointerBasePosPercent = Helper.Lerp(pointerBasePosPercent, -1, 0.2f);
                else
                    pointerBasePosPercent = Helper.Lerp(pointerBasePosPercent, 0, 0.2f);

                Vector2 center2 = center + new Vector2(pointerBasePosPercent * size.X / 2, size.Y / 2);

                float length = (pPos - center2).Length();
                float rot = (pPos - center2).ToRotation();

                Vector2 pointerLineScale = new(fadeFactor * length / magicPix.Width, lineWidth / magicPix.Height);

                for (int i = 0; i < 4; i++)
                    spriteBatch.Draw(magicPix, center2 + (rot + i * MathHelper.PiOver2).ToRotationVector2() * lineWidth, null, lineBackColor, rot, origin, pointerLineScale, 0, 0);

                spriteBatch.Draw(magicPix, center2, null, lineColor, rot, origin, pointerLineScale, 0, 0);
            }

            spriteBatch.Draw(magicPix, horizontalLinePos, null, lineColor, Rotation, origin, horizontalLineScale, 0, 0);
        }
    }
}
