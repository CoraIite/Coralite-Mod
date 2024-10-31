//using Coralite.Content.UI.UILib;
//using Coralite.Core;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter4
//{
//    public class C4_ConnectStaff : UIPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText TitleName { get; private set; }
//        public static LocalizedText ConnectStaff1 { get; private set; }
//        public static LocalizedText ConnectStaff2 { get; private set; }

//        public override void OnInitialize()
//        {
//            TitleName = this.GetLocalization("TitleName", () => "魔能连接仪");
//            ConnectStaff1 = this.GetLocalization("ConnectStaff1", () => "    魔能仪器之间需要手动地建立连接，而魔能连接仪就是为此而生的。使用链接仪左键单击一个具有发送功能的魔能仪器，将会唤出一个面板，上面的空位就是该仪器的连接槽位。点击槽位后就可以开始连接了，大部分仪器都有传输距离的限制，超出距离的话连接线将变为红色。");
//            ConnectStaff2 = this.GetLocalization("ConnectStaff2", () => "    点击一个范围内的魔能仪器就可以在两者之间建立起连接，魔能将会自动的从发送器发向接收器。");
//        }

//        public override bool CanShowInBook => true;

//        protected override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            Vector2 pos = PageTop + new Vector2(0, 60);
//            Utils.DrawBorderStringBig(spriteBatch, TitleName.Value, pos, Coralite.MagicCrystalPink
//                , 0.8f, 0.5f, 0.5f);

//            pos += new Vector2(0, 60);

//            //文字1
//            Helper.DrawText(spriteBatch, ConnectStaff1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

//            pos += new Vector2(0, textSize.Y + 10);

//            //文字2
//            Helper.DrawText(spriteBatch, ConnectStaff2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out textSize);

//            pos += new Vector2(0, textSize.Y + 10);

//            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C4_ConnectStaff").Value;
//            var origin = new Vector2(mainTex.Width / 2, 0);
//            spriteBatch.Draw(mainTex, pos + new Vector2(4, 4), null, new Color(50, 0, 28, 150), 0, origin, 0.9f, 0, 0);
//            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, origin, 0.9f, 0, 0);
//        }
//    }
//}
