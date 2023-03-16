using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class HellStoneHulu : BaseHulu
    {
        public HellStoneHulu() : base(3, ItemRarityID.Orange, Item.sellPrice(0, 0, 5, 0), 10, 1.5f) { }

        public override IHuluEffect SetHuluEffect()
        {
            return new HellStoneHuluEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HellStoneHuluEffect : IHuluEffect
    {
        public void AIEffect(Projectile projectile)
        {
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(20, 45), DustID.FlameBurst, -projectile.velocity);
                dust.noGravity = true;
            }
        }

        public void HitEffect(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, Main.rand.Next(120, 240));
        }

        public void PostDrawEffect(Projectile projectile, Color lightColor) { }

        public void PreDrawEffect(Projectile projectile, ref Color lightColor) { }
    }
}
