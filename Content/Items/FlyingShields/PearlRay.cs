using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class PearlRay : BaseFlyingShieldItem<PearlRayGuard>
    {
        public PearlRay() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.White, AssetDirectory.FlyingShieldItems)
        {
        }

        public bool PowerfulAttack;

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 17;
            Item.shoot = ModContent.ProjectileType<PearlRayProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 13.5f;
            Item.damage = 21;
        }

        public override void HoldItem(Player player)
        {
            if (PowerfulAttack)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(player.Center + (5 * Main.GlobalTimeWrappedHourly + i * MathHelper.Pi).ToRotationVector2() * 32,
                        DustID.Water, Vector2.Zero);
                    d.noGravity = true;
                }
            }
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            int ai2 = 0;
            if (PowerfulAttack)
            {
                ai2 = 1;
                damage = (int)(damage * 1.2f);
            }
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, ai2: ai2);

            PowerfulAttack = false;
        }
    }

    public class PearlRayProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "PearlRay";

        ref float Powerful => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 30;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 20;
            backSpeed = 14.5f;
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
            if (Powerful == 1)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Vector2 dir2 = (Projectile.rotation + 1.57f).ToRotationVector2();
                for (int j = 0; j < 3; j++)
                    for (int i = -1; i < 2; i += 2)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center + (j / 3f) * Projectile.velocity + dir * 8 * Projectile.scale + i * dir2 * Projectile.scale * Projectile.width / 2,
                            DustID.Water, -Projectile.velocity * Main.rand.NextFloat(0f, 0.5f), newColor: Color.White);
                        d.noGravity = true;
                    }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
            {
                if (Owner.HeldItem.ModItem is PearlRay pr)
                    pr.PowerfulAttack = true;
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
            if (Owner.HeldItem.ModItem is PearlRay pr)
                pr.PowerfulAttack = true;
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }
}
