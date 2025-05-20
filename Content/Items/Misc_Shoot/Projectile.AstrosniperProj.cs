using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Items.Misc_Shoot
{
    [AutoLoadTexture(Path = AssetDirectory.Misc_Shoot)]
    public class AstrosniperHeldProj : BaseGunHeldProj
    {
        public AstrosniperHeldProj() : base(0.4f, 30, -6, AssetDirectory.Misc_Shoot) { }

        private int dir;
        protected override float HeldPositionY => -12;

        public static ATex AstrosniperFire { get; private set; }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void InitializeGun()
        {
            Projectile.timeLeft = Owner.itemTimeMax*2;
            MaxTime = Owner.itemTimeMax * 2;

            float minRot = MathHelper.ToRadians(50);
            float maxRot = MathHelper.ToRadians(130);
            TargetRot = MathHelper.Clamp(ToMouseA + MathHelper.Pi, minRot, maxRot) - MathHelper.Pi;
            if (ToMouseA + MathHelper.Pi > MathHelper.ToRadians(270))
            {
                TargetRot = minRot - MathHelper.Pi;
            }
            Owner.direction = dir = TargetRot.ToRotationVector2().X > 0 ? 1 : -1;
            TargetRot += Owner.direction > 0 ? 0f : MathHelper.Pi;  //固定角度
        }

        public override void ModifyAI(float factor)
        {
            Owner.direction = dir;
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override void ApplyRecoil(float factor)
        {
            Projectile.rotation = TargetRot - (dir * factor * recoilAngle);
            HeldPositionX = heldPositionX + (factor * recoilLength);
            Projectile.Center = Owner.Center
                + (dir * Projectile.rotation.ToRotationVector2() * HeldPositionX)
                + ((Projectile.rotation + 1.57f).ToRotationVector2() * HeldPositionY);

        }

        public override void AfterAI(float factor)
        {
            SetHeld();
            Owner.itemRotation = TargetRot;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = AstrosniperFire.Value;
            Rectangle frameBox = effect.Frame(1, 4, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 48 + n.ToRotationVector2() * 2 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }
}