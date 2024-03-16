using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Introduce
{
    public class C0_P1_BookName : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText BookName { get; private set; }
        public static LocalizedText Author { get; private set; }

        public override void OnInitialize()
        {
            BookName = this.GetLocalization("BookName", () => "魔能辞典");
            Author = this.GetLocalization("Author", () => "作者：珊瑚");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, BookName.Value, Center, Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);

            Utils.DrawBorderString(spriteBatch, Author.Value, Bottom + new Vector2(0, -80), Color.White, 1, 0.5f, 1f);
        }
    }
}
