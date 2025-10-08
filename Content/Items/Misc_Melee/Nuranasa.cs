using Coralite.Content.WorldGeneration;
using Coralite.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Melee
{
    public class Nuranasa : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Muramasa);
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.useTurn = false;

            Item.shootSpeed = 3;
            Item.shoot = ModContent.ProjectileType<NuranasaProj>(); // The projectile is what makes a shortsword work
        }

        public override void UpdateInventory(Player player)
        {
            Transform();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Transform();
        }

        public void Transform()
        {
            if (!CoraliteWorld.CoralCatWorld)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Meowmere);
                Item.SetDefaults(ItemID.Muramasa);
            }
        }
    }

    public class NuranasaProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Melee + "Nuranasa";

        // The "width" of the blade
        public float CollisionWidth => 10f * Projectile.scale;

        private bool hited = true;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 360; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
            Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            const int FadeInDuration = 7;
            const int FadeOutDuration = 4;

            const int TotalDuration = 16;

            Timer += 1;
            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                Projectile.Kill();
                return;
            }
            else
            {
                // Important so that the sprite draws "in" the player's hand and not fully infront or behind the player
                player.heldProj = Projectile.whoAmI;
            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            Projectile.Center = playerCenter + (Projectile.velocity * (Timer - 1f));

            // Set spriteDirection based on moving left or right. Left -1, right 1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (MathHelper.PiOver4 * Projectile.spriteDirection);

            // The code in this method is important to align the sprite with the hitbox how we want it to
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 42 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

            // Vanilla configuration for "hitbox towards the end"
            //if (Projectile.spriteDirection == 1) {
            //	DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = (int)-DrawOriginOffsetX * 2;
            //	DrawOriginOffsetY = 0;
            //}
            //else {
            //	DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = 0;
            //	DrawOriginOffsetY = 0;
            //}
        }

        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            return false;
        }

        public override void CutTiles()
        {
            // "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f);
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.velocity * 6f);
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.immortal && hited)
            {
                int num5 = Main.rand.Next(1, 4);
                Player p = Main.player[Projectile.owner];

                for (int j = 0; j < num5; j++)
                {
                    NPC nPC = target;
                    Rectangle hitbox = nPC.Hitbox;
                    hitbox.Inflate(30, 16);
                    hitbox.Y -= 8;
                    Vector2 vector3 = Main.rand.NextVector2FromRectangle(hitbox);
                    Vector2 vector4 = hitbox.Center.ToVector2();
                    Vector2 spinningpoint = (vector4 - vector3).SafeNormalize(new Vector2(p.direction, p.gravDir)) * 8f;
                    Main.rand.NextFloat();
                    float num6 = ((Main.rand.Next(2) * 2) - 1) * (((float)Math.PI / 5f) + ((float)Math.PI * 4f / 5f * Main.rand.NextFloat()));
                    num6 *= 0.5f;
                    spinningpoint = spinningpoint.RotatedBy(0.7853981852531433);
                    int num7 = 3;
                    int num8 = 10 * num7;
                    int num9 = 5;
                    int num10 = num9 * num7;
                    vector3 = vector4;
                    for (int k = 0; k < num10; k++)
                    {
                        vector3 -= spinningpoint;
                        spinningpoint = spinningpoint.RotatedBy((0f - num6) / num8);
                    }

                    vector3 += nPC.velocity * num9;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), vector3
                        , spinningpoint, ProjectileID.Muramasa, (int)(hit.SourceDamage * 0.5f), 0f, Projectile.owner, num6);
                }

                hited = false;
            }
        }
    }
}
