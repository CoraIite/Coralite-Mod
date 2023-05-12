using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class IcicleYujian : BaseYujian
    {
        public IcicleYujian() : base(ItemRarityID.White, Item.sellPrice(0, 0, 50, 0), 14, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<IronYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IcicleYujianProj : BaseYujianProj
    {
        public IcicleYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(70,18,25,180,0.93f),
                new YujianAI_BetterSpurt(70,18,25,180,0.93f),
                 new YujianAI_Slash(startTime: 76,
                    slashWidth: 55,
                    slashTime: 36,
                    startAngle: -1.6f,
                    totalAngle: 1.84f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 4.5f,
                    Coralite.Instance.NoSmootherInstance),
                new YujianAI_Slash(startTime: 76,
                    slashWidth: 60,
                    slashTime: 36,
                    startAngle: -1.4f,
                    totalAngle: 1.7f,
                    turnSpeed: 2.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 3f,
                    Coralite.Instance.NoSmootherInstance),
            },
            null,
            new YujianAI_RedDash(75, 35, 30, 180, 0.96f),
            PowerfulAttackCost: 150,
            attackLength: 430,
            width: 30, height: 60,
            new Color(43, 43, 51), Coralite.Instance.IcicleCyan,
            trailCacheLength: 24
            )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(5))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

}
