using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalHook : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 13f; 
            Item.shoot = ModContent.ProjectileType<MagicCrystalHookProjectile>();
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(15)
                .AddIngredient<Basalt>(20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries1Item)]
    public class MagicCrystalHookProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        [VaultLoaden("{@classPath}" + "MagicCrystalHookChain")]
        public static ATex ChainTexture { get; private set; }

        private bool specialSpeed;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
            Projectile.width = Projectile.height = 28;
        }

        public override void PostAI()
        {
            if (Projectile.ai[0] == 2)
            {
                if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) < 16 * 6)
                {
                    //生成特效
                    Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
                    for (int i = 0; i < 4; i++)
                    {
                        PRTLoader.NewParticle<CrystalBurstParticle>(Projectile.Center + i * dir * 28, -(Projectile.rotation + 1.57f).ToRotationVector2().RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(1, 2), Color.White);
                    }

                    specialSpeed = true;
                    Player owner = Main.player[Projectile.owner];
                    Vector2 newVel = (Projectile.Center - owner.Center).SafeNormalize(Vector2.Zero) * 14;
                    if (MathF.Abs(newVel.X) > 11)
                        newVel.X = MathF.Sign(newVel.X) * 11;//别让X太快

                    owner.velocity = newVel;
                    owner.Center += new Vector2(0, 1);
                    owner.RemoveAllGrapplingHooks();

                    Helper.PlayPitched(CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact, Projectile.Center, pitch: -0.6f);
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (specialSpeed)
            {
            }
        }

        public override bool? CanUseGrapple(Player player)
        {
            if (player.ownedProjectileCounts[Projectile.type] > 0)
                return false;

            return null;
        }

        public override float GrappleRange()
        {
            return 360f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 14f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10.5f; 
        }

        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; // get unit vector
                directionToPlayer *= ChainTexture.Height(); // multiply by chain link length

                center += directionToPlayer; // update draw position
                directionToPlayer = playerCenter - center; // update distance
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                // Draw chain
                Main.EntitySpriteDraw(ChainTexture.Value, center - Main.screenPosition,
                    ChainTexture.Value.Bounds, drawColor, chainRotation,
                    ChainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            // Stop vanilla from drawing the default chain.
            return false;
        }
    }
}
