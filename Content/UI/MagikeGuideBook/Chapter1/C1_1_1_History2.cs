//using Coralite.Content.UI.UILib;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria.Localization;

//namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
//{
//    public class C1_1_1_History2 : UIPage
//    {
//        public override string LocalizationCategory => "MagikeSystem";

//        public static LocalizedText History2 { get; private set; }

//        public override void OnInitialize()
//        {
//            History2 = this.GetLocalization("History2", () => "    很快，他们学会了使用这种神奇的资源，并逐步在战争中取得了优势。但聪明的妖精族也掌控了这项技术。最终，人取得了胜利，尽管是因为我偷偷与其他种族签订了和平条约，不过这不重要。在漫长的和平岁月中，人们发展了商业，建立了城市，利用魔能制作出各种不可思议的装置，他们在飞速地发展着。");
//        }

//        public override bool CanShowInBook => true;

//        protected override void DrawSelf(SpriteBatch spriteBatch)
//        {
//            Vector2 pos = Position + new Vector2(0, 40);
//            Helpers.Helper.DrawText(spriteBatch, History2.Value, PageWidth, pos, Vector2.Zero, Vector2.One
//                , new Color(40, 40, 40), Color.White, out _);


//        }
//    }
//}
