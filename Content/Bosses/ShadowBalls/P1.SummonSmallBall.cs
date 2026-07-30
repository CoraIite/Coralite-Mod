using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        public void SummonSmallBall()
        {
            /*
             * 先检测小球的数量
             * 
             * 如果还足够就直接跳过
             * 不足就生成
             * 
             * 1.将锁链环的旋转调整到平行并不动
             * 2.重复AI，选择还存活的锁链将其弹出（此时生成小影子球NPC）
             */

            switch (SonState)
            {
                default:
                case 0://检测小球数量，并检测锁链环
                    {
                        //int maxSmallBall = GetSmallBallSameTimeLimit();

                        //检测现在是否能继续生成，不能生成就直接跳过
                        int canSpawnCount = 0;
                        foreach (var shadowLock in shadowLocks)
                        {
                            if (shadowLock.active)
                            {
                                canSpawnCount++;
                            }
                        }

                        if (canSpawnCount < 1)//直接跳过，切换二阶段
                        {
                            return;
                        }

                        //if (GetSmallBalls())//有小球
                        //{
                        //    //现有小球数量不足50%，生成新的填满
                        //    if (smallBalls.Count < maxSmallBall / 2)
                        //    {


                        //    }
                        //}
                    }
                    break;
                case 1:
                    break;
            }
        }
    }
}
