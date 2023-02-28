using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class PlatinumYujian : BaseYujian
    {
        public PlatinumYujian() : base(Item.sellPrice(0, 0, 20, 0), 14, 1.5f) { }

        public override int ProjType => ModContent.ProjectileType<PlatinumYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class PlatinumYujianProj : BaseYujianProj
    {
        public PlatinumYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(80, 4.5f, 60, 1.5f, 0.5f),
                 new Yujian_Spurts(90, 5.2f, 80, 1.5f, 0.5f),
            },
            null,
            new Yujian_Slash(startTime: 130,
                    slashWidth: 80,
                    slashTime: 90,
                    startAngle: -1.8f,
                    totalAngle: 3f,
                    turnSpeed: 1.5f,
                    roughlyVelocity: 0.5f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 50,
            width: 40, height: 58,
             new Color(63, 59, 57), new Color(151, 149, 163),
             trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "PlatinumYujian", true
            )
        { }

    }
}
