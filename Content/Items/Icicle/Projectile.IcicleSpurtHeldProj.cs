using Coralite.Core;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSpurtHeldProj : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSword";

        public Player Owner => Main.player[Projectile.owner];
        public ref float DistanceToOwner => ref Projectile.ai[0];

        public ref float _Rotation => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.localAI[0];
        public bool fadeIn = true;

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.width = 34;
            Projectile.height = 68;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = _Rotation;
            Owner.itemTime = 2;
            Projectile.Center = Owner.Center + _Rotation.ToRotationVector2() * DistanceToOwner;

            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = Math.Clamp(Owner.itemTimeMax,12,20);
                    Projectile.rotation = _Rotation + 0.785f;
                    DistanceToOwner = 8;
                }

                Alpha += 0.2f;
                if (Alpha > 0.99f)
                {
                    Alpha = 1;
                    fadeIn = false;
                    DistanceToOwner += 8;
                }
            }
            else if (DistanceToOwner < 36)
                DistanceToOwner += 4;

            if (Projectile.timeLeft < 10)
            {
                if (Alpha > 0)
                    Alpha -= 0.1f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Owner.MountedCenter, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
            {
                float a = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + _Rotation.ToRotationVector2() * Projectile.height / 2, Owner.MountedCenter, Projectile.width / 2, ref a);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}