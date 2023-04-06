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
    public class ShadowYujian:BaseYujian
    {
        public ShadowYujian() : base(ItemRarityID.Green,Item.sellPrice(0, 0, 20, 0), 3, 1.5f) { }

        public override int ProjType => ModContent.ProjectileType<ShadowYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShadowYujianProj : BaseYujianProj
    {
        public ShadowYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(65, 4.8f, 40, 2f, 0.4f),
                 new Yujian_ShadowSlash(startTime: 120,
                    slashWidth: 60,
                    slashTime: 80,
                    startAngle: -1.8f,
                    totalAngle: 3.5f,
                    turnSpeed: 2,
                    roughlyVelocity: 0.4f,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    new HeavySmoother()),
            },
            null,
            new Yujian_Slash(startTime: 90,
                slashWidth: 55,
                slashTime: 40,
                startAngle: -1.6f,
                totalAngle: 1.84f,
                turnSpeed: 2.5f,
                roughlyVelocity: 0.4f,
                halfShortAxis: 1f,
                halfLongAxis: 4.5f,
                new NoSmoother()),
            PowerfulAttackCost: 125,
            attackLength: 370,
            width: 30, height: 60,
            Color.MediumPurple, Color.Purple,
            trailCacheLength: 18
            )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            target.StrikeNPC(damage, 0, target.direction);
            target.StrikeNPC(damage, 0, target.direction);
            Owner.addDPS(damage * 2);
        }
    }
}
