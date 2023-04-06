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
    public class GoldYujian : BaseYujian
    {
        public GoldYujian() : base(ItemRarityID.Blue,Item.sellPrice(0, 0, 18, 0), 8, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<GoldYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GoldYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "LiteSlash";

        public GoldYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(80, 4.5f, 60, 1.3f, 0.4f),
                 new Yujian_Spurts(90, 5.2f, 80, 1.3f, 0.4f),
            },
            null,
            new Yujian_PreciseSlash(startTime: 130,
                    slashWidth: 70,
                    slashTime: 90,
                    startAngle: -2f,
                    totalAngle: 3f,
                    turnSpeed: 2,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            PowerfulAttackCost: 150,
            attackLength: 290,
            width: 30, height: 58,
            Color.DarkRed, Color.Gold,
            trailCacheLength: 18
            )
        { }

    }
}
