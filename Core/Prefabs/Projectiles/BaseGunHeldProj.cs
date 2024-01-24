using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 命名规则："XXX" + "HeldProj"
    /// </summary>
    public abstract class BaseGunHeldProj : BaseHeldProj
    {
        protected readonly float heldPositionX;
        protected readonly float recoilAngle;
        protected readonly float recoilLength;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        /// <summary> 目标角度 </summary>
        protected ref float TargetRot => ref Projectile.ai[0];
        /// <summary> 总时间 </summary>
        protected ref float MaxTime => ref Projectile.ai[1];

        protected ref float HeldPositionX => ref Projectile.localAI[0];

        public bool initialized;

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : (TexturePath + (PathHasName ? string.Empty : Name)).Replace("HeldProj", "");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        protected BaseGunHeldProj(float recoilAngle, float heldPositionX, float recoilLength, string texturePath, bool pathHasName = false)
        {
            this.recoilAngle = recoilAngle;
            this.heldPositionX = heldPositionX;
            this.recoilLength = recoilLength;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public sealed override void AI()
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            float factor = Ease();
            ApplyRecoil(factor);
            ModifyAI(factor);
            AfterAI(factor);
        }

        public virtual void Initialize()
        {
            Projectile.timeLeft = Owner.itemTimeMax;
            MaxTime = Owner.itemTimeMax;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (OwnerDirection > 0 ? 0f : MathHelper.Pi);
            }

            HeldPositionX = heldPositionX;
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// 获取曲线，默认为 x * sin(x * x) 的曲线，返回值从0到1再到0
        /// 哦！数学魔法！
        /// </summary>
        /// <returns></returns>
        public virtual float Ease()
        {
            float x = 1.772f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x) / 1.3076f;
        }

        public virtual void ApplyRecoil(float factor)
        {
            Projectile.rotation = TargetRot - OwnerDirection * factor * recoilAngle;
            HeldPositionX = heldPositionX + factor * recoilLength;
            Projectile.Center = Owner.Center + OwnerDirection * Projectile.rotation.ToRotationVector2() * HeldPositionX;
        }

        public virtual void ModifyAI(float factor) { }

        public virtual void AfterAI(float factor)
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (Owner.gravDir > 0 ? 0f : MathHelper.Pi) + OwnerDirection * 0.3f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, effects, 0f);
            return false;
        }
    }
}
