using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /// <summary>
    /// 基本复制的巨鹿的弹幕
    /// </summary>
    public class IceThorn : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_961";

        public ref float Timer => ref Projectile.ai[0];

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Timer += 1f;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.frame = Main.rand.Next(5);
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.Cloud, Projectile.velocity * 0.75f * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                    dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }

                for (int j = 0; j < 5; j++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.Cloud, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.75f * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust2.fadeIn = 1f;
                }

                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Projectile.Center);

            }

            if (Timer < 10)
            {
                Projectile.Opacity += 0.1f;
                Projectile.scale = Projectile.Opacity * Projectile.ai[1];
                return;
            }

            if (Timer >= 10)
            {
                Projectile.Opacity -= 0.2f;
            }

            if (Timer >= 20)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint16 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, ref collisionPoint16))
                return true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f) * Projectile.scale + Projectile.velocity.SafeNormalize(Vector2.UnitY) * i * 200f * Projectile.scale, 16, Main.rand.NextVector2Circular(3f, 3f));
                dust.velocity.Y += -0.3f;
                dust.velocity += Projectile.velocity * 0.2f;
                dust.scale = 1f;
                dust.alpha = 100;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.Black, 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle frame = mainTex.Frame(1, 5, 0, Projectile.frame);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(16f, frame.Height / 2);
            Color Color = Projectile.GetAlpha(lightColor);
            Vector2 scale = new Vector2(Projectile.scale);

            float lerpValue5 = Utils.GetLerpValue(30f, 25f, Timer, clamped: true);
            scale.Y *= lerpValue5;
            Vector4 color_1 = lightColor.ToVector4();
            Vector4 color_2 = new Color(67, 17, 17).ToVector4();
            color_2 *= color_1;

            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity * Projectile.scale * 0.5f, null,
                Projectile.GetAlpha(new Color(color_2.X, color_2.Y, color_2.Z, color_2.W)) * 1f, Projectile.rotation + (float)Math.PI / 2f, TextureAssets.Extra[98].Value.Size() / 2f, Projectile.scale * 0.9f, effects, 0);
            Color color_3 = Projectile.GetAlpha(Color.White) * Utils.Remap(Timer, 0f, 20f, 0.5f, 0f);
            color_3.A = 0;
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(mainTex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Projectile.rotation.ToRotationVector2().RotatedBy((float)Math.PI / 2f * i) * 2f * scale, frame, color_3, Projectile.rotation, origin, scale, effects, 0);

            Main.EntitySpriteDraw(mainTex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), frame, Color, Projectile.rotation, origin, scale, effects, 0);
            return false;
        }
    }
}
