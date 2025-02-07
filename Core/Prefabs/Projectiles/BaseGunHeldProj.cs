using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 命名规则："XXX" + "HeldProj"
    /// ai0和ai1已使用
    /// </summary>
    public abstract class BaseGunHeldProj(float recoilAngle, float heldPositionX, float recoilLength, string texturePath, bool pathHasName = false) : BaseHeldProj
    {
        protected readonly float recoilAngle = recoilAngle;
        protected readonly float recoilLength = recoilLength;

        /// <summary> 目标角度 </summary>
        protected ref float TargetRot => ref Projectile.ai[0];
        /// <summary> 总时间 </summary>
        protected ref float MaxTime => ref Projectile.ai[1];

        protected readonly float heldPositionX = heldPositionX;

        protected float HeldPositionX { get; set; } = heldPositionX;
        protected virtual float HeldPositionY { get; set; }

        public bool init;

        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : (texturePath + (pathHasName ? string.Empty : Name)).Replace("HeldProj", "");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
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
                Owner.direction = MousePos.X > Owner.Center.X ? 1 : -1;
                TargetRot = (MousePos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);
            }

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
            Projectile.rotation = TargetRot - (DirSign * factor * recoilAngle);
            HeldPositionX = heldPositionX + (factor * recoilLength);
            Projectile.Center = Owner.Center
                + (DirSign * Projectile.rotation.ToRotationVector2() * HeldPositionX)
                + ((Projectile.rotation + 1.57f).ToRotationVector2() * HeldPositionY);
        }

        public virtual void ModifyAI(float factor) { }

        public virtual void AfterAI(float factor)
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (Owner.gravDir > 0 ? 0f : MathHelper.Pi) + (DirSign * 0.3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            GetFrame(mainTex, out Rectangle? frame, out Vector2 origin);

            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame, lightColor
                , Projectile.rotation, origin, Projectile.scale, effects, 0f);
            return false;
        }

        public virtual void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = null;
            origin = mainTex.Size() / 2;
        }
    }
}
