using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class MidasGunpowder : BaseAccessory
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public MidasGunpowder() : base(ItemRarityID.Yellow, Item.sellPrice(0, 10))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetKnockback(DamageClass.Ranged) += 0.15f;
            player.bulletDamage *= 1.15f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.MidasGunpowderEffect > 0)
                    cp.MidasGunpowderEffect--;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RoseGunpowder>()
                .AddIngredient(ItemID.BlackFairyDust)
                .AddIngredient(ItemID.GoldDust, 10)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }

    public class MidasGunpowderProj : ModProjectile
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
            if (Main.rand.NextBool(6))
            {
                Projectile.SpawnTrailDust(DustID.GoldCoin, Main.rand.NextFloat(0.1f, 0.3f),Scale:Main.rand.NextFloat(1,1.5f), noGravity: false);

                Color c = Main.rand.Next(3) switch
                {
                    0 => Color.Gold,
                    1 => Color.DarkGoldenrod,
                    _ => Color.Goldenrod
                };
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                    Helper.NextVec2Dir(1, 2), 0, c, 0.25f);
            }

            Projectile.SpawnTrailDust(DustID.CorruptTorch, Main.rand.NextFloat(-0.1f, 0.1f), noGravity: !Main.rand.NextBool(3));
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4) - (Projectile.velocity / 3 * i)
                    , DustID.YellowTorch, Vector2.Zero, Scale: Main.rand.NextFloat(0.6f, 1.2f));
                dust.noGravity = Main.rand.NextBool(5);
                dust.velocity = -Projectile.velocity * Main.rand.NextFloat(-0.05f, 0.05f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = Helper.NextVec2Dir();
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectileFromThis<MidasGunpowdeProj2>(Projectile.Center, dir.RotatedBy(MathHelper.TwoPi/6*i)*Main.rand.NextFloat(8,10)
                    , (int)(Projectile.damage * 0.6f), Projectile.knockBack);
            }

            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.FireworkFountain_Yellow, Helper.NextVec2Dir(1, 2));

            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.CorruptTorch, Helper.NextVec2Dir(0.3f, 1.5f), Scale: Main.rand.NextFloat(0.8f, 1f));

            for (int i = 0; i < 4; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.GoldCoin, Helper.NextVec2Dir(1, 1.5f), Scale: Main.rand.NextFloat(1, 2f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBallDrath_NPCDeath3, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Midas, 60 * 5);
            target.AddBuff(ModContent.BuffType<PollenFire>(), 60 * 4);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class MidasGunpowdeProj2 : ModProjectile
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
            if (Projectile.timeLeft > 7)
            {
               LightTrailParticle.Spawn(Projectile.Center, Projectile.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.3f, 0.5f),
                    Color.DarkGoldenrod, Main.rand.NextFloat(0.2f, 0.4f),Color.Purple with { A=0});
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.FireworkFountain_Yellow, Helper.NextVec2Dir(1, 2));

            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.CorruptTorch, Helper.NextVec2Dir(0.3f, 0.8f), Scale: Main.rand.NextFloat(0.8f, 1f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBallDrath_NPCDeath3, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Midas, 60 * 5);
            target.AddBuff(ModContent.BuffType<PollenFire>(), 60 * 4);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
