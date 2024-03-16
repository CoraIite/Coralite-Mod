using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// ai0 用于存储玩家方向
    /// ai1 用于存储玩家的使用物品时间
    /// </summary>
    public class IcicleSwordSplash : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[0];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = (int)MaxTime;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(44, 44), DustID.FrostStaff, -Projectile.velocity * 0.6f);
                dust.noGravity = true;
            }

            if (canDamage)
            {
                Vector2 targetDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 4; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + targetDir * i * 16).HasSolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }
            }

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.8f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }

            Timer += 1;
        }

        public override bool? CanDamage() => canDamage;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Projectile.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
                return base.Colliding(projHitbox, targetHitbox);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 15)
                Projectile.timeLeft -= 10;

            Projectile.damage = (int)(Projectile.damage * 0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> mainTex = TextureAssets.Projectile[Type];
            Vector2 center = Projectile.Center - Main.screenPosition;
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float factor = Timer / MaxTime;
            float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);

            Main.spriteBatch.Draw(mainTex.Value, center, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, effects, 0f);

            float rotation = Projectile.rotation - OwnerDirection * 0.4f;
            for (int i = -1; i < 2; i++)
            {
                float scale = 2 - Math.Abs(i);
                Vector2 drawPos = center + (rotation + i * 0.6f + Utils.Remap(factor, 0f, 2f, 0f, (float)Math.PI / 2f) * OwnerDirection).ToRotationVector2() * (mainTex.Width() * 0.5f - 4f) * Projectile.scale;
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * num3 * 0.5f, Coralite.Instance.IcicleCyan, factor, 0f, 0.5f, 0.5f, 1f, (float)Math.PI / 4f, new Vector2(scale, scale), Vector2.One);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(canDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            canDamage = reader.ReadBoolean();
        }
    }
}
