using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
        public Asset<Texture2D> PanelTex;
        public readonly int topPageMargins;
        public readonly int bottomPageMargins;
        public readonly int leftPageMargins;
        public readonly int rightPageMargins;

        public float scale;
        public float alpha;

        public UIPageGroup[] pageGroups;
        //public Dictionary<UIPageGroup,int> pageGroupIndexes;

        /// <summary>
        /// 左边那一页的ID
        /// </summary>
        public int currentDrawingPage;

        /// <summary>
        /// 请注意！！！
        /// <br>左右页边距以书的左侧页为准，别写反了</br>
        /// </summary>
        /// <param name="PanelTex">面板贴图</param>
        /// <param name="topPageMargins">顶部页边距</param>
        /// <param name="leftPageMargins">左侧内页边距</param>
        /// <param name="rightPageMargins">右侧内页边距</param>
        public UI_BookPanel(Asset<Texture2D> PanelTex, int topPageMargins, int bottomPageMargins, int leftPageMargins, int rightPageMargins, float scale = 1f)
        {
            this.PanelTex = PanelTex;
            this.topPageMargins = (int)(topPageMargins * scale);
            this.bottomPageMargins = (int)(bottomPageMargins * scale);
            this.leftPageMargins = (int)(leftPageMargins * scale);
            this.rightPageMargins = (int)(rightPageMargins * scale);
            this.scale = scale;
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

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //就只是绘制面板
            if (!OverflowHidden)
                DrawPanel(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            //存在该页，并且不在渐入渐出的时候才会绘制
            if (OverflowHidden)
            {
                DrawPanel(spriteBatch);
                return;
            }

            if (alpha == 1f)
            {
                bool hasRightPage = Elements.Count > currentDrawingPage + 1;

                Elements[currentDrawingPage]?.Draw(spriteBatch);
                if (hasRightPage)
                    Elements[currentDrawingPage + 1]?.Draw(spriteBatch);

                //绘制Non，先判断一下是否继承了接口然后再进行绘制
                bool shouldDrawLeftNon = Elements[currentDrawingPage] is IDrawNonPremultiplied;
                bool shouldDrawRightNon = false;
                if (hasRightPage)
                    shouldDrawRightNon = Elements[currentDrawingPage + 1] is IDrawNonPremultiplied;

                if (shouldDrawLeftNon || shouldDrawLeftNon)     //如果都不是的话那就不浪费CPU去end begin了
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap,
                                    spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                    if (shouldDrawLeftNon)
                        (Elements[currentDrawingPage] as IDrawNonPremultiplied).DrawNonPremultiplied(spriteBatch);
                    if (shouldDrawRightNon)
                        (Elements[currentDrawingPage + 1] as IDrawNonPremultiplied).DrawNonPremultiplied(spriteBatch);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                }

            }
        }

        public void DrawPanel(SpriteBatch spriteBatch)
        {
            Vector2 position = GetDimensions().Position();
            spriteBatch.Draw(PanelTex.Value, position, null, Color.White * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public override void Recalculate()
        {
            Elements.Clear();

            for (int i = 0; i < pageGroups.Length; i++)//刷新一下
            {
                if (!pageGroups[i].CanShowInBook)
                    continue;
                for (int j = 0; j < pageGroups[i].Pages.Length; j++)
                    if (pageGroups[i].Pages[j].CanShowInBook)
                        Append(pageGroups[i].Pages[j]);
            }

            //防止出现越界的情况
            if (Elements.Count >= 2)
                currentDrawingPage = Math.Clamp(currentDrawingPage, 0, Elements.Count - 1);
            else
                currentDrawingPage = 0;
            currentDrawingPage = currentDrawingPage / 2 * 2;//利用神奇算法让它变为偶数

            //设置可以控制的UI页
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                UIElement Element = Elements[i];
                if (i == currentDrawingPage || i == (currentDrawingPage + 1))
                {
                    Element.IgnoresMouseInteraction = false;
                    continue;
                }

                Element.IgnoresMouseInteraction = true;
            }

            SetIndexes();
            InitalizePages();

            base.Recalculate();
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
            if (currentDrawingPage >= Elements.Count || Elements.Count == 0)
                return;
            Elements[currentDrawingPage]?.Recalculate();
            if (Elements.Count > currentDrawingPage + 1)
                Elements[currentDrawingPage + 1]?.Recalculate();
        }

        public int GetPageIndex<T>() where T : UIPage => Elements.FindIndex(n => n is T);

        /// <summary>
        /// 初始化尺寸，请保证你调用了这个方法
        /// </summary>
        public void InitSize()
        {
            Width.Set(PanelTex.Width() * scale, 0f);
            Height.Set(PanelTex.Height() * scale, 0f);
        }

        /// <summary>
        /// 初始化位置，请保证你调用了这个方法
        /// </summary>
        /// <param name="center"></param>
        public void SetPosition(Vector2 center)
        {
            Top.Set(center.Y - (PanelTex.Height() * scale / 2), 0f);
            Left.Set(center.X - (PanelTex.Width() * scale / 2), 0f);
        }

        /// <summary>
        /// 设置所有书页的默认位置
        /// </summary>
        public void InitalizePages()
        {
            float halfPage = PanelTex.Width() * scale / 2f;
            float pageHeigh = PanelTex.Height() * scale;
            for (int i = 0; i < Elements.Count; i++)
            {
                //设置尺寸及上下的内页边距
                Elements[i].Width.Set(halfPage, 0f);
                Elements[i].Height.Set(pageHeigh, 0f);
                Elements[i].PaddingTop = topPageMargins;
                Elements[i].PaddingBottom = topPageMargins;

                Elements[i].Top.Set(0, 0f);

                if (i % 2 == 0)
                {
                    //设置左侧的位置以及内页边距
                    Elements[i].Left.Set(0, 0f);
                    Elements[i].PaddingLeft = leftPageMargins;
                    Elements[i].PaddingRight = rightPageMargins;
                }
                else
                {
                    //右侧的页数要反一下
                    Elements[i].Left.Set(halfPage, 0f);
                    Elements[i].PaddingRight = leftPageMargins;
                    Elements[i].PaddingLeft = rightPageMargins;
                }
            }
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
            currentDrawingPage -= 2;
            Recalculate();
        }

        /// <summary>
        /// 前往下一页
        /// </summary>
        public void NextPage()
        {
            currentDrawingPage += 2;
            Recalculate();
        }

        /// <summary>
        /// 前往指定的页数
        /// </summary>
        /// <param name="index">目标页数</param>
        public void GoToPage(int index)
        {
            currentDrawingPage = index % 2 == 0 ? index : index - 1;
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
            return new Vector2((PanelTex.Width() * scale / 2) - leftPageMargins, (PanelTex.Height() * scale) - (2 * topPageMargins));
        }
    }
}
