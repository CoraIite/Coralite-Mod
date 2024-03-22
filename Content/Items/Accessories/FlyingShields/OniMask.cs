using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class OniMask : BaseAccessory, IFlyingShieldAccessory, IDashable
    {
        public OniMask() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 80, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 35;
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
                projectile.OnGuardNPC();
            }

            projectile.Timer = 0;
            projectile.Owner.velocity.X = (-Math.Sign(projectile.Owner.velocity.X)) * 6;
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

                flyingShieldGuard.TurnToDashing(this, 16, dashDirection, 12f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
                int damage = Player.GetWeaponDamage(Item);
                for (int i = -1; i < 2; i++)
                {
                    int index = Projectile.NewProjectile(Player.GetSource_ItemUse(Item), Player.Center, (dashDirection + i * 0.2f).ToRotationVector2() * 10,
                         ProjectileID.BoneGloveProj, damage, 0, Player.whoAmI);
                    Main.projectile[index].usesLocalNPCImmunity = true;
                    Main.projectile[index].localNPCHitCooldown = 30;
                }

                isDashing = true;
                hit = false;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RemainsOfSamurai>( )
                .AddIngredient(ItemID.Topaz, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
