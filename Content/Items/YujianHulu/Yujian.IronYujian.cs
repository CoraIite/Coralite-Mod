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
        public IronYujian() : base(ItemRarityID.White,Item.sellPrice(0, 0, 10, 0), 7, 1.3f) { }

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
                 new Yujian_Spurts(120, 2.8f, 70, 0.8f, 0.4f),
                 new Yujian_Spurts(140, 3f, 80, 1f, 0.4f),
            },
            null,
            new Yujian_PreciseSlash(startTime: 150,
                    slashWidth: 55,
                    slashTime: 100,
                    startAngle: -2f,
                    totalAngle: 3f,
                    turnSpeed: 2,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 150,
            attackLenth: 220,
            width: 30, height: 58,
            new Color(42, 37, 41), new Color(169, 162, 135),
             trailCacheLenth: 16
            )
        { }

    }
}
