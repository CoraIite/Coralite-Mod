using Coralite.Content.UI.BookUI;
using Coralite.Content.UI.MagikeGuideBook.Chapter1;
using Coralite.Content.UI.MagikeGuideBook.Chapter2;
using Coralite.Content.UI.MagikeGuideBook.Chapter3;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeGuideBook.Introduce
{
    public class C0_P2_Catalog : UIPage
    {
        public override bool CanShowInBook => true;

        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Index { get; private set; }
        public static LocalizedText Chapter1 { get; private set; }
        public static LocalizedText Chapter2 { get; private set; }
        public static LocalizedText Chapter3 { get; private set; }

        public C0_P2_Catalog()
        {
            Append(new Chapter1Jump());
            Append(new Chapter2Jump());
            Append(new Chapter3Jump());
        }

        public override void Recalculate()
        {
            float widdth = PageWidth;

            for (int i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Width.Set(widdth, 0);
                element.Height.Set(50, 0);
                element.Top.Set(80 + i * 50, 0);
                element.Left.Set(10, 0);
            }

            base.Recalculate();
        }

        public override void OnInitialize()
        {
            Index = this.GetLocalization("Index", () => "目录");
            Chapter1 = this.GetLocalization("Chapter1", () => "-第一章 认识魔能-");
            Chapter2 = this.GetLocalization("Chapter2", () => "-第二章 探索发现-");
            Chapter3 = this.GetLocalization("Chapter3", () => "-第三章 基础仪器-");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Index.Value, PageTop + new Vector2(0,60), Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);
        }
    }

    public class Chapter1Jump : PageJumpButton
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_ChapterName>();
        public virtual string Text => C0_P2_Catalog.Chapter1.Value;

        public Chapter1Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagikeButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagikeButton_Outline"));
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            int index = PageToJump;
            if (index >= 0)
            {
                MagikeGuideBookUI.BookPanel.currentDrawingPage = index;
                UILoader.GetUIState<MagikeGuideBookUI>().Recalculate();
                Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);
            }
            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            const float r = 40;
            Vector2 pos = GetDimensions().Position() + new Vector2(r, Height.Pixels / 2);

            float scale = GetScale();

            spriteBatch.Draw(texture.Value, pos, null, Color.White, 0, texture.Size() / 2, scale, 0, 0);
            Color textColor = Color.White;
            if (IsMouseHovering)
            {
                spriteBatch.Draw(borderTexture.Value, pos, null, Color.White, 0, borderTexture.Size() / 2, scale, 0, 0);
                textColor = Coralite.Instance.MagicCrystalPink;
            }

            pos += new Vector2(r, 4);
            Utils.DrawBorderString(spriteBatch, Text, pos, textColor, scale, anchory: 0.5f);
        }
    }

    public class Chapter2Jump: Chapter1Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C2_ChapterName>();
        public override string Text => C0_P2_Catalog.Chapter2.Value;

        public Chapter2Jump() : base() { }
    }

    public class Chapter3Jump : Chapter1Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C3_ChapterName>();
        public override string Text => C0_P2_Catalog.Chapter3.Value;

        public Chapter3Jump() : base() { }
    }
}
