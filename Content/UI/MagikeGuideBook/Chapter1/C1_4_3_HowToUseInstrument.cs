//using Coralite.Content.UI.UILib;
//using Coralite.Core;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
//{
//    public class C1_4_3_HowToUseInstrument : UIPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText _4_3_Name { get; private set; }
//        public static LocalizedText HowToUse { get; private set; }

//        public override void OnInitialize()
//        {
//            _4_3_Name = this.GetLocalization("_4_3_Name", () => "1.4.3 魔能仪器的使用");
//            HowToUse = this.GetLocalization("HowToUse", () => "    具有发送能力的装置需要使用魔能链接仪将他们连接起来才能传输魔能。能够执行特定工作的“魔能工厂”需要使用魔能激活杖或电路将其激活后才能开始工作。具体使用方法点击下方按钮跳转到指定位置。");
//        }

//        public override bool CanShowInBook => true;

//        protected override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_4_2_ClassifyOfInstrument").Value;
//            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -60), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), PageWidth / mainTex.Width, 0, 0);

//            Vector2 pos = Position + new Vector2(0, 30);
//            Utils.DrawBorderString(spriteBatch, _4_3_Name.Value, pos, Coralite.MagicCrystalPink
//                , 1, 0f, 00f);
//            pos = PageTop + new Vector2(0, 60 + 30);

//            Helpers.Helper.DrawText(spriteBatch, HowToUse.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out _);
//        }
//    }
//}
