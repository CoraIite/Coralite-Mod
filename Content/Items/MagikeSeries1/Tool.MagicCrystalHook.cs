﻿using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            Item.shootSpeed = 16f; // This defines how quickly the hook is shot.
            Item.shoot = ModContent.ProjectileType<MagicCrystalHookProjectile>(); // Makes the item shoot the hook's projectile when used.
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

    public class MagicCrystalHookProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        private static Asset<Texture2D> chainTexture;

        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>(AssetDirectory.MagikeSeries1Item + "MagicCrystalHookChain");
        }

        public override void Unload()
        {
            chainTexture = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < 1000; l++)
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                    hooksOut++;

            return hooksOut <= 2;
        }

        // Use this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
        // You can also change the projectile like: Dual Hook, Lunar Hook
        // public override void UseGrapple(Player player, ref int type)
        // {
        //	int hooksOut = 0;
        //	int oldestHookIndex = -1;
        //	int oldestHookTimeLeft = 100000;
        //	for (int i = 0; i < 1000; i++)
        //	{
        //		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
        //		{
        //			hooksOut++;
        //			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
        //			{
        //				oldestHookIndex = i;
        //				oldestHookTimeLeft = Main.projectile[i].timeLeft;
        //			}
        //		}
        //	}
        //	if (hooksOut > 1)
        //	{
        //		Main.projectile[oldestHookIndex].Kill();
        //	}
        // }

        // Amethyst Hook is 300, Static Hook is 600.
        public override float GrappleRange()
        {
            return 350f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        // default is 11, Lunar is 24
        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 12f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10; // How fast you get pulled to the grappling hook projectile's landing position
        }

        // Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
        //public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
        //{
        //    Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
        //    float hangDist = 50f;
        //    grappleX += dirToPlayer.X * hangDist;
        //    grappleY += dirToPlayer.Y * hangDist;
        //}

        // Can customize what tiles this hook can latch onto, or force/prevent latching alltogether, like Squirrel Hook also latching to trees
        //public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        //{
        //    // By default, the hook returns null to apply the vanilla conditions for the given tile position (this tile position could be air or an actuated tile!)
        //    // If you want to return true here, make sure to check for Main.tile[x, y].HasUnactuatedTile (and Main.tileSolid[Main.tile[x, y].TileType] and/or Main.tile[x, y].HasTile if needed)

        //    // We make this hook latch onto trees just like Squirrel Hook

        //    // Tree trunks cannot be actuated so we don't need to check for that here
        //    Tile tile = Main.tile[x, y];
        //    if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree)
        //    {
        //        return true;
        //    }

        //    // In any other case, behave like a normal hook
        //    return null;
        //}

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
                directionToPlayer *= chainTexture.Height(); // multiply by chain link length

                center += directionToPlayer; // update draw position
                directionToPlayer = playerCenter - center; // update distance
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                // Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            // Stop vanilla from drawing the default chain.
            return false;
        }
    }
}
