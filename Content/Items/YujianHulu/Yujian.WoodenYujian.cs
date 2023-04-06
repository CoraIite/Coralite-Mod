using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class WoodenYujian : BaseYujian
    {
        public WoodenYujian() : base(ItemRarityID.White, 0, 5, 1f) { }

        public override int ProjType => ModContent.ProjectileType<WoodenYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 15)
                .Register();
        }
    }

    public class WoodenYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "LiteSlash";

        public WoodenYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(120, 2.6f, 60, 1f, 0.3f),
            },
            null,
            new Yujian_PreciseSlash(startTime: 160,
                    slashWidth: 40,
                    slashTime: 100,
                    startAngle: -1.6f,
                    totalAngle: 3f,
                    turnSpeed: 1.6f,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.3f,
                    new HeavySmoother()),
            PowerfulAttackCost: 150,
            attackLength: 200,
            width: 30, height: 58,
            new Color(16, 7, 17), new Color(107, 82, 75), 
            trailCacheLength: 12
            )
        { }

        public override void PostSetDefaults()
        {
            Projectile.localNPCHitCooldown = 35;
        }
    }
}
