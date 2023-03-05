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
        public PlatinumYujian() : base(ItemRarityID.White,Item.sellPrice(0, 0, 20, 0), 9, 1.5f) { }

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
            new Yujian_PreciseSlash(startTime: 130,
                    slashWidth: 80,
                    slashTime: 90,
                    startAngle: -2f,
                    totalAngle: 3f,
                    turnSpeed: 1.5f,
                    roughlyVelocity: 0.5f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 150,
            attackLenth: 300,
            width: 30, height: 58,
             new Color(63, 59, 57), new Color(151, 149, 163),
             trailCacheLenth: 18
            )
        { }

    }
}
