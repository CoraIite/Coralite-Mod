using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
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
            Item.shootSpeed = 15;
            Item.damage = 38;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield++;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RemainsOfSamurai>()
                .AddIngredient(ItemID.GoldBar, 10)
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
            flyingTime = 20;
            backTime = 6;
            backSpeed = 14;
            trailCachesLength = 6;
            trailWidth = 20 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(238, 202, 158) * factor;
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
