using Coralite.Content.WorldGeneration;
using Coralite.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Melee
{
    public class BladeOfCatnip : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public static LocalizedText craftCondition;

        public override void Load()
        {
            craftCondition = this.GetLocalization("CraftCondition", () => "在CoralCat的世界中合成");
        }

        public override void Unload()
        {
            craftCondition = null;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BladeofGrass);
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.shootSpeed = 1.5f;
            Item.shoot = ModContent.ProjectileType<BladeOfCatnipProj>(); // The projectile is what makes a shortsword work
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector55 = player.MountedCenter + (new Vector2(70f, -40f) * player.Directions);
            int npcTargetIndex3;
            bool zenithTarget2 = GetZenithTarget(vector55, 150f, out npcTargetIndex3);
            if (zenithTarget2)
            {
                NPC nPC4 = Main.npc[npcTargetIndex3];
                vector55 = Main.rand.NextVector2FromRectangle(nPC4.Hitbox);
            }
            else
            {
                vector55 += Main.rand.NextVector2Circular(20f, 20f);
            }

            Vector2 vector56 = player.Center + (new Vector2(Main.rand.NextFloatDirection() * player.width / 2f, player.height / 2) * player.Directions);
            Vector2 v6 = vector55 - vector56;
            float num175 = ((float)Math.PI + ((float)Math.PI * 2f * Main.rand.NextFloat() * 1.5f)) * (-player.direction * player.gravDir);
            int num176 = 60;
            float num177 = num175 / num176;
            float num178 = 16f;
            float num179 = v6.Length();
            if (Math.Abs(num177) >= 0.17f)
                num177 *= 0.7f;

            Vector2 vector57 = Vector2.UnitX * num178;
            Vector2 v7 = vector57;
            int num180 = 0;
            while (v7.Length() < num179 && num180 < num176)
            {
                num180++;
                v7 += vector57;
                vector57 = vector57.RotatedBy(num177);
            }

            float num181 = v7.ToRotation();
            Vector2 spinningpoint2 = v6.SafeNormalize(Vector2.UnitY).RotatedBy(0f - num181 - num177) * num178;
            if (num180 == num176)
                spinningpoint2 = new Vector2(player.direction, 0f) * num178;

            if (!zenithTarget2)
            {
                vector56.Y -= player.gravDir * 24f;
                spinningpoint2 = spinningpoint2.RotatedBy(player.direction * player.gravDir * ((float)Math.PI * 2f) * 0.14f);
            }

            Projectile.NewProjectile(source, vector56, spinningpoint2, ProjectileID.BladeOfGrass, (int)(damage * 0.25f), knockback, player.whoAmI, num177, num180);
            return true;
        }

        private bool GetZenithTarget(Vector2 searchCenter, float maxDistance, out int npcTargetIndex)
        {
            npcTargetIndex = 0;
            int? num = null;
            float num2 = maxDistance;
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(this))
                {
                    float num3 = searchCenter.Distance(nPC.Center);
                    if (!(num2 <= num3))
                    {
                        num = i;
                        num2 = num3;
                    }
                }
            }

            if (!num.HasValue)
                return false;

            npcTargetIndex = num.Value;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Stinger, 12)
                .AddIngredient(ItemID.JungleSpores, 15)
                .AddIngredient(ItemID.Vine, 3)
                .AddTile(TileID.Anvils)
                .AddCondition(craftCondition, () => CoraliteWorld.coralCatWorld)
                .Register();
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
            if (!CoraliteWorld.coralCatWorld)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Meowmere);
                Item.SetDefaults(ItemID.BladeofGrass);
            }
        }
    }

    public class BladeOfCatnipProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Melee + "BladeOfCatnip";

        // The "width" of the blade
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(46); // This sets width and height to the same value (important when projectiles can rotate)
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

            const int TotalDuration = 18;

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

            int num7 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.JunglePlants, (Projectile.velocity.X * 0.2f) + (Main.player[Projectile.owner].direction * 3),
                Projectile.velocity.Y * 0.2f, 0, default, 1.2f);
            Main.dust[num7].noGravity = true;

            // The code in this method is important to align the sprite with the hitbox how we want it to
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            const int HalfSpriteWidth = 46 / 2;
            const int HalfSpriteHeight = 46 / 2;

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
            if (Main.rand.NextBool(4))
                target.AddBuff(20, 420);
        }
    }
}
