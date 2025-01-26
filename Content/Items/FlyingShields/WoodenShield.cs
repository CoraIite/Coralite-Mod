using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class WoodenShield : BaseFlyingShieldItem<WoodenShieldGuard>
    {
        public WoodenShield() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.White, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<WoodenShieldProj>();
            Item.knockBack = 3;
            Item.shootSpeed = 10f;
            Item.damage = 9;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 15)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class WoodenShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "WoodenShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 28;
        }

        public override void SetOtherValues()
        {
            flyingTime = 16;
            backTime = 17;
            backSpeed = 11;
            trailCachesLength = 5;
            trailWidth = 24 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);
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
            return Color.Brown * factor;
        }
    }

    public class WoodenShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "WoodenShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 28;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.05f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);
        }
    }
}
