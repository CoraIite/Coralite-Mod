using Coralite.Content.Particles;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System.CodeDom;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class ShadowBall
    {
        #region SmashDown 向下冲刺->升龙/回旋斩

        public void SmashDown()
        {
            switch (SonState)
            {
                default:
                case 0://跳起到指定位置
                    {
                        if (Timer == 2)
                        {
                            NPC.velocity *= 0;
                            //生成跳起的粒子
                        }

                        if (Recorder2 == 0)//自身高度还没达到指定高度时
                        {
                            Vector2 targetPos = Target.Center + new Vector2(0, -320);
                            SetDirection(targetPos, out float xLength, out _);

                            if (xLength > 250)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else if (xLength < 150)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.X *= 0.92f;

                            if (NPC.Center.Y > targetPos.Y)//没达到指定高度就向上加速，否则向下
                            {
                                if (NPC.velocity.Y > -20)//向上加速度逐渐递减
                                {
                                    float factor = MathHelper.Clamp(1 - Timer / 15, 0, 1);
                                    NPC.velocity.Y -= 0.2f + factor * 1.8f;
                                }
                            }
                            else
                            {
                                Recorder = targetPos.Y + 20;
                                Recorder2 = 1;
                            }
                        }
                        else//自然下落至指定高度
                        {
                            Vector2 targetPos = new Vector2(Target.Center.X, Recorder);
                            SetDirection(targetPos, out float xLength, out _);

                            if (xLength > 250)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else if (xLength < 150)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 5f, 0.1f, 0.18f, 0.97f);
                            else
                                NPC.velocity.X *= 0.92f;

                            if (NPC.velocity.Y < 20)
                            {
                                NPC.velocity.Y += 0.6f;
                            }

                            if (NPC.Center.Y >= targetPos.Y)//到达指定高度,准备斩击
                            {
                                SonState++;
                                Timer = 0;
                                NPC.velocity *= 0;
                            }
                        }
                    }
                    break;
                case 1://向下冲刺到指定位置
                    {
                        const int ReadyTime = 35;
                        const int DashTime = ReadyTime + 25;

                        if (Timer < ReadyTime)//生成闪光
                        {
                            if (Timer == 2)
                            {
                                Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(),
                                    Color.Purple, 1.2f);

                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();

                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                Recorder2 = ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.SmashDown_SmashDown, NPC.rotation);
                            }

                            if (Timer < 10)
                            {
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                NPC.rotation = MathHelper.Clamp(NPC.rotation, 0, 3.141f);
                            }
                        }
                        else if (Timer == ReadyTime)
                        {
                            NPC.velocity = NPC.rotation.ToRotationVector2() * 44;//冲刺速度
                            CanDamage = true;
                        }
                        else if (Timer < DashTime)//向下冲刺
                        {
                            NPC.velocity *= 0.99f;
                            if (NPC.Center.Y > Target.Center.Y + 120)//低于玩家后停止并在脚下生成一个弹幕平台
                                OnSmashDown();
                        }
                        else
                            OnSmashDown();
                    }
                    break;
                case 2://进行准备后向上升龙，角度根据玩家位置进行微微调整
                    {
                        const int ChannelTime = 30;
                        const int ShouryuukennTime = ChannelTime + 18;

                        if (Timer < ChannelTime)
                        {
                            if (Timer == 2)
                            {
                                float xLength = Target.Center.X - NPC.Center.X;
                                float velocityX = MathHelper.Clamp((xLength / 30), -5.5f, 5.5f);

                                Vector2 velocity = new Vector2(velocityX, -30);

                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.SmashDown_Shouryuukenn, velocity.ToRotation());
                            }
                            //生成蓄力粒子
                        }
                        else if (Timer == ChannelTime)
                        {
                            //向上冲刺并生成弹幕，让地面逐渐消失并生成爆炸弹幕
                            float xLength = Target.Center.X - NPC.Center.X;
                            float velocityX = MathHelper.Clamp((xLength / 30), -5.5f, 5.5f);

                            NPC.velocity = new Vector2(velocityX, -30);
                            CanDamage = true;

                            if (ProjectilesHelper.GetProjectile<ShadowGround>((int)Recorder, out Projectile p))//让地面消失
                                (p.ModProjectile as ShadowGround).Fade();
                        }
                        else if (Timer < ShouryuukennTime)
                        {
                            float factor = (Timer - ChannelTime) / (ShouryuukennTime - ChannelTime);

                            NPC.velocity.Y = -40 * (1.1f - Coralite.Instance.SqrtSmoother.Smoother(factor));
                            NPC.velocity.X *= 0.97f;
                        }
                        else
                        {
                            CanDamage = false;
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://悬停后朝玩家进行回旋斩
                    {
                        const int ChannelTime = 35;
                        const int SlashTime = ChannelTime + 80;
                        if (Timer < ChannelTime)
                        {
                            if (Timer == 2)
                            {
                                //生成斩击弹幕
                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                int index = ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.SmashDown_Rolling, Target.Center.X > NPC.Center.X ? 0 : 3.141f);

                                (Main.projectile[index].ModProjectile as ShadowBallSlash).extraScaleAngle = MathHelper.Clamp((Target.Center.X - NPC.Center.X) / 300, -1, 1) * 0.25f;
                            }
                            //生成蓄力粒子
                            NPC.velocity *= 0.94f;
                        }
                        else if (Timer == ChannelTime)
                        {
                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 14;

                            if (ProjectilesHelper.GetProjectile<ShadowGround>((int)Recorder, out Projectile p))//让地面消失
                                (p.ModProjectile as ShadowGround).Fade();
                        }
                        else if (Timer < SlashTime)
                        {
                            NPC.velocity *= 0.96f;
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 4://后摇阶段
                    {
                        ResetState();
                    }
                    break;
            }
        }

        /// <summary>
        /// 检测玩家位置进行出招，同时生成地面，使用<see cref="Recorder"/>进行记录
        /// </summary>
        public void OnSmashDown()
        {
            NPC.velocity *= 0;
            CanDamage = false;

            //检测玩家位置，如果离自身较高就升龙，否则直接回旋斩
            Vector2 targetPos = new Vector2(Target.Center.X, Recorder);
            SetDirection(targetPos, out float xLength, out _);

            if (xLength < 200 && Target.Center.Y < NPC.Center.Y - 100)
                SonState = 2;
            else
                SonState = 3;

            Timer = 0;

            if (ProjectilesHelper.GetProjectile<ShadowBallSlash>((int)Recorder2, out Projectile p))
                p.Kill();

            Recorder2 = 0;

            Recorder = NPC.NewProjectileInAI<ShadowGround>(NPC.Center + new Vector2(0, NPC.height / 2)
                , Vector2.Zero, 1, 0, NPC.target, NPC.whoAmI);
        }

        #endregion

        #region VerticalRolling 横砍->旋转

        public void VerticalRolling()
        {
            switch (SonState)
            {
                default:
                case 0://移动到与玩家平齐后开砍！
                case 1://同上
                case 2://同上
                    {
                        const int ChasingTime = 75;
                        const int ReadyTime = ChasingTime + 30;
                        const int SlashTIme = ReadyTime + 20;

                        if (Timer < ChasingTime)
                        {
                            Vector2 targetPos = Target.Center;
                            SetDirection(targetPos, out float xLength, out float yLength);

                            if (xLength > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 13f, 0.45f, 0.58f, 0.97f);
                            else if (xLength < 140)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 13f, 0.45f, 0.58f, 0.97f);
                            else
                                NPC.velocity.X *= 0.8f;

                            //控制Y方向的移动
                            if (yLength > 5)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                    , 12f, 0.44f, 0.58f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.8f;
                        }
                        else if (Timer == ChasingTime)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                           Recorder= ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.VerticalRolling, NPC.spriteDirection > 0 ? 0 : 3.141f);
                        }
                        else if (Timer < ReadyTime)
                        {
                            if (Timer < ReadyTime + 10)
                            {
                                Vector2 targetPos = Target.Center;
                                SetDirection(targetPos, out float xLength, out float yLength);

                                if (xLength > 160)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                        , 6f, 0.35f, 0.48f, 0.97f);
                                else if (xLength < 140)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                        , 6f, 0.35f, 0.48f, 0.97f);
                                else
                                    NPC.velocity.X *= 0.8f;

                                //控制Y方向的移动
                                if (yLength > 5)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                        , 4f, 0.24f, 0.38f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.8f;
                            }
                            else
                                NPC.velocity *= 0.7f;
                        }
                        else if (Timer == ReadyTime)//冲刺！冲刺！
                        {
                            NPC.velocity = new Vector2(NPC.spriteDirection * 14, 0);
                            //if (ProjectilesHelper.GetProjectile<ShadowBallSlash>((int)Recorder,out Projectile p))
                            //{
                            //    (p.ModProjectile as ShadowBallSlash).sta = NPC.velocity.ToRotation();
                            //}
                            CanDamage = true;
                        }
                        else if (Timer < SlashTIme)
                        {
                            NPC.velocity *= 0.99f;
                        }
                        else//如果玩家与自身距离较远那么就接着尝试接近并砍，否则直接进入转圈圈阶段
                        {
                            CanDamage = false;
                            NPC.velocity *= 0;
                            if (Vector2.Distance(NPC.Center, Target.Center) < 140)
                                SonState = 3;
                            else
                                SonState++;

                            Timer = 0;
                        }
                    }
                    break;
                case 3://向上运动后转圈圈，自身消散后结束时重组
                    {
                        const int ReadyTime = 25;
                        const int SlashTime = ReadyTime + 90;
                        const int DelayTime = SlashTime + 30;

                        if (Timer < ReadyTime)
                        {
                            //消散
                            if (Timer == 2)
                            {
                                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                ShadowBallSlash2.Spawn(NPC, damage, angle);
                                ShadowBallSlash2.Spawn(NPC, damage, angle + MathHelper.Pi);
                            }
                        }
                        else if (Timer == ReadyTime)
                        {
                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12;
                            CanDamage = true;
                        }
                        else if (Timer < SlashTime)
                        {
                            float factor = (Timer - ReadyTime) / (SlashTime - ReadyTime);

                            float targetAngle = (Target.Center - NPC.Center).ToRotation();
                            float velAngle = NPC.velocity.ToRotation();

                            NPC.velocity = velAngle.AngleLerp(targetAngle, 0.4f).ToRotationVector2()
                                * (1 - Coralite.Instance.SqrtSmoother.Smoother(factor)) * 12;
                        }
                        else if (Timer == SlashTime)
                        {
                            NPC.velocity *= 0;
                            CanDamage = false;
                        }
                        else if (Timer < DelayTime)
                        {
                            //聚集重组回来
                        }
                        else
                        {
                            ResetState();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region SkJump 斜上方冲刺->下砸

        public void SkyJump()
        {
            const int yOffset = 340;

            switch (SonState)
            {
                default:
                case 0://斜上方冲刺
                    {
                        const int ChasingTime = 70;
                        const int DashTime = ChasingTime + 20;

                        //先尝试与玩家平齐
                        if (Timer < ChasingTime)
                        {
                            Vector2 targetPos = Target.Center;
                            SetDirection(targetPos, out float xLength, out float yLength);

                            if (xLength > 320)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , 13f, 0.45f, 0.58f, 0.97f);
                            else if (xLength < 280)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                    , 13f, 0.45f, 0.58f, 0.97f);
                            else
                                NPC.velocity.X *= 0.8f;

                            //控制Y方向的移动
                            if (yLength > 5)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                    , 12f, 0.44f, 0.58f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.8f;

                            if (Timer == ChasingTime - 30)
                            {
                                //生成斩击弹幕
                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.SkyJump_JumpUp, NPC.velocity.ToRotation());
                            }
                        }
                        else if (Timer == ChasingTime)//记录玩家头顶的点
                        {
                            Recorder = Target.Center.X;
                            Recorder2 = Target.Center.Y - yOffset;
                            Vector2 targetPos = new Vector2(Recorder, Recorder2);

                            float distance = (targetPos - NPC.Center).Length();
                            NPC.velocity = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * distance / (DashTime - ChasingTime);
                        }
                        else if (Timer < DashTime)
                        {

                        }
                        else
                        {
                            NPC.velocity *= 0;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://在玩家头顶悬停，并跟随
                    {
                        const int StayTime = 40;

                        if (Timer == 2)//生成球弹幕
                        {

                        }
                        if (Timer < StayTime)
                        {
                            Vector2 targetPos = Target.Center + new Vector2(0, -yOffset - 50);
                            SetDirection(targetPos, out float xLength, out float yLength);

                            float velMax = Helper.ScaleValueForDiffMode(8, 10, 12, 12);
                            if (xLength > 20)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                    , velMax, 0.45f, 0.58f, 0.9f);
                            else
                                NPC.velocity.X *= 0.85f;

                            //控制Y方向的移动
                            if (yLength > 20)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                    , 6f, 0.44f, 0.58f, 0.9f);
                            else
                                NPC.velocity.Y *= 0.85f;
                        }
                        else
                        {
                            SonState++;
                            Timer = 0;
                            Vector2 targetPos = Target.Center;
                            SetDirection(targetPos, out float xLength, out _);

                            NPC.velocity = new Vector2(MathHelper.Clamp(NPC.direction * xLength / 30, -6, 6), 40);
                        }
                    }
                    break;
                case 2://下砸
                    {
                        const int SmashDownTime = 20;
                        const int DelayTime = SmashDownTime + 40;

                        if (Timer < SmashDownTime)
                        {
                            NPC.velocity.X *= 0.95f;

                            if (Target.Center.Y + 100 < NPC.Center.Y)
                            {
                                Timer = SmashDownTime - 1;
                            }
                        }
                        else if (Timer == SmashDownTime)
                        {
                            NPC.velocity = Vector2.Zero;
                            //生成地面和其他弹幕和粒子
                            Recorder = NPC.NewProjectileInAI<ShadowGround>(NPC.Center + new Vector2(0, NPC.height / 2)
                                , Vector2.Zero, 1, 0, NPC.target, NPC.whoAmI);
                        }
                        else if (Timer < DelayTime)
                        {

                        }
                        else
                        {
                            if (ProjectilesHelper.GetProjectile<ShadowGround>((int)Recorder, out Projectile p))//让地面消失
                                (p.ModProjectile as ShadowGround).Fade();

                            ResetState();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region HorizontalDash 水平冲刺

        public void HorizontalDash()
        {
            switch (SonState)
            {
                default:
                case 0://尝试与玩家水平对齐
                    {
                        Vector2 targetPos = Target.Center;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        bool xReady = false;
                        bool yReady = false;

                        if (xLength > 180)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                , 16f, 0.45f, 0.58f, 0.97f);
                        else if (xLength < 160)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                , 16f, 0.45f, 0.58f, 0.97f);
                        else
                        {
                            NPC.velocity.X *= 0.8f;
                            xReady = true;
                        }

                        //控制Y方向的移动
                        if (yLength > 20)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                , 14f, 0.54f, 0.68f, 0.97f);
                        else
                        {
                            NPC.velocity.Y *= 0.8f;
                            yReady = true;
                        }

                        if (xReady && yReady)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                        }
                    }
                    break;
                case 1://水平冲刺
                    {
                        const int ReadyTime = 20;
                        const int DashTime = ReadyTime + 22;
                        const int DelayTime = DashTime + 20;

                        if (Timer == 2)//生成球状弹幕
                        {

                        }
                        if (Timer < ReadyTime)
                        {

                        }
                        else if (Timer == ReadyTime)
                        {
                            NPC.velocity = new Vector2(NPC.spriteDirection * 25, 0);
                        }
                        else if (Timer < DashTime)
                        {
                            //生成冲刺粒子
                        }
                        else if (Timer == DashTime)
                        {
                            NPC.velocity *= 0.1f;
                        }
                        else if (Timer < DelayTime)//停止
                        {
                            NPC.velocity *= 0.9f;
                        }
                        else
                        {
                            ResetState();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region NightmareKingDash 水平冲刺+抛下弹幕

        public void NightmareKingDash()
        {
            switch (SonState)
            {
                default:
                case 0://与玩家平齐
                    {
                        Vector2 targetPos = Target.Center;
                        SetDirection(targetPos, out float xLength, out float yLength);

                        bool xReady = false;
                        bool yReady = false;

                        if (xLength > 270)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction
                                , 16f, 0.45f, 0.58f, 0.97f);
                        else if (xLength < 250)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction
                                , 16f, 0.45f, 0.58f, 0.97f);
                        else
                        {
                            NPC.velocity.X *= 0.8f;
                            xReady = true;
                        }

                        //控制Y方向的移动
                        if (yLength > 20)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY
                                , 14f, 0.54f, 0.68f, 0.97f);
                        else
                        {
                            NPC.velocity.Y *= 0.8f;
                            yReady = true;
                        }

                        if (xReady && yReady)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                        }
                    }
                    break;
                case 1://水平突刺
                    {
                        const int ReadyTime = 30;
                        const int DashTime = ReadyTime + 20;
                        if (Timer < ReadyTime)
                        {
                            if (Timer==2)
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 30, 25, 25);
                                Recorder2 = ShadowBallSlash.Spawn(NPC, damage, ShadowBallSlash.ComboType.NightmareKingDash,NPC.spriteDirection>0?0:3.141f);
                            }
                        }
                        else if (Timer == ReadyTime)//冲刺！
                        {
                            NPC.velocity = new Vector2(NPC.spriteDirection * 30, 0);
                        }
                        else if (Timer < DashTime)//生成速度粒子
                        {

                        }
                        else
                        {
                            NPC.velocity *= 0;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://冲向灯之影位置
                    {
                        const int ReadyTime = 25;
                        const int DashTime = ReadyTime + 22;

                        if (Timer < ReadyTime)
                        {

                        }
                        else if (Timer == ReadyTime)
                        {
                            Vector2 targetPos = CoraliteWorld.shadowBallsFightArea.TopLeft() + new Vector2(CoraliteWorld.shadowBallsFightArea.Width / 2, 9 * 16);
                            NPC.velocity = (targetPos - NPC.Center) / (DashTime - ReadyTime);
                        }
                        else if (Timer < DashTime)
                        {
                            Vector2 targetPos = CoraliteWorld.shadowBallsFightArea.TopLeft() + new Vector2(CoraliteWorld.shadowBallsFightArea.Width / 2, 9 * 16);
                            if (Vector2.Distance(targetPos, NPC.Center) < 16)
                            {
                                NPC.velocity *= 0f;
                                NPC.Center = targetPos;
                                Timer = DashTime;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0f;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://发射弹幕并自身消散后重组
                    {
                        const int FadeTime = 30;
                        const int WaitTime = FadeTime + 20;
                        const int ReTime = WaitTime + 30;

                        if (Timer == 2)
                        {
                            //射出来了
                            float angle = Main.rand.NextFloat(1f, 1.3f);

                            float baseAngle = -angle - MathHelper.PiOver2;

                            for (int i = 0; i < 9; i++)
                            {
                                Vector2 vel = baseAngle.ToRotationVector2() * 8;
                                int damage = Helper.ScaleValueForDiffMode(50, 45, 40, 40);
                                NPC.NewProjectileInAI<ShadowFire>(NPC.Center, vel, damage, 0, NPC.target);
                                baseAngle += angle * 2 / 8;
                            }
                        }

                        if (Timer < FadeTime)//消散
                        {

                        }
                        else if (Timer < WaitTime) { }
                        else if (Timer < ReTime)
                        {

                        }
                        else
                            ResetState();
                    }
                    break;
            }
        }

        #endregion

        #region SpaceSlash 瞄准玩家冲刺后留下拖尾，最后进行次元斩！



        #endregion
    }
}
