//using Coralite.Core;
//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ModLoader;

//namespace Coralite.Content.Projectiles.Projectiles_Shoot
//{
//    public class BarrageMicProj : ModProjectile
//    {
//        private string barrageMessage;

//        public override string Texture => AssetDirectory.DefaultItem;

//        public override void SetDefaults()
//        {
//            Projectile.friendly = true;

//            Projectile.aiStyle = -1;
//            Projectile.penetrate = -1;
//        }

//        public override void Initialize()
//        {
//            barrageMessage = Main.rand.Next(7) switch
//            {
//                1 => "宫廷玉液酒，一百八一杯",
//                2 => "群英荟萃？我看是萝卜开会",
//                3 => "是你把敌人引到这来的？",
//                4 => "改革春风吹满地",
//                5 => "要啥自行车？",
//                6 => "下蛋公鸡公鸡中的战斗机 欧耶",
//                _ => "小锤40，大锤80",
//            };
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            if (barrageMessage != null)
//                Utils.DrawBorderString(Main.spriteBatch, barrageMessage, Projectile.position, Color.White);

//            return false;
//        }
//    }
//}
