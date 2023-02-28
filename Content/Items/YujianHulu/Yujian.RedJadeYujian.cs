using Coralite.Content.Items.RedJadeItems;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class RedJadeYujian : BaseYujian
    {
        public RedJadeYujian() : base(Item.sellPrice(0, 0, 20, 0), 14, 1f) { }

        public override int ProjType => ModContent.ProjectileType<RedJadeYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeYujianProj : BaseYujianProj
    {
        public RedJadeYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(65, 4f, 40, 2f, 0.4f),
                 new Yujian_Spurts(65, 4.8f, 40, 2f, 0.4f),
                 new Yujian_Slash(startTime: 90,
                    slashWidth: 55,
                    slashTime: 40,
                    startAngle: -1.6f,
                    totalAngle: 1.8f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.4f,
                    halfShortAxis: 1f,
                    halfLongAxis: 5f,
                    new NoSmoother()),
                new Yujian_Slash(startTime: 90,
                    slashWidth: 60,
                    slashTime: 40,
                    startAngle: -1.4f,
                    totalAngle: 1.7f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.4f,
                    halfShortAxis: 1f,
                    halfLongAxis: 3f,
                    new NoSmoother()),
            },
            null,
            new Yujian_Slash(startTime: 130,
                    slashWidth: 60,
                    slashTime: 90,
                    startAngle: -1.8f,
                    totalAngle: 3.5f,
                    turnSpeed: 2,
                    roughlyVelocity: 0.5f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 50,
            width: 40, height: 60,
             new Color(43, 43, 51), Coralite.Instance.RedJadeRed,
             trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "RedJadeYujian", true
            )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            Helpers.Helper.NotOnServer(() =>
            {
                if (Main.rand.NextBool(4))
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            });
        }

    }
}
