using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class HellStoneYujian : BaseYujian
    {
        public HellStoneYujian() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 20, 0), 19, 6f) { }

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
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "FireTrail";

        public HellStoneYujianProj() : base(
            new YujianAI[]
            {
                 new YujianAI_BetterSpurt(70, 16, 20, 160, 0.94f),
                 new YujianAI_HellStoneDoubleSlash(),
            },
            null,
            new YujianAI_PreciseSlash(startTime: 135,
               slashWidth: 55,
               slashTime: 125,
               startAngle: -2f,
               totalAngle: 22f,
               turnSpeed: 2.3f,
               roughlyVelocity: 0.5f,
               halfShortAxis: 1f,
               halfLongAxis: 1f,
               Coralite.Instance.NoSmootherInstance),
            PowerfulAttackCost: 100,
            attackLength: 460,
            width: 30, height: 64,
            new Color(20, 22, 30, 100), new Color(247, 225, 180),
             trailCacheLength: 16
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
            target.AddBuff(BuffID.OnFire, Main.rand.Next(120, 240));
        }
    }

    public class YujianAI_HellStoneDoubleSlash : YujianAI_DoubleSlash
    {
        public YujianAI_HellStoneDoubleSlash() : base(90, 70, 70, -2.4f, 4.5f, 2f, 0.6f, 1f, 1f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 85;

            SlashTime = 65;
            StartAngle = 2f;

            halfShortAxis = 1f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 85;

            SlashTime = 35;
            StartAngle = -2f;

            halfShortAxis = 1f;
            halfLongAxis = 2f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }
    }
}