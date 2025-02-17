using Coralite.Content.ModPlayers;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    public abstract class BaseFlyingShieldGuard : BaseHeldProj
    {
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public override bool CanFire => true;
        #region 设置类字段

        /// <summary> 完美防御时间 </summary>
        public int parryTime;
        /// <summary> 伤害削减 </summary>
        public float damageReduce;
        /// <summary> 后摇时间 </summary>
        public int delayTime = 15;

        /// <summary> 削减弹幕的穿透数的概率 </summary>
        public float strongGuard;
        /// <summary> 决定了举盾时每帧的距离增加量，这个数越大举盾速度越快 </summary>
        public float distanceAdder = 1.5f;

        /// <summary> 到最远时候的比例......这个数越小那么防御时盾就越扁 </summary>
        public float scalePercent = 2.8f;

        /// <summary> 冲刺时间 </summary>
        public int dashTime;
        /// <summary> 冲刺角度 </summary>
        public float dashDir;
        /// <summary> 冲刺速度 </summary>
        public float dashSpeed;

        #endregion

        public float extraRotation;
        public float DistanceToOwner = 0;

        public bool dashInit = false;

        public int[] localProjectileImmunity = new int[Main.maxProjectiles];

        public IFlyingShieldAccessory_Guard dashFunction;

        /// <summary>
        /// 是否完全举起盾牌
        /// </summary>
        public bool CompletelyHeldUpShield;

        public enum GuardState
        {
            Dashing,
            Parry,
            ParryDelay,
            Guarding,
            Delay,
        }

        public enum GuardType
        {
            notGuard = 0,
            Projectile = 1,
            NPC = 2
        }

        #region 属性设置

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2000;

            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        /// <summary>
        /// 在这里设置大部分属性的值<br></br>
        /// 伤害削减量 <see cref="damageReduce"/><br></br>
        /// 后摇时间，默认25 <see cref="delayTime"/><br></br>
        /// 是否能削减弹幕的穿透数 <see cref="strongGuard"/><br></br>
        /// </summary>
        public virtual void SetOtherValues() { }

        #endregion

        #region AI

        public override void Initialize()
        {
            Projectile.scale *= Owner.GetAdjustedItemScale(Item);
            Projectile.Resize((int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale));
            Projectile.originalDamage = Projectile.damage;//记录原本伤害，因为会受到额外加成影响

            SetOtherValues();
            UpdateShieldAccessory(accessory => accessory.OnGuardInitialize(this));
            LimitFields();

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (!dashInit)
            {
                Timer = parryTime;
                InitState();
            }
        }

        public virtual void LimitFields()
        {
            if (damageReduce > 0.8f)
                damageReduce = 0.8f;
            if (strongGuard > 0.75f)
                strongGuard = 0.75f;
        }

        public virtual void InitState()
        {
            if (Timer > 0)//完美防御
            {
                State = (int)GuardState.Parry;
                DistanceToOwner = GetWidth();
                Owner.immuneTime = parryTime;
                Owner.immune = true;
            }
            else
                State = (int)GuardState.Guarding;
        }

        public override void AI()
        {
            if (Math.Abs(InMousePos.X - Owner.Center.X) > 6)//防止边界问题
                Projectile.velocity.X = Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

            Projectile.timeLeft = 4;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.FlyingShieldGuardIndex = Projectile.whoAmI;

            switch (State)
            {
                default: Projectile.Kill(); break;
                case (int)GuardState.Dashing:
                    {
                        SetPos();
                        OnHoldShield();

                        if (dashFunction != null)
                        {
                            dashFunction.OnDashing(this);
                        }
                        else
                            OnDashOver();

                        Timer--;
                        if (Timer < 1)
                            OnDashOver();
                    }
                    break;
                case (int)GuardState.Parry:
                    {
                        Owner.itemTime = Owner.itemAnimation = 2;

                        if (!DownRight)
                            TurnToDelay();

                        SetPos();
                        OnHoldShield();

                        if (CheckCollide(out _) > 0)
                        {
                            State = (int)GuardState.Guarding;
                            CompletelyHeldUpShield = true;
                            OnParry();
                            UpdateShieldAccessory(accessory => accessory.OnParry(this));
                            UpdateShieldAccessory(accessory => accessory.OnParryEffect(this));
                        }

                        Timer--;
                        if (Timer < 1)
                        {
                            State = (int)GuardState.ParryDelay;
                            Timer = parryTime * 2;
                        }
                    }
                    break;
                case (int)GuardState.ParryDelay:
                    {
                        Owner.itemTime = Owner.itemAnimation = 2;
                        DistanceToOwner = Helper.Lerp(0, GetWidth(), Timer / (parryTime * 2));
                        SetPos();

                        Timer--;
                        if (Timer < 1)
                        {
                            State = (int)GuardState.Guarding;
                        }
                    }
                    break;
                case (int)GuardState.Guarding:
                    Guarding();
                    break;
                case (int)GuardState.Delay:
                    {
                        DistanceToOwner = Helper.Lerp(0, GetWidth(), Timer / delayTime);
                        SetPos();
                        Timer--;
                        if (Timer < 1)
                            Projectile.Kill();
                    }
                    break;
            }

            //更新弹幕的无敌帧
            //for (int i = 0; i < Main.maxProjectiles; i++)
            //{
            //    if (localProjectileImmunity[i] > 0)
            //    {
            //        localProjectileImmunity[i]--;
            //        if (!Main.projectile[i].active || Main.projectile[i].friendly)
            //            localProjectileImmunity[i] = 0;
            //    }
            //}
        }

        public virtual void Guarding()
        {
            Owner.itemTime = Owner.itemAnimation = 2;

            if (!DownRight)
                TurnToDelay();

            SetPos();
            OnHoldShield();

            if (DistanceToOwner < GetWidth())
            {
                DistanceToOwner += distanceAdder;
                return;
            }

            CompletelyHeldUpShield = true;
            int which = CheckCollide(out int index);
            if (which > 0)
            {
                UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                OnGuard();
                if (which == (int)GuardType.Projectile)
                    OnGuardProjectile(index);
                else if (which == (int)GuardType.NPC)
                    OnGuardNPC(index);
            }
        }

        /// <summary>
        /// 常规设置自身位置
        /// </summary>
        public virtual void SetPos()
        {
            float baseAngle = Owner.direction > 0 ? 0 : MathHelper.Pi;
            Projectile.rotation = baseAngle.AngleLerp(ToMouseA, 0.4f);

            Projectile.Center = Owner.Center + (Projectile.rotation.ToRotationVector2() * DistanceToOwner);
        }

        /// <summary>
        /// 用于获取弹幕离玩家的距离
        /// </summary>
        /// <returns></returns>
        public virtual float GetWidth()
        {
            return Projectile.width * 0.6f / Projectile.scale;
        }

        /// <summary>
        /// 检测与弹幕或NPC的碰撞
        /// </summary>
        /// <returns></returns>
        public virtual int CheckCollide(out int index)
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    float damageR = damageReduce;
                    if (proj.penetrate < 0)//对于无限穿透的弹幕额外减伤
                        damageR += Main.rand.NextFloat(0, strongGuard / 3);

                    OnGuard_DamageReduce(damageR);

                    float percent = MathHelper.Clamp(strongGuard, 0, 1);
                    if (Main.rand.NextBool((int)(percent * 100), 100) && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                        OnStrongGuard();
                    }
                    localProjectileImmunity[i] = Projectile.localNPCHitCooldown;
                    index = i;
                    return (int)GuardType.Projectile;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || !Projectile.localNPCImmunity.IndexInRange(i) || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    OnGuard_DamageReduce(damageReduce);

                    Projectile.localNPCImmunity[i] = Projectile.localNPCHitCooldown;
                    if (!npc.dontTakeDamage)
                        npc.StrikeNPC(npc.CalculateHitInfo(Projectile.damage, Projectile.direction, false, Projectile.knockBack, DamageClass.Melee, true));

                    index = i;
                    return (int)GuardType.NPC;
                }
            }

            index = -1;
            return (int)GuardType.notGuard;
        }

        #region 特定时期触发类方法

        public virtual void TurnToDelay()
        {
            State = (int)GuardState.Delay;
            Timer = delayTime;
        }

        public virtual void OnParry() { }

        /// <summary>
        /// 用于设置距离，来有一个盾牌被打回来的效果，以及播放音效，以及生成粒子
        /// </summary>
        public virtual void OnGuard()
        {
            DistanceToOwner /= 3;
            Helper.PlayPitched("Misc/ShieldGuard", 0.3f, 0f, Projectile.Center);
        }

        /// <summary>
        /// 格挡时的提供伤害减免
        /// </summary>
        /// <param name="damageR"></param>
        public virtual void OnGuard_DamageReduce(float damageR)
        {
            damageR = Math.Clamp(damageR, 0f, 0.8f);
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.Guard(damageR);
        }

        public virtual void OnGuardProjectile(int projIndex) { }

        public virtual void OnGuardNPC(int npcIndex) { }

        public virtual void OnHoldShield() { }

        public virtual void OnStrongGuard()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
        }

        /// <summary>
        /// 结束冲刺时触发
        /// </summary>
        public virtual void OnDashOver()
        {
            State = (int)GuardState.Guarding;
            Projectile.damage = Projectile.originalDamage;

            UpdateShieldAccessory(accessory => accessory.OnDashOver(this));
        }

        /// <summary>
        /// 切换到盾冲
        /// </summary>
        /// <param name="dashFunction"></param>
        public virtual void TurnToDashing(IFlyingShieldAccessory dashFunction, int dashTime, float dashDir, float dashSpeed)
        {
            dashInit = true;
            this.dashFunction = dashFunction;
            State = (int)GuardState.Dashing;
            DistanceToOwner = GetWidth();
            this.dashTime = dashTime;
            this.dashDir = dashDir;
            this.dashSpeed = dashSpeed;
            UpdateShieldAccessory(accessory => accessory.OnStartDashing(this));

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.DashTimer = this.dashTime;
            Timer = this.dashTime;
            Owner.velocity = this.dashDir.ToRotationVector2() * this.dashSpeed;
            if (Owner.velocity.Y == 0)
                Owner.velocity.Y = -0.00001f;
        }

        #endregion

        #endregion

        #region 帮助方法

        public virtual bool CanDash()
        {
            return State == (int)GuardState.Guarding && CompletelyHeldUpShield;
        }

        public void UpdateShieldAccessory(Action<IFlyingShieldAccessory_Guard> action)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                foreach (var accessory in cp.FlyingShieldAccessories)
                    action(accessory);
            //for (int i = 3; i < 10; i++)
            //{
            //    if (!Owner.IsItemSlotUnlockedAndUsable(i))
            //        continue;
            //    if (!Owner.armor[i].active)
            //        continue;
            //    if (Owner.armor[i].ModItem is IFlyingShieldAccessory accessory)
            //    {
            //        action(accessory);
            //    }
            //}

            //var loader = LoaderManager.Get<AccessorySlotLoader>();

            //ModAccessorySlotPlayer masp = Owner.GetModPlayer<ModAccessorySlotPlayer>();
            //for (int k = 0; k < masp.SlotCount; k++)
            //{
            //    if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Owner))
            //    {
            //        Item i = loader.Get(k, Owner).FunctionalItem;
            //        if (!i.IsAir && i.ModItem is IFlyingShieldAccessory accessory)
            //        {
            //            action(accessory);
            //        }
            //    }
            //}
        }

        public void UpdateShieldAccessory(Func<IFlyingShieldAccessory_Guard, bool> action)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                foreach (var accessory in cp.FlyingShieldAccessories)
                    if (action(accessory))
                        return;
            //for (int i = 3; i < 10; i++)
            //{
            //    if (!Owner.IsItemSlotUnlockedAndUsable(i))
            //        continue;
            //    if (!Owner.armor[i].active)
            //        continue;
            //    if (Owner.armor[i].ModItem is IFlyingShieldAccessory accessory)
            //    {
            //        if (action(accessory))
            //            return;
            //    }
            //}

            //var loader = LoaderManager.Get<AccessorySlotLoader>();

            //ModAccessorySlotPlayer masp = Owner.GetModPlayer<ModAccessorySlotPlayer>();
            //for (int k = 0; k < masp.SlotCount; k++)
            //{
            //    if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Owner))
            //    {
            //        Item i = loader.Get(k, Owner).FunctionalItem;
            //        if (i.active && i.ModItem is IFlyingShieldAccessory accessory)
            //        {
            //            if (action(accessory))
            //                return;
            //        }
            //    }
            //}
        }

        #endregion

        #region 伤害

        public override bool? CanDamage()
        {
            if (State == (int)GuardState.Delay || State == (int)GuardState.ParryDelay || DistanceToOwner < GetWidth())
                return false;
            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == (int)GuardState.Dashing)
                dashFunction?.OnDashHit(this, target, ref modifiers);
        }

        #endregion

        #region 绘制

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Vector2 scale = new(1 - (DistanceToOwner / (Projectile.width * scalePercent)), 1);
            scale *= Projectile.scale;
            var effect = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float rotation = Projectile.rotation + extraRotation;

            DrawSelf(mainTex, pos, rotation, lightColor, scale, effect);

            if (State == (int)GuardState.Parry)
            {
                float factor = Timer / parryTime;
                lightColor.A = 0;
                DrawSelf(mainTex, pos, rotation, lightColor * factor, scale * (1f + (factor * 0.4f)), effect);
            }
            else if (State == (int)GuardState.Dashing)
            {
                float factor = Timer / dashTime;
                lightColor.A = 0;
                DrawSelf(mainTex, pos, rotation, lightColor * factor, scale * (1f + (factor * 0.4f)), effect);
            }

            return false;
        }

        public virtual void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            var origin = mainTex.Size() / 2;
            Vector2 dir = Projectile.rotation.ToRotationVector2() * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), null, c, rotation
                , origin, scale, effect, 0);

            c = lightColor * 0.5f;
            c.A = lightColor.A;
            Main.spriteBatch.Draw(mainTex, pos - (dir * 10), null, c, rotation
                , origin, scale, effect, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, rotation
                , origin, scale, effect, 0);
        }

        #endregion
    }
}
