
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public partial class BabyIceDragon
    {
        public void SmashDown()
        {
            switch (movePhase)
            {
                case 0:
                    if (NPC.Center.Y - Target.Center.Y > -240)
                    {
                        FlyUp();
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3, 0.08f, 0.08f, 0.96f);
                        if (Timer > 300)
                        {
                            ResetStates();
                            return;
                        }
                    }
                    else
                    {
                        movePhase = 1;
                        Timer = 0;
                        NPC.velocity.X *= 0;
                        NPC.velocity.Y = -3;
                        NPC.noGravity = false;
                        NPC.noTileCollide = true;
                        SetDirection();
                        NPC.netUpdate = true;
                    }

                    break;
                case 1:
                    NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation(), 0.04f);

                    //向玩家加速
                    if (Timer < 40)
                    {
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3, 0.08f, 0.08f, 0.96f);
                        break;
                    }

                    //TODO:生成下落特效的粒子
                    //下落
                    NPC.velocity.X *= 0.96f;
                    NPC.velocity.Y += 0.02f;
                    if (NPC.velocity.Y > 16)
                        NPC.velocity.Y = 16;

                    if (Timer < 60)
                        break;

                    if (Timer == 60)
                        NPC.noTileCollide = false;

                    if (Timer > 60)
                    {
                        //检测下方物块
                        Point position = NPC.BottomLeft.ToPoint();
                        position.Y += 1;
                        for (int i = 0; i < 3; i++)
                        {
                            if (WorldGen.ActiveAndWalkableTile(position.X, position.Y))    //砸地，生成冰刺弹幕
                            {
                                SpawnIceThorns();
                                movePhase = 2;
                                Timer = 0;
                                NPC.netUpdate = true;
                            }
                            position.X += 1;
                        }
                    }

                    if (Timer > 300)
                    {
                        NPC.velocity *= 0;
                        int restTime2 = Main.masterMode ? 80 : 140;
                        HaveARest(restTime2);
                    }

                    break;
                default:
                    ResetStates();
                    return;
            }

            Timer++;
        }
    }
}
