using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class LeafShield : BaseFlyingShieldItem<LeafShieldGuard>
    {
        public LeafShield() : base(Item.sellPrice(0, 0, 5), ItemRarityID.Blue, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<LeafShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12.5f;
            Item.damage = 19;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.LivingLoom)
                .Register();
        }
    }

    public class LeafShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "LeafShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            flyingTime = 19;
            backTime = 15;
            backSpeed = 13f;
            trailCachesLength = 6;
            trailWidth = 8;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
        }

        public override Color GetColor(float factor)
        {
            return Color.Green * factor;
        }
    }

    public class LeafShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "LeafShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.05f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
        }
    }
}
