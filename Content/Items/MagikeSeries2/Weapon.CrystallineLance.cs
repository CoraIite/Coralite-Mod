using Coralite.Content.Dusts;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineLance : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToSpear(ModContent.ProjectileType<CrystallineLanceProj>(), 1f, 24);
            Item.DamageType = DamageClass.MeleeNoSpeed; // We need to use MeleeNoSpeed here so that attack speed doesn't effect our held projectile.
            Item.SetWeaponValues(60, 12f, 0); // A special method that sets the damage, knockback, and bonus critical strike chance.
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);
            Item.channel = true; // Channel is important for our projectile.

            Item.StopAnimationOnHurt = true;

            Item.GetMagikeItem().MagikeMax = 7500;
            //MagikeHelper.CalculateMagikeCost(Core.Systems.MagikeSystem.MALevel.CrystallineMagike, 12, 60 * 2);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (MagikeHelper.TryCosumeMagike(40, Item, player))
                Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 15
                    , ModContent.ProjectileType<CrystallineSpikeFriendly>(), damage * 3, knockback, player.whoAmI);

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class CrystallineLanceProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DismountsPlayersOnHit[Type] = true;
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // Sync this projectile if a player joins mid game.

            Projectile.width = 25;
            Projectile.height = 25;

            Projectile.aiStyle = -1;

            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. Our projectile will fade in (see the AI() below).
            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.penetrate = -1; // Infinite penetration. The projectile can hit an infinite number of enemies.
            Projectile.tileCollide = false; // Don't kill the projectile if it hits a tile.
            Projectile.scale = 1f; // The scale of the projectile. This only effects the drawing and the width of the collision.
            Projectile.hide = true; // We are drawing the projectile ourselves. See PreDraw() below.
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Set the damage to melee damage.
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner]; // Get the owner of the projectile.
            Projectile.direction = owner.direction; // Direction will be -1 when facing left and +1 when facing right. 
            owner.heldProj = Projectile.whoAmI; // Set the owner's held projectile to this projectile. heldProj is used so that the projectile will be killed when the player drops or swap items.

            int itemAnimationMax = owner.itemAnimationMax;
            // Remember, frames count down from itemAnimationMax to 0
            // Frame at which the lance is fully extended. Hold at this frame before retracting.
            // Scale factor (0.34f) means the last 34% of the animation will be used for retracting.
            int holdOutFrame = (int)(itemAnimationMax * 0.34f);
            if (owner.channel && owner.itemAnimation < holdOutFrame)
            {
                owner.SetDummyItemTime(holdOutFrame); // This makes it so the projectile never dies while we are holding it (except when we take damage, see ExampleJoustingLancePlayer).
            }

            // If the Jousting Lance is no longer being used, kill the projectile.
            if (owner.ItemAnimationEndingOrEnded)
            {
                Projectile.Kill();
                return;
            }

            int itemAnimation = owner.itemAnimation;
            // extension and retraction factors (0-1). As the animation plays out, extension goes from 0-1 and stays at 1 while holding, then retraction goes from 0-1.
            float extension = 1 - Math.Max(itemAnimation - holdOutFrame, 0) / (float)(itemAnimationMax - holdOutFrame);
            float retraction = 1 - Math.Min(itemAnimation, holdOutFrame) / (float)holdOutFrame;

            // Distances are in pixels
            float extendDist = 24; // How far to fly out during extension
            float retractDist = extendDist / 2; // How far to fly back during retraction
            float tipDist = 98 + extension * extendDist - retraction * retractDist; // If your Jousting Lance is larger or smaller than the standard size, it is recommended to change the shoot speed of the item instead of this value.

            Vector2 center = owner.RotatedRelativePoint(owner.MountedCenter); // Get the center of the owner. This accounts for the player being shifted up or down while riding a mount, sitting in a chair, etc.
            Projectile.Center = center; // Set the center of the projectile to the center of the owner. Projectile.Center is now actually the tip of the Jousting Lance.
            Projectile.position += Projectile.velocity * tipDist; // The projectile velocity contains the orientation of the lance, multiply it by the tipDist to position the tip.

            // Set the rotation of the projectile.
            // For reference, 0 is the top left, 180 degrees or pi radians is the bottom right.
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + (float)Math.PI * 3 / 4f;

            // Fade the projectile in when it first spawns
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // This will increase or decrease the knockback of the Jousting Lance depending on how fast the player is moving.
            modifiers.Knockback *= Main.player[Projectile.owner].velocity.Length() / 7f;

            // This will increase or decrease the damage of the Jousting Lance depending on how fast the player is moving.
            modifiers.SourceDamage *= 0.1f + Main.player[Projectile.owner].velocity.Length() / 7f * 0.9f;
        }

        // This is the custom collision that Jousting Lances uses. 
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float rotationFactor = Projectile.rotation + (float)Math.PI / 4f; // The rotation of the Jousting Lance.
            float scaleFactor = 95f; // How far back the hit-line will be from the tip of the Jousting Lance. You will need to modify this if you have a longer or shorter Jousting Lance. Vanilla uses 95f
            float widthMultiplier = 23f; // How thick the hit-line is. Increase or decrease this value if your Jousting Lance is thicker or thinner. Vanilla uses 23f
            float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.

            // This Rectangle is the width and height of the Jousting Lance's hitbox which is used for the first step of collision.
            // You will need to modify the last two numbers if you have a bigger or smaller Jousting Lance.
            // Vanilla uses (0, 0, 300, 300) which that is quite large for the size of the Jousting Lance.
            // The size doesn't matter too much because this rectangle is only a basic check for the collision (the hit-line is much more important).
            Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);

            // Set the position of the large rectangle.
            lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
            lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

            // This is the back of the hit-line with Projectile.Center being the tip of the Jousting Lance.
            Vector2 hitLineEnd = Projectile.Center + rotationFactor.ToRotationVector2() * scaleFactor;

            // The following is for debugging the size of the hit line. This will allow you to easily see where it starts and ends.
            // Dust.NewDustPerfect(Projectile.Center, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
            // Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);

            // First check that our large rectangle intersects with the target hitbox.
            // Then we check to see if a line from the tip of the Jousting Lance to the "end" of the lance intersects with the target hitbox.
            if (lanceHitboxBounds.Intersects(targetHitbox)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }

        // We need to draw the projectile manually. If you don't include this, the Jousting Lance will not be aligned with the player.
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects change which direction the sprite is drawn.
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Get texture of projectile.
            Texture2D texture = Projectile.GetTexture();

            // Get the currently selected frame on the texture.
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            // The origin in this case is (0, 0) of our projectile because Projectile.Center is the tip of our Jousting Lance.
            Vector2 origin = Vector2.Zero;

            // The rotation of the projectile.
            float rotation = Projectile.rotation;

            // If the projectile is facing right, we need to rotate it by -90 degrees, move the origin, and flip the sprite horizontally.
            // This will make it so the bottom of the sprite is correctly facing down when shot to the right.
            if (Projectile.direction > 0)
            {
                rotation -= (float)Math.PI / 2f;
                origin.X += sourceRectangle.Width;
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            // The position of the sprite. Not subtracting Main.player[Projectile.owner].gfxOffY will cause the sprite to bounce when walking up blocks.
            Vector2 position = new(Projectile.Center.X, Projectile.Center.Y - Main.player[Projectile.owner].gfxOffY);

            // Apply lighting and draw our projectile
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

    }

    public class CrystallineSpikeFriendly : ModProjectile
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + "CrystallineSpike";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 2 * 180;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 8)
                Projectile.velocity.Y += 0.05f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() / 2);
        }

        public override void OnKill(int timeLeft)
        {
            if (!VaultUtils.isClient)
            {
                Vector2 dir2 = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = -1; i < 2; i++)
                    Projectile.NewProjectileFromThis<CrystallinePiecesFriendly>(Projectile.Center
                        , dir2.RotatedBy(i * 0.65f) * 12, (int)(Projectile.damage * 0.5f), 0, i + 1);
            }

            Helper.PlayPitched(CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact, Projectile.Center);
            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center - dir * 32, ModContent.DustType<CrystallineDustSmall>()
                    , dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.6f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 7, 1, 7, 1, 0.785f, -1);
            Projectile.QuickDraw(lightColor, 0.785f);
            return false;
        }
    }

    public class CrystallinePiecesFriendly : ModProjectile
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + "CrystallinePieces";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 5);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.velocity.Y += 0.25f;
            Projectile.rotation += Projectile.velocity.Length() * 0.04f;
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() / 2);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CrystallineDustSmall>()
                    , Helper.NextVec2Dir(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            var frameBox = tex.Frame(3, 1, (int)Projectile.ai[0], 0);

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 5, 1, 5, 1, Projectile.scale, frameBox, 0);
            Projectile.QuickDraw(frameBox, lightColor, 0);
            return false;
        }
    }

}
