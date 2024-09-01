using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class PiezoArmorPanel : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public PiezoArmorPanel() : base(ItemRarityID.Yellow, Item.sellPrice(0, 2, 50, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 75;
        }

        public bool isDashing;
        public int SpawnEXProjCount;

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
            projectile.Owner.noKnockback = true;
            Dust d = Dust.NewDustPerfect(projectile.Projectile.Center + (Main.rand.NextVector2Circular(projectile.Projectile.width, projectile.Projectile.height) / 2), DustID.Electric,
                projectile.Owner.velocity * Main.rand.NextFloat(0.1f, 0.6f), Scale: Main.rand.NextFloat(1f, 1.4f));
            d.noGravity = true;
            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SpawnEXProjCount < 2)
            {
                if (SpawnEXProjCount == 0)
                {
                    projectile.dashSpeed *= 1.2f;
                    projectile.Timer += projectile.dashTime / 3;
                }
                SoundEngine.PlaySound(CoraliteSoundID.ElectricExplosion_Item94, projectile.Projectile.Center);
                int i = projectile.Projectile.NewProjectileFromThis(projectile.Projectile.Center
                     , Vector2.Zero, ProjectileID.Electrosphere, projectile.Owner.GetWeaponDamage(Item), 0);
                Main.projectile[i].usesLocalNPCImmunity = true;
                Main.projectile[i].timeLeft = 80;
                Main.projectile[i].localNPCHitCooldown = 30;
                SpawnEXProjCount++;
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
                SoundEngine.PlaySound(CoraliteSoundID.ElectricExplosion_Item94, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 24, dashDirection, 10f);
                Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;

                SpawnEXProjCount = 0;
                isDashing = true;
                return true;
            }

            return false;
        }
    }
}
