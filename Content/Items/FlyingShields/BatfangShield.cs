using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class BatfangShield : BaseFlyingShieldItem<BatfangShieldGuard>
    {
        public BatfangShield() : base(Item.sellPrice(0, 0, 5), ItemRarityID.Blue, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<BatfangShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12;
            Item.damage = 16;
        }
    }

    public class BatfangShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "BatfangShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 12;
            backSpeed = 13;
            trailCachesLength = 6;
            trailWidth = 16 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool()&& !Owner.moonLeech && !target.immortal && State ==(int)FlyingShieldStates.Shooting)
            {
                float num = damageDone * 0.05f;
                if ((int)num != 0 && !(Owner.lifeSteal <= 0f))
                {
                    Owner.lifeSteal -= num;
                    int num2 = Projectile.owner;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, 305, 0, 0f, Projectile.owner, num2, num);
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color GetColor(float factor)
        {
            return Color.Red*factor;
        }
    }

    public class BatfangShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "BatfangShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 32;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.1f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center);
        }
    }
}
