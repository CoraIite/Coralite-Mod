using Coralite.Content.UI.BookUI;
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

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_ChapterName : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText ChapterName { get; private set; }
        public static LocalizedText CrystalCave { get; private set; }

        public C2_ChapterName()
        {
            Append(new C2_CrystalCave_Jump());
        }

        public override void Recalculate()
        {
            float widdth = PageWidth;

            for (int i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.Width.Set(widdth, 0);
                element.Height.Set(50, 0);
                element.Top.Set(120 + i * 50, 0);
                element.Left.Set(10, 0);
            }

            base.Recalculate();
        }

        public override void OnInitialize()
        {
            ChapterName = this.GetLocalization("ChapterName", () => "第二章 探索发现");
            CrystalCave = this.GetLocalization("CrystalCave", () => "发现魔力水晶洞");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, ChapterName.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);
        }
    }

    public class C2_CrystalCave_Jump : PageJumpButton
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C2_1_CrystalCave7>();
        public virtual string Text => C2_ChapterName.CrystalCave.Value;
        public bool CanShow;

        public C2_CrystalCave_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "PageButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "PageButton_Outline"));
        }

        public override void Recalculate()
        {
            RegetCanShow();
            base.Recalculate();
        }

        public virtual void RegetCanShow()
        {
            if (MagikeGuideBookUI.BookPanel.TryGetGroup(out Chapter2Group c2p))
                if (c2p.TryGetPage(out C2_1_CrystalCave7 c21cc7))
                    CanShow = c21cc7.CanShowInBook;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!CanShow)
                return;

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
            string drawText = Text;
            if (!CanShow)
                drawText = "? ? ?";
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
            Utils.DrawBorderString(spriteBatch, drawText, pos, textColor, scale, anchory: 0.5f);
        }
    }
}