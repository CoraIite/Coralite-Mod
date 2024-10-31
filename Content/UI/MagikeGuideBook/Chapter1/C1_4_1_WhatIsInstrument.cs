//using Coralite.Content.UI.UILib;
//using Coralite.Core;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
//{
//    public class C1_4_1_WhatIsInstrument : UIPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText _4_Name { get; private set; }
//        public static LocalizedText _4_1_Name { get; private set; }
//        public static LocalizedText WhatIsInstrument { get; private set; }

//        public override void OnInitialize()
//        {
//            _4_Name = this.GetLocalization("_4_Name", () => "1.4 认识魔能仪器");
//            _4_1_Name = this.GetLocalization("_4_1_Name", () => "1.4.1 什么是魔能仪器");
//            WhatIsInstrument = this.GetLocalization("WhatIsInstrument", () => "    经过古代人们的不懈研究，发明了许多神奇的机构，将这些机构组合起来便成为了各式各样的魔能仪器。有的仪器能够消耗魔能并执行对应的工作，也有的仅仅是将魔能在不同仪器间传递。这些仪器大多听上去像是光学仪器，因为魔能的传递形式与光的传递形式十分相似。");
//        }

//        public override bool CanShowInBook => true;

//        protected override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_4_1_WhatIsInstrument").Value;
//            spriteBatch.Draw(mainTex, Bottom + new Vector2(0, -40), null, Color.White, 0, new Vector2(mainTex.Width / 2, mainTex.Height), 1.2f, 0, 0);

//            Vector2 pos = PageTop + new Vector2(0, 60);
//            Utils.DrawBorderStringBig(spriteBatch, _4_Name.Value, pos, Coralite.MagicCrystalPink
//                , 0.8f, 0.5f, 0.5f);

//            pos = Position + new Vector2(0, 100);
//            Utils.DrawBorderString(spriteBatch, _4_1_Name.Value, pos, Coralite.MagicCrystalPink
//                , 1, 0f, 00f);

//            pos += new Vector2(0, 60);
//            Helpers.Helper.DrawText(spriteBatch, WhatIsInstrument.Value, PageWidth, pos, Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out _);

//        }
//    }
//}
