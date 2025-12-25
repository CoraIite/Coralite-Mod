using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class FaintEagle : BaseAlchorthentItem
    {
        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<FaintEagleProj>();

            Item.SetWeaponValues(15, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 50));
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center + new Vector2(player.direction * 20, 0), new Vector2(player.direction *4, -8), type, damage, knockback, player.whoAmI, 1);

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 0);

            player.AddBuff(ModContent.BuffType<FaintEagleBuff>(), 60);

            Helper.PlayPitched(CoraliteSoundID.SummonStaff_Item44, player.Center);
            Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_DD2_BetsyFireballImpact, player.Center, pitchAdjust: 0.4f);
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.CheckMana(1,true,true))
                return;

            player.manaRegenDelay = 40;

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 1);
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.StoneBlock, 3)
            //    .AddIngredient(ItemID.ClayBlock, 10)
            //    .AddIngredient(ItemID.CopperBar, 6)
            //    .AddTile(TileID.Campfire)
            //    .AddCondition(Condition.NearWater)
            //    .Register();

            //CreateRecipe()
            //    .AddIngredient(ItemID.StoneBlock, 3)
            //    .AddIngredient(ItemID.ClayBlock, 10)
            //    .AddIngredient(ItemID.TinBar, 6)
            //    .AddTile(TileID.Campfire)
            //    .AddCondition(Condition.NearWater)
            //    .Register();
        }
    }

    public class FaintEagleBuff: BaseAlchorthentBuff<FaintEagleProj>
    {
    }

    /// <summary>
    /// 火石鼎鹰召唤物，ai0控制火焰能量
    /// </summary>
    public class FaintEagleProj : BaseAlchorthentMinion<FaintEagleBuff>
    {
        public static int MaxFlameEnergy = 14;

        /// <summary> 总帧数 </summary>
        public const int TotalFrame = 31;
        public const int TotalFlyingCoreFrame = 14;

        const int backWingFrame = 0;
        const int backShellBackFrame = 1;
        const int backShellFrontFrame = 16;
        const int tailFrame = 17;
        const int bodyFrame = 18;
        const int headFrame = 19;
        const int headHighlightFrame = 20;
        const int frontWingFrame = 21;

        const int backWingFrameL = 22;
        const int backShellBackFrameL = 23;
        const int coreFrameL = 24;
        const int backShellFrontFrameL = 25;
        const int tailFrameL = 26;
        const int bodyFrameL = 27;
        const int headFrameL = 28;
        const int headHighlightFrameL = 29;
        const int frontWingFrameL = 30;

        const int TeleportDistance = 2000;

        /// <summary> 火焰能量 </summary>
        public ref float FlameCharge => ref Projectile.ai[0];
        public ref float Recorder => ref Projectile.ai[1];
        public ref float Recorder2 => ref Projectile.ai[2];
        public ref float Recorder3 => ref Projectile.localAI[1];
        public ref float Recorder4 => ref Projectile.localAI[2];

        /// <summary> 运动状态 </summary>
        public MoveStates MoveState
        {
            get => (MoveStates)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (int)value;
            }
        }

        /*
         * 一些用于平滑运动和动画的东西
         * 无需同步
         */
        private SecondOrderDynamics_Vec2 WingSmoother;
        private SecondOrderDynamics_Vec2 BackSmoother;
        private SecondOrderDynamics_Vec2 CoreSmoother;
        private SecondOrderDynamics_Vec2 HeadSmoother;
        private SecondOrderDynamics_Vec2 TailSmoother;

        private float SpecialRot;
        private Vector2 EXOffsetJump = Vector2.Zero;

        private enum AIStates : byte
        {
            /// <summary>
            /// 刚召唤出来，弹出
            /// </summary>
            OnSummon,
            /// <summary>
            /// 飞回玩家的过程
            /// </summary>
            BackToOwner,
            /// <summary>
            /// 在玩家身边盘旋
            /// </summary>
            Idle,
            /// <summary>
            /// 玩家挂机一会后就落地睡觉zzz
            /// </summary>
            Sleep,
            /// <summary>
            /// 冲刺攻击
            /// </summary>
            DashAttack,
            /// <summary>
            /// 强化版冲刺攻击
            /// </summary>
            ReinforcedDashAtack,
            /// <summary>
            /// 撞碎后重组自身
            /// </summary>
            Reassemble
        }

        public enum MoveStates
        {
            Land,
            Flying,
            Dashing,
            Reassemble
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 46;
            Projectile.decidesManualFallThrough = true;
        }

        #region AI

        public override void Initialize()
        {
            if (!VaultUtils.isServer)
            {
                WingSmoother = new SecondOrderDynamics_Vec2(7f, 0.5f, 0, Projectile.Center);
                BackSmoother = new SecondOrderDynamics_Vec2(9f, 0.6f, 0.2f, Projectile.Center);
                CoreSmoother = new SecondOrderDynamics_Vec2(20f, 1f, 0.4f, Projectile.Center);
                HeadSmoother = new SecondOrderDynamics_Vec2(17, 0.7f, 0.3f, Projectile.Center);
                TailSmoother = new SecondOrderDynamics_Vec2(5f, 0.5f, 0, Projectile.Center);
            }
        }

        public override void AIMoves()
        {
            switch (State)
            {
                default:
                case (byte)AIStates.OnSummon:
                    OnSummon();
                    Gravity(12, 0.4f);

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();
                    break;
                case (byte)AIStates.BackToOwner:
                    Timer++;

                    BackToOwner();

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();
                    break;
                case (byte)AIStates.Idle:
                    if (true)
                    {

                    }

                    Timer++;

                    Idle();

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();

                    break;
            }

            switch (MoveState)
            {
                case MoveStates.Land:
                    LandingBodyPartMovement();
                    break;
                case MoveStates.Flying:
                    Projectile.UpdateFrameNormally(3, 13);
                    FlyingBodyPartMovement();
                    break;
                case MoveStates.Reassemble:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 刚召唤出来，喷出来，自然落地
        /// </summary>
        public void OnSummon()
        {
            Timer++;

            Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(-0.2f, 0.2f));

            if (Timer > 40)
            {
                MoveState = MoveStates.Land;
                SwitchState(AIStates.BackToOwner);
                return;
            }
        }

        /// <summary>
        /// 回到玩家身边
        /// </summary>
        public void BackToOwner()
        {
            Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
            Vector2 aimPos = GetIdlePos(index, total);

            /*
             * 距离大于3000直接传送
             * 
             * 普通：蹦蹦跳跳回归
             * 每次跳跃开始时检测和玩家间的距离，如果和之前的距离相比没有减小那么就增加计数器
             *  计数到达8后变为穿墙飞行到目标位置
             *  
             * 如果Y距离太大那么直接变为穿墙飞行
             * 
             * 时间大于一定值直接传送
             */

            Projectile.shouldFallThrough = aimPos.Y - 12f > Projectile.Center.Y;
            float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

            if (distanceToAimPos > TeleportDistance || Timer > 60 * 16)
            {
                Projectile.velocity *= 0;
                Projectile.Center = aimPos;
                MoveState = MoveStates.Land;
                Projectile.tileCollide = true;
                ResetBodyPart();

                SwitchState(AIStates.Idle);

                return;
            }

            if (MoveState == MoveStates.Flying)//飞回来
            {
                Projectile.tileCollide = false;
                EXOffsetJump = Vector2.Zero;
                Recorder4 = Projectile.velocity.X/2;

                FlyBack(aimPos, 0.25f, 10);

                if (CanSwitchToLand(200, aimPos))
                {
                    Recorder = 0;
                    MoveState = MoveStates.Land;
                }

                if (distanceToAimPos < 40)
                {
                    Projectile.velocity.Y -= 2;
                    SwitchState(AIStates.Idle);
                }

                return;
            }

            /*
             * Recorder用于记录跳跃时间
             * Recorder2用于记录没能缩短距离的跳跃次数
             * Recorder3用于记录上一次的与玩家间距离
             */

            Projectile.tileCollide = true;

            if (distanceToAimPos < 20)
            {
                EXOffsetJump = Vector2.Zero;
                SwitchState(AIStates.Idle);
                Projectile.velocity.X *= 0.6f;
                Gravity(12, 0.4f);
                return;
            }

            if (Recorder > 60 * 3 || OnGround)//落地了，再次起跳
            {
                if (Recorder < 60 * 3 + 1)
                    Recorder = 60 * 3 + 1;
                Recorder++;

                Projectile.velocity.X *= 0.5f;
                UpdateExtraOffsetJump();

                if (Recorder > 60 * 3 + 8 + Projectile.whoAmI % 10 && MathF.Abs(Projectile.velocity.X) < 0.5f)//x方向减速，减小到一定值后才能再次起跳
                {
                    if (Recorder3 <= distanceToAimPos + 16 * 2)
                        Recorder2++;
                    else
                        Recorder2 = 0;

                    Recorder3 = distanceToAimPos;

                    //检测是否需要直接起飞，弹幕距离目标点Y值过远或者跳了好多次都没能缩短与目标点的距离
                    if (((MathF.Abs(Projectile.Center.Y - aimPos.Y) > 16 * 14 || Recorder2 > 5)
                        && Vector2.Distance(aimPos, Projectile.Center) > 200)
                        || Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        MoveState = MoveStates.Flying;
                        Recorder2 = 0;
                        Projectile.velocity.Y = -4;
                        return;
                    }

                    Jump(aimPos);
                }

                return;
            }

            //持续给予X方向速度
            float exMult = 1;
            if (MathF.Abs(Projectile.Center.Y - aimPos.Y) > 16 * 6)
                exMult = Math.Clamp(MathF.Abs(Projectile.Center.X - aimPos.X) / 200f, 0.05f, 1);

            Projectile.velocity.X = Recorder4 * exMult;
            Recorder++;
            Gravity(12, 0.4f);
        }

        /// <summary>
        /// 保持在玩家身边
        /// </summary>
        public void Idle()
        {
            Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
            Vector2 aimPos = GetIdlePos(index, total);

            /*
             * 距离大于3000直接传送
             * 
             * 普通：蹦蹦跳跳回归
             * 每次跳跃开始时检测和玩家间的距离，如果和之前的距离相比没有减小那么就增加计数器
             *  计数到达8后变为穿墙飞行到目标位置
             *  
             * 如果Y距离太大那么直接变为穿墙飞行
             * 
             * 时间大于一定值直接传送
             */

            Projectile.shouldFallThrough = aimPos.Y - 12f > Projectile.Center.Y;
            float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

            if (distanceToAimPos > TeleportDistance)
            {
                Projectile.velocity *= 0;
                Projectile.Center = aimPos;
                MoveState = MoveStates.Land;
                Projectile.tileCollide = true;
                ResetBodyPart();

                return;
            }

            if (MoveState == MoveStates.Flying)//飞回来
            {
                Projectile.tileCollide = false;
                Recorder4 = Projectile.velocity.X / 2;
                EXOffsetJump = Vector2.Zero;

                if (distanceToAimPos > 80)
                    FlyBack(aimPos, 0.25f, 10);
                else if (Recorder3 < 70 && distanceToAimPos > 70)//在目标点旁边绕圈飞行
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Projectile.velocity = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero).RotateByRandom(-0.7f, 0.7f) * Main.rand.NextFloat(0.5f,2f);
                        Projectile.netUpdate = true;
                    }
                }
                else if (Projectile.velocity.Length() > 4)
                    Projectile.velocity *= 0.97f;

                Recorder3 = distanceToAimPos;

                if (CanSwitchToLand(200, aimPos))
                {
                    Recorder = 0;
                    MoveState = MoveStates.Land;
                }

                return;
            }

            /*
             * Recorder用于记录跳跃时间
             * Recorder2用于记录没能缩短距离的跳跃次数
             * Recorder3用于记录上一次的与玩家间距离
             */

            Projectile.tileCollide = true;

            if (distanceToAimPos < 20 || (MathF.Abs(Projectile.Center.X - aimPos.X) < 20 && MathF.Abs(Projectile.Center.Y - aimPos.Y) < 16 * 7 && Projectile.velocity.Y == 0))
            {
                EXOffsetJump = Vector2.Zero;
                Projectile.velocity.X *= 0.6f;
                Gravity(12, 0.4f);
                return;
            }

            if (Recorder > 60 * 3 || OnGround)//落地了，再次起跳
            {
                if (Recorder < 60 * 3 + 1)
                    Recorder = 60 * 3 + 1;
                Recorder++;

                Projectile.velocity.X *= 0.5f;

                UpdateExtraOffsetJump();

                if (Recorder > 60 * 3 + 15 + Projectile.whoAmI % 10 && MathF.Abs(Projectile.velocity.X) < 0.5f)//x方向减速，减小到一定值后才能再次起跳
                {
                    if (Recorder3 <= distanceToAimPos+16*2)
                        Recorder2++;
                    else
                        Recorder2 = 0;

                    Recorder3 = distanceToAimPos;

                    //检测是否需要直接起飞，弹幕距离目标点Y值过远或者跳了好多次都没能缩短与目标点的距离
                    if ((MathF.Abs(Projectile.Center.Y - aimPos.Y) > 16 * 14 || Recorder2 > 5)
                        && Vector2.Distance(aimPos, Projectile.Center) > 200
                        || Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        MoveState = MoveStates.Flying;
                        Recorder2 = 0;
                        Projectile.velocity.Y = -4;
                        return;
                    }

                    Jump(aimPos);
                }

                return;
            }

            //持续给予X方向速度
            float exMult = 1;
            if (MathF.Abs(Projectile.Center.Y - aimPos.Y)>16*6)
                exMult = Math.Clamp(MathF.Abs(Projectile.Center.X - aimPos.X) / 200f, 0.05f, 1);

            Projectile.velocity.X = Recorder4 *exMult;
            Recorder++;
            Gravity(12, 0.4f);
        }


        /// <summary>
        /// 根据与目标点的Y距离决定跳跃高度，X距离决定跳跃长度
        /// </summary>
        /// <param name="aimPos"></param>
        private void Jump(Vector2 aimPos)
        {
            float ySpeed = Math.Clamp(MathF.Abs(Projectile.Center.Y - aimPos.Y) / 10, 5, 12);
            float xSpeed = Math.Clamp(MathF.Abs(Projectile.Center.X - aimPos.X) / 30, 1, 8);
            Projectile.velocity = new Vector2((aimPos.X > Projectile.Center.X ? 1 : -1) * xSpeed, -ySpeed);
            Recorder4 = Projectile.velocity.X;
            Recorder = 0;
            EXOffsetJump = Vector2.Zero;
        }

        private void UpdateExtraOffsetJump()
        {
            if (MathF.Abs(EXOffsetJump.X) < 6)
                EXOffsetJump.X += Projectile.spriteDirection * 0.6f;
            if (EXOffsetJump.Y < 8)
                EXOffsetJump.Y += 0.8f;
        }

        public override Vector2 GetIdlePos(int selfIndex, int totalCount)
        {
            //左边一个右边一个
            int dir = selfIndex % 2 == 0 ? -1 : 1;
            return Owner.Bottom + new Vector2(dir * (selfIndex * 20+28), -Projectile.height / 2);
        }

        private void SwitchState(AIStates targetState)
        {
            State = (byte)targetState;

            Timer = 0;

            if (Projectile.IsOwnedByLocalPlayer())
                Projectile.netUpdate = true;
        }

        /// <summary>
        /// 获得能量
        /// </summary>
        public void GetFlameEnergy()
        {
            if (FlameCharge < MaxFlameEnergy)
                FlameCharge++;
        }

        public void SetRotNoramlly()
        {
            Projectile.rotation = (Projectile.spriteDirection > 0 ? 0 : MathHelper.Pi) + Projectile.spriteDirection * Math.Clamp(Projectile.velocity.Y / 40, -0.4f, 0.4f);
        }

        public void SetSpriteDirectionNormally()
        {
            //if (Projectile.velocity.Length() < 0.1f)
            //{
            //    Projectile.spriteDirection = Owner.direction;
            //    return;
            //}

            if (MathF.Abs(Projectile.velocity.X) > 0.3f)
                Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);
        }

        #region 身体部件运动部分

        /// <summary>
        /// 在地面状态的身体部件运动
        /// </summary>
        public void LandingBodyPartMovement()
        {
            Vector2 basePos = Projectile.Center + Projectile.velocity + EXOffsetJump;
            Vector2 normal = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            WingSmoother.Update(1 / 60f, basePos + normal * MathF.Sin(Timer * 0.05f) * 2);
            BackSmoother.Update(1 / 60f, basePos + normal * MathF.Cos(Timer * 0.03f) * 2);
            CoreSmoother.Update(1 / 60f, basePos);
            HeadSmoother.Update(1 / 60f, basePos + dir * MathF.Abs(MathF.Sin(Timer * 0.015f)) * 2 + (-Timer * 0.025f).ToRotationVector2() * 1);
            TailSmoother.Update(1 / 60f, basePos + dir * MathF.Cos(Timer * 0.05f) * 2);
        }

        /// <summary>
        /// 在地面状态的身体部件运动
        /// </summary>
        public void FlyingBodyPartMovement()
        {
            Vector2 basePos = Projectile.Center + Projectile.velocity;
            Vector2 normal = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            WingSmoother.Update(1 / 60f, basePos + normal * MathF.Sin(Timer * 0.2f) * 6);
            BackSmoother.Update(1 / 60f, basePos + normal * MathF.Cos(Timer * 0.075f) * 2);
            CoreSmoother.Update(1 / 60f, basePos);
            HeadSmoother.Update(1 / 60f, basePos + (Timer * 0.1f).ToRotationVector2() * 2);
            TailSmoother.Update(1 / 60f, basePos + Projectile.rotation.ToRotationVector2() * MathF.Cos(Timer * 0.05f) * 2);
        }

        public void ResetBodyPart()
        {
            WingSmoother.Reset(Projectile.Center);
            BackSmoother.Reset(Projectile.Center);
            CoreSmoother.Reset(Projectile.Center);
            HeadSmoother.Reset(Projectile.Center);
            TailSmoother.Reset(Projectile.Center);
        }

        #endregion

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = Projectile.rotation + (Projectile.spriteDirection > 0 ? 0 : MathHelper.Pi);

            switch (MoveState)
            {
                case MoveStates.Land:
                    DrawLanding(mainTex, lightColor, rot, effect);
                    break;
                case MoveStates.Flying:
                    DrawFlying(mainTex, lightColor, rot, effect);
                    break;
                case MoveStates.Reassemble:
                    DrawLanding(mainTex, lightColor, rot, effect);
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// 绘制飞行帧
        /// </summary>
        /// <param name="mainTex"></param>
        /// <param name="lightColor"></param>
        /// <param name="rot"></param>
        /// <param name="effect"></param>
        public void DrawFlying(Texture2D mainTex,Color lightColor, float rot, SpriteEffects effect)
        {
            //绘制后面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, backWingFrame, rot, effect);

            //绘制背壳后层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellBackFrame, rot, effect);

            //绘制核心
            if (CoreSmoother != null)
                DrawLayer(mainTex, CoreSmoother.y, Color.White*0.8f, 2 + Projectile.frame, rot, effect, false);

            //绘制背壳前层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellFrontFrame, rot, effect);

            //绘制以巴
            if (TailSmoother != null)
                DrawLayer(mainTex, TailSmoother.y, lightColor, tailFrame, rot, effect);

            //绘制胸壳
            DrawLayer(mainTex, Projectile.Center, lightColor, bodyFrame, rot, effect);

            //绘制头
            if (HeadSmoother != null)
            {
                DrawLayer(mainTex, HeadSmoother.y, lightColor, headFrame, rot, effect);
                DrawLayer(mainTex, HeadSmoother.y, Color.White * 0.8f, headHighlightFrame, rot, effect, false);
            }

            //绘制前面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, frontWingFrame, rot, effect);
        }

        public void DrawLanding(Texture2D mainTex, Color lightColor, float rot, SpriteEffects effect)
        {
            //绘制后面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, backWingFrameL, rot, effect);

            //绘制背壳后层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellBackFrameL, rot, effect);

            //绘制核心
            if (CoreSmoother != null)
                DrawLayer(mainTex, CoreSmoother.y, Color.White * 0.8f, coreFrameL, rot, effect, false);

            //绘制背壳前层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellFrontFrameL, rot, effect);

            //绘制以巴
            if (TailSmoother != null)
                DrawLayer(mainTex, TailSmoother.y, lightColor, tailFrameL, rot, effect);

            //绘制胸壳
            DrawLayer(mainTex, Projectile.Center, lightColor, bodyFrameL, rot, effect);

            //绘制头
            if (HeadSmoother != null)
            {
                DrawLayer(mainTex, HeadSmoother.y, lightColor, headFrameL, rot, effect);
                DrawLayer(mainTex, HeadSmoother.y, Color.White * 0.8f, headHighlightFrameL, rot, effect, false);
            }

            //绘制前面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, frontWingFrameL, rot, effect);
        }

        public void DrawReassemble(Texture2D mainTex, Color lightColor, float rot, SpriteEffects effect)
        {
            //绘制后面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, backWingFrameL, rot, effect);

            //绘制背壳后层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellBackFrameL, rot, effect);

            //绘制核心
            if (CoreSmoother != null)
                DrawLayer(mainTex, CoreSmoother.y, Color.White * 0.8f, coreFrameL, rot, effect,false);

            //绘制背壳前层
            if (BackSmoother != null)
                DrawLayer(mainTex, BackSmoother.y, lightColor, backShellFrontFrameL, rot, effect);

            //绘制以巴
            if (TailSmoother != null)
                DrawLayer(mainTex, TailSmoother.y, lightColor, tailFrameL, rot, effect);

            //绘制胸壳
            DrawLayer(mainTex, Projectile.Center, lightColor, bodyFrameL, rot, effect);

            //绘制头
            if (HeadSmoother != null)
            {
                DrawLayer(mainTex, HeadSmoother.y, lightColor, headFrameL, rot, effect);
                DrawLayer(mainTex, HeadSmoother.y, Color.White * 0.8f, headHighlightFrameL, rot, effect,false);
            }

            //绘制前面的翅膀
            if (WingSmoother != null)
                DrawLayer(mainTex, WingSmoother.y, lightColor, frontWingFrameL, rot, effect);
        }

        /// <summary>
        /// 绘制单层
        /// </summary>
        /// <param name="mainTex"></param>
        /// <param name="color"></param>
        /// <param name="frame"></param>
        /// <param name=""></param>
        public void DrawLayer(Texture2D mainTex, Vector2 pos, Color color, int frame, float rot, SpriteEffects effect, bool drawHighlight = true)
        {
            mainTex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, frame, 1, TotalFrame), pos - Main.screenPosition, color, rot, Projectile.scale, effect);

            if (drawHighlight && FlameCharge > 0)//有能量时绘制一层描边
            {
                float factor = 0.5f * FlameCharge / MaxFlameEnergy + (FlameCharge == MaxFlameEnergy ? 0.5f : 0);

                Color lightC = Color.Lerp(Color.Transparent, new Color(235, 180, 150, 100), factor);
                float scale = 1 + 0.05f * factor;
                Vector2 pos2 = pos
                    - Main.screenPosition
                    - Projectile.rotation.ToRotationVector2() * (MathF.Sin((int)Main.timeForVisualEffects * 0.1f) + 1)*0.5f;
                mainTex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, frame, 1, TotalFrame), pos2, lightC, rot, Projectile.scale * scale, effect);
            }
        }

        #endregion
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class FaintEagleHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public static ATex FaintEagleHeldProjHighlight { get; set; }

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ModContent.ItemType<FaintEagle>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.UpdateFrameNormally(3, 7);
            SetHeld();


            if (State == 1)
                SpecialAttack();
            else
                NormalState();
        }

        public void SpecialAttack()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.direction = ToMouse.X > 0 ? 1 : -1;
            }

            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Projectile.rotation = ToMouseA;
            Projectile.Center = Owner.Center + UnitToMouseV * 26 + new Vector2(0, Owner.gfxOffY);

            Timer++;

            Dust d = Dust.NewDustPerfect(Projectile.Center + UnitToMouseV * 8 + Main.rand.NextVector2Circular(12, 12)
                 , DustID.Torch, UnitToMouseV * Main.rand.NextFloat(1, 5), Scale: Main.rand.NextFloat(0.8f, 1.2f));
            d.noGravity = true;

            if (Timer > 20)
            {
                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 25;
                    Helper.PlayPitched("Misc/FireWhoosh" + (Timer%2==0?1:2), 0.4f, 0, Projectile.Center);
                }

                //生成火焰弹幕
                if (Timer % 4 == 0)
                {
                    if (!Owner.CheckMana(1, true, true))
                    {
                        Timer = -30;
                        return;
                    }

                    Owner.manaRegenDelay = 40;

                    Projectile.NewProjectileFromThis<FaintEagleFire>(Projectile.Center + UnitToMouseV * 30, UnitToMouseV.RotateByRandom(-0.05f, 0.05f) * Main.rand.NextFloat(9, 13), Projectile.damage / 2, Projectile.knockBack, Main.rand.Next(3), Main.rand.Next(2));
                }
            }
        }

        public void NormalState()
        {
            if (Timer == 0)
            {
                Timer++;
                Projectile.timeLeft = Owner.itemTimeMax;
            }

            Owner.itemRotation = 0;
                Projectile.rotation = DirSign > 0 ? 0 : MathHelper.Pi;
            Projectile.Center = Owner.Center + new Vector2(DirSign * 26, 0) + new Vector2(0, Owner.gfxOffY);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var rect = mainTex.Frame(1, 8, 0, Projectile.frame);

            bool dir = Owner.direction > 0;
            float rot = Projectile.rotation + (dir ? -MathHelper.PiOver4 : MathHelper.Pi + MathHelper.PiOver4);
            SpriteEffects effect = dir ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 pos = Projectile.Center - Main.screenPosition
                + (Projectile.rotation + (dir ? MathHelper.PiOver2 : -MathHelper.PiOver2)).ToRotationVector2() * 4;

            Main.spriteBatch.Draw(mainTex, pos, rect, lightColor, rot,
                rect.Size() / 2, Projectile.scale, effect, 0);
            Main.spriteBatch.Draw(FaintEagleHeldProjHighlight.Value, pos, rect, Color.White*0.8f, rot,
                rect.Size() / 2, Projectile.scale, effect, 0);

            return false;
        }
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class FaintEagleFire : ModProjectile
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public static ATex FaintEagleFire2 { get; set; }
        public static ATex FaintEagleFire3 { get; set; }

        public ref float TexType => ref Projectile.ai[0];
        public ref float Flip => ref Projectile.ai[1];
        public bool CanHit
        {
            get => Projectile.localAI[0] == 0;
            set
            {
                if (value)
                    Projectile.localAI[0] = 0;
                else
                    Projectile.localAI[0] = 1;
            }
        }

        public ref float Timer => ref Projectile.localAI[1];
        public ref float Alpha => ref Projectile.localAI[2];
        public bool Heated
        {
            get => Projectile.ai[2] == 1;
            set
            {
                if (value)
                    Projectile.ai[2] = 1;
                else
                    Projectile.ai[2] = 0;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 48;
            Projectile.scale = 0.8f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage()
        {
            if (CanHit)
                return null;

            return false;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(2, 16);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOwnedByLocalPlayer() && Projectile.frame > 15)
                Projectile.Kill();

            if (Timer == 0)
            {
                Alpha = 1;
            }

            if (Projectile.frame > 10)
            {
                Alpha *= 0.95f;
                Projectile.velocity *= 0.89f;
            }
            else
                Projectile.velocity *= 0.975f;

            Timer++;
            if (Timer == 15)
                Projectile.Resize(50, 50);

            HeatEagle();
            SpawnParticle();
        }

        public void HeatEagle()
        {
            if (Heated)
                return;

            if (Timer % 2 == 0)//找火鹰弹幕并给它加能量
            {
                int type = ModContent.ProjectileType<FaintEagleProj>();
                foreach (var proj in Main.ActiveProjectiles)
                    if (proj.owner == Projectile.owner && proj.type == type)
                    {
                        (proj.ModProjectile as FaintEagleProj).GetFlameEnergy();
                        Heated = true;
                        break;
                    }
            }
        }

        public void SpawnParticle()
        {
            if (Projectile.frame > 10)
                return;

            if (Timer % 10 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.7f, Projectile.height * 0.7f);
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0://生成代表火的三角形
                        PRTLoader.NewParticle<FaintEagleParticle1>(pos, Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-0.7f, 0.7f), Color.White with { A = 120 }, Main.rand.NextFloat(0.7f, 1f));
                        break;
                    case 1:
                        {
                            Vector2 dir = Projectile.velocity.RotateByRandom(-0.2f, 0.2f);
                            var particle = PRTLoader.NewParticle<FaintEagleParticle2>(pos, dir * 0.3f, Color.White with { A = 120 }, Main.rand.NextFloat(1f, 1.5f));

                            particle.Rotation = dir.ToRotation();
                        }
                        break;
                    case 2:
                        {
                            Vector2 dir = Projectile.velocity.RotateByRandom(-0.2f, 0.2f);
                            var particle = PRTLoader.NewParticle<FaintEagleParticle3>(pos, dir * 0.4f, Color.White with { A = 120 }, Main.rand.NextFloat(1f, 1.5f));

                            particle.Rotation = dir.ToRotation();
                        }
                        break;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.2f;
            Projectile.tileCollide = false;
            CanHit = false;

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.damage > 2)
                Projectile.damage--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;

            SpriteEffects effect = Flip == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color backColor = Color.White with { A = 0 } * Alpha;

            Texture2D mainTex;

            if (TexType == 0)
                mainTex = TextureAssets.Projectile[Projectile.type].Value;
            else if (TexType == 1)
                mainTex = FaintEagleFire2.Value;
            else
                mainTex = FaintEagleFire3.Value;

            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);
            var rect = mainTex.Frame(1, 15, 0, Projectile.frame);

            for (int i = 1; i < 6; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, rect,
                    backColor * (0.2f - (i * 0.2f / 6)), Projectile.oldRot[i] + MathHelper.Pi, rect.Size() / 2, Projectile.scale, effect, 0);


            //绘制一层更大的在后面
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.Pi;

            Main.spriteBatch.Draw(mainTex, pos, rect, backColor * 0.15f, rot,
                rect.Size() / 2, Projectile.scale * 1.2f, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, rect, Color.White * Alpha * 0.8f, rot,
                rect.Size() / 2, Projectile.scale, effect, 0);

            return false;
        }
    }

    /// <summary>
    /// 三角形的粒子，炼金术元素的火
    /// </summary>
    public class FaintEagleParticle1() : BaseFrameParticle(1, 15, 2)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.98f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }

    public class FaintEagleParticle2() : BaseFrameParticle(1, 9, 1)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.96f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }

    public class FaintEagleParticle3() : BaseFrameParticle(1, 8, 1)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.96f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }
}
