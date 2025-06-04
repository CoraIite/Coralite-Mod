using Coralite.Content.ModPlayers;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    /// <summary>
    /// 规则：<br></br>
    /// ai0记录状态<br></br>
    /// ai1记录时间
    /// </summary>
    public abstract class BaseFlyingShield : BaseHeldProj
    {
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

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

        public bool firstShoot = true;
        public bool recordTileCollide;

        private bool init = true;

        public enum FlyingShieldStates
        {
            Shooting,
            JustHited,
            Backing
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = Projectile.width / 2;
            height = Projectile.height / 2;
            return true;
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
            if (init)
            {
                init = false;
                trailWidth = Projectile.width / 2;
                shootSpeed = Projectile.velocity.Length();
                SetOtherValues();
                UpdateShieldAccessory(accessory => accessory.OnInitialize(this));
                UpdateShieldAccessory(accessory => accessory.PostInitialize(this));
                Timer = flyingTime;

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * shootSpeed;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.InitOldPosCache(trailCachesLength);
                Projectile.InitOldRotCache(trailCachesLength);
                State = (int)FlyingShieldStates.Shooting;
                recordTileCollide = Projectile.tileCollide;
            }

            switch (State)
            {
                default:
                case (int)FlyingShieldStates.Shooting:
                    Shooting();
                    OnShootDusts();
                    break;
                case (int)FlyingShieldStates.JustHited:
                    OnJustHited();
                    break;
                case (int)FlyingShieldStates.Backing:
                    OnBacking();
                    OnBackDusts();
                    break;
            }

            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
        }

        public virtual void Shooting()
        {
            Chasing();
            if (firstShoot && Timer >= flyingTime - 2)
            {
                Projectile.tileCollide = false;
                if (Timer == flyingTime - 2)
                {
                    firstShoot = false;
                    Projectile.tileCollide = recordTileCollide;
                }
            }
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
                if (Helper.TryFindClosestEnemy(Projectile.Center, (Timer * shootSpeed) + (Projectile.width * 4),
                    n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI)
                        && Projectile.localNPCImmunity[n.whoAmI] == 0
                        && Collision.CanHit(Projectile, n), out NPC target))
                {
                    float selfAngle = Projectile.velocity.ToRotation();
                    float targetAngle = (target.Center - Projectile.Center).ToRotation();

                    Projectile.velocity = selfAngle.AngleLerp(targetAngle, 1 - (Timer / flyingTime)).ToRotationVector2() * shootSpeed;
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
            if (Helper.TryFindClosestEnemy(Projectile.Center, flyingTime * shootSpeed,
                n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI)
                    && Projectile.localNPCImmunity[n.whoAmI] == 0
                    && Collision.CanHit(Projectile, n), out NPC target))
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
            else if (Timer == backTime)
            {
                Projectile.damage /= 4;
            }
            else
            {
                if (Owner.GetModPlayer<CoralitePlayer>().FlyingShieldAccBack && Timer > backTime + 14)
                    backSpeed *= 1.03f;

                Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * backSpeed;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (Vector2.Distance(Owner.Center, Projectile.Center) < backSpeed + 8)
                Projectile.Kill();
        }

        public virtual void TurnToBack()
        {
            State = (int)FlyingShieldStates.Backing;
            Timer = 0;
            Projectile.tileCollide = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = (int)FlyingShieldStates.JustHited;
            Projectile.velocity = -Projectile.velocity;
            UpdateShieldAccessory(accessory => accessory.OnTileCollide(this));
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State != (int)FlyingShieldStates.Backing)
                State = (int)FlyingShieldStates.JustHited;
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(this, target, hit, damageDone));
        }

        public void UpdateShieldAccessory(Action<IFlyingShieldAccessory_Fly> action)
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

        /// <summary>
        /// 获取拖尾贴图
        /// </summary>
        /// <returns></returns>
        public virtual ATex GetTrailTex()
        {
            return CoraliteAssets.Trail.EdgeA;
        }

        public virtual void DrawTrails(Color lightColor)
        {
            Texture2D Texture = GetTrailTex().Value;

            List<ColoredVertex> bars = [];
            float r = 0.2989f * lightColor.R / 255 + 0.5870f * lightColor.G / 255 + 0.1140f * lightColor.B / 255;

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * trailWidth);
                Vector2 Bottom = Center - (normal * trailWidth);

                var Color = GetColor(factor) * r;
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
}
