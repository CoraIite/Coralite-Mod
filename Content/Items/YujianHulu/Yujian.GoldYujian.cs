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
    public class GoldYujian:BaseYujian
    {
        public GoldYujian() : base(Item.sellPrice(0, 0, 18, 0), 12, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<GoldYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldYujianProj:BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "LiteSlash";

        public GoldYujianProj() : base(
            new YujianAI[]
            {
                new Yujian_Spurts(35,2.5f,100,3,0),
                new Yujian_Spurts(40,3f,120,4,0),
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
            PowerfulAttackCost:50,
            width: 40, height: 58,
             new Color(40, 40, 40), new Color(150, 150, 150),
             trailCacheLenth: 18,
            AssetDirectory.YujianHulu + "GoldYujian", true
            )
        { }

    }
}
