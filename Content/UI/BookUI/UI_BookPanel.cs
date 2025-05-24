using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.UILib
{
    /// <summary>
    /// 面板，用于存储各种的信息
    /// <br>内部存储一堆书页</br>
    /// <para><b>!!!无法用于存储其他UIElement!!!</b></para>
    /// </summary>
    public abstract class UI_BookPanel : UIElement
    {
        public ATex PanelTex;
        public ATex PanelBackTex;
        public readonly int topPageMargins;
        public readonly int bottomPageMargins;
        public readonly int leftPageMargins;
        public readonly int rightPageMargins;

        public float alpha;

        public UIPageGroup[] pageGroups;
        //public Dictionary<UIPageGroup,int> pageGroupIndexes;

        public BookPageArrow LeftArrow;
        public BookPageArrow RightArrow;

        /// <summary>
        /// 左边那一页的ID
        /// </summary>
        public int CurrentDrawingPage { get; set; }

        public List<UIElement> PanelElements => Elements;

        public List<UIElement> Pages = new List<UIElement>();

        /// <summary>
        /// 请注意！！！
        /// <br>左右页边距以书的左侧页为准，别写反了</br>
        /// </summary>
        /// <param name="PanelTex">面板贴图</param>
        /// <param name="topPageMargins">顶部页边距</param>
        /// <param name="leftPageMargins">左侧内页边距</param>
        /// <param name="rightPageMargins">右侧内页边距</param>
        public UI_BookPanel(ATex PanelTex, int topPageMargins, int bottomPageMargins, int leftPageMargins, int rightPageMargins)
        {
            this.PanelTex = PanelTex;
            this.topPageMargins = topPageMargins;
            this.bottomPageMargins = bottomPageMargins;
            this.leftPageMargins = leftPageMargins;
            this.rightPageMargins = rightPageMargins;
        }

        public abstract void InitPageGroups();

        /// <summary>
        /// 帮助方法，用于调用所有group的初始化方法
        /// </summary>
        public void InitGroups()
        {
            //pageGroupIndexes = new Dictionary<UIPageGroup, int>();

            foreach (var group in pageGroups)
            {
                group.InitPages();
                //pageGroupIndexes.Add(group, 0);
                foreach (var element in group.Pages)
                    element.OnInitialize();
            }
        }

        public void InitArrows(ATex leftTex, ATex rightTex)
        {
            LeftArrow = new BookPageArrow(this, leftTex, BookPageArrow.ArrowType.Left);
            RightArrow = new BookPageArrow(this, rightTex, BookPageArrow.ArrowType.Right);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //就只是绘制面板
            //if (!OverflowHidden)
            DrawPanel(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            //存在该页，并且不在渐入渐出的时候才会绘制
            if (OverflowHidden)
            {
                //DrawPanel(spriteBatch);
                return;
            }

            if (alpha == 1f)
            {
                bool hasRightPage = Pages.Count > CurrentDrawingPage + 1;

                Pages[CurrentDrawingPage]?.Draw(spriteBatch);
                if (hasRightPage)
                    Pages[CurrentDrawingPage + 1]?.Draw(spriteBatch);

                //绘制Non，先判断一下是否继承了接口然后再进行绘制
                bool shouldDrawLeftNon = Pages[CurrentDrawingPage] is IDrawNonPremultiplied;
                bool shouldDrawRightNon = false;
                if (hasRightPage)
                    shouldDrawRightNon = Pages[CurrentDrawingPage + 1] is IDrawNonPremultiplied;

                if (shouldDrawLeftNon || shouldDrawLeftNon)     //如果都不是的话那就不浪费CPU去end begin了
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap,
                                    spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                    if (shouldDrawLeftNon)
                        (Pages[CurrentDrawingPage] as IDrawNonPremultiplied).DrawNonPremultiplied(spriteBatch);
                    if (shouldDrawRightNon)
                        (Pages[CurrentDrawingPage + 1] as IDrawNonPremultiplied).DrawNonPremultiplied(spriteBatch);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                }

                LeftArrow.Draw(spriteBatch);
                RightArrow.Draw(spriteBatch);
            }
        }

        public void DrawPanel(SpriteBatch spriteBatch)
        {
            Vector2 position = GetDimensions().Center();
            if (PanelBackTex != null)
                spriteBatch.Draw(PanelBackTex.Value, position, null, Color.White * alpha, 0f, PanelBackTex.Size() / 2, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(PanelTex.Value, position, null, Color.White * alpha, 0f, PanelTex.Size() / 2, 1, SpriteEffects.None, 0f);
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            Pages.Clear();

            if (pageGroups is null)
                return;

            for (int i = 0; i < pageGroups.Length; i++)//刷新一下
            {
                if (!pageGroups[i].CanShowInBook)
                    continue;
                for (int j = 0; j < pageGroups[i].Pages.Length; j++)
                    if (pageGroups[i].Pages[j].CanShowInBook)
                        AppendPage(pageGroups[i].Pages[j]);
            }

            //防止出现越界的情况
            if (Pages.Count >= 2)
                CurrentDrawingPage = Math.Clamp(CurrentDrawingPage, 0, Pages.Count - 1);
            else
                CurrentDrawingPage = 0;
            CurrentDrawingPage = CurrentDrawingPage / 2 * 2;//利用神奇算法让它变为偶数

            //设置可以控制的UI页
            for (int i = Pages.Count - 1; i >= 0; i--)
            {
                UIElement Element = Pages[i];
                if (i == CurrentDrawingPage || i == (CurrentDrawingPage + 1))
                {
                    Element.IgnoresMouseInteraction = false;
                    continue;
                }

                Element.IgnoresMouseInteraction = true;
            }

            SetIndexes();
            InitalizePages();
            SetArrows();

            base.Recalculate();
        }

        /// <summary>
        /// 设置翻页箭头
        /// </summary>
        public void SetArrows()
        {
            LeftArrow.SetTopLeft(PanelTex.Height() - bottomPageMargins, leftPageMargins);
            RightArrow.SetTopLeft(PanelTex.Height() - bottomPageMargins, PanelTex.Width() - leftPageMargins - RightArrow.Width.Pixels);

            Append(LeftArrow);
            Append(RightArrow);
        }

        public void SetIndexes()
        {
            //int count = 0;
            //pageGroupIndexes.Clear();

            //for (int i = 0; i < pageGroups.Length; i++)//刷新一下
            //{
            //    if (!pageGroups[i].CanShowInBook)
            //    {
            //        pageGroupIndexes.Add(pageGroups[i],count);
            //        continue;
            //    }

            //    for (int j = 0; j < pageGroups[i].Pages.Length; j++)
            //    {
            //        if (pageGroups[i].Pages[j].CanShowInBook)
            //            count++;
            //    }

            //    pageGroupIndexes.Add(pageGroups[i], count);
            //}
        }

        /// <summary>
        /// 少遍历点，省点CPU
        /// </summary>
        public override void RecalculateChildren()
        {
            if (CurrentDrawingPage >= Pages.Count || Pages.Count == 0)
                return;
            Pages[CurrentDrawingPage]?.Recalculate();
            if (Pages.Count > CurrentDrawingPage + 1)
                Pages[CurrentDrawingPage + 1]?.Recalculate();
        }

        public int GetPageIndex<T>() where T : UIPage => Pages.FindIndex(n => n is T);

        /// <summary>
        /// 初始化尺寸，请保证你调用了这个方法
        /// </summary>
        public void InitSize()
        {
            Width.Set(PanelTex.Width(), 0f);
            Height.Set(PanelTex.Height(), 0f);
        }

        /// <summary>
        /// 初始化位置，请保证你调用了这个方法
        /// </summary>
        /// <param name="center"></param>
        public void SetPosition()
        {
            //Top.Set(- (PanelTex.Height() * scale / 2), 0.5f);
            //Left.Set( - (PanelTex.Width() * scale / 2), 0.5f);

            this.SetTopLeft(-PanelTex.Height() / 2 + 40, -PanelTex.Width() / 2, 0.5f, 0.5f);
            //this.SetTopLeft(0, 0, 0f, 0f);
            HAlign = 0f;
            VAlign = 0f;
        }

        /// <summary>
        /// 设置所有书页的默认位置
        /// </summary>
        public void InitalizePages()
        {
            float halfPage = PanelTex.Width() / 2f;
            float pageHeigh = PanelTex.Height();
            for (int i = 0; i < Pages.Count; i++)
            {
                //设置尺寸及上下的内页边距
                Pages[i].Width.Set(halfPage-leftPageMargins-rightPageMargins, 0f);
                Pages[i].Height.Set(pageHeigh-topPageMargins-bottomPageMargins, 0f);
                Pages[i].MarginTop = topPageMargins;
                Pages[i].MarginBottom = bottomPageMargins;

                Pages[i].Top.Set(0, 0f);

                if (i % 2 == 0)
                {
                    //设置左侧的位置以及内页边距
                    Pages[i].Left.Set(0, 0f);
                    Pages[i].MarginLeft = leftPageMargins;
                    Pages[i].MarginRight = rightPageMargins;
                }
                else
                {
                    //右侧的页数要反一下
                    Pages[i].Left.Set(halfPage, 0f);
                    Pages[i].MarginRight = leftPageMargins;
                    Pages[i].MarginLeft = rightPageMargins;
                }
            }
        }

        public void AppendPage(UIElement element)
        {
            Append(element);
            Pages.Add(element);
        }

        public bool TryGetGroup<T>(out T group) where T : UIPageGroup
        {
            if (pageGroups == null)
            {
                group = null;
                return false;
            }

            foreach (var g in pageGroups)
            {
                if (g is T t)
                {
                    group = t;
                    return true;
                }
            }

            group = null;
            return false;
        }



        /// <summary>
        /// 前往上一页
        /// </summary>
        public void PreviousPage()
        {
            CurrentDrawingPage -= 2;
            Recalculate();
        }

        /// <summary>
        /// 前往下一页
        /// </summary>
        public void NextPage()
        {
            CurrentDrawingPage += 2;
            Recalculate();
        }

        /// <summary>
        /// 前往指定的页数
        /// </summary>
        /// <param name="index">目标页数</param>
        public void GoToPage(int index)
        {
            CurrentDrawingPage = index % 2 == 0 ? index : index - 1;
            Recalculate();
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);

            if (evt.ScrollWheelValue > 0)
                PreviousPage();
            else
                NextPage();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Terraria.GameInput.PlayerInput.LockVanillaMouseScroll(Coralite.Instance.Name + "/UIBookPanel");
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// 获取页的长宽
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPageSize()
        {
            return new Vector2((PanelTex.Width() / 2f) - leftPageMargins - rightPageMargins, PanelTex.Height() - (topPageMargins + bottomPageMargins));
        }
    }
}
