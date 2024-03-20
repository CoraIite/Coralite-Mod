using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class SolarTwinkle : BaseAccessory, IFlyingShieldAccessory, IDashable
    {
        public SolarTwinkle() : base(ItemRarityID.Cyan, Item.sellPrice(0, 4, 50, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Melee;
            Item.damage = 120;
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
            for (int i = 0; i < 2; i++)
            {
                projectile.Projectile.SpawnTrailDust(DustID.Wraith, Main.rand.NextFloat(0.3f, 0.5f),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
            }

            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SpawnEXProjCount < 3)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, projectile.Projectile.Center);
                int damage = projectile.Owner.GetWeaponDamage(Item);
                int damage2 = (int)(damage * 0.9f);
                if (SpawnEXProjCount == 0)
                {
                    projectile.Projectile.NewProjectileFromThis<ForbiddenLampExplosion>(projectile.Projectile.Center
                        , Vector2.Zero, damage, 0);
                    for (int i = 0; i < 3; i++)
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0, 35 + i * 10);
                }
                else
                {
                    projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                        , (-1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                        , damage2, 0, 35);
                    if (Main.rand.NextBool())
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0, 45);
                    if (Main.rand.NextBool(4))
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0, 55);
                }

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
                SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 20, dashDirection, 12f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
                SpawnEXProjCount = 0;
                isDashing = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<ForbiddenLamp>()
            //    .AddIngredient<PiezoArmorPanel>()
            //    .AddIngredient(ItemID.FragmentSolar, 8)
            //    .AddTile(TileID.LunarCraftingStation)
            //    .Register();
        }
    }
}
