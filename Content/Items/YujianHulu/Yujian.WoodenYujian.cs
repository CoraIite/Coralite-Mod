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
        public WoodenYujian() : base(0, 6, 1f) { }

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
                 new Yujian_Spurts(140, 3f, 70, 1f, 0.3f),
            },
            null,
            new Yujian_Slash(startTime: 130,
                    slashWidth: 60,
                    slashTime: 90,
                    startAngle: -1.8f,
                    totalAngle: 3f,
                    turnSpeed: 2,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 50,
            width: 40, height: 58,
            new Color(16, 7, 17), new Color(107, 82, 75), 
            trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "WoodenYujian", true
            )
        { }
    }
}
