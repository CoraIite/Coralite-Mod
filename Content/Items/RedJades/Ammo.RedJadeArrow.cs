using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeArrow : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Arrow;
            Item.damage = 7;
            Item.knockBack = 3f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 3.5f;
            Item.consumable = true;

            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<RedJadeArrowProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient<RedJade>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class RedJadeArrowProj : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeArrow";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.aiStyle = -1;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(DustID.GemRuby, Main.rand.NextFloat(0.3f, 0.5f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                if (Main.rand.NextBool())
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<RedJadeBoom>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }
    }
}
