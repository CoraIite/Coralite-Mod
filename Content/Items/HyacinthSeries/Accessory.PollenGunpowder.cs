using Coralite.Content.Dusts;
using Coralite.Content.GlobalNPCs;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class PollenGunpowder : BaseAccessory
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public PollenGunpowder() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 50))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.bulletDamage *= 1.05f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.PollenGunpowderEffect > 0)
                    cp.PollenGunpowderEffect--;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicalPowder>(5)
                .AddIngredient(ItemID.Fireblossom, 3)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PollenGunpowderProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 50;
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
                    0 => Color.Pink,
                    1 => Color.SkyBlue,
                    _ => new Color(121, 114, 193)
                };
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                    Helper.NextVec2Dir(1, 2), 0, c, 0.25f);

            }

            Projectile.SpawnTrailDust(DustID.PurpleTorch, Main.rand.NextFloat(-0.1f, 0.1f), noGravity: !Main.rand.NextBool(3));
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4) - (Projectile.velocity / 3 * i)
                    , DustID.UnusedWhiteBluePurple, Vector2.Zero);
                dust.noGravity = Main.rand.NextBool(5);
                dust.velocity = -Projectile.velocity * Main.rand.NextFloat(-0.05f, 0.05f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.PinkTorch, Helper.NextVec2Dir(1, 2));

            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(5, 12), DustID.UnusedWhiteBluePurple, Helper.NextVec2Dir(0.3f, 1.5f), Scale: Main.rand.NextFloat(1.5f, 2f));

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

    public class PollenFire : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC cgnpc))
            {
                cgnpc.PollenFire = true;
            }

            if (Main.rand.Next(4) < 3)
            {
                Dust dust4 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4
                    , Main.rand.NextBool(4) ? DustID.UnusedWhiteBluePurple : DustID.PurpleTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 2f);
                dust4.noGravity = true;
                dust4.velocity *= 1.8f;
                dust4.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                {
                    dust4.noGravity = false;
                    dust4.scale *= 0.5f;
                }
            }

            Lighting.AddLight((int)(npc.position.X / 16f), (int)((npc.position.Y / 16f) + 1f), 0.3f, 0.1f, 1f);
        }
    }
}
