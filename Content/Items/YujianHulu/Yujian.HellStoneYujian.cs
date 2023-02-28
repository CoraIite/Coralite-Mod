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
    public class HellStoneYujian : BaseYujian
    {
        public HellStoneYujian() : base(Item.sellPrice(0, 0, 20, 0), 14, 1.5f) { }

        public override int ProjType => ModContent.ProjectileType<HellStoneYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HellStoneYujianProj : BaseYujianProj
    {
        public HellStoneYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(40, 3f,120, 3, 0),
                 new Yujian_Spurts(45, 3.5f,150, 3, 0),
                 new Yujian_Slash(startTime: 200,
                    slashWidth: 40,
                    slashTime: 180,
                    startAngle: -2f,
                    totalAngle: 10f,
                    turnSpeed: 4,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1f,
                    new NoSmoother()),
            },
            null,
            new Yujian_Slash(startTime: 130,
                    slashWidth: 100,
                    slashTime: 90,
                    startAngle: -1.8f,
                    totalAngle: 3.5f,
                    turnSpeed: 2,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 50,
            width: 40, height: 64,
             new Color(80, 80, 80), new Color(200, 200, 200),
             trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "HellStoneYujian", true
            )
        { }

    }

}
