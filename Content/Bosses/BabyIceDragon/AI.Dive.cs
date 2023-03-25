using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
   public partial class BabyIceDragon
   {
      /// <summary>
      /// 俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
      /// </summary>
      public void Dive()
      {
         switch (movePhase)
         {
            case 0:     //飞上去的阶段
               {
                  if (NPC.Center.Y > Target.Center.Y + 200)
                  {
                     FlyUp();

                     if (Timer > 400)
                        ChangeToDive();

                     break;
                  }

                  ChangeToDive();
               }

               break;
            default:
            case 1:    //俯冲阶段
               do
               {
                  if (Timer < 3)
                     SetDirection();

                  if (Timer == 3)
                  {
                     NPC.velocity.X = NPC.direction * 8f;
                     NPC.velocity.Y = 1f;
                     NPC.rotation = NPC.velocity.ToRotation();
                  }

                  if (Timer < 250)
                  {
                     //生成粒子

                     //检测面前的物块，如果有物块那么就会撞晕自己
                     Point position = (NPC.direction > 0 ? NPC.TopLeft : NPC.TopRight).ToPoint();
                     for (int i = 0; i < 3; i++)
                     {
                        Tile tile = Framing.GetTileSafely(position);
                        if (tile.HasSolidTile())
                        {
                           int dizzyTime = Main.masterMode ? 180 : 300;
                           NPC.velocity.X *= -1;
                           NPC.velocity.Y = -3f;
                           Dizzy(dizzyTime);
                           return;
                        }

                        position.Y += 1;
                     }
                     break;
                  }

                  if (Timer < 300)
                  {
                     NPC.velocity *= 0.99f;
                     break;
                  }

                  ResetStates();

               } while (false);

               break;
         }

         Timer++;
      }

      public void ChangeToDive()
      {
         if (Main.myPlayer == NPC.target)
         {
            bool canDive = NPC.Center.Y > Target.Center.Y + 100;

            if (canDive)
            {
               //前往下潜攻击
               movePhase = 1;
               Timer = 0;
               NPC.netUpdate = true;
            }
            else
               //结束该行动
               ResetStates();
         }
      }
   }
}
