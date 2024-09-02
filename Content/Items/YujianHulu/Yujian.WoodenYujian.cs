using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class WoodenYujian : BaseYujian
    {
        public WoodenYujian() : base(ItemRarityID.White, 0, 6, 1f) { }

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
        public override string SlashTexture => AssetDirectory.Trails + "LiteSlash";

        public WoodenYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(80,22,28,120,0.93f),
            },
            new YujianAI_PreciseSlash(startTime: 110,
                    slashWidth: 40,
                    slashTime: 70,
                    startAngle: -1.6f,
                    totalAngle: 3f,
                    turnSpeed: 1.8f,
                    roughlyVelocity: 0.5f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.3f,
                    Coralite.Instance.HeavySmootherInstance),
            PowerfulAttackCost: 150,
            attackLength: 300,
            width: 30, height: 58,
            new Color(16, 7, 17), new Color(107, 82, 75),
            trailCacheLength: 12
            )
        { }
    }
}
