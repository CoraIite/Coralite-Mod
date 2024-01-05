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
    public class C2_1_CrystalCave3 : FragmentPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Date { get; private set; }
        public static LocalizedText CrystalCave1 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月18日】");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    午后在向下探索时发现了从未见过的怪物，它身上的水晶散发出粉色的光芒，一定是来到水晶洞附近了。可是它的实力太过于强大，发出的激光更是引发了洞内部分坍塌。由于怪物的冲击，我和大伙走散了，这可怎么办啊！唯一的好消息是附近出现了粉色晶体和没见过的黑色石头，明明曾经来探索过多次都没发现这样的东西，看样子现在的坐标就在水晶洞附近。");
        }

        public override bool CanShowInBook => MagikeSystem.MagikeCave_3;

        protected override void DrawOthers(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note3").Value;
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
