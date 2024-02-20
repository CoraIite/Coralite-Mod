using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class GoldenSamurai : BaseFlyingShieldItem<GoldenSamuraiGuard>
    {
        public GoldenSamurai() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Orange, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<GoldenSamuraiProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 14;
            Item.damage = 36;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RemainsOfSamurai>()
                .AddIngredient(ItemID.GoldBar,10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldenSamuraiProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GoldenSamurai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 17;
            backTime = 4;
            backSpeed = 14;
            trailCachesLength = 6;
            trailWidth = 20 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(238,202,158)*factor;
        }
    }

    public class GoldenSamuraiGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "GoldenSamurai";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 54;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.1f;
        }
    }
}
