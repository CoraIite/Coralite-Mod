using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_1_CrystalCave6 : FragmentPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Date { get; private set; }
        public static LocalizedText CrystalCave1 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月23日】");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    在研究这些魔力水晶时，我发现附近的玄武岩也有一些反应，于是我尝试制作了一个粗糙的底座，并把椭圆形晶体放置上去，结果这个晶体发出了耀眼的粉色光芒，差点引来那些怪物。我决定先把研究成果带回地面上去，这里实在是太危险了。");
        }

        public override bool CanShowInBook => MagikeSystem.MagikeCave_6;

        protected override void DrawOthers(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note6").Value;
            spriteBatch.Draw(mainTex, BottomRight + new Vector2(2, -58), null, new Color(50, 0, 28), 0, mainTex.Size(), 1, 0, 0);
            spriteBatch.Draw(mainTex, BottomRight + new Vector2(0, -60), null, Color.White, 0, mainTex.Size(), 1, 0, 0);

            Vector2 pos = Position + new Vector2(0, 60);
            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0f, 0f);

            pos += new Vector2(0, 60);

            //文字段1
            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 10);
        }
    }
}
