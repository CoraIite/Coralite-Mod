using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
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

        public enum AIStates
        {
            OnSpawnAnmi,
            OnKillAnmi,
            RollingLaser,
        }

        public enum SignType
        {
            Nothing,
            Ready
        }

        public override void SetDefaults()
        {

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
            }
        }

        #region RollingLaser 旋转激光
        public void RollingLaser(NPC owner)
        {
            const int ReadyLength = 64;
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

                        float factor = Math.Clamp(Timer / 180, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 100, 0, 1) * 16;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        if (length < 16)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
                case 1://开转！转速会逐渐减慢
                    {
                        const int RollingTime = 90;

                        float factor = Timer / RollingTime;

                        //增加旋转，此状态中的记录者代表初始的自身相对于主人的角度
                        float currentRot = Recorder + Coralite.Instance.SqrtSmoother.Smoother(factor) * (MathHelper.TwoPi + 0.5f);
                        float length = Helper.Lerp(ReadyLength, ShrinkLength, factor);

                        NPC.Center = owner.Center + currentRot.ToRotationVector2() * length;
                        NPC.rotation = (NPC.Center - owner.Center).ToRotation();

                        if (Timer>=90)
                        {
                            SonState++;
                            Timer = 0;
                            Recorder = ShrinkLength;
                        }
                    }
                    break;
                case 2://射激光
                    {
                        const int ReadyShootTime = 30;
                        const int ShootTime = 90;

                        if (Timer < ReadyShootTime)
                        {
                            float factor = Timer / ReadyShootTime;

                        }
                        else if (Timer == ReadyShootTime)//生成激光弹幕
                        {

                        }
                        else if (Timer < ShootTime)//后坐力
                        {

                        }
                        else//射完了虚一会
                        {

                        }
                    }
                    break;
                case 3://后摇
                    {

                    }
                    break;
            }
        }

        public void RollingLaser_OnAllReady(NPC owner)
        {
            SonState++;
            Timer = 0;
            //生成预判线弹幕

            NPC.velocity *= 0;
            Recorder = (NPC.Center - owner.Center).ToRotation();
        }
        #endregion


        #endregion

        #region States

        public void ResetState(AIStates targetState)
        {
            if (State==(int)AIStates.OnKillAnmi)//死亡动画时不会被改状态
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
    }
}
