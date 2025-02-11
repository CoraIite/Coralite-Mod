using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;

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
        internal ref float Recorder2 => ref NPC.localAI[2];

        public Vector2 eyeRuneOffset;
        public float ballRotation;
        public int smallBallType;
        public float ballScale = 1;
        public float ballAlpha = 1;

        public ShadowCircleController shadowCircle;

        public enum AIStates
        {
            OnSpawnAnmi,
            OnKillAnmi,
            Idle,
            /// <summary> 转转激光 </summary>
            RollingLaser,
            /// <summary> 汇集激光 </summary>
            ConvergeLaser,
            /// <summary> 激光+光束-激光 </summary>
            LaserWithBeam_Laser,
            /// <summary> 激光+光束-光束 </summary>
            LaserWithBeam_Beam,
            /// <summary> 左右激光 </summary>
            LeftRightLaser,
            /// <summary> 旋转并射影子玩家 </summary>
            RollingShadowPlayer,
            /// <summary> 随机射激光 </summary>
            RandomLaser,

        }

        public enum SignType
        {
            Nothing,
            Ready
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.lifeMax = 3500;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 2f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        private bool span;
        #region AI

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.frame.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.frame.Y = reader.ReadInt32();
        }

        public override void AI()
        {
            if (!span && VaultUtils.isServer)
            {
                NPC.frame.Y = Main.rand.Next(7);
                span = true;
            }
            if (!GetOwner(out NPC owner))
                return;

            shadowCircle ??= new ShadowCircleController(ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallCircle0", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            UpdateFrame();
            Lighting.AddLight(NPC.Center, new Vector3(0.5f, 0.4f, 0.6f));

            switch (State)
            {
                default:
                case (int)AIStates.Idle:
                    {
                        NPC.velocity *= 0.9f;
                        NPC.rotation += 0.05f;
                        if (Timer <= 0)
                        {
                            ResetState((AIStates)Recorder);
                            break;
                        }

                        Timer--;
                    }
                    break;
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
                case (int)AIStates.LaserWithBeam_Laser:
                    {
                        LaserWithBeam_Laser(owner);
                        Timer++;
                    }
                    break;
                case (int)AIStates.LaserWithBeam_Beam:
                    {
                        LaserWithBeam_Beam(owner);
                        Timer++;
                    }
                    break;
                case (int)AIStates.LeftRightLaser:
                    {
                        LeftRightLaser(owner);
                        Timer++;
                    }
                    break;
                case (int)AIStates.RollingShadowPlayer:
                    {
                        RollingShadowPlayer(owner);
                        Timer++;
                    }
                    break;
                case (int)AIStates.RandomLaser:
                    {
                        RandomLaser(owner);
                        Timer++;
                    }
                    break;

            }

            shadowCircle.zRotation = NPC.rotation - 1.57f;
            shadowCircle.Update();
        }

        #region RollingLaser 旋转激光
        public void RollingLaser(NPC owner)
        {
            //最开始与主人的距离
            const int ReadyLength = 64 + 48;
            //聚集后与主人的距离
            const int ShrinkLength = 32 + 32;

            ballRotation += 0.05f;

            switch (SonState)
            {
                default:
                case 0: //朝向指定位置
                    {
                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        Vector2 dir = (owner.rotation + (index * MathHelper.TwoPi / totalIndexes)).ToRotationVector2();
                        Vector2 targetPos = owner.Center + (dir * ReadyLength);

                        float factor = Math.Clamp(Timer / 20, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        NPC.rotation += 0.2f;
                        eyeRuneOffset = NPC.velocity;
                        shadowCircle.xRotation += 0.1f;
                        //shadowCircle.yRotation += 0.15f;

                        if (length < 16)
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
                        float currentRot = Recorder + (Coralite.Instance.BezierEaseSmoother.Smoother(factor) * ((MathHelper.TwoPi * 1.5f) + 0.5f));

                        NPC.Center = owner.Center + (currentRot.ToRotationVector2() * ReadyLength);
                        NPC.rotation = NPC.rotation.AngleLerp((NPC.Center - owner.Center).ToRotation(), 0.5f);
                        eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, (factor * MathHelper.TwoPi).ToRotationVector2() * 8, 0.2f);
                        shadowCircle.xRotation += 0.1f;

                        if (Timer >= RollingTime)
                        {
                            SonState++;
                            Timer = 0;
                            Recorder = ShrinkLength;
                            Recorder2 = NPC.rotation;
                        }
                    }
                    break;
                case 2://蓄力，与主人距离向内缩小
                    {
                        const int SmallTime = 15;

                        float factor = Timer / SmallTime;

                        float length = Helper.Lerp(ReadyLength, ShrinkLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                        float currentRot = Recorder2;
                        NPC.Center = owner.Center + (currentRot.ToRotationVector2() * length);
                        NPC.rotation = Recorder2;
                        ballScale = Helper.Lerp(1, 1.15f, factor);
                        eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, Vector2.Zero, 0.2f);
                        shadowCircle.xRotation = shadowCircle.xRotation.AngleLerp(-1.4f, factor);

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
                        const int ReadyShootLength = 120 + 32;
                        //受到后坐力后与主人的距离
                        const int RecoilLength = 64 + 32;

                        if (Timer < ReadyShootTime)//准备射，与主人距离拉远
                        {
                            float factor = Timer / ReadyShootTime;

                            float currentRot = Recorder2;
                            float targetLength = Helper.Lerp(Recorder, ReadyShootLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                            NPC.Center = owner.Center + (currentRot.ToRotationVector2() * targetLength);
                            NPC.rotation = currentRot;
                            ballScale = Helper.Lerp(1.15f, 0.8f, factor);
                            ballAlpha = Helper.Lerp(1f, 0.4f, factor);

                            eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, NPC.rotation.ToRotationVector2() * 16, 0.2f);
                            shadowCircle.xRotation = (-1.4f).AngleLerp(-1f, factor);
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

                            float currentRot = Recorder2;
                            float targetLength = Helper.Lerp(ReadyShootLength, RecoilLength, Coralite.Instance.SqrtSmoother.Smoother(factor));

                            NPC.Center = owner.Center + (currentRot.ToRotationVector2() * targetLength);
                            NPC.rotation = currentRot;
                            ballScale = Helper.Lerp(0.8f, 1f, factor);
                            ballAlpha = Helper.Lerp(0.4f, 1f, factor);
                            eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, Vector2.Zero, 0.2f);
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            ballScale = 1;
                            ballAlpha = 1;
                            //NPC.velocity = Helper.NextVec2Dir();
                        }
                    }
                    break;
                case 4://射完了虚一会
                    {
                        //NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.05f) * (Timer / 30) * 8;

                        //if (Timer > 10)
                        //{
                        Sign = (int)SignType.Ready;
                        //}
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
            const int ConvergeCenterLength = 12;
            //蓄力向后缩时聚合中心点与主人的距离
            const int ConvergeCenterLengthOnChannel = 0;
            const int ConvergeCenterLengthOnShoot = 40;
            //自身与聚合中心点开始时的距离
            const int ReadyLongAxis = 160;
            const int ReadyShortAxis = 80;

            switch (SonState)
            {
                default:
                case 0://聚合到指定位置
                    {
                        Player target = Main.player[owner.target];

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        float dir = owner.rotation + (index * MathHelper.TwoPi / totalIndexes);
                        Vector2 toConvergeCenter = (target.Center - owner.Center).SafeNormalize(Vector2.Zero);
                        float aimRot = toConvergeCenter.ToRotation();

                        Vector2 targetPos = owner.Center
                           + (toConvergeCenter * ConvergeCenterLength) //到汇聚中心的向量
                           + ((dir + aimRot).ToRotationVector2() * Helper.EllipticalEase(dir + 1.57f, ReadyShortAxis, ReadyLongAxis));
                        //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                        float factor = Math.Clamp(Timer / 40, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 24;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        NPC.rotation += 0.1f;
                        eyeRuneOffset = NPC.velocity;

                        if (length < 16)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
                case 1://向后缩，此时微微瞄准
                    {
                        //蓄力时间
                        const int ChannelTime = 80;
                        const int AimTime = 30;
                        float factor = Timer / ChannelTime;

                        Player target = Main.player[owner.target];

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);

                        float dir = owner.rotation + (index * MathHelper.TwoPi / totalIndexes);
                        float toConvergeCenter;

                        if (Timer < AimTime)
                        {
                            toConvergeCenter = (target.Center - owner.Center).ToRotation();
                            Recorder = toConvergeCenter;
                        }
                        else
                        {
                            toConvergeCenter = Recorder;
                        }

                        //随时间降低对玩家的跟踪性能
                        Vector2 aimDir = toConvergeCenter.ToRotationVector2();
                        Vector2 targetPos = owner.Center
                           + (aimDir * Helper.Lerp(ConvergeCenterLength, ConvergeCenterLengthOnChannel,
                                Coralite.Instance.SqrtSmoother.Smoother(factor))) //到汇聚中心的向量
                           + ((dir + toConvergeCenter).ToRotationVector2() * Helper.EllipticalEase(dir + 1.57f, ReadyShortAxis, ReadyLongAxis));
                        //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * velocity;
                        NPC.rotation = NPC.rotation.AngleLerp(toConvergeCenter, 0.2f);
                        eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, (factor * MathHelper.TwoPi).ToRotationVector2() * 8, 0.2f);

                        if (Timer > ChannelTime)
                        {
                            Timer = 0;
                            SonState++;
                            Recorder = NPC.rotation;
                            NPC.velocity *= 0;
                            Sign = (int)SignType.Ready;
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
                        NPC.rotation = Recorder;

                        if (Timer < ReadyShootTime)//准备射，与主人距离拉远
                        {
                            float factor = Timer / ReadyShootTime;
                            float targetLength = Helper.Lerp(ConvergeCenterLengthOnChannel, ConvergeCenterLengthOnShoot, Coralite.Instance.SqrtSmoother.Smoother(factor));
                            float dir = owner.rotation + (index * MathHelper.TwoPi / totalIndexes);

                            Vector2 aimDir = Recorder.ToRotationVector2();
                            Vector2 targetPos = owner.Center
                           + (aimDir * targetLength) //到汇聚中心的向量
                           + ((dir + Recorder).ToRotationVector2() * Helper.EllipticalEase(dir + 1.57f, ReadyShortAxis, ReadyLongAxis));
                            //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                            NPC.Center = targetPos;
                            ballScale = Helper.Lerp(1.15f, 0.8f, factor);
                            ballAlpha = Helper.Lerp(1f, 0.4f, factor);
                            eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, NPC.rotation.ToRotationVector2() * 16, 0.2f);
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
                            float dir = owner.rotation + (index * MathHelper.TwoPi / totalIndexes);

                            Vector2 aimDir = Recorder.ToRotationVector2();
                            Vector2 targetPos = owner.Center
                           + (aimDir * targetLength) //到汇聚中心的向量
                           + ((dir + Recorder).ToRotationVector2() * Helper.EllipticalEase(dir + 1.57f, ReadyShortAxis, ReadyLongAxis));
                            //         👆 额外的椭圆形旋转，这次不像赤玉灵就先不搞什么3D了

                            NPC.Center = targetPos;
                            ballScale = Helper.Lerp(0.8f, 1f, factor);
                            ballAlpha = Helper.Lerp(0.4f, 1f, factor);
                            eyeRuneOffset = Vector2.Lerp(eyeRuneOffset, Vector2.Zero, 0.2f);
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            //NPC.velocity = Helper.NextVec2Dir();
                        }
                    }
                    break;
                case 3://萎了
                    {
                        //NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.05f) * (Timer / 30) * 8;

                        //if (Timer > 10)
                        //{
                        Sign = (int)SignType.Ready;
                        //}
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
            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero, 1, 2, NPC.target, NPC.whoAmI, 80);

            NPC.velocity *= 0;
            Player target = Main.player[owner.target];

            Recorder = (target.Center - owner.Center).ToRotation();
        }
        #endregion

        #region LaserWithBeam 激光+光束
        public void LaserWithBeam_Laser(NPC owner)
        {
            switch (SonState)
            {
                default:
                case 0:
                    {
                        Timer = 0;
                        SonState = 1;
                        Recorder = (Main.player[owner.target].Center - NPC.Center).ToRotation();
                        NPC.TargetClosest();
                    }
                    break;
                case 1://朝向玩家身边运动
                    {
                        const int RollingTime = 160;
                        const int PredictTime = 80;
                        Player target = Main.player[owner.target];

                        float factor = Math.Clamp(1 - (Timer / (RollingTime * 3)), 0, 1);

                        Recorder += 0.02f + (factor * 0.06f);

                        Vector2 dirToTarget = target.Center + (Recorder.ToRotationVector2() * 340) - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 80, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        NPC.rotation = (target.Center - NPC.Center).ToRotation();
                        eyeRuneOffset = NPC.velocity;

                        if (Timer == PredictTime)//生成预判线
                        {
                            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero, 1, 2, NPC.target, NPC.whoAmI, 110);
                        }

                        if (Timer > RollingTime)
                        {
                            NPC.velocity *= 0.3f;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://射激光
                    {
                        if (Timer < 30)
                        {
                            NPC.velocity *= 0.8f;
                            break;
                        }

                        if (Timer == 30)
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2, NPC.target, NPC.whoAmI, 25);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 8;
                        }

                        if (Timer < 55)
                        {
                            NPC.velocity *= 0.98f;
                            break;
                        }

                        SonState++;
                        Timer = 0;
                    }
                    break;
                case 3://虚一会
                    {
                        if (Timer > 20)
                        {
                            SonState = 1;
                            Timer = 0;
                        }
                    }
                    break;
            }
        }

        public void LaserWithBeam_Beam(NPC owner)
        {
            switch (SonState)
            {
                default:
                case 0:
                    {
                        Timer = 0;
                        SonState++;
                        Recorder = (Main.player[owner.target].Center - NPC.Center).ToRotation();
                        NPC.TargetClosest();
                    }
                    break;
                case 1://选定玩家附近一个点
                    {
                        if (Timer <= 1)
                        {
                            Recorder = Main.rand.NextFloat(6.282f);
                            Recorder2 = Main.rand.NextFloat(240, 400);
                            break;
                        }
                        const int RollingTime = 120;
                        Player target = Main.player[owner.target];

                        Recorder += 0.025f;

                        float factor = Math.Clamp(Timer / RollingTime, 0, 1);

                        Vector2 dirToTarget = target.Center + (Recorder.ToRotationVector2() * Recorder2) - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 80, 0, 1) * 20;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        NPC.rotation = (target.Center - NPC.Center).ToRotation();

                        if (Timer > RollingTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://射光束
                    {
                        if (Timer == 20)
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<ShadowPlayerSpurt>(NPC.Center,
                                NPC.rotation.ToRotationVector2() * 6, damage, 2, NPC.target);
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 8;
                        }

                        if (Timer < 55)
                        {
                            NPC.velocity *= 0.98f;
                            break;
                        }

                        SonState++;
                        Timer = 0;
                    }
                    break;
                case 3://虚一会
                    {
                        if (Timer > 20)
                        {
                            SonState = 1;
                            Timer = 0;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region LeftRightLaser 左右激光

        /// <summary>
        /// 记录器存储左边还是右边，记录器2存储高度
        /// </summary>
        /// <param name="Owner"></param>
        public void LeftRightLaser(NPC Owner)
        {
            const int PredictTime = 70;

            switch (SonState)
            {
                default:
                case 0:
                    {
                        Timer = 0;
                        SonState = 1;
                        Recorder = Main.rand.NextFromList(-1, 1);
                        Recorder2 = Main.rand.NextFloat(20, CoraliteWorld.shadowBallsFightArea.Height - 20);
                        NPC.TargetClosest();
                    }
                    break;
                case 1://运动向目标位置
                    {
                        const int MoveTime = 15;

                        Vector2 targetPos = new(
                            CoraliteWorld.shadowBallsFightArea.X + (Recorder > 0 ? 100 : CoraliteWorld.shadowBallsFightArea.Width - 100),
                            CoraliteWorld.shadowBallsFightArea.Y + Recorder2);
                        SetDirection(targetPos, out float xLength, out float yLength);

                        float factor = Math.Clamp(Timer / MoveTime, 0, 1);

                        float acc = 0.1f + (0.45f * factor);
                        float speed = 2f + (18f * factor);

                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                            , speed, 32, acc, 0.65f, 0.8f);
                        Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.Y, yLength, NPC.directionY
                            , speed / 2, 16, acc / 2, 0.65f, 0.8f);

                        if (Vector2.Distance(targetPos, NPC.Center) < 32)
                        {
                            NPC.velocity *= 0f;
                            NPC.rotation = Recorder > 0 ? 0 : MathHelper.Pi;
                            SonState++;
                            Timer = 0;
                            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero, 1, 2, NPC.target, NPC.whoAmI, PredictTime - 10);
                        }
                    }
                    break;
                case 2://射激光
                    {
                        const int DelayTime = PredictTime + 25;
                        if (Timer < PredictTime)
                        {
                            NPC.velocity *= 0.9f;
                            break;
                        }

                        if (Timer == PredictTime)
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2, NPC.target, NPC.whoAmI, 25);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 8;
                        }

                        if (Timer < DelayTime)
                        {
                            Vector2 targetPos = new(
                                CoraliteWorld.shadowBallsFightArea.X + (Recorder > 0 ? 100 : CoraliteWorld.shadowBallsFightArea.Width - 80),
                                Recorder2);
                            SetDirection(targetPos, out float xLength, out _);

                            Helper.Movement_SimpleOneLine_Limit(ref NPC.velocity.X, xLength, NPC.direction
                                , 3, 32, 0.08f, 0.14f, 0.97f);
                            break;
                        }

                        SonState++;
                        Timer = 0;
                    }
                    break;
                case 3://虚一会
                    {
                        //if (Timer > 20)
                        //{
                        SonState = 0;
                        Timer = 0;
                        //}
                    }
                    break;
                case 4://idle
                    {
                        NPC.velocity *= 0.96f;
                    }
                    break;
            }
        }

        #endregion

        #region RollingShadowPlayer 释放影子玩家
        public void RollingShadowPlayer(NPC owner)
        {
            const int ReadyLength = 280;

            switch (SonState)
            {
                default:
                case 0://靠近目标点
                    {
                        Player Target = Main.player[owner.target];

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        Vector2 dir = (Recorder + (index * MathHelper.TwoPi / totalIndexes)).ToRotationVector2();
                        Vector2 targetPos = Target.Center + (dir * ReadyLength);

                        float factor = Math.Clamp(Timer / 20, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 32;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;
                        Recorder += 0.04f;

                        if (length < 24)
                        {
                            Sign = (int)SignType.Ready;
                        }
                    }
                    break;
                case 1://射弹幕 🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍🐍
                    {
                        const int ReadyTime = 25;
                        const int ShootTime = 40;

                        const int ReadyShootLength = 60;
                        const int RecoilLength = 340;
                        Player Target = Main.player[owner.target];

                        float factor;
                        float length2 = 0;
                        if (Timer <= ReadyTime)
                        {
                            factor = Timer / ReadyTime;
                            length2 = Helper.Lerp(ReadyLength, ReadyShootLength, Coralite.Instance.SqrtSmoother.Smoother(factor));
                            if (Timer == ReadyTime)
                            {
                                int damage = Helper.ScaleValueForDiffMode(40, 35, 35, 30);
                                NPC.NewProjectileInAI<ShadowPlayerSpurt>(NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 6
                                    , damage, 2, owner.target);
                            }
                        }
                        else if (Timer < ReadyTime + ShootTime)
                        {
                            factor = (Timer - ReadyTime) / ShootTime;
                            length2 = Helper.Lerp(ReadyShootLength, RecoilLength, Coralite.Instance.SqrtSmoother.Smoother(factor));
                        }
                        else
                        {
                            Sign = (int)SignType.Ready;
                        }

                        Helper.GetMyNpcIndexWithModNPC<SmallShadowBall>(NPC, out int index, out int totalIndexes);
                        //直线运动到目标位置
                        Vector2 dir = (Recorder + (index * MathHelper.TwoPi / totalIndexes)).ToRotationVector2();
                        Vector2 targetPos = Target.Center + (dir * length2);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 32;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * velocity;
                        Recorder += 0.04f;
                    }
                    break;
                case 2:
                    {
                        NPC.velocity *= 0.9f;
                    }
                    break;
            }
        }

        public void SetRollingShadowPlayer(float baseangle)
        {
            ResetState(AIStates.RollingShadowPlayer);
            Recorder = baseangle;
        }

        public void RollingShadowPlayerAllReady()
        {
            SonState++;
            Timer = 0;
            Sign = (int)SignType.Nothing;
        }

        #endregion

        #region RandomLaser 随机射激光

        public void RandomLaser(NPC owner)
        {
            switch (SonState)
            {
                default:
                case 0://随便找一个点
                    {
                        Rectangle rect = CoraliteWorld.shadowBallsFightArea;
                        rect.X += 80;
                        rect.Y += 80;
                        rect.Width -= 80 * 2;
                        rect.Height -= 80 * 2;

                        Vector2 targetPos = Main.rand.NextVector2FromRectangle(rect);
                        Recorder = targetPos.X;
                        Recorder2 = targetPos.Y;

                        SonState++;
                        Timer = 0;
                    }
                    break;
                case 1://移动过去
                    {
                        //直线运动到目标位置
                        Vector2 targetPos = new(Recorder, Recorder2);

                        float factor = Math.Clamp(Timer / 20, 0, 1);

                        Vector2 dirToTarget = targetPos - NPC.Center;
                        float length = dirToTarget.Length();
                        float velocity = Math.Clamp(length / 40, 0, 1) * 24;
                        NPC.velocity = dirToTarget.SafeNormalize(Vector2.Zero) * factor * velocity;

                        if (length < 16)
                        {
                            SonState++;
                            Timer = 0;
                            Recorder = 0;
                            Recorder2 = 0;
                            NPC.velocity *= 0;
                            NPC.NewProjectileInAI<SmallLaserPredictionLine>(NPC.Center, Vector2.Zero
                                , 1, 2, NPC.target, NPC.whoAmI, 100);
                        }
                    }
                    break;
                case 2://瞄准玩家
                    {
                        Player target = Main.player[owner.target];
                        const int aimTime = 50;
                        const int shootTime = 105;
                        if (Timer < aimTime)
                        {
                            Recorder = (target.Center - NPC.Center).ToRotation();
                        }
                        else if (Timer == shootTime)
                        {
                            NPC.TargetClosest();
                            int damage = Helper.ScaleValueForDiffMode(30, 50, 40, 40);
                            NPC.NewProjectileInAI<SmallLaser>(NPC.Center, Vector2.Zero, damage, 2
                                , NPC.target, NPC.whoAmI, 60);
                            Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, NPC.Center);
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2() * 8;
                        }
                        else
                        {
                            NPC.velocity *= 0.95f;
                            if (Timer > shootTime + 60)
                            {
                                SonState++;
                                Timer = 0;
                            }
                        }

                        NPC.rotation = Recorder;
                    }
                    break;
                case 3://后摇
                    {
                        NPC.rotation += 0.05f;
                        NPC.velocity *= 0.95f;
                        if (Timer > 15)
                        {
                            SonState = 0;
                            Timer = 0;
                        }
                    }
                    break;
                case 4://切换状态时
                    {

                    }
                    break;
            }
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
            Recorder2 = 0;
            NPC.TargetClosest();
        }

        public void Idle(AIStates afterIdleState, int idleTime)
        {
            if (State == (int)AIStates.OnKillAnmi)//死亡动画时不会被改状态
                return;
            Timer = idleTime;
            State = (int)AIStates.Idle;
            SonState = 0;
            Sign = (int)SignType.Nothing;
            Recorder = (int)afterIdleState;
            Recorder2 = 0;
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
                NPC.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public void SetDirection(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            NPC.direction = xLength > 0 ? -1 : 1;
            NPC.directionY = yLength > 0 ? -1 : 1;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        public void UpdateFrame()
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 6)
                    NPC.frame.Y = 0;
            }
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 pos = NPC.Center - screenPos;

            shadowCircle?.DrawBackCircle(spriteBatch, pos, drawColor);
            DrawSelf(spriteBatch, screenPos, drawColor);
            shadowCircle?.DrawFrontCircle(spriteBatch, pos, drawColor);
            return false;
        }

        public void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D ballTex = NPC.GetTexture();
            Texture2D eyeTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallShadowBallEye").Value;
            //Texture2D frontTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallShadowBallFront").Value;
            //Texture2D backTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallShadowBallBack").Value;

            var pos = NPC.Center - screenPos;
            var ballFrameBox = ballTex.Frame(1, 7, 0, NPC.frame.Y);
            var eyeFrameBox = eyeTex.Frame(1, 5, 0, smallBallType);
            //var frameBox = frontTex.Frame();
            var origin = eyeFrameBox.Size() / 2;

            //绘制背后
            //spriteBatch.Draw(backTex, pos, frameBox, drawColor, NPC.rotation - 1.57f, origin, NPC.scale, 0, 0);

            //绘制球
            spriteBatch.Draw(ballTex, pos, ballFrameBox, drawColor * ballAlpha, ballRotation, origin, ballScale, 0, 0);

            //绘制眼睛
            spriteBatch.Draw(eyeTex, pos + eyeRuneOffset, eyeFrameBox, Color.White, 0, origin, 1, 0, 0);

            //绘制前面
            //spriteBatch.Draw(frontTex, pos, frameBox, drawColor, NPC.rotation - 1.57f, origin, NPC.scale, 0, 0);
        }

        #endregion
    }
}
