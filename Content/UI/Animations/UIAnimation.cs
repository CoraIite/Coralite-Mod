using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimation : UIElement
    {
        /// <summary>
        /// 当前的时间
        /// </summary>
        public int Timer { get; private set; }
        /// <summary>
        /// 最大时间
        /// </summary>
        public int MaxTime { get; private set; }
        /// <summary>
        /// 是否处于暂停期间
        /// </summary>
        public bool Pause { get; private set; } = true;

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

        private List<int> _keyFrames;
        /// <summary>
        /// 存储所有的关键帧
        /// </summary>
        public List<int> KeyFrames
        {
            get
            {
                _keyFrames ??= [];
                return _keyFrames;
            }
        }

        /// <summary>
        /// 初始化阶段调用，拖动时间轴
        /// </summary>
        /// <param name="passTime"></param>
        public UIAnimation LetTimePass(int passTime)
        {
            TempTimer += passTime;
            return this;
        }

        /// <summary>
        /// 设置时间轴终点，一般初始化最后调用
        /// </summary>
        public void EndTime()
        {
            MaxTime = TempTimer;
            //将最后一帧帧设置为关键帧
            AddKeyFrame();
        }

        #region 添加组件

        /// <summary>
        /// 在当前时间上添加一个图片绘制动画
        /// </summary>
        /// <param name="texPath"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationTexture CreateTexture(string texPath, Vector2 center)
        {
            var element = new UIAnimationTexture(texPath, center)
            {
                StartTime = TempTimer
            };
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
        public UIAnimationText CreateText(LocalizedText text, Vector2 center, float maxWidth = -1)
        {
            var element = new UIAnimationText(text, center, maxWidth)
            {
                StartTime = TempTimer
            };
            Components.Add(element);
            Append(element);
            return element;
        }

        /// <summary>
        /// 在当前时间上添加一个物品动画
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationItem CreateItem(int itemType, Vector2 center)
        {
            var element = new UIAnimationItem(itemType, center)
            {
                StartTime = TempTimer
            };
            Components.Add(element);
            Append(element);
            return element;
        }

        /// <summary>
        /// 在当前时间上添加一个物品动画
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationItem CreateItem<T>( Vector2 center) where T : ModItem
            => CreateItem(ModContent.ItemType<T>(), center);

        /// <summary>
        /// 将一组物块设置结束时间
        /// </summary>
        /// <param name="animations"></param>
        public UIAnimation ComponentSetEnd(UIAnimationComponent text)
        {
            text.SetEnd(this);
            return this;
        }

        public UIAnimation ComponentAddPosMove(UIAnimationComponent component,Vector2 newPos,int PreFactorTime)
        {
            Vector2 pre = Vector2.Zero;
            if (component.PosKeyFrameInit!=null&& component.PosKeyFrameInit.Count>0)
                pre = component.PosKeyFrameInit[^1].Item2;

            component.AddPosOffsetKeyFrame(this, -PreFactorTime, pre);
            component.AddPosOffsetKeyFrame(this, 0, newPos);
            return this;
        }

        /// <summary>
        /// 添加文字动画的指针位置运动
        /// </summary>
        /// <param name="component"></param>
        /// <param name="newPointPos"></param>
        /// <param name="PreFactorTime"></param>
        /// <returns></returns>
        public UIAnimation TextAddPointerMove(UIAnimationText component, Vector2 newPointPos, int PreFactorTime)
        {
            Vector2 pre = Vector2.Zero;
            if (component.PointerKeyFrameInit != null && component.PointerKeyFrameInit.Count > 0)
                pre = component.PointerKeyFrameInit[^1].Item2;

            if (pre != Vector2.Zero)
                component.AddPointerOffsetKeyFrame(this, -PreFactorTime, pre);
            component.AddPointerOffsetKeyFrame(this, 0, newPointPos);
            return this;
        }

        /// <summary>
        /// 在当前位置添加一个
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="frame"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public UIAnimationSingleTile CreateTile(int tileType, AnimationBlockFrame frame, Vector2 center)
        {
            var element = new UIAnimationSingleTile(tileType, frame, center)
            {
                StartTime = TempTimer
            };
            Components.Add(element);
            Append(element);
            return element;
        }

        /// <summary>
        /// 创建一组物块动画
        /// </summary>
        /// <param name="center"></param>
        /// <param name="fadeTime"></param>
        /// <param name="tileDatas"></param>
        /// <param name="fadeOff"></param>
        /// <returns></returns>
        public UIAnimationSingleTile[] CreateTiles(Vector2 center, int fadeTime, (int tileType, AnimationBlockFrame frame, Point offset)[] tileDatas, Vector2? fadeOff = null)
        {
            UIAnimationSingleTile[] animations = new UIAnimationSingleTile[tileDatas.Length];

            for (int i = 0; i < tileDatas.Length; i++)
            {
                animations[i] = new UIAnimationSingleTile(tileDatas[i].tileType, tileDatas[i].frame, center + tileDatas[i].offset.ToVector2() * 16)
                {
                    StartTime = TempTimer,
                };
                animations[i].SetFadeValues(fadeTime, fadeOff ?? new Vector2(0, -8));
                Components.Add(animations[i]);
                Append(animations[i]);
                LetTimePass(fadeTime / 2);
            }

            return animations;
        }

        /// <summary>
        /// 将一组物块设置结束时间
        /// </summary>
        /// <param name="animations"></param>
        public UIAnimation TilesSetEnd(UIAnimationSingleTile[] animations)
        {
            foreach (var animation in animations)
            {
                animation.SetEnd(this);
                LetTimePass(animation.FadeTime / 2);
            }

            return this;
        }

        /// <summary>
        /// 创建一组物块动画
        /// </summary>
        /// <param name="center"></param>
        /// <param name="fadeTime"></param>
        /// <param name="wallDatas"></param>
        /// <param name="fadeOff"></param>
        /// <returns></returns>
        public UIAnimationSingleWall[] CreateWalls(Vector2 center, int fadeTime, (int wallType, AnimationBlockFrame frame, Point offset)[] wallDatas, Vector2? fadeOff = null)
        {
            UIAnimationSingleWall[] animations = new UIAnimationSingleWall[wallDatas.Length];

            for (int i = 0; i < wallDatas.Length; i++)
            {
                animations[i] = new UIAnimationSingleWall(wallDatas[i].wallType, wallDatas[i].frame, center + wallDatas[i].offset.ToVector2() * 16)
                {
                    StartTime = TempTimer,
                };
                animations[i].SetFadeValues(fadeTime, fadeOff ?? new Vector2(0, -8));
                Components.Add(animations[i]);
                Append(animations[i]);
                LetTimePass(fadeTime / 2);
            }

            return animations;
        }

        /// <summary>
        /// 将一组墙壁设置结束时间
        /// </summary>
        /// <param name="animations"></param>
        public UIAnimation WallsSetEnd(UIAnimationSingleWall[] animations)
        {
            foreach (var animation in animations)
            {
                animation.SetEnd(this);
                LetTimePass(animation.FadeTime / 2);
            }

            return this;
        }

        public UIAnimation AddKeyFrame()
        {
            if (!KeyFrames.Contains(TempTimer))
                KeyFrames.Add(TempTimer);
            return this;
        }

        /// <summary>
        /// 直接设置时间，如果不是必要请不要使用它！
        /// </summary>
        /// <param name="newTime"></param>
        public void SetTimer(int newTime)
            => Timer = Math.Clamp(newTime, 0, MaxTime);

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            if (!Pause)
                Timer++;

            Timer = Math.Clamp(Timer, 0, MaxTime);

            if (!Pause && Timer == MaxTime)
                Pause = true;

            foreach (var element in Components)
                if (Timer >= element.StartTime && Timer <= element.EndTime)
                    element.UpdateAnimationInner(Timer);
        }

        public override void Recalculate()
        {
            Pause = true;

            base.Recalculate();
        }

        public void ResetAnimations()
        {
            RemoveAllChildren();
            _components = [];
            _keyFrames = [];
            TempTimer = 0;
            MaxTime = 0;
        }

        public void SetPause()
        {
            Pause = !Pause;
            if (Timer == MaxTime)
                Timer = 0;
        }

        public void SetPause(bool pause)
        {
            Pause = pause;
        }

        #endregion

        #region 绘制

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            //Helper.DrawDebugFrame(this, spriteBatch);

            foreach (var element in Components)
                element.DrawAnimationInner(spriteBatch, Timer);
        }

        #endregion
    }
}
