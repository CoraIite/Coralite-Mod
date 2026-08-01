using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        /// <summary>
        /// 在进入此状态前设置<see cref="Recorder"/>表示将要生成多少个小球
        /// </summary>
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

                        //检测现在是否能继续生成，不能生成就直接跳过
                        int canSpawnCount = 0;
                        foreach (var shadowLock in shadowLocks)
                            if (shadowLock.active)
                                canSpawnCount++;

                        if (canSpawnCount < 1)//直接跳过，切换二阶段
                        {
                            SwitchToP1P2Exchange();
                            return;
                        }

                        SonState = 1;
                        Timer = 0;

                    }
                    break;
                case 1:
                    {
                        //逐渐停下，切换小球状态
                        NPC.velocity *= 0.94f;

                        if (Timer == 10)
                        {
                            SwitchLockState(LockStates.ConcentricCircles);
                        }
                        else if (Timer < 55)
                        {
                            LockDistancePercent = Helper.Lerp(LockDistancePercent, 1.3f, 0.07f);
                        }
                        else if (Timer > 55)
                        {
                            SonState = 2;
                            Timer = 0;
                            LockDistancePercent = 1.3f;
                        }
                    }
                    break;
                case 2://稍微聚拢锁环后将锁推出
                    {
                        const int time1 = 40;
                        const int time2 = 10;

                        if (Timer < time1)
                        {
                            LockDistancePercent = Helper.Lerp(1.3f, 0.8f,Helper.X2Ease( Timer / time1));
                        }
                        else if (Timer < time1+ time2)
                        {
                            LockDistancePercent = Helper.Lerp(0.8f, 1.5f, Helper.BezierEase((Timer - time1) / time2));
                        }
                        else if (Timer == time1 + time2)
                        {
                            if (!VaultUtils.isClient)
                            {
                                //挑选环上的锁扣弹出
                                List<ShadowLock> tempLocks = [];

                                foreach (var shadowLock in shadowLocks)
                                    if (shadowLock.active)
                                        tempLocks.Add(shadowLock);

                                for (int i = 0; i < Recorder; i++)
                                {
                                    ShadowLock lock2 = tempLocks[Main.rand.Next(tempLocks.Count)];

                                    //生成小影子球
                                    NPC smallBall = NPC.NewNPCDirect(NPC.GetSource_FromThis(), lock2.center + lock2.offset, ModContent.NPCType<SmallShadowBall>(), ai0: NPC.whoAmI, target: NPC.target);

                                    //生成影子弹幕追逐小球


                                    lock2.LockOut(smallBall);

                                    tempLocks.Remove(lock2);
                                    if (tempLocks.Count < 1)//没了
                                    {
                                        SpawnOver();
                                        break;
                                    }
                                }
                            }
                        }
                        else if (Timer > time1 + time2)
                            SpawnOver();
                    }
                    break;
                case 3://等待一段时间的小球生成动画
                    {
                        const int waitTime = 60 * 2;

                        LockDistancePercent = Helper.Lerp(1.5f, 1, Helper.BezierEase(Timer  / waitTime));


                        if (Timer > waitTime)
                        {
                            Timer = 0;
                            SonState = 4;
                            LockDistancePercent = 1;
                        }
                    }
                    break;
                case 4://环的状态变回去
                    {
                        if (Timer==2)
                        {
                            SwitchLockState(LockStates.Normal);
                        }
                        else if (Timer>53+100)
                        {
                            SwitchP1State();
                        }
                    }
                    break;
            }

            void SpawnOver()
            {
                Timer = 0;
                SonState = 3;
            }
        }
    }
}
