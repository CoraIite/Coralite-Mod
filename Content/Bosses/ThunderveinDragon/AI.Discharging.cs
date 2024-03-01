using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void Discharging()
        {
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://蓄力，微微靠近玩家
                    {
                        //由于已经是离玩家很近才会使用的招式所以直接微微靠近

                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 6f, 0.15f, 0.4f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.35f, 15, 0.9f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.15f, 0.3f, 0.95f);
                            FlyingFrame();
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.95f;
                            FlyingFrame();
                        }

                        if (NPC.frame.Y == 0)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://挥下翅膀
                    {
                        NPC.velocity *= 0.96f;
                        if (NPC.frame.Y != 4)
                            FlyingFrame();
                        else
                        {
                            Timer++;
                            if (Timer > 10)
                            {
                                SonState++;
                                Timer = 0;
                                //生成爆炸弹幕
                            }
                        }
                    }
                    break;
                case 2://爆！！！！！！！！！
                    {

                    }
                    break;
                case 3://短暂后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 20)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
