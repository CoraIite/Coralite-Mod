using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class AstrosniperHeldProj : BaseGunHeldProj
    {
        public AstrosniperHeldProj() : base(0.4f, 34, -6, AssetDirectory.Misc_Shoot) { }

        private int dir;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.7f;

        }

        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemAnimation;
            MaxTime = Owner.itemAnimation;

            Owner.direction = dir = TargetRot.ToRotationVector2().X > 0 ? 1 : -1;
            TargetRot += Owner.direction > 0 ? 0f : MathHelper.Pi;  //固定角度

            Projectile.netUpdate = true;
        }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void ModifyAI(float factor)
        {
            Owner.direction = dir;
        }

        public override void ApplyRecoil(float factor)
        {
            Projectile.rotation = TargetRot - (dir * factor * recoilAngle);
            HeldPositionX = heldPositionX + (factor * recoilLength);
            Projectile.Center = Owner.Center + (dir * Projectile.rotation.ToRotationVector2() * HeldPositionX);
        }

        public override void AfterAI(float factor)
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (dir * 0.3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            SpriteEffects effects = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.scale, effects, 0f);
            return false;
        }
    }
}