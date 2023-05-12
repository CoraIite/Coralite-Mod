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
        public HellStoneYujian() : base(ItemRarityID.Orange,Item.sellPrice(0, 0, 20, 0), 14, 1.8f) { }

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
        public override string SlashTexture => AssetDirectory.OtherProjectiles+ "FireTrail";

        public HellStoneYujianProj() : base(
            new YujianAI[]
            {
                 new Yujian_Spurts(90, 4.5f, 80, 1.5f, 0.4f),
                 new Yujian_Spurts(100, 5.2f, 80, 1.5f, 0.4f),
                 new YujianAI_PreciseSlash(startTime: 130,
                    slashWidth: 70,
                    slashTime: 90,
                    startAngle: -1.8f,
                    totalAngle: 3.5f,
                    turnSpeed: 2,
                    roughlyVelocity: 0,
                    halfShortAxis: 1f,
                    halfLongAxis: 1.5f,
                    Coralite.Instance.HeavySmootherInstance),
            },
            null,
            new YujianAI_PreciseSlash(startTime: 145,
               slashWidth: 50,
               slashTime: 110,
               startAngle: -2f,
               totalAngle: 14f,
               turnSpeed: 2.3f,
               roughlyVelocity: 0.5f,
               halfShortAxis: 1f,
               halfLongAxis: 1.3f,
               Coralite.Instance.NoSmootherInstance),
            PowerfulAttackCost: 125,
            attackLength: 360,
            width: 30, height: 64,
            new Color(20, 22, 30,100), new Color(247, 225, 180),
             trailCacheLength: 20
            )
        { }

        public override void AIEffect()
        {
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 45), DustID.FlameBurst, -Projectile.velocity);
                dust.noGravity = true;
            }
        }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, Main.rand.Next(120,240));
        }
    }

}
