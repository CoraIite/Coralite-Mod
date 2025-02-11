using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    /// <summary>
    /// ai0用于控制角度，0是小角度，1是大角度
    /// </summary>
    public class DunkleosteusHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;
        public override bool CanFire => true;
        /// <summary> 目标角度 </summary>
        protected ref float TargetRot => ref Projectile.ai[1];
        /// <summary> 总时间 </summary>
        protected ref float MaxTime => ref Projectile.localAI[0];
        protected ref float HeldPositionX => ref Projectile.localAI[1];
        public const int HELD_LENGTH = 20;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemAnimation;
            MaxTime = Owner.itemAnimation;
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;
            TargetRot = (InMousePos - Owner.Center).ToRotation() + (Owner.gravDir * Owner.direction > 0 ? 0f : MathHelper.Pi);
            if (TargetRot == 0f)
                TargetRot = 0.0001f;
            HeldPositionX = HELD_LENGTH;
            Projectile.netUpdate = true;
        }
        public sealed override void AI()
        {
            float x = 1.772f * Projectile.timeLeft / MaxTime;
            float factor = x * MathF.Sin(x * x) / 1.3076f;
            switch (Projectile.ai[0])
            {
                default:
                case 0:
                    Projectile.rotation = TargetRot - (Owner.gravDir * Owner.direction * factor * 0.06f);
                    HeldPositionX = HELD_LENGTH + (factor * -4);
                    break;
                case 1:
                    Projectile.rotation = TargetRot - (Owner.gravDir * Owner.direction * factor * 0.3f);
                    HeldPositionX = HELD_LENGTH + (factor * -16);

                    float rotation = Projectile.rotation + (Owner.gravDir * Owner.direction > 0 ? 0 : MathHelper.Pi);
                    Vector2 center = Projectile.Center + (rotation.ToRotationVector2() * 48) + Main.rand.NextVector2Circular(6, 6);
                    Dust dust = Dust.NewDustPerfect(center, DustID.Smoke, -Vector2.UnitY.RotatedByRandom(0.1f) * 2f, newColor: Color.Black, Scale: Main.rand.NextFloat(1.4f, 1.8f));
                    dust.noGravity = true;
                    break;
            }

            Projectile.Center = Owner.Center + (Owner.gravDir * Owner.direction * Projectile.rotation.ToRotationVector2() * HeldPositionX);

            SetHeld();
            Owner.itemRotation = Projectile.rotation + (Owner.direction * 0.3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;
            bool ownerDir = Owner.gravDir * Owner.direction > 0;
            SpriteEffects effects = ownerDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(TextureValue, center, null, lightColor, Projectile.rotation, new Vector2(TextureValue.Width / 2, TextureValue.Height / 2), 0.9f, effects, 0f);

            if (Projectile.ai[0] == 1)
            {
                float factor = Projectile.timeLeft / (float)Owner.itemTimeMax;
                float rotation = Projectile.rotation + (ownerDir ? -0.4f : 3.541f); //额...魔法数字，3.141f+0.36f
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center + (rotation.ToRotationVector2() * 22), new Color(255, 255, 255, 0) * 0.8f, Color.Red, factor, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(3, 1.5f), Vector2.One);
            }

            return false;
        }
    }
}