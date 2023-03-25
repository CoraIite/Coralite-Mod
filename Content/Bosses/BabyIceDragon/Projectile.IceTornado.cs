using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
   /// <summary>
   /// 请使用Projectile.ai[0]以控制存活时间
   /// </summary>
    public class IceTornado:ModProjectile
    {
      public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

      public override bool ShouldUpdatePosition() => false;

      public override void SetDefaults()
      {
         Projectile.width = Projectile.height = 64;
         Projectile.maxPenetrate = -1;
         Projectile.aiStyle = -1;
         Projectile.timeLeft = 1000;
         Projectile.hostile = true;
         Projectile.tileCollide = false;
         Projectile.ignoreWater = true;
      }

      public override void OnSpawn(IEntitySource source)
      {
         Projectile.timeLeft = (int)Projectile.ai[0];
      }

      public override bool PreDraw(ref Color lightColor) => false;

   }
}