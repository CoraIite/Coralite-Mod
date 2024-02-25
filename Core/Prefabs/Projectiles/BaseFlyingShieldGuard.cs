using Coralite.Content.ModPlayers;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseFlyingShieldGuard : ModProjectile
    {
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];

        public int parryTime;
        public float parryFactor;
        /// <summary>
        /// 弹反的特效颜色
        /// </summary>
        public Color parryColor = Color.White;

        /// <summary>
        /// 伤害削减
        /// </summary>
        public float damageReduce;
        /// <summary>
        /// 后摇时间
        /// </summary>
        public int delayTime = 15;

        public float extraRotation;
        /// <summary>
        /// 到最远时候的比例......这个数越小那么防御时盾就越扁
        /// </summary>
        public float scalePercent = 2.8f;
        public float DistanceToOwner = 0;

        public int[] localProjectileImmunity = new int[Main.maxProjectiles];

        /// <summary>
        /// 是否能削减弹幕的穿透数的概率
        /// </summary>
        public float StrongGuard;
        /// <summary>
        /// 决定了举盾时每帧的距离增加量，这个数越大举盾速度越快
        /// </summary>
        public float distanceAdder = 2;

        public enum GuardState
        {
            Parry,
            ParryDelay,
            Guarding,
            Delay,
        }

        public enum GuardType
        {
            notGuard=0,
            Projectile=1,
            NPC=2
        }

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

        public override bool? CanDamage()
        {
            if (State == (int)GuardState.Delay || State == (int)GuardState.ParryDelay|| DistanceToOwner < GetWidth())
                return false;
            return base.CanDamage();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale *= Owner.GetAdjustedItemScale(Owner.HeldItem);
            Projectile.Resize((int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale));

            SetOtherValues();
            UpdateShieldAccessory(accessory => accessory.OnGuardInitialize(this));
            if (damageReduce > 0.8f)
                damageReduce = 0.8f;
            if (StrongGuard > 0.75f)
                StrongGuard = 0.75f;

            Timer = parryTime;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer > 0)
            {
                State = (int)GuardState.Parry;
                DistanceToOwner = GetWidth();
                Owner.immuneTime = parryTime;
                Owner.immune = true;
            }
            else
                State = (int)GuardState.Guarding;
        }

        /// <summary>
        /// 在这里设置大部分属性的值<br></br>
        /// 弹反特效的颜色 <see cref="parryColor"/><br></br>
        /// 伤害削减量 <see cref="damageReduce"/><br></br>
        /// 后摇时间，默认25 <see cref="delayTime"/><br></br>
        /// 是否能削减弹幕的穿透数 <see cref="StrongGuard"/><br></br>
        /// </summary>
        public virtual void SetOtherValues() { }

        public override void AI()
        {
            //Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.velocity.X = Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.timeLeft = 4;

            switch (State)
            {
                default: Projectile.Kill(); break;
                case (int)GuardState.Parry:
                    {
                        if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();
                        OnHoldShield();

                        if (CheckCollide()>0)
                        {
                            State = (int)GuardState.Guarding;
                            parryFactor = 0;
                            OnParry();
                            UpdateShieldAccessory(accessory => accessory.OnParry(this));
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
                    {
                        if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();
                        OnHoldShield();

                        if (DistanceToOwner < GetWidth())
                        {
                            DistanceToOwner += distanceAdder;
                            break;
                        }

                        int which = CheckCollide();
                        if (which > 0)
                        {
                            UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                            OnGuard();
                            if (which == (int)GuardType.Projectile)
                                OnGuardProjectile();
                            else if (which == (int)GuardType.NPC)
                                OnGuardNPC();
                        }
                    }
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
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (localProjectileImmunity[i] > 0)
                {
                    localProjectileImmunity[i]--;
                    if (!Main.projectile[i].active || Main.projectile[i].friendly)
                        localProjectileImmunity[i] = 0;
                }
            }
        }

        public virtual void SetPos()
        {
            float baseAngle = Owner.direction > 0 ? 0 : MathHelper.Pi;
            Projectile.rotation = baseAngle.AngleLerp((Main.MouseWorld - Owner.Center).ToRotation(), 0.4f);

            Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * DistanceToOwner;
        }

        /// <summary>
        /// 用于获取弹幕离玩家的距离
        /// </summary>
        /// <returns></returns>
        public virtual float GetWidth()
        {
            return Projectile.width / 2/Projectile.scale + 8;
        }

        public virtual int CheckCollide()
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.friendly || proj.whoAmI == Projectile.whoAmI || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                        cp.Guard(damageReduce);

                    float percent = MathHelper.Clamp(StrongGuard, 0, 1);
                    if (Main.rand.NextBool((int)(percent * 100), 100) && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                        OnStrongGuard();
                    }
                    localProjectileImmunity[i] = Projectile.localNPCHitCooldown;
                    return (int)GuardType.Projectile;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                        cp.Guard(damageReduce);

                    return (int)GuardType.NPC;
                }
            }

            return (int)GuardType.notGuard;
        }

        public virtual void TurnToDelay()
        {
            State = (int)GuardState.Delay;
            Timer = delayTime;
        }

        public virtual void OnParry()
        {
        }

        /// <summary>
        /// 用于设置距离，来有一个盾牌被打回来的效果，以及播放音效，以及生成粒子
        /// </summary>
        public virtual void OnGuard()
        {
            DistanceToOwner /= 3;
            Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, Projectile.Center);
        }

        public virtual void OnGuardProjectile() { }

        public virtual void OnGuardNPC() { }

        public virtual void OnHoldShield() { }

        public virtual void OnStrongGuard()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
        }

        public void UpdateShieldAccessory(Action<IFlyingShieldAccessory> action)
        {
            for (int i = 3; i < 10; i++)
            {
                if (!Owner.IsItemSlotUnlockedAndUsable(i))
                    continue;
                if (!Owner.armor[i].active)
                    continue;
                if (Owner.armor[i].ModItem is IFlyingShieldAccessory accessory)
                {
                    action(accessory);
                }
            }

            var loader = LoaderManager.Get<AccessorySlotLoader>();

            ModAccessorySlotPlayer masp = Owner.GetModPlayer<ModAccessorySlotPlayer>();
            for (int k = 0; k < masp.SlotCount; k++)
            {
                if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Owner))
                {
                    Item i = loader.Get(k, Owner).FunctionalItem;
                    if (i.active && i.ModItem is IFlyingShieldAccessory accessory)
                    {
                        action(accessory);
                    }
                }
            }
        }

        public void UpdateShieldAccessory(Func<IFlyingShieldAccessory, bool> action)
        {
            for (int i = 3; i < 10; i++)
            {
                if (!Owner.IsItemSlotUnlockedAndUsable(i))
                    continue;
                if (!Owner.armor[i].active)
                    continue;
                if (Owner.armor[i].ModItem is IFlyingShieldAccessory accessory)
                {
                    if (action(accessory))
                        return;
                }
            }

            var loader = LoaderManager.Get<AccessorySlotLoader>();

            ModAccessorySlotPlayer masp = Owner.GetModPlayer<ModAccessorySlotPlayer>();
            for (int k = 0; k < masp.SlotCount; k++)
            {
                if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Owner))
                {
                    Item i = loader.Get(k, Owner).FunctionalItem;
                    if (i.active && i.ModItem is IFlyingShieldAccessory accessory)
                    {
                        if (action(accessory))
                            return;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Vector2 scale = new Vector2(1 - DistanceToOwner / (Projectile.width * scalePercent), 1);
            scale *= Projectile.scale;
            var effect = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float rotation = Projectile.rotation + extraRotation;

            DrawSelf(mainTex, pos, rotation, lightColor, scale, effect);

            if (State == (int)GuardState.Parry)
            {
                float factor = Timer / parryTime;
                lightColor.A = 0;
                DrawSelf(mainTex, pos, rotation, lightColor * factor, scale * (1f + factor * 0.4f), effect);
            }

            return false;
        }

        public virtual void DrawSelf(Texture2D mainTex,Vector2 pos,float rotation,Color lightColor,Vector2 scale,SpriteEffects effect)
        {
            var origin = mainTex.Size() / 2;
            Vector2 dir = Projectile.rotation.ToRotationVector2() * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, null, c, rotation
                , origin, scale, effect, 0);

            c = lightColor * 0.5f;
            c.A = lightColor.A;
            Main.spriteBatch.Draw(mainTex, pos - dir * 10, null, c, rotation
                , origin, scale, effect, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, rotation
                , origin, scale, effect, 0);
        }
    }
}
