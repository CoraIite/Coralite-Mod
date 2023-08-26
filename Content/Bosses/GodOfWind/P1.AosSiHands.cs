//using Coralite.Core;
//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria;
//using Terraria.ModLoader;

//namespace Coralite.Content.Bosses.GodOfWind
//{
//    public class AosSiHands:AosSiBodyPart
//    {
//        public override string Texture => AssetDirectory.GodOfWind + "AosSiLeftHand";

//        public static Asset<Texture2D> RightHand;

//        public override void Load()
//        {
//            if (Main.dedServ)
//                return;

//            RightHand = ModContent.Request<Texture2D>(AssetDirectory.GodOfWind + "AosSiRightHand");
//        }

//        public override void Unload()
//        {
//            if (Main.dedServ)
//                return;

//            RightHand = null;
//        }
//    }
//}
