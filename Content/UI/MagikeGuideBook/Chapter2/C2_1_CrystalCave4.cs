//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
//{
//    public class C2_1_CrystalCave4 : FragmentPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText Date { get; private set; }
//        public static LocalizedText CrystalCave1 { get; private set; }

//        public override void OnInitialize()
//        {
//            Date = this.GetLocalization("Date", () => "【泰拉历235年11月19日】");
//            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    躲过了危险的水晶怪物，我冒险冲入水晶洞，这里能见到很多大型的晶簇。在里面走了一会后，我在水晶洞的中心处找到了一处落脚点，这里是个很隐蔽的洞口，应该不会被怪物发现，算我好运。暂时安定下来后我打算试着把周围这些粉色的晶体组合一下，根据珊瑚向我们展示过的方法。但是就这样看上去蛮简单的结构能有如此大的能量输出吗，我还是不太相信。");
//        }

//        public override bool CanShowInBook => MagikeSystem.MagikeCave_4;

//        protected override void DrawOthers(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note4").Value;
//            spriteBatch.Draw(mainTex, BottomRight + new Vector2(2, -58), null, new Color(50, 0, 28), 0, mainTex.Size(), 1, 0, 0);
//            spriteBatch.Draw(mainTex, BottomRight + new Vector2(0, -60), null, Color.White, 0, mainTex.Size(), 1, 0, 0);

//            Vector2 pos = Position + new Vector2(0, 60);
//            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.MagicCrystalPink
//                , 0.8f, 0f, 0f);

//            pos += new Vector2(0, 60);

//            //文字段1
//            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

//            pos += new Vector2(0, textSize.Y + 10);
//        }
//    }
//}
