//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
//{
//    public class C2_1_CrystalCave5 : FragmentPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText Date { get; private set; }
//        public static LocalizedText CrystalCave1 { get; private set; }

//        public override void OnInitialize()
//        {
//            Date = this.GetLocalization("Date", () => "【泰拉历235年11月22日】");
//            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    成了成了！捣鼓了几天后终于有了成果，经过这几天的加工，打磨成椭圆形后的魔力晶体呈现出了特殊的性质，能够近地悬浮一小段时间，这可真神奇！有时能这样一个人沉浸在研究中也蛮不错的，这样就不会去胡思乱想了。");
//        }

//        public override bool CanShowInBook => MagikeSystem.MagikeCave_5;

//        protected override void DrawOthers(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Note5").Value;
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
