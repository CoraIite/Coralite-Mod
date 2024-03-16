using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_1_History3 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText History3 { get; private set; }

        public override void OnInitialize()
        {
            History3 = this.GetLocalization("History3", () => "    但变化发生地就是如此的突然，和我一样来自其他世界的强大生物坠落到了这里。就只是因为它们之间无聊的争斗！我尽力重创并赶走了它们，可一切还是晚了，不仅是人们，连其他种族也遭受重创。整个大陆变得一片死寂。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(0, 40);
            Helpers.Helper.DrawText(spriteBatch, History3.Value, PageWidth, pos, Vector2.Zero, Vector2.One, new Color(40, 40, 40), Color.White, out _);


        }
    }
}
