using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.TheHyacinthSeries
{
    public class RoseGunpowder : BaseAccessory
    {
        public override string Texture => AssetDirectory.TheHyacinthSeriesItems + Name;

        public RoseGunpowder() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 2))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetKnockback(DamageClass.Ranged) += 0.1f;
            player.bulletDamage *= 1.1f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.RoseGunpowderEffect > 0)
                    cp.RoseGunpowderEffect--;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PollenGunpowder>()
                .AddIngredient(ItemID.JungleRose)
                .AddIngredient(ItemID.ExplosivePowder, 12)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }

    public class RoseGunpowderProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 70;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 8)
            {
                Projectile.velocity.Y += 0.03f;
            }

            if (Main.rand.NextBool(6))
            {
                Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(0.1f, 0.3f), noGravity: false);

                Color c = Main.rand.Next(3) switch
                {
                    0 => Color.DarkRed,
                    1 => Color.IndianRed,
                    _ => new Color(121, 114, 193)
                };
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                    Helper.NextVec2Dir(1, 2), 0, c, 0.25f);

            }

            Projectile.SpawnTrailDust(DustID.PurpleTorch, Main.rand.NextFloat(-0.1f, 0.1f), noGravity: !Main.rand.NextBool(3));
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4) - Projectile.velocity / 3 * i
                    , DustID.RedTorch, Vector2.Zero, Scale: Main.rand.NextFloat(0.6f, 0.8f));
                dust.noGravity = Main.rand.NextBool(5);
                dust.velocity = -Projectile.velocity * Main.rand.NextFloat(-0.05f, 0.05f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            int howmany = Main.rand.Next(3, 6);
            for (int i = 0; i < howmany; i++)
            {
                Projectile.NewProjectileFromThis<RoseGunpowderProj2>(Projectile.Center, Helper.NextVec2Dir(8, 10)
                    , (int)(Projectile.damage * 0.8f), Projectile.knockBack);
            }

            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.Firework_Red, Helper.NextVec2Dir(1, 2));

            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.RedTorch, Helper.NextVec2Dir(0.3f, 1.5f), Scale: Main.rand.NextFloat(0.8f, 1f));

            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.Torch, Helper.NextVec2Dir(1, 1.5f), Scale: Main.rand.NextFloat(1, 2f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBallDrath_NPCDeath3, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<PollenFire>(), 60 * 4);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class RoseGunpowderProj2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 12;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 6)
                LightTrailParticle.Spawn(Projectile.Center, Projectile.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.3f, 0.5f),
                    Color.DarkRed, Main.rand.NextFloat(0.2f, 0.4f));
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.Firework_Red, Helper.NextVec2Dir(1, 2));

            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.RedTorch, Helper.NextVec2Dir(0.3f, 0.8f), Scale: Main.rand.NextFloat(0.8f, 1f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBallDrath_NPCDeath3, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<PollenFire>(), 60 * 4);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
