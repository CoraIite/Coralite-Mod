using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

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
                        if (NPC.Center.Y > (Target.Center.Y - 460))
                        {
                            SetDirection();
                            NPC.velocity.X *= 0.97f;
                            NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                            FlyUp();

                            if (Timer > 400)
                            {
                                ResetStates();
                                return;
                            }

                            break;
                        }

                        //前往下潜攻击
                        SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        movePhase = 1;
                        NPC.frame.Y = 0;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                default:
                case 1:    //俯冲阶段
                    do
                    {
                        if (Timer < 3)
                            SetDirection();

                        if ((int)Timer == 3)
                        {
                            NPC.velocity = (Target.Center - new Vector2(0, 30) - NPC.Center).SafeNormalize(Vector2.Zero) * 13f;
                            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f);
                            canDrawShadows = true;
                            InitCaches();
                        }

                        if (Timer < 100)
                        {
                            //生成粒子
                            if (NPC.Center.Y > (Target.Center.Y - 20))
                            {
                                Timer = 100;
                                NPC.netUpdate = true;
                                break;
                            }

                            //检测面前的物块，如果有物块那么就会撞晕自己
                            GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter);
                            for (int i = -1; i < 2; i++)
                            {
                                Vector2 position = mouseCenter + i * 16 * targetDir.RotatedBy(1.57f);
                                Tile tile = Framing.GetTileSafely(position);
                                if (tile.HasSolidTile())
                                {
                                    Dizzy(300);
                                    return;
                                }
                            }
                            break;
                        }

                        if (Timer < 130)
                        {
                            canDrawShadows = false;
                            NPC.velocity *= 0.95f;
                            NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                            ChangeFrameNormally();
                            break;
                        }

                        ResetStates();

                    } while (false);

                    break;
            }

            Timer++;
        }
    }
}
