using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_P1_ChapterName:UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText ChapterName { get; private set; }
        public static LocalizedText Author { get; private set; }

        public override void OnInitialize()
        {
            ChapterName = this.GetLocalization("ChapterName", () => "第一章 认识魔能");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, ChapterName.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);


        }
    }
}
