//using Coralite.Core;
//using Coralite.Core.Systems.SwingWeapon;

//namespace Coralite.Content.Items.ShieldPlus
//{
//    public class EternalVengeance:ModItem
//    {
//        public override string Texture => AssetDirectory.ShieldPlusItems + Name;

//    }

//    public class EternalVengeanceSword : SwingProjMK2
//    {
//        public override string Texture => AssetDirectory.ShieldPlusItems + Name;

//        public enum SwingState
//        {
//            Down1,
//            Down2,
//            Spurt1,
//            DashSpurt,
//            DashUp,
//            Rolling1,

//            Finish,
//        }

//        public override SwingController GetSwingController()
//        {
//            return null;

//            switch ((SwingState)Combo)
//            {
//                case SwingState.Down1:
//                    break;
//                case SwingState.Down2:
//                    break;
//                case SwingState.Spurt1:
//                    break;
//                case SwingState.Rolling1:
//                    break;
//                case SwingState.Finish:
//                    break;
//                default:
//                    Projectile.Kill();
//                    return null;
//            }
//        }
//    }
//}
