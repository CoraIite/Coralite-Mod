using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public abstract class UIAnimationComponent(Vector2 center) : UIElement
    {
        public int StartTime;
        public int EndTime;

        /// <summary>
        /// 位置移动的关键帧，会根据fadeTime来进行缩放
        /// </summary>
        public List<(int, Vector2)> PosKeyFrameInit { get; set; } = null;
        protected (int, Vector2)[] PosKeyFrame { get; private set; } = null;
        /// <summary>
        /// 旋转的关键帧，会在两者之间持续改变
        /// </summary>
        public List<(int, float)> RotKeyFrameInit { get; set; } = null;
        protected (int, float)[] RotKeyFrame { get; private set; } = null;

        protected ISmoother PosSmoother { get; private set; }
        protected ISmoother RotSmoother { get; private set; }

        private Vector2 CenterOffset = Vector2.Zero;
        public Vector2 FadeOffset { get; protected set; } = new Vector2(0, -20);
        public int FadeTime { get; protected set; } = 10;
        public float Rotation { get; protected set; }
        public Color DrawColor { get; protected set; } = Color.White;
        public Vector2 origin = Vector2.One / 2;

        /// <summary>
        /// 位置的运动偏移，一般用于简单的周期性运动效果
        /// </summary>
        public Func<int, Vector2> PosOffset;

        public int HoverItemType = -1;

        #region 各类设置

        /// <summary>
        /// 添加位置偏移关键帧，注意不要重复添加到同一帧
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="addTime"></param>
        /// <returns></returns>
        public UIAnimationComponent AddPosOffsetKeyFrame(UIAnimation anim, int addTime, Vector2 posOffset)
        {
            PosKeyFrameInit ??= [];
            PosKeyFrameInit.Add((anim.TempTimer + addTime, posOffset));
            return this;
        }

        /// <summary>
        /// 添加旋转关键帧，注意不要重复添加到同一帧
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="addTime"></param>
        /// <returns></returns>
        public UIAnimationComponent AddRotKeyFrame(UIAnimation anim, int addTime, float rot)
        {
            RotKeyFrameInit ??= [];
            RotKeyFrameInit.Add((anim.TempTimer + addTime, rot));
            return this;
        }

        /// <summary>
        /// 根据当前时间轴设置结束时间，必须调用！
        /// </summary>
        /// <param name="ani"></param>
        public virtual void SetEnd(UIAnimation ani)
        {
            EndTime = ani.TempTimer;

            if (PosKeyFrameInit != null)
                PosKeyFrame = [.. PosKeyFrameInit];
            if (RotKeyFrameInit != null)
                RotKeyFrame = [.. RotKeyFrameInit];

            RotKeyFrameInit = null;
            RotKeyFrame = null;

            PosSmoother ??= Coralite.Instance.NoSmootherInstance;
            RotSmoother ??= Coralite.Instance.NoSmootherInstance;
        }

        /// <summary>
        /// 切换位置时候的平滑模式
        /// </summary>
        /// <param name="smoother"></param>
        /// <returns></returns>
        public UIAnimationComponent SetPosSmoother(ISmoother smoother)
        {
            PosSmoother = smoother;
            return this;
        }

        /// <summary>
        /// 切换旋转时候的平滑模式
        /// </summary>
        /// <param name="smoother"></param>
        /// <returns></returns>
        public UIAnimationComponent SetRotSmoother(ISmoother smoother)
        {
            RotSmoother = smoother;
            return this;
        }

        /// <summary>
        /// 设置位置偏移的曲线，一般用于随动画进度增加而产生的固定轨迹运动
        /// </summary>
        /// <param name="ease"></param>
        /// <returns></returns>
        public UIAnimationComponent SetPosOffsetEase(Func<int, Vector2> ease)
        {
            PosOffset = ease;
            return this;
        }

        /// <summary>
        /// 设置鼠标悬浮时候的物品类型
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public UIAnimationComponent SetHoverItemType(int itemType)
        {
            HoverItemType = itemType;
            return this;
        }

        /// <summary>
        /// 设置鼠标悬浮时候的物品类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UIAnimationComponent SetHoverItemType<T>() where T : ModItem
        {
            HoverItemType = ModContent.ItemType<T>();
            return this;
        }



        /// <summary>
        /// 设置渐入渐出的小动画时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="fadeoffset"></param>
        public UIAnimationComponent SetFadeTime(int time)
        {
            FadeTime = time;
            return this;
        }

        /// <summary>
        /// 设置渐入渐出的小动画时间和位移
        /// </summary>
        /// <param name="time"></param>
        /// <param name="fadeoffset"></param>
        public UIAnimationComponent SetFadeValues(int time, Vector2 fadeoffset)
        {
            FadeTime = time;
            FadeOffset = fadeoffset;
            return this;
        }

        /// <summary>
        /// 设置主要颜色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public UIAnimationComponent SetColor(Color c)
        {
            DrawColor = c;
            return this;
        }

        public UIAnimationComponent SetOrigin(Vector2 origin)
        {
            this.origin = origin;
            return this;
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        //public UIAnimationComponent SetRotation(float rot)
        //{
        //     Rotation = rot;
        //    return this;
        //}

        #endregion

        public override void Recalculate()
        {
            RecalculateOthers();
            this.SetCenter(center, Vector2.One / 2, origin);

            base.Recalculate();
        }

        public virtual void RecalculateOthers() { }

        public void UpdateAnimationInner(int timer)
        {
            UpdatePosOffset(timer);
            UpdateRotOffset(timer);
            UpdateAnimation(timer);
        }

        private void UpdatePosOffset(int timer)
        {
            if (PosKeyFrame == null)
                return;

            for (int i = 0; i < PosKeyFrame.Length; i++)
            {
                int currTime = PosKeyFrame[i].Item1;
                if (timer < currTime)//在当前范围内
                {
                    if (i == 0)//第一帧，直接从初始时间开始
                    {
                        if (currTime == StartTime)//和起始帧重合，为了避免除以0所以单独设置一下
                        {
                            CenterOffset = PosKeyFrame[i].Item2;
                            return;
                        }

                        float f = (timer - StartTime) / (float)(currTime - StartTime);
                        CenterOffset = Vector2.Lerp(Vector2.Zero, PosKeyFrame[i].Item2, PosSmoother.Smoother(f));

                        return;
                    }

                    int PrevTime = PosKeyFrame[i - 1].Item1;
                    float f2 = (timer - PrevTime) / (float)(currTime - PrevTime);
                    CenterOffset = Vector2.Lerp(PosKeyFrame[i - 1].Item2, PosKeyFrame[i].Item2, PosSmoother.Smoother(f2));

                    return;
                }

                if (i == PosKeyFrame.Length - 1)
                {
                    CenterOffset = PosKeyFrame[i].Item2;
                }
            }
        }

        private void UpdateRotOffset(int timer)
        {
            if (RotKeyFrame == null)
                return;

            for (int i = 0; i < RotKeyFrame.Length; i++)
            {
                int currTime = RotKeyFrame[i].Item1;
                if (timer < currTime)//在当前范围内
                {
                    if (i == 0)//第一帧，直接从初始时间开始
                    {
                        if (currTime == StartTime)//和起始帧重合，为了避免除以0所以单独设置一下
                        {
                            Rotation = RotKeyFrame[i].Item2;
                            return;
                        }

                        float f = (timer - StartTime) / (float)(currTime - StartTime);
                        Rotation = Helper.Lerp(0, RotKeyFrame[i].Item2, RotSmoother.Smoother(f));

                        return;
                    }

                    int PrevTime = RotKeyFrame[i - 1].Item1;
                    float f2 = (timer - PrevTime) / (float)(currTime - PrevTime);
                    Rotation = Helper.Lerp(RotKeyFrame[i - 1].Item2, RotKeyFrame[i].Item2, PosSmoother.Smoother(f2));

                    return;
                }

                if (i == PosKeyFrame.Length - 1)
                {
                    Rotation = RotKeyFrame[i].Item2;
                }
            }
        }

        public virtual void UpdateAnimation(int timer)
        {
            
        }

        public void DrawAnimationInner(SpriteBatch spriteBatch, int timer)
        {
            if (timer < StartTime || timer > EndTime)
                return;

            Vector2 pos = GetDimensions().Center() + CenterOffset;

            if (PosOffset != null)
                pos += PosOffset(timer);

            float f = 1;
            if (timer < StartTime + FadeTime)//根据时间渐变消失，从0到1
                f = (float)(timer - StartTime) / FadeTime;
            if (timer > EndTime - FadeTime)//从1到0
                f = 1 - (float)(timer - (EndTime - FadeTime)) / FadeTime;

            DrawAnimation(spriteBatch, timer, pos, f);

            if (IsMouseHovering)
            {
                if (HoverItemType > 0)
                {
                    Main.HoverItem = ContentSamples.ItemsByType[HoverItemType].Clone();
                    Main.hoverItemName = "a";
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="timer"></param>
        /// <param name="center"></param>
        /// <param name="fadeFactor">根据渐变时间从0到1再到0，使用时请注意</param>
        public virtual void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {

        }
    }
}
