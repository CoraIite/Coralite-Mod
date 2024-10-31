//using Coralite.Content.UI.UILib;
//using Coralite.Core;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
//{
//    public class C1_3_Function : UIPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText _3_Name { get; private set; }
//        public static LocalizedText Function1 { get; private set; }
//        public static LocalizedText Function2 { get; private set; }

//        public override void OnInitialize()
//        {
//            _3_Name = this.GetLocalization("_3_Name", () => "1.3 魔能的作用");
//            Function1 = this.GetLocalization("Function1", () => "    一些伟大的魔法师不满足于自身魔力的限制，选择了拿起消耗魔能来释放魔法的法杖。尽管魔能不像魔力那样能够随着呼吸而回复，但好处是魔能的应用范围更广。例如魔力晶体杖，使用它制造出的水晶框架比冰雪魔杖制造的碎冰更加稳定，不会自然消失。");
//            Function2 = this.GetLocalization("Function2", () => "    除了这种便携式的物品，其他大部分魔能仪器都需要放置在地上后才能运作，把鼠标放到这些魔能仪器上，就可以直观地看到该仪器存储的魔能量。");
//        }

//        public override bool CanShowInBook => true;

//        protected override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            Vector2 pos = PageTop + new Vector2(0, 60);
//            Utils.DrawBorderStringBig(spriteBatch, _3_Name.Value, pos, Coralite.MagicCrystalPink
//                , 0.8f, 0.5f, 0.5f);

//            pos += new Vector2(0, 60);

//            //文字段1
//            Helpers.Helper.DrawText(spriteBatch, Function1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

//            pos += new Vector2(0, textSize.Y + 30);

//            //插图1
//            Texture2D mainTex1 = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_3_Function1").Value;
//            spriteBatch.Draw(mainTex1, pos, null, Color.White, 0, new Vector2(mainTex1.Width / 2, 0), 1, 0, 0);

//            pos += new Vector2(0, mainTex1.Height + 30);

//            //文字段2
//            Helpers.Helper.DrawText(spriteBatch, Function2.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out textSize);

//            pos += new Vector2(0, textSize.Y + 20);

//            Texture2D mainTex2 = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "C1_3_Function2").Value;
//            spriteBatch.Draw(mainTex2, pos, null, Color.White, 0, new Vector2(mainTex2.Width / 2, 0), 1f, 0, 0);
//        }
//    }
//}
