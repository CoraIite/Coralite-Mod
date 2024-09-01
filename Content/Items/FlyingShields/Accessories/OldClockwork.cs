using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class OldClockwork : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public OldClockwork() : base(ItemRarityID.LightRed, Item.sellPrice(0, 2, 50, 0))
        {
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

            if (projectile.parryTime > 0 && projectile.Timer > projectile.dashTime - (projectile.parryTime * 1.5f))
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

            projectile.Owner.AddBuff(ModContent.BuffType<TightenTheClockwork>(), 60 * 4);

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

                isDashing = true;
                hit = false;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cog, 20)
                .AddIngredient(ItemID.PalladiumBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Cog, 20)
                .AddIngredient(ItemID.CobaltBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class TightenTheClockwork : ModBuff
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.moveSpeed += 0.1f;
        }
    }
}
