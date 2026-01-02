using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Loaders;
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
using Terraria.Graphics.Effects;
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
            PRTLoader.NewParticle<AlchSymbolFire>(Main.MouseWorld, Vector2.Zero, new Color(203, 66, 66));

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
        public static int MaxFlameEnergy = 32;

        /// <summary> 总帧数 </summary>
        public const int TotalFrame = 47;
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

        const int backWingFlyingBase = 31;
        const int frontWingFlyingBase = 39;

        const int TeleportDistance = 2000;

        /// <summary> 火焰能量 </summary>
        public ref float FlameCharge => ref Projectile.ai[0];
        /// <summary> 完全充能 </summary>
        public bool FullFlameCharge => FlameCharge == MaxFlameEnergy;

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

        private short moveStateSwitchTimer;

        /*
         * 一些用于平滑运动和动画的东西
         * 无需同步
         */
        private SecondOrderDynamics_Vec2 WingSmoother;
        private SecondOrderDynamics_Vec2 BackSmoother;
        private SecondOrderDynamics_Vec2 CoreSmoother;
        private SecondOrderDynamics_Vec2 HeadSmoother;
        private SecondOrderDynamics_Vec2 TailSmoother;

        private LineDrawer FlameEffect;

        private float SpecialRot;
        private Vector2 EXOffsetJump = Vector2.Zero;
        private byte wingFrame;
        private byte wingFrameCounter;
        private short SleepingTimer;
        private short FlameEffectTimer;



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
            Sleep,
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
            Timer++;

            switch (State)
            {
                default:
                case (byte)AIStates.OnSummon://刚召唤出来，蹦出来一小会后进入回归玩家阶段
                    OnSummon();
                    Gravity(12, 0.4f);

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();
                    break;
                case (byte)AIStates.BackToOwner://回归玩家，目标是来到距离目标点小于一定值的位置上
                    BackToOwner();

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();
                    break;
                case (byte)AIStates.Idle:
                    //寻敌，找到敌怪就进入攻击状态
                    if (Timer > 40 && Main.rand.NextBool(20))
                        if (FindEnemy())
                        {
                            SwitchState(AIStates.DashAttack);
                            break;
                        }

                    Idle();

                    SetSpriteDirectionNormally();
                    SetRotNoramlly();

                    break;
            }

            switch (MoveState)
            {
                case MoveStates.Land:
                    wingFrame = wingFrameCounter = 0;
                    LandingBodyPartMovement();
                    break;
                case MoveStates.Flying:
                    float factor = Math.Clamp(Projectile.velocity.Length(), 2, 15) / 15f;
                    if (++wingFrameCounter > (7 - 6 * factor))
                    {
                        wingFrameCounter = 0;
                        wingFrame++;
                        if (wingFrame > 7)
                            wingFrame = 0;
                    }

                    if (Main.rand.NextBool(7))
                    {
                        if (Main.rand.NextBool())
                            Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(-0.2f, 0.2f));
                        else
                        {
                            PRTLoader.NewParticle<FaintEagleParticle1>(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), new Vector2(-Projectile.spriteDirection, 0) * Main.rand.NextFloat(-0.7f, 0.7f), Color.White with { A = 120 }, Main.rand.NextFloat(0.7f, 1f));
                        }
                    }

                    Projectile.UpdateFrameNormally(3, 13);
                    FlyingBodyPartMovement(false);
                    break;
                case MoveStates.Dashing:
                    Projectile.UpdateFrameNormally(3, 13);
                    FlyingBodyPartMovement(true);
                    break;
                case MoveStates.Reassemble:
                    wingFrame = wingFrameCounter = 0;
                    break;
                case MoveStates.Sleep:
                    wingFrame = wingFrameCounter = 0;
                    SleepingBodyPartMovement();
                    break;
                default:
                    break;
            }

            if (moveStateSwitchTimer > 0)
                moveStateSwitchTimer--;

            UpdateFlameEffect();
        }

        /// <summary>
        /// 刚召唤出来，喷出来，自然落地
        /// </summary>
        public void OnSummon()
        {
            Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(-0.2f, 0.2f));

            if (Timer > 40 || OnGround)
            {
                SwitchMoveState(MoveStates.Land,true);
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
                Projectile.tileCollide = true;
                Recorder = 0;

                PRTLoader.NewParticle<AlchSymbolFire>(Projectile.Center, Vector2.Zero, new Color(203, 66, 66));

                ResetBodyPart();

                SwitchMoveState(MoveStates.Land, true);
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
                    SwitchMoveState(MoveStates.Land);
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
                Recorder4 = 0;
                Recorder = 60 * 3;

                Gravity(12, 0.4f);
                return;
            }

            if (Recorder > 60 * 3 || OnGround)//落地了，再次起跳
            {
                if (Recorder < 60 * 3 + 1)
                {
                    OnLand();
                    Recorder = 60 * 3 + 1;
                }

                Recorder++;

                Projectile.velocity.X *= 0.5f;
                UpdateExtraOffsetJump();

                if (Recorder > 60 * 3 + 12 + MathF.Sin(Projectile.whoAmI * 0.2f) * 7 && MathF.Abs(Projectile.velocity.X) < 0.5f)//x方向减速，减小到一定值后才能再次起跳
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
                        SwitchMoveState(MoveStates.Flying,true);
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
                Projectile.tileCollide = true;
                Recorder = 0;

                PRTLoader.NewParticle<AlchSymbolFire>(Projectile.Center, Vector2.Zero, new Color(203, 66, 66));

                SwitchMoveState(MoveStates.Land, true);
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
                    SwitchMoveState(MoveStates.Land);
                }

                return;
            }

            /*
             * Recorder2用于记录跳跃时间
             * Recorder3用于记录没能缩短距离的跳跃次数
             * Recorder4用于记录上一次的与玩家间距离
             */

            Projectile.tileCollide = true;

            //不用跳的状态，就保持原地不动，一段时间后睡觉
            if ((distanceToAimPos < 20
                || (MathF.Abs(Projectile.Center.X - aimPos.X) < 20 && MathF.Abs(Projectile.Center.Y - aimPos.Y) < 16 * 7))
                && OnGround)
            {
                EXOffsetJump = Vector2.Zero;
                Projectile.velocity.X *= 0.6f;
                Recorder = 60 * 3;
                Recorder4 = 0;

                if (Projectile.velocity.Length() < 0.001f)
                    SleepingTimer++;
                else
                    SleepingTimer = 0;

                Gravity(12, 0.4f);

                if (!FullFlameCharge && SleepingTimer > 60 * 5)//开睡
                    SwitchMoveState(MoveStates.Sleep);

                return;
            }

            //取消睡觉
            SleepingTimer = 0;
            if (MoveState != MoveStates.Land)
                SwitchMoveState(MoveStates.Land, true);

            if (Recorder > 60 * 3 || OnGround)//落地了，再次起跳
            {
                if (Recorder < 60 * 3 + 1)
                {
                    OnLand();
                    Recorder = 60 * 3 + 1;
                }

                Recorder++;

                Projectile.velocity.X *= 0.5f;

                UpdateExtraOffsetJump();

                if (Recorder > 60 * 3 + 17 + MathF.Sin(Projectile.whoAmI * 0.5f) * 7 && MathF.Abs(Projectile.velocity.X) < 0.5f)//x方向减速，减小到一定值后才能再次起跳
                {
                    if (Recorder3 <= distanceToAimPos + 16 * 2)
                        Recorder2++;
                    else
                        Recorder2 = 0;

                    Recorder3 = distanceToAimPos;

                    //检测是否需要直接起飞，弹幕距离目标点Y值过远或者跳了好多次都没能缩短与目标点的距离
                    if ((MathF.Abs(Projectile.Center.Y - aimPos.Y) > 16 * 14 || Recorder2 > 5)
                        && Vector2.Distance(aimPos, Projectile.Center) > 200
                        || Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        SwitchMoveState(MoveStates.Flying, true);
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

        public void DashAttack()
        {
            /*
             * 第一步：切换为飞行状态
             * 第二步，飞到目标上空指定位置
             * 第三步：冲向目标
             * 第四步：冲刺过程中像目标微调
             * 第五步：冲刺一定时间后停止
             * 第六步：检测能否重新攻击
             * 
             * 使用Recorder记录子状态
             */

            //敌人消失，返回玩家
            if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
            {
                if (FindEnemy())//能找到新敌人，继续攻击
                {
                    SwitchState(AIStates.DashAttack);
                    return;
                }

                SwitchState(AIStates.BackToOwner);
                return;
            }

            switch (Recorder)
            {
                default:
                case 0: //切换为飞行状态
                    {
                        SwitchMoveState(MoveStates.Flying, true);
                        Recorder = 1;
                    }
                    break;
                case 1://根据自身whoami定位目标位置
                    break;
            }

            Vector2 GetAttackStartPos(NPC target)
            {
                Vector2 pos = target.Center;



                return pos;
            }
        }

        public bool FindEnemy()
        {
            if (!Target.GetNPCOwner(out NPC target, () => Target = -1))//目前没有敌人，找一下
            {
                int targetInd1 = Helper.MinionFindTarget(Projectile, maxChaseLength: 800);
                if (targetInd1 == -1)
                    return false;

                Target = targetInd1;
                return true;
            }

            //有敌人，检测敌人是否能再次被攻击
            if (!target.CanBeChasedBy()
                || Vector2.Distance(Projectile.Center, target.Center) > 800
                || !Projectile.CanHitWithOwnBody(target))//无法造成伤害
            {
                Target = -1;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 落地时调用，向两侧生成落地粒子
        /// </summary>
        private void OnLand()
        {
            if (Recorder < 60 * 3 && OnGround)
            {
                Helper.PlayPitched("Misc/HeavyLanding", 0.1f, Main.rand.NextFloat(0.7f, 1.5f), Projectile.Center);

                for (int i = -1; i < 2; i += 2)
                {
                    var p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Bottom + new Vector2(i * -6, 0), new Vector2(i * 0.4f, 0), new Color(203, 66, 66), 1f);
                    p.direction = i;
                    p.Rotation = p.direction > 0 ? 0 : MathHelper.Pi;
                    p.scale2 = new Vector2(Main.rand.NextFloat(0.6f, 1f), Main.rand.NextFloat(0.5f, 0.8f));
                    p.alpha = 0.4f;
                    p.addAlpha = 0.6f;
                    p.addDraw = true;

                    p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Bottom + new Vector2(i * -3, 0), new Vector2(i * 0.2f, 0), new Color(253, 133, 81), 1f);
                    p.direction = i;
                    p.Rotation = p.direction > 0 ? 0 : MathHelper.Pi;
                    p.scale2 = new Vector2(Main.rand.NextFloat(0.4f, 0.6f), Main.rand.NextFloat(1.1f, 1.3f));
                    p.alpha = 0.5f;
                    p.addAlpha = 0.6f;
                    p.addDraw = true;

                    for (int k = 0; k < 6; k++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Bottom + new Vector2(i * 6, 0) + Main.rand.NextVector2Circular(6, 3), DustID.Torch, new Vector2(i * Main.rand.NextFloat(1, 4), Main.rand.NextFloat(-2, 0)),Scale:Main.rand.NextFloat(1,2));
                        d.noGravity = true;
                    }
                }
            }
        }

        /// <summary>
        /// 根据与目标点的Y距离决定跳跃高度，X距离决定跳跃长度
        /// </summary>
        /// <param name="aimPos"></param>
        private void Jump(Vector2 aimPos)
        {
            Projectile.spriteDirection = (aimPos.X > Projectile.Center.X ? 1 : -1);

            //生成跳跃粒子
            if (OnGround)
            {
                var p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Bottom + new Vector2(-Projectile.spriteDirection * 6, 0), new Vector2(-Projectile.spriteDirection * 0.4f, 0), new Color(244, 216, 182), 1f);
                p.direction = -Projectile.spriteDirection;
                p.Rotation = p.direction > 0 ? 0 : MathHelper.Pi;
                p.scale2 = new Vector2(Main.rand.NextFloat(0.8f, 1.2f), Main.rand.NextFloat(0.5f, 0.8f));

                p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Bottom + new Vector2(Projectile.spriteDirection * 4, 0), new Vector2(-Projectile.spriteDirection * 0.2f, 0), new Color(244, 216, 182), 1f);
                p.direction = -Projectile.spriteDirection;
                p.Rotation = p.direction > 0 ? 0 : MathHelper.Pi;
                p.scale2 = new Vector2(0.7f, 1.2f);
                p.alpha = 0.7f;
            }

            float ySpeed = Math.Clamp(MathF.Abs(Projectile.Center.Y - aimPos.Y) / 10, 5, 12);
            float xSpeed = Math.Clamp(MathF.Abs(Projectile.Center.X - aimPos.X) / 30, 1, 8);
            Projectile.velocity = new Vector2((aimPos.X > Projectile.Center.X ? 1 : -1) * xSpeed, -ySpeed);
            Recorder4 = Projectile.velocity.X;
            Recorder = 0;
            EXOffsetJump = Vector2.Zero;
        }


        /// <summary>
        /// 更新火焰三角形特效
        /// </summary>
        private void UpdateFlameEffect()
        {
            if (FullFlameCharge)
            {
                if (FlameEffectTimer == 0)//初始化线段
                {
                    float height = 2 * 1.732f - 8 / 3f;

                    FlameEffect ??= new LineDrawer([
                        new LineDrawer.StraightLine(new Vector2(0,-8/3f),new Vector2(2,height)),
                        new LineDrawer.StraightLine(new Vector2(2,height),new Vector2(-2,height)),
                        new LineDrawer.StraightLine(new Vector2(-2,height),new Vector2(0,-8/3f)),
                        ]);

                    FlameEffect.SetLineWidth(12);
                }

                if (FlameEffectTimer < 45)//更新线段
                {
                    FlameEffect.SetScale(6 * Helper.BezierEase(FlameEffectTimer / 45f));

                    FlameEffectTimer++;
                }
            }
            else
            {
                if (FlameEffectTimer != 0)
                {
                    FlameEffectTimer = 0;
                    FlameEffect.SetScale(0);
                }
            }
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
            Recorder = 0;
            SleepingTimer = 0;
            moveStateSwitchTimer = 0;

            if (Projectile.IsOwnedByLocalPlayer())
                Projectile.netUpdate = true;
        }

        /// <summary>
        /// 切换运动状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="forced">是否强制执行切换</param>
        private void SwitchMoveState(MoveStates state, bool forced = false)
        {
            if (!forced && moveStateSwitchTimer != 0)
                return;

            if ((MoveState is MoveStates.Flying or MoveStates.Dashing or MoveStates.Sleep && state == MoveStates.Land)
                || (MoveState == MoveStates.Land && state is MoveStates.Flying))//飞行和落地切换时生成声音
            {
                Helper.PlayPitched("Misc/HeavyLanding", 0.1f, Main.rand.NextFloat(0.7f, 1.5f), Projectile.Center);
                Helper.PlayPitched("Misc/FireWhoosh2", 0.05f, 0, Projectile.Center
                    , s =>
                    {
                        s.SoundLimitBehavior = Terraria.Audio.SoundLimitBehavior.ReplaceOldest;
                        s.MaxInstances = 3;
                    });

                for (int i = 0; i < 7; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.7f, Projectile.height * 0.7f);

                    if (i % 2 != 0)
                    {
                        PRTLoader.NewParticle<FaintEagleParticle1>(pos, new Vector2(-Projectile.spriteDirection, 0) * Main.rand.NextFloat(-0.7f, 0.7f), Color.White with { A = 120 }, Main.rand.NextFloat(0.7f, 1f));
                    }
                    else
                    {
                        int pType = Main.rand.NextFromList(PRTLoader.GetParticleID<FaintEagleParticle2>(), PRTLoader.GetParticleID<FaintEagleParticle3>());
                        Vector2 dir = new Vector2(-Projectile.spriteDirection, 0).RotateByRandom(-0.2f, 0.2f);
                        var particle = PRTLoader.NewParticle(pos, dir * Main.rand.NextFloat(0.5f, 1f), pType, Color.White with { A = 225 }, Main.rand.NextFloat(1f, 1.5f));

                        particle.Rotation = dir.ToRotation();
                    }

                    Vector2 dir2 = Helper.NextVec2Dir();
                    Dust d = Dust.NewDustPerfect(Projectile.Center + dir2 * Main.rand.NextFloat(5, Projectile.width / 2), DustID.Torch, dir2 * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(1, 1.5f));
                    d.noGravity = true;
                }
            }

            MoveState = state;

            moveStateSwitchTimer = 60 * 2 + 30;

        }

        /// <summary>
        /// 获得能量
        /// </summary>
        public bool GetFlameEnergy()
        {
            if (FlameCharge < MaxFlameEnergy)
            {
                FlameCharge++;
                return true;
            }

            return false;
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
            HeadSmoother.Update(1 / 60f, basePos + dir * MathF.Abs(MathF.Sin(Timer * 0.015f)) * 2 + (Projectile.spriteDirection*-Timer * 0.025f).ToRotationVector2() * 1);
            TailSmoother.Update(1 / 60f, basePos + dir * MathF.Cos(Timer * 0.05f) * 2);
        }

        /// <summary>
        /// 在地面状态的身体部件运动
        /// </summary>
        public void FlyingBodyPartMovement(bool dashing)
        {
            Vector2 basePos = Projectile.Center + Projectile.velocity;
            Vector2 normal = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            if (dashing)
                WingSmoother.Update(1 / 60f, basePos + normal * MathF.Sin(Timer * 0.2f) * 6);
            else
                WingSmoother.Update(1 / 60f, basePos);
            BackSmoother.Update(1 / 60f, basePos + normal * MathF.Cos(Timer * 0.075f) * 2);
            CoreSmoother.Update(1 / 60f, basePos);
            HeadSmoother.Update(1 / 60f, basePos + (Projectile.spriteDirection * Timer * 0.1f).ToRotationVector2() * 2);
            TailSmoother.Update(1 / 60f, basePos + Projectile.rotation.ToRotationVector2() * MathF.Cos(Timer * 0.05f) * 2);
        }

        public void SleepingBodyPartMovement()
        {
            Vector2 basePos = Projectile.Center + Projectile.velocity + EXOffsetJump;
            WingSmoother.Update(1 / 60f, basePos + new Vector2(0, 6));
            BackSmoother.Update(1 / 60f, basePos + new Vector2(Projectile.spriteDirection*4, 4));
            CoreSmoother.Update(1 / 60f, basePos);
            HeadSmoother.Update(1 / 60f, basePos + new Vector2(0, 4));
            TailSmoother.Update(1 / 60f, basePos);
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

            if (FullFlameCharge)
            {
                Effect shader = ShaderLoader.GetShader("LineAdditive");
                float factor = FlameEffectTimer / 45f;
                Color c = Color.Lerp(Color.Transparent, new Color(253, 133, 81), Helper.SqrtEase(factor));

                shader.Parameters["uFlowTex"]?.SetValue(CoraliteAssets.Laser.TwistLaser.Value);
                shader.Parameters["uTime"]?.SetValue((int)Main.timeForVisualEffects * 0.02f);
                shader.Parameters["flowAdd"]?.SetValue(4);
                shader.Parameters["lineO"]?.SetValue(Helper.X2Ease(factor));
                shader.Parameters["lineC"]?.SetValue(c.ToVector4());
                shader.Parameters["transformMatrix"]?.SetValue(VaultUtils.GetTransfromMatrix());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend
                    , SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

                FlameEffect?.Draw( CoreSmoother.y+new Vector2(Projectile.spriteDirection*4,-34));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            switch (MoveState)
            {
                case MoveStates.Land:
                case MoveStates.Sleep:
                    DrawLanding(mainTex, lightColor, rot, effect);
                    break;
                case MoveStates.Flying:
                case MoveStates.Dashing:
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
            {
                if (MoveState == MoveStates.Dashing)
                    DrawLayer(mainTex, WingSmoother.y, lightColor, backWingFrame, rot, effect);
                else
                    DrawLayer(mainTex, WingSmoother.y, lightColor, backWingFlyingBase + wingFrame, rot, effect);
            }

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
            {
                if (MoveState == MoveStates.Dashing)
                    DrawLayer(mainTex, WingSmoother.y, lightColor, frontWingFrame, rot, effect);
                else
                    DrawLayer(mainTex, WingSmoother.y, lightColor, frontWingFlyingBase + wingFrame, rot, effect);
            }
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
                if (MoveState != MoveStates.Sleep)
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
            mainTex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, frame, 1, TotalFrame), pos - Main.screenPosition+new Vector2(0,-2.75f), color, rot, Projectile.scale, effect);

            if (drawHighlight && FlameCharge > 0)//有能量时绘制一层描边
            {
                float factor = 0.5f * FlameCharge / MaxFlameEnergy + (FlameCharge == MaxFlameEnergy ? 0.5f : 0);

                Color lightC = Color.Lerp(Color.Transparent, new Color(235, 180, 150, 100), factor);
                float scale = 1 + 0.05f * factor;
                Vector2 pos2 = pos
                    - Main.screenPosition
                    - Projectile.rotation.ToRotationVector2() * (MathF.Sin((int)Main.timeForVisualEffects * 0.1f) + 1)*0.5f
                    + new Vector2(0, -2.75f);
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
            Projectile.tileCollide = false;
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
                    Helper.PlayPitched("Misc/FireWhoosh" + (Timer % 2 == 0 ? 1 : 2), 0.4f, 0, Projectile.Center);
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

                    Projectile.NewProjectileFromThis<FaintEagleFire>(Projectile.Center + UnitToMouseV * 30, UnitToMouseV.RotateByRandom(-0.05f, 0.05f) * Main.rand.NextFloat(8, 11.5f), Projectile.damage / 2, Projectile.knockBack, Main.rand.Next(3), Main.rand.Next(2));
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
            Projectile.scale = 0.75f;
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
                Alpha *= 0.93f;
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
                Rectangle r = Projectile.getRect();
                foreach (var proj in Main.ActiveProjectiles)
                    if (proj.owner == Projectile.owner && proj.type == type && proj.Colliding(proj.getRect(), r))
                    {
                        if((proj.ModProjectile as FaintEagleProj).GetFlameEnergy())
                        {
                            Heated = true;
                            break;
                        }
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

    public class AlchSymbolFire : Particle
    {
        public LineDrawer line;
        public float maxScale = 16;
        public float fadeTime = 12;
        public float ShineTime = 7;

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
            AlchorthentShaderData temp;
            shader = temp = new AlchorthentShaderData(CoraliteAssets.Laser.TwistLaser, Coralite.Instance.Assets.Request<Effect>("Effects/LineAdditive", ReLogic.Content.AssetRequestMode.ImmediateLoad), "MyNamePass");

            temp.SetFlowAdd(4);
            temp.SetLineColor(Color.Transparent);

            float height = 2 * 1.732f - 8 / 3f;

            line = new LineDrawer([
                new LineDrawer.StraightLine(new Vector2(0,-8/3f),new Vector2(2,height)),
                new LineDrawer.StraightLine(new Vector2(2,height),new Vector2(-2,height)),
                new LineDrawer.StraightLine(new Vector2(-2,height),new Vector2(0,-8/3f)),
                ]);

            //line.SetScale(16);
            line.SetLineWidth(20);
        }

        public override void AI()
        {
            if (shader is not AlchorthentShaderData data)
                return;
            
            data.SetTime((int)Main.timeForVisualEffects * 0.05f);

            if (Opacity == 0)
            {
                data.SetLineColor(Color.Transparent);
            }

            //先连接，然后闪一下，最后消失
            if (Opacity <= fadeTime)
            {
                float factor = Opacity / fadeTime;
                line.SetScale(maxScale * Helper.BezierEase(factor));

                data.SetLineColor(Color.Lerp(Color.Transparent, Color, Helper.SqrtEase(factor)));
                data.SetLineOffset(Helper.X2Ease(factor));
            }
            else if (Opacity < fadeTime + ShineTime)
            {
                data.SetLineColor(Color.Lerp(Color, new Color(253, 133, 81), Helper.SqrtEase((Opacity - fadeTime) / ShineTime)));
            }
            else if (Opacity < fadeTime + ShineTime * 2)
            {
                data.SetLineColor(Color.Lerp(new Color(253, 133, 81), Color, Helper.SqrtEase((Opacity - fadeTime - ShineTime) / ShineTime)));
            }
            else if (Opacity < fadeTime * 2 + ShineTime * 2)
            {
                data.SetLineColor(Color.Lerp(Color, Color.Transparent, Helper.BezierEase((Opacity - fadeTime - ShineTime * 2) / fadeTime)));
            }
            else
                active = false;

            Opacity++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            line?.Draw(Position);
            return false;
        }
    }
}
