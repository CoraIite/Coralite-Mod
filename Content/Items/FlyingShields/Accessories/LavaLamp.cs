using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class LavaLamp : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public LavaLamp() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 80, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 40;
        }

        public bool isDashing;
        public bool canSpawnEXProj;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }

            if (isDashing)
            {
                player.noKnockback = true;
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            projectile.Projectile.SpawnTrailDust(DustID.LavaMoss, Main.rand.NextFloat(0.3f, 0.5f),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
            projectile.Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(0.3f, 0.5f),
                Scale: Main.rand.NextFloat(2f, 2.5f));

            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(BuffID.OnFire3, 60 * 2);
            if (canSpawnEXProj)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, projectile.Projectile.Center);
                for (int i = 0; i < 3; i++)
                    projectile.Projectile.NewProjectileFromThis<LavaDroplet>(projectile.Projectile.Center
                        , (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(8, 13)
                        , projectile.Owner.GetWeaponDamage(Item), 0);
                canSpawnEXProj = false;
            }
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            projectile.Owner.velocity *= 0.7f;
            isDashing = false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        DashDir = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 15, dashDirection, 12f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;

                canSpawnEXProj = true;
                isDashing = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlamingLamps>()
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LavaDroplet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 12)
            {
                Projectile.velocity.Y += 0.4f;
            }

            Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(0.1f, 0.5f), Scale: Main.rand.NextFloat(1.4f, 2.4f));
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.LavaMoss, Main.rand.NextFloat(0.1f, 0.5f), Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(2, 3f));
                d.noGravity = true;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.LavaMoss, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(1, 1.2f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }
    }
}