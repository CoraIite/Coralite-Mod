using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 规则：<br></br>
    /// </summary>
    public abstract class BaseFlyingShield : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public virtual string TrailTexture { get => AssetDirectory.OtherProjectiles + "EdgeTrail"; }

        /// <summary>
        /// 是否能追踪
        /// </summary>
        public bool canChase = false;
        /// <summary>
        /// 飞行时间
        /// </summary>
        public int flyingTime = 20;
        /// <summary>
        /// 返回速度
        /// </summary>
        public float backSpeed = 10;
        /// <summary>
        /// 射击速度
        /// </summary>
        public float shootSpeed = 10;
        /// <summary>
        /// 拖尾数组长度
        /// </summary>
        public int trailCachesLength = 10;
        /// <summary>
        /// 返回时间
        /// </summary>
        public int backTime = 60;

        private int justHitNPC;

        /// <summary>
        /// 反弹次数
        /// </summary>
        public int jumpCount;
        /// <summary>
        /// 最大反弹次数
        /// </summary>
        public int maxJump;

        public float extraRotation;
        public int trailWidth;

        public enum FlyingShieldStates
        {
            Shooting,
            JustHited,
            Backing
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
        }

        public override void OnSpawn(IEntitySource source)
        {
            trailWidth = Projectile.width / 2;
            shootSpeed = Projectile.velocity.Length();
            SetOtherValues();
            UpdateShieldAccessory(accessory => accessory.OnInitialize(this));
            UpdateShieldAccessory(accessory => accessory.PostInitialize(this));
            Timer = flyingTime;

            Projectile.oldPos = new Vector2[trailCachesLength];
            Projectile.oldRot = new float[trailCachesLength];
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < trailCachesLength; i++)
            {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.rotation;
            }
            State = (int)FlyingShieldStates.Shooting;
        }

        /// <summary>
        /// 在这里设置其他属性
        /// 射击时间 <see cref="flyingTime"/><br></br>
        /// 返回时间 <see cref="backTime"/><br></br>
        /// 返回速度 <see cref="backSpeed"/><br></br>
        /// 拖尾长度 <see cref="trailCachesLength"/><br></br>
        /// 能在NPC间弹跳的次数 <see cref="maxJump"/><br></br>
        /// </summary>
        public virtual void SetOtherValues() { }

        public override void AI()
        {
            switch (State)
            {
                default:
                case (int)FlyingShieldStates.Shooting:
                    OnShootDusts();
                    Shooting();
                    break;
                case (int)FlyingShieldStates.JustHited:
                    OnJustHited();
                    break;
                case (int)FlyingShieldStates.Backing:
                    OnBackDusts();
                    OnBacking();
                    break;
            }

            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
        }

        public virtual void Shooting()
        {
            Chasing();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer--;
            if (Timer < 0)
                TurnToBack();
        }

        /// <summary>
        /// 追踪目标NPC<br></br>
        /// 需要<see cref="canChase"/> 为true才行
        /// </summary>
        public virtual void Chasing()
        {
            if (canChase)
                if (ProjectilesHelper.TryFindClosestEnemy(Projectile.Center, 1000, n => true, out NPC target))
                {
                    Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = dir * shootSpeed;
                }
        }

        public virtual void OnShootDusts() { }
        public virtual void OnBackDusts() { }

        public virtual void OnJustHited()
        {
            jumpCount++;
            if (jumpCount > maxJump)
                TurnToBack();
            else
                JumpInNpcs();
            
            UpdateShieldAccessory(accessory => accessory.OnJustHited(this));
        }

        public virtual void JumpInNpcs()
        {
            if (ProjectilesHelper.TryFindClosestEnemy(Projectile.Center, flyingTime*shootSpeed, n => n.whoAmI!=justHitNPC, out NPC target))
            {
                Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = dir * shootSpeed;
                Projectile.rotation = Projectile.velocity.ToRotation();
                State = (int)FlyingShieldStates.Shooting;
                Timer = flyingTime;
            }
            else
                TurnToBack();
        }

        public virtual void OnBacking()
        {
            Projectile.tileCollide = false;

            if (Timer < backTime)
            {
                float factor = Timer / backTime;
                float angle = Projectile.velocity.ToRotation();
                float targetAngle = (Owner.Center - Projectile.Center).ToRotation();

                Projectile.velocity = angle.AngleLerp(targetAngle, factor).ToRotationVector2() * backSpeed;
            }
            else
                Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * backSpeed;

            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (Vector2.Distance(Owner.Center, Projectile.Center) < backSpeed + 8)
                Projectile.Kill();
        }

        public virtual void TurnToBack()
        {
            State = (int)FlyingShieldStates.Backing;
            Timer = 0;
            Projectile.damage /= 4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = (int)FlyingShieldStates.JustHited;
            //Projectile.velocity = oldVelocity;
            UpdateShieldAccessory(accessory => accessory.OnTileCollide(this));
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State!=(int)FlyingShieldStates.Backing)
            State = (int)FlyingShieldStates.JustHited;
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(this, target, hit, damageDone));
            justHitNPC = target.whoAmI;
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

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails(lightColor);
            DrawSelf(lightColor);

            return false;
        }

        public virtual void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation - 1.57f + extraRotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
        }

        public virtual void DrawTrails(Color lightColor)
        {
            Texture2D Texture = ModContent.Request<Texture2D>(TrailTexture).Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + normal * trailWidth;
                Vector2 Bottom = Center - Main.screenPosition - normal * trailWidth;

                var Color = GetColor(factor);//.MultiplyRGB(lightColor);
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            //List<CustomVertexInfo> Vx = new List<CustomVertexInfo>();
            //if (bars.Count > 2)
            //{
            //    for (int i = 0; i < bars.Count - 2; i += 2)
            //    {
            //        Vx.Add(bars[i]);
            //        Vx.Add(bars[i + 1]);
            //        Vx.Add(bars[i + 2]);

            //        Vx.Add(bars[i + 2]);
            //        Vx.Add(bars[i + 3]);
            //        Vx.Add(bars[i + 1]);
            //    }
            //}

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }

        public virtual Color GetColor(float factor)
        {
            return Color.White * factor;
        }

        #endregion
    }

    public interface IFlyingShieldAccessory
    {
        virtual void OnInitialize(BaseFlyingShield projectile) { }
        virtual void PostInitialize(BaseFlyingShield projectile) { }
        virtual void OnGuardInitialize(BaseFlyingShieldGuard projectile) { }

        virtual void OnTileCollide(BaseFlyingShield projectile) { }

        virtual void OnJustHited(BaseFlyingShield projectile) { }

        virtual void OnGuard(BaseFlyingShieldGuard projectile) { }

        virtual void OnHitNPC(BaseFlyingShield projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
    }
}
