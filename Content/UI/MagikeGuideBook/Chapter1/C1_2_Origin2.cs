using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_2_Origin2 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Origin3 { get; private set; }

        public override void OnInitialize()
        {
            Origin3 = this.GetLocalization("Origin3", () => "    或许你已经发现了，在天黑后的地表或者魔力水晶洞附近会出现一些水晶风格的怪物。这些依靠魔能驱动的生命体们只会按照设定好的程序运作。击败它们后有概率掉落魔力晶体，这也是魔能的重要来源之一。尽管这些魔能并非天然形成，而是由什么东西制造出来的。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            //文字段1
            Helpers.Helper.DrawText(spriteBatch, Origin3.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);

            //插图1
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_2_Origin3").Value;
            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -60), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), PageWidth / mainTex.Width, 0, 0);
        }
    }
}
