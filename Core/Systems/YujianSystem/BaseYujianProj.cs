﻿using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.YujianSystem
{
    /// <summary>
    /// 御剑弹幕基类，请确保所有的御剑弹幕都继承自这个基类
    /// </summary>
    public class BaseYujianProj : ModProjectile, IDrawNonPremultiplied, IDrawAdditive, IDrawPrimitive
    {
        public readonly Color color1;
        public readonly Color color2;
        public readonly int trailCacheLength;
        private readonly int Width;
        private readonly int Height;
        private readonly string TexturePath;
        private readonly bool PathHasName;
        private readonly float PowerfulAttackCost;
        public readonly bool TileCollide;
        public bool AimMouse;
        public int targetIndex;
        private int attackLength;
        public Vector2 aimCenter;
        public IHuluEffect huluEffect;
        /// <summary>
        /// 御剑的攻击AI
        /// </summary>
        public YujianAI[] yujianAIs;
        /// <summary>
        /// 御剑的随机攻击AI
        /// </summary>
        public int[] yujianAIsRandom;
        /// <summary>
        /// 随机攻击AI的上限
        /// </summary>
        protected int AIsRandomMax;
        /// <summary>
        /// 御剑消耗念力的攻击状态
        /// </summary>
        public YujianAI powerfulAI;
        /// <summary>
        /// 御剑生成源
        /// </summary>
        public BaseYujian SourceYujian;

        public const float PowerfulMoveState = 0.1f;

        public int AttackLength { get => attackLength; set => attackLength = value; }
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public virtual string SlashTexture => AssetDirectory.OtherProjectiles + "NormalSlashTrail";

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name.Replace("Proj", ""));

        public BaseYujianProj(YujianAI[] yujianAIs, YujianAI powerfulAI, float PowerfulAttackCost, int attackLength, int width, int height, Color color1, Color color2, int trailCacheLength = 30, bool tileCollide = true, string texturePath = AssetDirectory.YujianHulu, bool pathHasName = false, int[] yujianAIsRandom = null)
        {
            if (yujianAIs is null || powerfulAI is null)
                throw new Exception("普通攻击或强化攻击不能为null ! ! ! ! ! ! ! ! ! ! ! !");

            this.yujianAIs = yujianAIs;
            this.powerfulAI = powerfulAI;
            this.PowerfulAttackCost = PowerfulAttackCost;
            AttackLength = attackLength;
            Width = width;
            Height = height;
            this.color1 = color1;
            this.color2 = color2;
            this.trailCacheLength = trailCacheLength;
            TileCollide = tileCollide;
            TexturePath = texturePath;
            PathHasName = pathHasName;
            if (yujianAIsRandom == null)
            {
                yujianAIsRandom = new int[yujianAIs.Length];
                for (int i = 0; i < yujianAIsRandom.Length; i++)
                {
                    yujianAIsRandom[i] = 1;
                }
            }
            if (yujianAIsRandom.Length != yujianAIs.Length)
                throw new Exception("御剑攻击随机度数量不能与御剑AI数量不同");
            this.yujianAIsRandom = yujianAIsRandom;
        }

        #region SetDefaults

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Width;
            Projectile.height = Height;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.localNPCHitCooldown = 25;
            Projectile.extraUpdates = 2;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = TileCollide;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;

            PostSetDefaults();
        }

        public virtual void PostSetDefaults() { }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            if (source is YujianSource yujianSource)
            {
                SourceYujian = yujianSource.Yujian;
            }
            Projectile.oldPos = new Vector2[trailCacheLength];
            Projectile.oldRot = new float[trailCacheLength];

            InitTrailCaches();
        }

        public sealed override void AI()
        {
            if (Owner.HeldItem.ModItem is not BaseHulu && !SourceYujian.MainYujian) // 如果手上不是葫芦并且不是主御剑，则清除自己
            {
                Projectile.Kill();
                return;
            }

            if (!AIBefore())
                return;

            //如果能运行到这里说明已经找到
            //攻击找到的目标，或是进行特殊攻击动作
            YujianAI currentAI = GetCurrentAI();
            if (!CheckCanAttack(currentAI))
                ChangeState();

            currentAI.AttackAI(this);

            UpdateCaches();
            huluEffect?.AIEffect(Projectile);
            AIEffect();
        }

        public bool AIBefore()
        {
            Player player = Main.player[Projectile.owner];
            CoralitePlayer cp = player.GetModPlayer<CoralitePlayer>();
            if (player.dead)
                cp.ownedYujianProj = false;

            Projectile.timeLeft = 2;
            if (Owner.HeldItem.ModItem is BaseHulu && Owner.controlUseItem)
            {
                Owner.itemTime = Owner.itemAnimation = 6;
                Vector2 vector2 = Main.MouseWorld - Owner.Center;
                Owner.itemRotation = MathF.Atan2(vector2.Y * Owner.direction, vector2.X * Owner.direction);
            }
            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                State = -1f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            switch (State)
            {
                default:
                    break;
                case -2f: // 因为各种原因没检测到强制重置攻击
                    Helper.GetMyProjIndexWithModProj<BaseYujianProj>(Projectile, out var index0, out var totalIndexesInGroup0);
                    GetIdlePosition(index0, totalIndexesInGroup0, out var idleSpot0, out var idleRotation0);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.Center.MoveTowards(idleSpot0, 10f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation0, 0.08f);
                    Projectile.tileCollide = false;
                    Timer = 0;
                    targetIndex = -1;
                    TryAttackingNPCs(Projectile);
                    GetYujianRandomState();
                    return false;
                case -1f:       //回到玩家身边
                    Helper.GetMyProjIndexWithModProj<BaseYujianProj>(Projectile, out var index, out var totalIndexesInGroup);
                    GetIdlePosition(index, totalIndexesInGroup, out var idleSpot, out var idleRotation);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 10f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.08f);
                    Projectile.tileCollide = false;
                    Timer = 0;
                    targetIndex = -1;
                    if (Projectile.Distance(idleSpot) < 2f)
                    {
                        State = -0.5f;
                        Projectile.netUpdate = true;
                    }

                    return false;
                case -0.5f:     //尝试开始攻击
                    Helper.GetMyProjIndexWithModProj<BaseYujianProj>(Projectile, out var index2, out var totalIndexesInGroup2);
                    GetIdlePosition(index2, totalIndexesInGroup2, out var idleSpot2, out var idleRotation2);
                    Projectile.velocity = Vector2.UnitY.RotatedBy((MathHelper.PiOver2 * 1.5f * Owner.direction) + (0.2 * index2 * totalIndexesInGroup2 * Owner.direction));
                    Projectile.Center = idleSpot2 + Projectile.velocity * 10;
                    Projectile.rotation = idleRotation2;
                    Projectile.tileCollide = false;

                    if (Owner.HeldItem.ModItem is BaseHulu && Owner.controlUseItem) // 手持葫芦则左键攻击,否则自动攻击
                    {
                        State = 0;
                        //State = Main.rand.Next(1, yujianAIs.Length);
                        //yujianAIs[(int)State - 1].OnStart(this);
                    }
                    else if (Owner.HeldItem.ModItem is not BaseHulu && Main.rand.NextBool(20))
                    {
                        int targetNPCIndex = TryAttackingNPCs(Projectile);
                        if (targetNPCIndex != -1)
                        {
                            //随机一个攻击状态，或进行指定攻击状态
                            targetIndex = targetNPCIndex;
                            State = 0;
                            //State = Main.rand.Next(1, yujianAIs.Length);
                            //yujianAIs[(int)State - 1].OnStart(this);
                        }
                    }
                    return false;
                case 0f:        //拔刀

                    if (Timer++ > 60)   //30帧后切换AI，否则处于拔刀AI
                    {
                        Timer = 0;  //重置计时器
                        if (Owner.HeldItem.ModItem is not BaseHulu)
                        {
                            int targetNPCIndex1 = TryAttackingNPCs(Projectile);
                            if (targetNPCIndex1 == -1)
                            {
                                Timer = 0;
                                targetIndex = -1;
                                State = -1;
                            }
                        }

                        GetYujianRandomState();
                    }
                    else
                    {
                        Projectile.Center = Owner.Center + Projectile.velocity * (MathHelper.SmoothStep(1, 100, Timer / 60f) + 10);
                    }
                    return false;
            }

            return true;
        }

        public void ChangeState()
        {
            CoralitePlayer cp = Owner.GetModPlayer<CoralitePlayer>();

            if (ChangeState(cp))
                return;

            int targetNPCIndex = TryAttackingNPCs(Projectile);
            if ((AimMouse && Owner.controlUseItem) || (!AimMouse && targetNPCIndex != -1))
            {
                //随机一个攻击状态，或进行指定攻击状态
                targetIndex = targetNPCIndex;
                GetYujianRandomState();
                //State = Main.rand.Next(1, yujianAIs.Length + 1);
                //yujianAIs[(int)State - 1].OnStart(this);
            }
            else
            {
                State = -1f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }
        }

        private bool ChangeState(CoralitePlayer cp)
        {
            bool CanAttack = Vector2.Distance(GetTargetCenter(true), Owner.Center) < AttackLength;
            AimMouse = Owner.HeldItem.ModItem is BaseHulu; // 如果手持葫芦则瞄准鼠标，不是则瞄准敌人
            if (AimMouse)
            {
                CanAttack = CanAttack && Owner.controlUseItem;
            }
            else
                CanAttack = CanAttack && State > 0;
            if (SourceYujian.MainYujian && AimMouse && cp.useSpecialAttack && Vector2.Distance(GetTargetCenter(true), Owner.Center) < AttackLength && cp.nianli > PowerfulAttackCost)
            {
                State = PowerfulMoveState;
                powerfulAI.OnStart(this);
                cp.nianli -= PowerfulAttackCost;
                return true;
            }
            else if (CanAttack)
            {
                //State = Main.rand.Next(1, yujianAIs.Length + 1);
                //yujianAIs[(int)State - 1].OnStart(this);
                GetYujianRandomState();
                return true;
            }

            return false;
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
            if (targetIndex == -1)
            {
                State = -2;
                return Owner.Center;
            }
            else if (!Main.npc[targetIndex].active || Main.npc[targetIndex].life < 0 || !Main.npc[targetIndex].CanBeChasedBy())
            {
                State = -2;
                targetIndex = -1;
                return Owner.Center;
            }

            return Main.npc[targetIndex].Center;
        }

        /// <summary>
        /// 计算出在玩家身边的位置
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="stackedIndex"></param>
        /// <param name="totalIndexes"></param>
        /// <param name="idleSpot"></param>
        /// <param name="idleRotation"></param>
        public void GetIdlePosition(int stackedIndex, int totalIndexes, out Vector2 idleSpot, out float idleRotation)
        {
            idleRotation = (float)MathHelper.PiOver2 * 1.5f * Owner.direction + Owner.direction * 0.2f * totalIndexes * stackedIndex;
            //idleRotation = (-Vector2.UnitX).RotatedBy(stackedIndex * 0.1f * totalIndexes * Owner.direction).ToRotation();
            //float num2 = (totalIndexes - 1f) / 2f;
            //idleSpot = Owner.Center - Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 33f - new Vector2(Owner.direction * 16, 8);
            //idleSpot += Main.GlobalTimeWrappedHourly.ToRotationVector2() * 8;
            idleSpot = Owner.Center;
        }
        /// <summary>
        /// 获取御剑的随机攻击
        /// </summary>
        protected virtual void GetYujianRandomState()
        {
            if (AIsRandomMax <= 0)
                for (int i = 0; i < yujianAIs.Length; i++)
                {
                    AIsRandomMax += yujianAIsRandom[i];
                }
            int rand = Main.rand.Next(AIsRandomMax);
            for (int j = 0; j < yujianAIs.Length; j++)
            {
                rand -= yujianAIsRandom[j];
                if (rand <= 0)
                {
                    State = j + 1;
                    yujianAIs[(int)State - 1].OnStart(this);
                    break;
                }
            }
        }
        public virtual void AIEffect() { }

        #region HelperMethods

        public static void StartAttack(Projectile Projectile)
        {
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        public int TryAttackingNPCs(Projectile Projectile)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;

            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float npcDistance2Owner = Vector2.Distance(ownerCenter, nPC.Center);
                    if (npcDistance2Owner <= AttackLength &&
                            (npcDistance2Owner <= num || num == -1f) &&
                            Collision.CanHitLine(Projectile.Center, 1, 1, nPC.Center, 1, 1))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }

        public YujianAI GetCurrentAI()
        {
            if (State == PowerfulMoveState)
                return powerfulAI;

            return yujianAIs[(int)(State - 1)];
        }

        public bool CheckCanAttack(YujianAI currentAI)
        {
            if (currentAI.IsAimingMouse)
                return true;

            if (!Main.npc.IndexInRange(targetIndex))
                return false;

            NPC target = Main.npc[targetIndex];
            return target.CanBeChasedBy();
        }

        public void ResetTileCollide()
        {
            Projectile.tileCollide = TileCollide;
        }

        public void InitTrailCaches()
        {
            for (int i = 0; i < trailCacheLength; i++)
            {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.rotation;
            }
        }

        public void UpdateCaches()
        {
            for (int i = 0; i < trailCacheLength - 1; i++)
            {
                Projectile.oldRot[i] = Projectile.oldRot[i + 1];
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];
            }

            Projectile.oldPos[trailCacheLength - 1] = Projectile.Center;
            Projectile.oldRot[trailCacheLength - 1] = Projectile.rotation;

        }

        #endregion

        #endregion

        #region Collision

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            height = width;
            return true;
        }

        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    Vector2 dir = (Projectile.rotation - 1.57f).ToRotationVector2() * Projectile.height / 2;
        //    float a = 0f;

        //    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + dir, Projectile.Center - dir, Projectile.width / 2, ref  a) &&
        //    Collision.CanHitLine(targetHitbox.Center.ToVector2(), 1, 1, Projectile.Center, 1, 1);
        //}

        public override bool? CanDamage()
        {
            if (State > 0f)
                return GetCurrentAI().canDamage;

            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owner.HeldItem.ModItem is not BaseHulu)
                modifiers.SourceDamage *= 0.5f;
        }
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitEffect(target, hit.Damage, hit.Knockback, hit.Crit);
            huluEffect?.HitEffect(Projectile, target, hit.Damage, hit.Knockback, hit.Crit);
        }

        public virtual void HitEffect(NPC target, int damage, float knockback, bool crit) { }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        #endregion

        public override void OnKill(int timeLeft)
        {
            yujianAIs = null;
            powerfulAI = null;
        }

        #region draw

        public sealed override bool PreDraw(ref Color lightColor)
        {
            huluEffect?.PreDrawEffect(Projectile, ref lightColor);
            PreDrawEffect(ref lightColor);
            if (State < 0.01f)
                DrawSelf(Main.spriteBatch, lightColor);
            return false;
        }

        public void DrawPrimitives()
        {
            if (State > 0)
                GetCurrentAI().DrawPrimitives(this);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State > 0)
                GetCurrentAI().DrawAdditive(spriteBatch, this);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            if (State > 0)
                DrawSelf(spriteBatch, lightColor);
            PostDrawEffect(spriteBatch, lightColor);
            huluEffect?.PostDrawEffect(Projectile, lightColor);
        }

        //并不好，所以弃用了，来源是抄的BossBag的绘制
        //public virtual void DrawShadowWhenCharged(Color lightColor)
        //{
        //    CoralitePlayer cp = Owner.GetModPlayer<CoralitePlayer>();

        //    if (cp.nianli == cp.nianliMax)
        //    {
        //        Texture2D mainTex = TextureAssets.Projectile[Type].Value;
        //        Rectangle frame=mainTex.Frame();

        //        Vector2 frameOrigin = frame.Size() / 2f;
        //        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        //        float time = Main.GlobalTimeWrappedHourly;
        //        float timer =  time * 0.2f;

        //        time %= 4f;
        //        time /= 2f;

        //        if (time >= 1f)
        //            time = 2f - time;

        //        time = time * 0.5f + 0.5f;

        //        for (float i = 0f; i < 1f; i += 0.25f)
        //        {
        //            float radians = (i + timer) * MathHelper.TwoPi;
        //            Main.spriteBatch.Draw(mainTex, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
        //        }

        //        for (float i = 0f; i < 1f; i += 0.34f)
        //        {
        //            float radians = (i + timer) * MathHelper.TwoPi;
        //            Main.spriteBatch.Draw(mainTex, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
        //        }
        //    }
        //}

        public virtual void PreDrawEffect(ref Color lightColor) { }

        public void DrawSelf(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            SpriteEffects effect = SpriteEffects.None;
            if (State > 0)
                effect = GetCurrentAI().GetSpriteEffect(this);

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(), lightColor, Projectile.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.scale, effect, 0f);
        }

        public virtual void PostDrawEffect(SpriteBatch spriteBatch, Color lightColor) { }

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
