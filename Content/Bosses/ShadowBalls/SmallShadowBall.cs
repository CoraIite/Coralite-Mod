using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// ai0: 主人Index<br></br>
    /// ai1: 状态<br></br>
    /// ai2: 子状态<br></br>
    /// ai3: 用于向主人传递信号
    /// </summary>
    public class SmallShadowBall : ModNPC
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        internal ref float OwnerIndex => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Sign => ref NPC.ai[3];

        internal ref float Timer => ref NPC.localAI[0];
        internal ref float Recorder => ref NPC.localAI[1];
        //internal ref float Recorder2 => ref NPC.localAI[2];

        public enum AIStates
        {
            OnSpawnAnmi,
            OnKillAnmi,
            /// <summary> 转转激光 </summary>
            RollingLaser,
            /// <summary> 汇集激光 </summary>
            ConvergeLaser,
        }

        public enum SignType
        {
            Nothing,
            Ready
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.lifeMax = 3500;
            NPC.knockBackResist = 0f;
            //NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        #region AI
        public override void AI()
        {
            if (!GetOwner(out NPC owner))
                return;

            switch (State)
            {
                default:
                case (int)AIStates.RollingLaser:
                    {
                        RollingLaser(owner);
                        Timer++;
                    }
                    break;
                case (int)AIStates.ConvergeLaser:
                    {
                        ConvergeLaser(owner);
                        Timer++;
                    }
                    break;
            }
        }

        #region RollingLaser 旋转激光
        public void RollingLaser(NPC owner)
        {
            //最开始与主人的距离
            const int ReadyLength = 64;
            //聚集后与主人的距离
            const int ShrinkLength = 32;

            switch (SonState)
            {
                default:
                case 0: //朝向指定位置
                    {
                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        Vector2 dir = (owner.rotation + index * MathHelper.TwoPi / totalIndexes).ToRotationVector2();
                        Vector2 targetPos = owner.Center + dir * ReadyLength;

                        float factor = Math.Clamp(Timer / 80, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;

                        if (length < 8)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
                case 1://开转！转速会逐渐减慢
                    {
                        const int RollingTime = 70;

                        float factor = Timer / RollingTime;

                        //增加旋转，此状态中的记录者代表初始的自身相对于主人的角度
                        float currentRot = Recorder + Coralite.Instance.BezierEaseSmoother.Smoother(factor) * (MathHelper.TwoPi*1.5f + 0.5f);

                        NPC.Center = owner.Center + currentRot.ToRotationVector2() * ReadyLength;
                        NPC.rotation = (NPC.Center - owner.Center).ToRotation();

                        if (Timer >= RollingTime)
                        {
                            SonState++;
                            Timer = 0;
                            Recorder = ShrinkLength;
                        }
                    }
                    break;
                case 2://蓄力，与主人距离向内缩小
                    {
                        const int SmallTime = 15;

                        float factor = Timer / SmallTime;

                        float length = Helper.Lerp(ReadyLength, ShrinkLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                        float currentRot = (NPC.Center - owner.Center).ToRotation();
                        NPC.Center = owner.Center + currentRot.ToRotationVector2() * length;
                        NPC.rotation = (NPC.Center - owner.Center).ToRotation();
                        if (Timer > SmallTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://射激光
                    {
                        //准备时间
                        const int ReadyShootTime = 10;
                        //射击时间
                        const int ShootTime = ReadyShootTime + 25;

                        //射击时与主人距离，比较远
                        const int ReadyShootLength = 120;
                        //受到后坐力后与主人的距离
                        const int RecoilLength = 64;

                        if (Timer < ReadyShootTime)//准备射，与主人距离拉远
                        {
                            float factor = Timer / ReadyShootTime;

                            float currentRot = (NPC.Center - owner.Center).ToRotation();
                            float targetLength = Helper.Lerp(Recorder, ReadyShootLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                            NPC.Center = owner.Center + currentRot.ToRotationVector2() * targetLength;
                            NPC.rotation = currentRot;
                        }
                        else if (Timer == ReadyShootTime)//生成激光弹幕
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2, NPC.target, NPC.whoAmI, 25);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                        }
                        else if (Timer < ShootTime)//后坐力，与主人距离逐渐减小
                        {
                            float factor = (Timer - ReadyShootTime) / ShootTime;

                            float currentRot = (NPC.Center - owner.Center).ToRotation();
                            float targetLength = Helper.Lerp(ReadyShootLength, RecoilLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                            NPC.Center = owner.Center + currentRot.ToRotationVector2() * targetLength;
                            NPC.rotation = currentRot;
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity = Helper.NextVec2Dir();
                        }
                    }
                    break;
                case 4://射完了虚一会
                    {
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.05f) * (Timer / 30) * 8;

                        if (Timer > 10)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
            }
        }

        public void RollingLaser_OnAllReady(NPC owner)
        {
            SonState++;
            Timer = 0;
            Sign = (int)SignType.Nothing;
            //生成预判线弹幕
            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero, 1, 2, NPC.target, NPC.whoAmI, 90);

            NPC.velocity *= 0;
            Recorder = (NPC.Center - owner.Center).ToRotation();
        }
        #endregion

        #region ConvergeLaser 聚合射击
        public void ConvergeLaser(NPC owner)
        {
            //聚合中心点与主人的距离
            const int ConvergeCenterLength = 80;
            //蓄力向后缩时聚合中心点与主人的距离
            const int ConvergeCenterLengthOnChannel = 48;
            const int ConvergeCenterLengthOnShoot = 100;
            //自身与聚合中心点开始是的距离
            const int ReadyLongAxis = 160;
            const int ReadyShortAxis = 160;

            switch (SonState)
            {
                default:
                case 0://聚合到指定位置
                    {
                        Player target = Main.player[owner.target];

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        float dir = owner.rotation + index * MathHelper.TwoPi / totalIndexes;
                        Vector2 toConvergeCenter = (target.Center - owner.Center).SafeNormalize(Vector2.Zero);
                        Vector2 targetPos = owner.Center
                           + toConvergeCenter * ConvergeCenterLength //到汇聚中心的向量
                           + toConvergeCenter.RotatedBy(1.57f) * (Helper.EllipticalEase(dir, ReadyShortAxis, ReadyLongAxis));
                        //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                        float factor = Math.Clamp(Timer / 80, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        NPC.rotation = toConvergeCenter.ToRotation();
                        if (length < 8)
                        {
                            Sign = (int)SignType.Ready;
                        }

                    }
                    break;
                case 1://向后缩，此时微微瞄准
                    {
                        //蓄力时间
                        const int ChannelTime = 60;
                        float factor = Timer / ChannelTime;

                        Player target = Main.player[owner.target];

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);

                        float dir = owner.rotation + index * MathHelper.TwoPi / totalIndexes;
                        float toConvergeCenter = (target.Center - owner.Center).ToRotation();

                        //随时间降低对玩家的跟踪性能
                        float aimRot = Helper.Lerp(Recorder, toConvergeCenter, 1 - factor * 0.3f);
                        Vector2 aimDir = aimRot.ToRotationVector2();
                        Vector2 targetPos = owner.Center
                           + aimDir * Helper.Lerp(ConvergeCenterLength, ConvergeCenterLengthOnChannel,
                                Coralite.Instance.SqrtSmoother.Smoother(factor)) //到汇聚中心的向量
                           + (dir + aimRot).ToRotationVector2() * (Helper.EllipticalEase(dir + aimRot + 1.57f, ReadyShortAxis, ReadyLongAxis));
                        //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * velocity;
                        NPC.rotation = aimRot;

                        if (Timer>ChannelTime)
                        {
                            Timer = 0;
                            SonState++;
                            Recorder = NPC.rotation;
                        }
                    }
                    break;
                case 2://射出来了！
                    {
                        //准备时间
                        const int ReadyShootTime = 10;
                        //射击时间
                        const int ShootTime = ReadyShootTime + 25;

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);

                        if (Timer < ReadyShootTime)//准备射，与主人距离拉远
                        {
                            float factor = Timer / ReadyShootTime;
                            float targetLength = Helper.Lerp(ConvergeCenterLengthOnChannel, ConvergeCenterLengthOnShoot, Coralite.Instance.SqrtSmoother.Smoother(factor));
                            float dir = owner.rotation + index * MathHelper.TwoPi / totalIndexes;

                            Vector2 aimDir = Recorder.ToRotationVector2();
                            Vector2 targetPos = owner.Center
                           + aimDir * targetLength //到汇聚中心的向量
                               + aimDir.RotatedBy(1.57f) * (Helper.EllipticalEase(dir, ReadyShortAxis, ReadyLongAxis));
                            //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                            NPC.Center = targetPos;
                        }
                        else if (Timer == ReadyShootTime)//生成激光弹幕
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2, NPC.target, NPC.whoAmI, 25);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                        }
                        else if (Timer < ShootTime)//后坐力，与主人距离逐渐减小
                        {
                            float factor = (Timer - ReadyShootTime) / ShootTime;
                            float targetLength = Helper.Lerp(ConvergeCenterLengthOnShoot, ConvergeCenterLength, Coralite.Instance.SqrtSmoother.Smoother(factor));
                            float dir = owner.rotation + index * MathHelper.TwoPi / totalIndexes;

                            Vector2 aimDir = Recorder.ToRotationVector2();
                            Vector2 targetPos = owner.Center
                           + aimDir * targetLength //到汇聚中心的向量
                               + aimDir.RotatedBy(1.57f) * (Helper.EllipticalEase(dir, ReadyShortAxis, ReadyLongAxis));
                            //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                            NPC.Center = targetPos;
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity = Helper.NextVec2Dir();
                        }
                    }
                    break;
                case 3://萎了
                    {
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.05f) * (Timer / 30) * 8;

                        if (Timer > 10)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
            }
        }

        public void ConvergeLaser_OnAllReady(NPC owner)
        {
            SonState++;
            Timer = 0;
            Sign = (int)SignType.Nothing;
            //生成预判线弹幕
            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero, 1, 2, NPC.target, NPC.whoAmI, 60);

            NPC.velocity *= 0;
            Recorder = (NPC.Center - owner.Center).ToRotation();
            Player target = Main.player[owner.target];

            Recorder=(target.Center- owner.Center).ToRotation();
        }
        #endregion

        #endregion

        #region States

        public void ResetState(AIStates targetState)
        {
            if (State == (int)AIStates.OnKillAnmi)//死亡动画时不会被改状态
                return;
            Timer = 0;
            State = (int)targetState;
            SonState = 0;
            Sign = (int)SignType.Nothing;
            Recorder = 0;
        }



        #endregion

        #region HelperMethods

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                NPC.Kill();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<ShadowBall>())
            {
                npc.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }

        #endregion
    }
}
