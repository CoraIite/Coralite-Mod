using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class PearlRay : BaseFlyingShieldItem<PearlRayGuard>
    {
        public PearlRay() : base(Item.sellPrice(0, 0, 20), ItemRarityID.White, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 17;
            Item.shoot = ModContent.ProjectileType<PearlRayProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 13.5f;
            Item.damage = 21;
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            damage = (int)(damage * 0.75f);

            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(0.1f), type, damage, knockback, player.whoAmI, ai2: -1);
            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(-0.1f), type, damage, knockback, player.whoAmI, ai2: 1);
        }
    }

    public class PearlRayProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "PearlRay";

        public ref float Angle => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
        }

        public override void Shooting()
        {
            base.Shooting();

            if (firstShoot && Angle != 0 && !canChase)//转弯
            {
                if (Timer < flyingTime - 5)
                    Projectile.velocity = Projectile.velocity.RotatedBy(Angle * 0.03f);
            }
        }

        public override void SetOtherValues()
        {
            ShieldSlot = 0.5f;
            flyingTime = 18;
            backTime = 5;
            backSpeed = 15.5f;
            trailCachesLength = 6;
            trailWidth = 8 / 2;
        }

        public override void OnShootDusts()
        {
            SpecialDust();
        }

        public override void OnBackDusts()
        {
            SpecialDust();
        }

        public void SpecialDust()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 dir2 = (Projectile.rotation + 1.57f).ToRotationVector2();

            float rot = MathF.Sin(Timer * 0.2f) * 0.3f;

            for (int j = 0; j < 3; j++)
                for (int i = -1; i < 2; i += 2)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + (j / 3f * Projectile.velocity) + (dir * 8 * Projectile.scale) + (i * dir2 * Projectile.scale * Projectile.width / 2),
                        DustID.Water, -Projectile.velocity.RotatedBy(i * rot) * Main.rand.NextFloat(0f, 0.5f), newColor: Color.White);
                    d.noGravity = true;
                }
        }

        public override Color GetColor(float factor)
        {
            return new Color(235, 230, 223) * factor;
        }
    }

    public class PearlRayGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "PearlRay";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.4f;
            damageReduce = 0.1f;
            extraRotation = MathHelper.Pi;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Jellyfish_NPCHit25, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }
}
