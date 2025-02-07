using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeHulu : BaseHulu
    {
        public RedJadeHulu() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 9, 1.5f, AssetDirectory.RedJadeItems) { }

        public override IHuluEffect SetHuluEffect()
        {
            return new RedJadeHuluEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeHuluEffect : IHuluEffect
    {
        public void AIEffect(Projectile projectile)     //随机生成红色粒子
        {
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(20, 45), DustID.GemRuby, -projectile.velocity);
                dust.noGravity = true;
            }
        }

        public void HitEffect(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {       //6分之一概率产生爆炸
            if (Projectile.IsOwnedByLocalPlayer() && Main.rand.NextBool(6))
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), projectile.damage, projectile.knockBack, projectile.owner);
        }

        public void PostDrawEffect(Projectile projectile, Color lightColor) { }

        public void PreDrawEffect(Projectile projectile, ref Color lightColor) { }
    }
}
