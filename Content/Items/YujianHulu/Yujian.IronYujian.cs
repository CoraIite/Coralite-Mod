using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class IronYujian : BaseYujian
    {
        public IronYujian() : base(Item.sellPrice(0, 0, 10, 0), 10, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<IronYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    
    public class IronYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "LiteSlash";

        public IronYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(50, 4f, 40, 1.2f, 0.5f),
                 new Yujian_Spurts(55, 4.8f, 60, 1.2f, 0.5f),
            },
            null,
            new Yujian_Slash(startTime: 130,
                    slashWidth: 70,
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
             new Color(42, 37, 41), new Color(199, 192, 175),
             trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "IronYujian", true
            )
        { }

    }
}
