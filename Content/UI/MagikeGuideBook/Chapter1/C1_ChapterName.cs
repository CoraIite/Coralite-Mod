using Coralite.Content.UI.BookUI;
using Coralite.Content.UI.MagikeGuideBook.Introduce;
using Coralite.Content.UI.UILib;
using Coralite.Core.Loaders;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_ChapterName : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText ChapterName { get; private set; }

        public static LocalizedText History { get; private set; }
        public static LocalizedText Property { get; private set; }
        public static LocalizedText Classify { get; private set; }
        public static LocalizedText Origin { get; private set; }
        public static LocalizedText Function { get; private set; }
        public static LocalizedText Instrument { get; private set; }

        public C1_ChapterName()
        {
            Append(new C1_History_Jump());
            Append(new C1_Property_Jump());
            Append(new C1_Classify_Jump());
            Append(new C1_Origin_Jump());
            Append(new C1_Function_Jump());
            Append(new C1_Instrument_Jump());
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
            ChapterName = this.GetLocalization("ChapterName", () => "第一章 认识魔能");
            History = this.GetLocalization("History", () => "-魔能的历史-");
            Property = this.GetLocalization("Property", () => "-魔能的性质-");
            Classify = this.GetLocalization("Classify", () => "-魔能的分类-");
            Origin = this.GetLocalization("Origin", () => "-魔能的来源-");
            Function = this.GetLocalization("Function", () => "-魔能的作用-");
            Instrument = this.GetLocalization("Instrument", () => "-认识魔能仪器-");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, ChapterName.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);
        }
    }

    public class C1_History_Jump : PageJumpButton
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_1_1_History1>();
        public virtual string Text => C1_ChapterName.History.Value;

        public C1_History_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "PageButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "PageButton_Outline"));
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

    public class C1_Property_Jump : C1_History_Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_1_2_Property>();
        public override string Text => C1_ChapterName.Property.Value;

        public C1_Property_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagikeButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagikeButton_Outline"));
        }
    }

    public class C1_Classify_Jump : C1_History_Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_1_3_Classify>();
        public override string Text => C1_ChapterName.Classify.Value;

        public C1_Classify_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "TalantosInABottleButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "TalantosInABottleButton_Outline"));
        }
    }

    public class C1_Origin_Jump : C1_History_Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_2_Origin1>();
        public override string Text => C1_ChapterName.Origin.Value;

        public C1_Origin_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagicCrystalButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "MagicCrystalButton_Outline"));
        }
    }

    public class C1_Function_Jump : C1_History_Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_3_Function>();
        public override string Text => C1_ChapterName.Function.Value;

        public C1_Function_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "CrystalStaffButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "CrystalStaffButton_Outline"));
        }
    }

    public class C1_Instrument_Jump : C1_History_Jump
    {
        public override int PageToJump => MagikeGuideBookUI.BookPanel.GetPageIndex<C1_4_1_WhatIsInstrument>();
        public override string Text => C1_ChapterName.Instrument.Value;

        public C1_Instrument_Jump()
        {
            SetImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "CrystalLensButton"));
            SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "CrystalLensButton_Outline"));
        }
    }
}
