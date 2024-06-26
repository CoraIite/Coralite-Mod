using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeShield : BaseFlyingShieldItem<RedJadeShieldGuard>
    {
        public RedJadeShield() : base(Item.sellPrice(0, 0, 20, 0), ItemRarityID.Green, AssetDirectory.RedJadeItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<RedJadeShieldProj>();
            Item.shootSpeed = 14;
            Item.damage = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 38;
        }

        public override void SetOtherValues()
        {
            flyingTime = 23;
            backTime = 5;
            backSpeed = 14f;
            trailCachesLength = 9;
            trailWidth = 22 / 2;
        }

        public override void OnShootDusts()
        {
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.2f, Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void OnJustHited()
        {
            base.OnJustHited();
            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                    , Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }

        public override Color GetColor(float factor)
        {
            return Coralite.Instance.RedJadeRed * factor;
        }
    }

    public class RedJadeShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 26;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.DigStone_Tink, Projectile.Center);
            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                    , Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
        }
    }
}
