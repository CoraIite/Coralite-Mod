//using Terraria;
//using Terraria.ID;

//namespace Coralite.Core.Systems.MTBStructure
//{
//    public class TextMTBS : MultBlockStructure
//    {
//        private const int S = TileID.Stone;

//        public override int[,] StructureTile => new int[5, 5]
//            {
//                {-1,-1,S,-1,-1 },
//                {-1,S,-1,S,-1 },
//                {S,-1,ModContent.TileType<TestCenter>(),-1,S },
//                {-1,S,-1,S,-1 },
//                {-1,-1,S,-1,-1 },
//            };

//        public override void OnSuccess()
//        {
//            PopupText.NewText(new AdvancedPopupRequest()
//            {
//                Color = Color.Red,
//                Text = "成功！",
//                DurationInFrames = 60,
//                Velocity = -Vector2.UnitY
//            }, Main.LocalPlayer.Center - (Vector2.UnitY * 32));
//        }
//    }
//}
