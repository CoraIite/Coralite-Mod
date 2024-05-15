using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class SolarPanel : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public SolarPanel() : base(ItemRarityID.Yellow, Item.sellPrice(0, 4, 50, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 80;
        }

        public bool isDashing;
        public bool hit;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }

            if (isDashing)
            {
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);

            if (projectile.Timer > projectile.dashTime / 4)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }

            if (projectile.Timer == projectile.dashTime * 7 / 8)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);
                Projectile.NewProjectile(projectile.Owner.GetSource_ItemUse(Item), projectile.Projectile.Center,
                    projectile.Projectile.rotation.ToRotationVector2() * 10,
                     ProjectileID.ChargedBlasterOrb, damage, 0, projectile.Projectile.owner, ai1: 0.65f);
            }
            else if (projectile.Timer == projectile.dashTime * 6 / 8)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);
                Projectile.NewProjectile(projectile.Owner.GetSource_ItemUse(Item), projectile.Projectile.Center,
                    projectile.Projectile.rotation.ToRotationVector2() * 12,
                     ProjectileID.ChargedBlasterOrb, damage, 0, projectile.Projectile.owner, ai1: 0.7f);
            }
            else if (projectile.Timer == projectile.dashTime * 5 / 8)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);
                Projectile.NewProjectile(projectile.Owner.GetSource_ItemUse(Item), projectile.Projectile.Center,
                    projectile.Projectile.rotation.ToRotationVector2() * 14,
                     ProjectileID.ChargedBlasterOrb, damage, 0, projectile.Projectile.owner, ai1: 0.75f);
            }
            else if (projectile.Timer == projectile.dashTime / 2)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);
                Projectile.NewProjectile(projectile.Owner.GetSource_ItemUse(Item), projectile.Projectile.Center,
                    projectile.Projectile.rotation.ToRotationVector2() * 16,
                     ProjectileID.ChargedBlasterOrb, damage, 0, projectile.Projectile.owner, ai1: 1.2f);
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            hit = true;

            if (projectile.parryTime > 0 && projectile.Timer > projectile.dashTime - projectile.parryTime * 1.5f)
            {
                projectile.OnParry();
                projectile.UpdateShieldAccessory(accessory => accessory.OnParry(projectile));
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
            }
            else
            {
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
                projectile.OnGuard();
                projectile.OnGuardNPC(target.whoAmI);
            }

            if (projectile.Timer > projectile.dashTime / 2)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);

                Projectile.NewProjectile(projectile.Owner.GetSource_ItemUse(Item), projectile.Projectile.Center,
                    projectile.Projectile.rotation.ToRotationVector2() * 16,
                     ProjectileID.ChargedBlasterOrb, damage, 0, projectile.Projectile.owner, ai1: 1.2f);
            }

            projectile.Timer = 0;
            projectile.Owner.velocity.X = -Math.Sign(projectile.Owner.velocity.X) * 6;
            projectile.Owner.velocity.Y = -3f;
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            if (!hit)
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
                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 29, dashDirection, 14f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;

                int damage = Player.GetWeaponDamage(Item);
                Projectile.NewProjectile(Player.GetSource_ItemUse(Item), Player.Center,
                    flyingShieldGuard.Projectile.rotation.ToRotationVector2() * 8,
                     ProjectileID.ChargedBlasterOrb, damage, 0, Player.whoAmI, ai1: 0.6f);

                isDashing = true;
                hit = false;
                return true;
            }

            return false;
        }
    }
}
