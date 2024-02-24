using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class EekShield : BaseFlyingShieldItem<EekShieldGuard>
    {
        public EekShield() : base(Item.sellPrice(0, 5), ItemRarityID.Pink, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<EekShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 42;
        }
    }

    public class EekShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 14;
            backSpeed = 15;
            trailCachesLength = 6;
            trailWidth = 12 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
            {
                float angle = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = (angle + i * MathHelper.TwoPi / 3).ToRotationVector2();
                    Projectile.NewProjectileFromThis<EekShieldEXProj>(target.Center, dir * 5,
                        Projectile.damage / 2, Projectile.knockBack);
                }
            }
        }

        public override Color GetColor(float factor)
        {
            return Color.DarkRed * factor;
        }
    }

    public class EekShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 48;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.4f;
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_Zombie33, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    public class EekShieldEXProj:ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "EekShield";

        float alpha = 1;

        public override void SetStaticDefaults()
        {
            Helper.QuickSetTrailSets(Type, 2, 6);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.SpawnTrailDust(DustID.RedTorch, Main.rand.NextFloat(0.1f, 0.5f), (int)(255 - alpha * 255));
            Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.timeLeft * 0.12f) * 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 20)
                alpha -= 1 / 20f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor *= alpha;
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 6, 1, 6, 1, extraRot: -1.57f,scale: 0.5f);
            Projectile.QuickDraw(lightColor, 0.5f, -1.57f);
            return false;
        }
    }
}
