using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class C1_1_3_Classify : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText _1_3_Name { get; private set; }
        public static LocalizedText Classify { get; private set; }

        public override void OnInitialize()
        {
            _1_3_Name = this.GetLocalization("_1_3_Name", () => "1.1.3 魔能的分类");
            Classify = this.GetLocalization("Classify", () => "    大部分装置中使用的是最普通的，没有什么特质的魔能。但随着科技的发展，人们从这种普通的魔能中分离出了拥有特殊性质的魔能，并给这些特殊的魔能冠以一些特殊名字，例如：植生，冻，灼等。对于这些特殊魔能将在之后的章节详细说明他们。");
        }

        public override bool CanShowInBook => true;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_1_3_Classify").Value;
            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -40), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), PageWidth / mainTex.Width, 0, 0);

            Vector2 pos = Position + new Vector2(0, 30);
            Utils.DrawBorderString(spriteBatch, _1_3_Name.Value, pos, Coralite.Instance.MagicCrystalPink
                , 1, 0f, 00f);
            pos = PageTop + new Vector2(0, 60 + 30);

            Vector2 textPos = new Vector2(Position.X, pos.Y);
            Helpers.Helper.DrawText(spriteBatch, Classify.Value, PageWidth, textPos, Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out _);
        }
    }
}
