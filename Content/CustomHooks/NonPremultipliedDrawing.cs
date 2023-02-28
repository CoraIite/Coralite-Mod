//using Coralite.Core;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;

//namespace Coralite.Content.CustomHooks
//{
//    class NonPremultipliedDrawing : HookGroup
//    {
//        public override SafetyLevel Safety => SafetyLevel.Safe;

//        public override void Load()
//        {
//            if (Main.dedServ)
//                return;

//            On.Terraria.Main.DrawDust += DrawNonPremultiplied;
//        }

//        private void DrawNonPremultiplied(On.Terraria.Main.orig_DrawDust orig, Main self)
//        {
//            orig(self);


//        }
//    }
//}