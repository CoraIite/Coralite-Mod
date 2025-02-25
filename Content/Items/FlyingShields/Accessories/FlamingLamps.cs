using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlamingLamps : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public FlamingLamps() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 10, 0))
        {
        }

        public float Priority => IDashable.AccessoryDashLow;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 25;
        }

        public bool isDashing;
        public bool canSpawnEXProj;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
                cp.AddDash(this);
            }

            if (isDashing)
            {
                player.noKnockback = true;
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            for (int i = 0; i < 2; i++)
            {
                projectile.Projectile.SpawnTrailDust(DustID.Torch, Main.rand.NextFloat(0.3f, 0.5f),
                    Scale: Main.rand.NextFloat(2f, 3f));
            }

            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(BuffID.OnFire, 60 * 2);
            if (canSpawnEXProj)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, projectile.Projectile.Center);
                projectile.Projectile.NewProjectileFromThis<FlamingLampsExplosion>(projectile.Projectile.Center, Vector2.Zero
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
                        Player.direction = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                SoundEngine.PlaySound(CoraliteSoundID.FireShoot_DD2_BetsyFireballShot, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 12, dashDirection, 8f);
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
                .AddIngredient(ItemID.Torch, 5)
                .AddIngredient(ItemID.CopperBar, 5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Torch, 5)
                .AddIngredient(ItemID.TinBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FlamingLampsExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 120;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3),
                    Scale: Main.rand.NextFloat(2, 3f));
                d.noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Pixie, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3),
                    Scale: Main.rand.NextFloat(1, 1.2f));
                //d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }
    }
}
