using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
   public partial class BabyIceDragon
   {
      public const int IceTornadoStartTime = 60;

      public void IceTornado()
      {
         do
         {
            if (Timer < IceTornadoStartTime)
            {
               NPC.velocity *= 0.99f;
               break;
            }

            if (Timer == IceTornadoStartTime)
            {
               //TODO: 生成闪光粒子和音效
               NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 12;
            }

            if (Timer < 500)
            {
               Vector2 targetDir = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
               NPC.velocity = Vector2.Lerp(NPC.velocity, targetDir * 12, 0.2f);
               //生成透明弹幕
               if (Timer % 10 == 0)
               {
                  Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceTornado>(), 30, 8f, ai0: 120);
               }

               //生成龙卷风粒子
               if (Timer % 4 == 0)
               {

               }

               break;
            }

            if (Timer < 530)
            {
               NPC.velocity *= 0.99f;
               break;
            }

            HaveARest(60);

         } while (false);
         Timer++;
      }
   }
}