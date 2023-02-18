using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem
{
    /// <summary>
    /// 御剑弹幕基类，请确保所有的御剑弹幕都继承自这个基类
    /// </summary>
    public class BaseYujianProj : ModProjectile
    {
        public bool AimMouse;
        public bool specialMove;
        public bool canChannel;
        public int targetIndex;
        public int channelTime;
        public Vector2 aimCenter;
        public IHuluEffect huluEffect;
        public YujianAI[] yujianAIs;
        public YujianAI specialAI;
        public YujianAI powerfulAI;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public BaseYujianProj(YujianAI[] yujianAIs, YujianAI specialAI, YujianAI powerfulAI)
        {
            if (yujianAIs is null || powerfulAI is null)
                throw new System.Exception("普通攻击或强化攻击不能为null ! ! ! ! ! ! ! ! ! ! ! !");

            this.yujianAIs = yujianAIs;
            this.specialAI = specialAI;
            this.powerfulAI = powerfulAI;
        }

        #region SetDefaults

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool ShouldUpdatePosition() => false;

        #endregion

        #region AI

        public override void AI()
        {
            //如果玩家输入了左键，那么将开始计时，在完成一整个攻击动作后检测计时器状态，时间短的话那么将命令御剑瞄准鼠标位置进行攻击
            //时间长的话就执行特殊攻击动作（如果有的话）
            if (canChannel)
            {
                if (Owner.channel)
                {
                    channelTime++;
                    Owner.itemTime = 2;
                }
                else
                {
                    channelTime = 0;
                    specialMove = false;
                    if (State == 0.5f)
                        ChangeState();
                }

                //判断按住的时长，并根据时长去调整攻击动作
                if (channelTime > 15)
                    AimMouse = true;

                if (channelTime > 45)
                {
                    State = 0.5f;       //魔法数字，仅在这里使用
                    specialMove = true;
                    AimMouse = false;
                }
            }

            if (!AIBefore())
                return;

            //如果能运行到这里说明已经找到
            //攻击找到的目标，或是进行特殊攻击动作
            CoralitePlayer cp = Owner.GetModPlayer<CoralitePlayer>();

            if (AimMouse && cp.Nianli == cp.NianliMax)
                powerfulAI?.AttackAI(this);
            else if (specialMove && specialAI is not null)
                specialAI.AttackAI(this);
            else
                yujianAIs[(int)State].AttackAI(this);

            huluEffect?.AIEffect(this);
        }

        public bool AIBefore()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
                player.GetModPlayer<CoralitePlayer>().ownedYujianProj = false;

            if (player.GetModPlayer<CoralitePlayer>().ownedYujianProj)
                Projectile.timeLeft = 2;

            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                State = 0f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            switch (State)
            {
                default:
                    break;
                case -1f:       //回到玩家身边
                    ProjectilesHelper.GetMyProjIndexWhihModProj<BaseYujianProj>(Projectile, out var index, out var totalIndexesInGroup);
                    AI_156_GetIdlePosition(Projectile, index, totalIndexesInGroup, out var idleSpot, out var idleRotation);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.2f);
                    if (Projectile.Distance(idleSpot) < 2f)
                    {
                        State = 0f;
                        Projectile.netUpdate = true;
                    }

                    return false;
                case 0f:        //尝试开始攻击
                    ProjectilesHelper.GetMyProjIndexWhihModProj<BaseYujianProj>(Projectile, out var index2, out var totalIndexesInGroup2);
                    AI_156_GetIdlePosition(Projectile, index2, totalIndexesInGroup2, out var idleSpot2, out var idleRotation2);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot2, 0.45f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation2, 0.45f);

                    if (Main.rand.NextBool(20))
                    {
                        int targetNPCIndex = TryAttackingNPCs(Projectile);
                        if (targetNPCIndex != -1)
                        {
                            StartAttack(Projectile);
                            State = Main.rand.Next(1, yujianAIs.Length);
                            Timer = yujianAIs[(int)State - 1].StartTime;
                            targetIndex = targetNPCIndex;
                            Projectile.netUpdate = true;
                        }
                    }

                    return false;
            }

            return true;
        }

        public void ChangeState()
        {
            int targetNPCIndex = TryAttackingNPCs(Projectile, true);
            if (targetNPCIndex != -1)
            {
                //随机一个攻击状态
                StartAttack(Projectile);
                State = Main.rand.Next(1, yujianAIs.Length);
                Timer = yujianAIs[(int)State - 1].StartTime;
                targetIndex = targetNPCIndex;
                Projectile.netUpdate = true;
            }
            else
            {
                State = -1f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            AimMouse = false;
            specialMove = false;
        }

        public Vector2 GetTargetCenter(bool isAimingMouse)
        {
            if (isAimingMouse)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    aimCenter = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }

                return aimCenter;
            }

            return Main.npc[targetIndex].Center;
        }

        #region HelperMethods

        public static void StartAttack(Projectile Projectile)
        {
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        public static int TryAttackingNPCs(Projectile Projectile, bool skipBodyCheck = false)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;

            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float npcDistance2Owner = nPC.Distance(ownerCenter);
                    if (npcDistance2Owner <= 1000f && (npcDistance2Owner <= num || num == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(nPC)))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region Collision

        public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SelfHitEffect(target, damage, knockback, crit);
            huluEffect?.HitEffect(target, damage, knockback, crit);
        }

        public virtual void SelfHitEffect(NPC target, int damage, float knockback, bool crit) { }

        #endregion

        public override void Kill(int timeLeft)
        {
            yujianAIs = null;
            specialAI = null;
            powerfulAI = null;
        }

        #region draw

        public override bool PreDraw(ref Color lightColor)
        {
            huluEffect?.PreDrawEffect(ref lightColor);
            DrawShadowWhenCharged(lightColor);

            return true;
        }

        public virtual void DrawShadowWhenCharged(Color lightColor)
        {
            // TODO：当念力值满时开始绘制重影
        }

        public override void PostDraw(Color lightColor)
        {
            huluEffect?.PostDrawEffect(lightColor);
        }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetIndex);
            writer.WriteVector2(aimCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetIndex = reader.ReadInt32();
            aimCenter = reader.ReadVector2();
        }

        #endregion
    }
}
