using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        public virtual int ParryTime { get => 0; }
        public float ParryFactor;
        public bool ParryUpdate;
        /// <summary>
        /// 弹反的特效颜色
        /// </summary>
        public Color ParryColor = Color.White;

        /// <summary>
        /// 伤害削减
        /// </summary>
        public float DamageReduce;
        /// <summary>
        /// 后摇时间
        /// </summary>
        public int DelayTime = 15;

        public float DistanceToOwner = 0;

        public int[] localProjectileImmunity = new int[Main.maxProjectiles];

        /// <summary>
        /// 是否能削减弹幕的穿透数
        /// </summary>
        public bool StrongGuard;

        public enum GuardState
        {
            Parry,
            Guarding,
            Delay,
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2000;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Timer = ParryTime;

            SetOtherValues();
            UpdateShieldAccessory(accessory => accessory.OnGuardInitialize(this));
            if (DamageReduce > 1)
                DamageReduce = 1;

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer > 0)
            {
                State = (int)GuardState.Parry;
                DistanceToOwner = GetWidth();
            }
            else
                State = (int)GuardState.Guarding;
        }

        /// <summary>
        /// 在这里设置大部分属性的值<br></br>
        /// 弹反特效的颜色 <see cref="ParryColor"/><br></br>
        /// 伤害削减量 <see cref="DamageReduce"/><br></br>
        /// 后摇时间，默认25 <see cref="DelayTime"/><br></br>
        /// 是否能削减弹幕的穿透数 <see cref="StrongGuard"/><br></br>
        /// </summary>
        public virtual void SetOtherValues() { }

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.velocity.X = Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.timeLeft = 4;

            switch (State)
            {
                default: Projectile.Kill(); break;
                case (int)GuardState.Parry:
                    {
                        if (DistanceToOwner < GetWidth())
                            DistanceToOwner += 4;
                        else if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();

                        if (CheckCollide())
                            OnParry();

                        Timer--;
                        if (Timer < 1)
                            State = (int)GuardState.Guarding;
                    }
                    break;
                case (int)GuardState.Guarding:
                    {
                        if (DistanceToOwner < GetWidth())
                            DistanceToOwner += 4;
                        else if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();

                        if (CheckCollide())
                        {
                            UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                            OnGuard();
                        }
                    }
                    break;
                case (int)GuardState.Delay:
                    {
                        DistanceToOwner = Helper.Lerp(0, GetWidth(), Timer / DelayTime);
                        SetPos();
                        Timer--;
                        if (Timer < 1)
                            Projectile.Kill();
                    }
                    break;
            }

            if (ParryUpdate)
            {

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

        public virtual float GetWidth()
        {
            return Projectile.width / 2 + 8;
        }

        public bool CheckCollide()
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.friendly || proj.whoAmI == Projectile.whoAmI || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    proj.damage = (int)(proj.damage * (1 - DamageReduce));//削减伤害
                    if (proj.damage < 1)
                        proj.damage = 1;

                    if (StrongGuard && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                    }
                    localProjectileImmunity[i] = 30;
                    return true;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void TurnToDelay()
        {
            State = (int)GuardState.Delay;
            Timer = DelayTime;
        }

        public virtual void OnParry()
        {
            ParryUpdate = true;
            ParryFactor = 0;
        }

        /// <summary>
        /// 用于设置距离，来有一个盾牌被打回来的效果，以及播放音效，以及生成粒子
        /// </summary>
        public virtual void OnGuard()
        {
            DistanceToOwner /= 3;
            Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, Projectile.Center);
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Vector2 scale = new Vector2(1 - DistanceToOwner / (Projectile.width * 2.8f), 1);
            scale *= Projectile.scale;
            var effect = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation
                , origin, scale, effect, 0);
            return false;
        }
    }
}
