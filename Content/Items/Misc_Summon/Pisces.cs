using Coralite.Content.Items.Crimson;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Summon
{
    public class Pisces : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        private byte shootCount;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 50;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<PiscesSwing>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 12;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int rightType = ModContent.ProjectileType<PiscesRightSwing>();
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == player.whoAmI && proj.type == rightType && (int)proj.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit)
                    {
                        proj.StartAttack();
                        proj.ai[2] = (int)BaseSilkKnifeSpecialProj.AIStates.drag;
                        proj.netUpdate = true;
                        return false;
                    }
                }

                Projectile.NewProjectile(source, position, Vector2.Zero, rightType, damage, knockback, player.whoAmI, shootCount);

                return false;
            }

            if (shootCount < 2)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage * 2, knockback * 1.5f, player.whoAmI, shootCount);
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Stars + "PiscesSwing", 0.25f, -0.07f, 1, 5, player.Center);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 2.5f, ModContent.ProjectileType<PiscesSpurt>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PiscesMeteor>(), damage, knockback, player.whoAmI);

                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Stars + "PiscesSpurt", 0.4f, 0, 1, 3, player.Center);
            }

            shootCount++;
            if (shootCount > 2)
                shootCount = 0;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ReinforcedFishingPole)
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.ReinforcedFishingPole)
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.Misc_Summon)]
    public class PiscesSwing : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.Misc_Summon + "Pisces";

        public ref float Combo => ref Projectile.ai[0];

        public Particle chainParticle;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public int delay;
        public int alpha;

        [VaultLoaden("{@classPath}" + "PiscesGradient")]
        public static ATex GradientTexture { get; set; }

        public PiscesSwing() : base(0.785f, 24) { }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 40;
            Projectile.height = 95;
            trailTopWidth = 0;
            distanceToOwner = 12;
            onHitFreeze = 8;
            Projectile.hide = true;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 50 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0:
                    startAngle = 1.7f;
                    totalAngle = 4f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.4f);
                    break;
                case 1:
                    startAngle = 2.4f;
                    totalAngle = 3.5f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 18;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.4f);

                    break;
            }

            base.InitializeSwing();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.25f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            bool preSwing = timer < maxTime * 0.4f;
            int timePer = Projectile.MaxUpdates;
            if (preSwing)
                timePer = 2;

            if (Main.rand.NextBool(3) || timer % timePer == 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Projectile.height * 0.4f) + Main.rand.NextVector2Circular(12, 12)
                    , DustID.Clentaminator_Cyan, RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 2f)
                    , Scale: Main.rand.NextFloat(0.5f, 1f));

                d.noGravity = true;
            }

            if (timer % (Projectile.MaxUpdates) == 0 && preSwing)
            {
                var p = PRTLoader.NewParticle<StarChain>(Projectile.Center + (RotateVec2 * Projectile.height * 0.45f) + Main.rand.NextVector2CircularEdge(18, 18), Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 0.6f), Color.Cyan, 0.01f);
                if (chainParticle != null)
                    p.ChainedParticle = chainParticle;

                p.Alpha = 0.9f;
                p.TargetScale = 0.8f;
                p.ShineTime = 4;
                p.FadeTime = 6;
                p.LineWidth = 20;
                p.FollowPlayer = Owner;
                chainParticle = p;
            }

            if (alpha < 255)
                alpha += 8;

            if (Item.type == ModContent.ItemType<Pisces>())
                scale = Owner.GetAdjustedItemScale(Item);
            else
                Projectile.Kill();

            float angle = recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime));

            Projectile.scale = scale * Helper.EllipticalEase(angle, 0.9f, 1.4f);

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (DownRight)
            {
                Projectile.Kill();
                Owner.itemAnimation = Owner.itemTime = 0;
                return;
            }
            if (alpha > 20)
                alpha -= 10;

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Timer < minTime || (Timer > maxTime && Combo < 3))
                return false;

            if (target.noTileCollide || target.friendly || Projectile.hostile)
                return null;

            if (Collision.CanHit(Owner, target))
                return null;

            return false;
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);

            if (onHitTimer != 1 || !VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            Owner.MinionAttackTargetNPC = target.whoAmI;
            SpawnPiscesFish(target, damageDone, Owner, Projectile);
            PiscesStrikeParticle(target);
        }

        /// <summary>
        /// 特殊命中,生成双鱼
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damageDone"></param>
        /// <param name="Owner"></param>
        /// <param name="projectile"></param>
        public static void SpawnPiscesFish(NPC target, int damageDone, Player Owner, Projectile projectile)
        {
            if (target.life - damageDone < 0 || !target.HasBuff<PiscesTag>())
                return;

            target.DelBuff(target.FindBuffIndex(ModContent.BuffType<PiscesTag>()));

            int projType = ModContent.ProjectileType<PiscesMinion>();

            if (Owner.ownedProjectileCounts[projType] < 2)
            {
                foreach (var proj in Main.ActiveProjectiles)
                    if (proj.owner == Owner.whoAmI && proj.type == projType)
                        proj.Kill();

                Vector2 dir = (Owner.Center - target.Center).SafeNormalize(Vector2.Zero);
                int damage = Owner.GetWeaponDamage(Owner.HeldItem) / 2;

                for (int i = -1; i < 2; i += 2)
                    projectile.NewProjectileFromThis<PiscesMinion>(target.Center, dir.RotatedBy(i * 0.6f) * 12, damage, 4, target.whoAmI);
            }
            else
            {
                foreach (var proj in Main.ActiveProjectiles)
                    if (proj.owner == Owner.whoAmI && proj.type == projType)
                        proj.timeLeft = 60 * 10;
            }
        }

        public static void PiscesStrikeParticle(NPC target)
        {
            Vector2 center = Main.rand.NextVector2FromRectangle(target.getRect());
            var p = PRTLoader.NewParticle<StarChain>(center, Vector2.Zero, Color.Cyan, 0.01f);

            p.Alpha = 0.9f;
            p.TargetScale = 0.7f;
            p.ShineTime = 1;
            p.FadeTime = 10;
            p.LineWidth = 20;

            int count = Main.rand.Next(3, 6);
            for (int i = 0; i < count; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                var p2 = PRTLoader.NewParticle<StarChain>(center + dir * 24, dir * Main.rand.NextFloat(1, 2), Color.Cyan, 0.01f);

                p2.Alpha = 0.8f;
                p2.TargetScale = 0.5f;
                p2.ShineTime = 4;
                p2.FadeTime = 7;
                p2.LineWidth = 32;
                p2.ChainedParticle = p;
            }

            for (int i = 0; i < 3; i++)
            {
                var l = LightTrailParticle_NoPrimitive.Spawn(center, Helper.NextVec2Dir(1f, 2f), Color.Gold, Main.rand.NextFloat(0.1f, 0.15f), Color.DarkGoldenrod with { A = 0 });
                l.noGravity = true;
            }
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var c = new Color(255, 255, 255, Helper.Lerp(alpha, 0, 1 - factor));
                bars.Add(new(Top.Vec3(), c, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), c, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }
    }

    public class PiscesSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Summon + "Pisces";

        bool hitParticle = true;
        // The "width" of the blade
        public float CollisionWidth => 20f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(100);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            const int FadeInDuration = 8;
            const int FadeOutDuration = 10;

            const int TotalDuration = 40;

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
                player.itemTime = player.itemAnimation = 2;
            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            float time = Timer - 1f;
            if (time > 20)
            {
                time = 20;
            }
            Projectile.Center = playerCenter + (Projectile.velocity * time);

            // Set spriteDirection based on moving left or right. Left -1, right 1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (MathHelper.PiOver4 * Projectile.spriteDirection);

            // The code in this method is important to align the sprite with the hitbox how we want it to
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 66 / 2;
            const int HalfSpriteHeight = 72 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
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
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            int width = Projectile.width / 2;
            Vector2 start = Projectile.Center - dir * width;
            Vector2 end = Projectile.Center + dir * width;
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            if (hitParticle)
            {
                hitParticle = false;
                PiscesSwing.PiscesStrikeParticle(target);
            }
        }
    }

    public class PiscesMeteor : BaseHeldProj
    {
        public override string Texture => "Terraria/Images/Projectile_16";

        public Particle chainParticle;

        public const int trailCachesLength = 16;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 1;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(trailCachesLength);
            Projectile.InitOldRotCache(trailCachesLength);
        }

        public override void AI()
        {
            if (VaultUtils.isServer)
                return;

            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();

            Projectile.SpawnTrailDust(8f, DustID.Clentaminator_Cyan, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft % (Projectile.MaxUpdates * 3) == 0)
            {
                var p = PRTLoader.NewParticle<StarChain>(Projectile.Center + Main.rand.NextVector2CircularEdge(24, 24), Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 0.6f), Color.Cyan, 0.01f);
                if (chainParticle != null)
                    p.ChainedParticle = chainParticle;

                p.Alpha = 0.9f;
                p.TargetScale = 0.7f;
                p.ShineTime = 4;
                p.FadeTime = 4;
                p.LineWidth = 20;
                chainParticle = p;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.MinionAttackTargetNPC = target.whoAmI;
            PiscesSwing.SpawnPiscesFish(target, damageDone, Owner, Projectile);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTextureValue();
            Color c = Color.White;
            c.A = 0;

            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, 0.9f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.7f, MathHelper.PiOver4, origin, 0.7f, 0, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleSPA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * 9 * factor);
                Vector2 Bottom = Center - (normal * 9 * factor);

                var Color = new Color(20, 255, 199, 0) * factor;
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }

    [VaultLoaden(AssetDirectory.Misc_Summon)]
    public class PiscesRightSwing() : BaseSilkKnifeSpecialProj(16 * 35, 0, 20, 20)
    {
        [VaultLoaden("{@classPath}" + "PiscesChain")]
        public static ATex LineTex { get; set; }

        public override string Texture => AssetDirectory.Misc_Summon + "PiscesHook";

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 20;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void Dragging()
        {
            if ((int)Timer == 0)
            {
                canDamage = true;
                Owner.AddImmuneTime(ImmunityCooldownID.General, 10);
                Projectile.StartAttack();
                Timer++;
                return;
            }

            if ((int)Timer < 6)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter, 0.25f);
            }
            else
            {
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Stars + "PiscesSpurt", 0.4f, 0, 1, 3, Projectile.Center);
                Projectile.Kill();
            }

            Timer++;
        }

        public override void OnHitNPC_Draging(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.boss && target.knockBackResist != 0)
            {
                float speed = Helper.Clamp((Owner.MountedCenter - target.Center).Length() / 8, 0, 25);
                target.velocity = (Owner.MountedCenter - target.Center).SafeNormalize(Vector2.Zero) * speed * target.knockBackResist;
            }

            target.AddBuff(ModContent.BuffType<PiscesTag>(), 60 * 10);
        }

        public override void RollingInHand()
        {
            SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.hide = false;
            Projectile.Center = Owner.Center + (dir * 64);
            Projectile.velocity = dir * shootSpeed;
            Projectile.rotation = dir.ToRotation();
            HookState = (int)AIStates.shoot;
            Projectile.tileCollide = true;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (HookState == (int)AIStates.onHit)
            {
                Vector2 handlePos = Projectile.Center;
                Vector2 stringTipPos = Owner.MountedCenter + new Vector2(Owner.direction * 60, -60);
                float dis = Vector2.Distance(handlePos, stringTipPos);
                Vector2 middlePos = Vector2.Lerp(stringTipPos + new Vector2(0, 48), handlePos, Math.Clamp(dis / 600, 0, 0.5f));

                int LinePointCount = (int)(dis / 48) + 2;

                StarChain chainParticle = null;
                for (int i = 0; i < LinePointCount + 1; i++)
                {
                    float factor = (float)i / LinePointCount;

                    Vector2 P1 = Vector2.Lerp(handlePos, middlePos, factor);
                    Vector2 P2 = Vector2.Lerp(middlePos, stringTipPos, factor);

                    Vector2 Center = Vector2.Lerp(P1, P2, factor);

                    var p = PRTLoader.NewParticle<StarChain>(Center + Main.rand.NextVector2CircularEdge(22, 22), Helper.NextVec2Dir() * Main.rand.NextFloat(0.2f, 0.6f), Color.Cyan, 0.01f);
                    if (chainParticle != null)
                        p.ChainedParticle = chainParticle;

                    p.Alpha = 0.9f;
                    p.TargetScale = 0.8f;
                    p.ShineTime = 4 + i ;
                    p.FadeTime = 6;
                    p.LineWidth = 20;
                    chainParticle = p;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制连线
            Texture2D stringTex = LineTex.Value;
            float rot = 0;

            float halfLineWidth = stringTex.Height / 2;

            List<ColoredVertex> bars = new();

            Vector2 handlePos = Projectile.Center - Main.screenPosition;
            Vector2 recordPos = Projectile.Center - Main.screenPosition;
            Vector2 stringTipPos = Owner.MountedCenter + new Vector2(Owner.direction * 60, -60) - Main.screenPosition;

            float dis = Vector2.Distance(handlePos, stringTipPos);
            Vector2 middlePos = Vector2.Lerp(stringTipPos + new Vector2(0, 48), handlePos, Math.Clamp(dis / 600, 0, 0.5f));

            float recordUV = 0;

            //贝塞尔曲线
            int LinePointCount = (int)(dis / 8) + 2;

            for (int i = 0; i < LinePointCount + 1; i++)
            {
                float factor = (float)i / LinePointCount;

                Vector2 P1 = Vector2.Lerp(handlePos, middlePos, factor);
                Vector2 P2 = Vector2.Lerp(middlePos, stringTipPos, factor);

                Vector2 Center = Vector2.Lerp(P1, P2, factor);

                Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                Vector2 Top = Center + normal * halfLineWidth;
                Vector2 Bottom = Center - normal * halfLineWidth;

                recordUV += (Center - recordPos).Length() / stringTex.Width;
                Color c = Color.White * (0.2f + 0.8f * (1 - factor));

                bars.Add(new(Top, c, new Vector3(recordUV, 0, 1)));
                bars.Add(new(Bottom, c, new Vector3(recordUV, 1, 1)));

                if (i == 2)
                    rot = (Center - recordPos).ToRotation() + MathHelper.Pi;
                recordPos = Center;
            }

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = stringTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;

            //绘制钩子
            Texture2D projTex = Projectile.GetTextureValue();
            Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, null, Color.White, rot, new Vector2(4, projTex.Height / 2), Projectile.scale, 0, 0);

            //绘制钓竿
            Texture2D ItemTex = TextureAssets.Item[ModContent.ItemType<Pisces>()].Value;
            ItemTex.QuickCenteredDraw(Main.spriteBatch, Owner.MountedCenter + new Vector2(Owner.direction * ItemTex.Width / 2, -ItemTex.Height / 2 + 8) - Main.screenPosition, Color.White, effect: Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }

    public class PiscesTag : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(npc.getRect()), DustID.Clentaminator_Cyan, Helper.NextVec2Dir(0.2f, 0.3f));
            }
        }
    }

    public class PiscesMinion : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        private Player Owner => Main.player[Projectile.owner];

        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 60 * 10;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC target))
            {
                Timer = 0;

                int index;
                if (Owner.HasMinionAttackTargetNPC)
                    index = Owner.MinionAttackTargetNPC;
                else
                    index = Projectile.MinionFindTarget();
                Target = index;

                Timer = 0;
                if (Target == -1 || Vector2.Distance(Projectile.Center, Owner.Center) > 2400)
                    Projectile.Kill();

                Projectile.friendly = true;
                return;
            }

            Projectile.SpawnTrailDust(ModContent.DustType<PiscesMinionDust>(), Main.rand.NextFloat(0.2f, 0.4f), 0, Color.White);

            if (Timer < 3)
            {
                Projectile.ChaseGradually(target.Center, 17, 14, 15);
                if (Vector2.Distance(Projectile.Center, target.Center) < 32)
                    Timer++;
            }
            else if (Timer < 15 + 3)
            {
                Timer++;

                Projectile.velocity = Projectile.velocity.RotateByRandom(0.01f, 0.15f);
                if (Projectile.velocity.Length() < 1)
                    Projectile.velocity += Vector2.UnitX;
            }
            else
            {
                int index;
                if (Owner.HasMinionAttackTargetNPC)
                    index = Owner.MinionAttackTargetNPC;
                else
                    index = Projectile.MinionFindTarget();

                Target = index;
                Timer = 0;
                Projectile.friendly = true;
                if (Target == -1 || Vector2.Distance(Projectile.Center, Owner.Center) > 2400)
                    Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateFrameNormally(3, 3);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PRTLoader.NewParticle<StarRot>(target.Center, Helper.NextVec2Dir(1, 4), Color.LightCyan, Main.rand.NextFloat(0.4f, 0.6f));
            Projectile.friendly = false;
            if (Timer < 3)
                Timer = 3;

            int dustType = ModContent.DustType<PiscesMinionDust>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, dustType, dir * (1 + (j * 0.8f)), newColor: Color.White, Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            var p = PRTLoader.NewParticle<StarChain>(center, Vector2.Zero, Color.Cyan, 0.01f);

            p.Alpha = 0.9f;
            p.TargetScale = 0.7f;
            p.ShineTime = 1;
            p.FadeTime = 10;
            p.LineWidth = 20;

            int count = Main.rand.Next(2, 5);
            for (int i = 0; i < count; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                var p2 = PRTLoader.NewParticle<StarChain>(center + dir * 24, dir * Main.rand.NextFloat(0.5f, 1.5f), Color.Cyan, 0.01f);

                p2.Alpha = 0.8f;
                p2.TargetScale = 0.5f;
                p2.ShineTime = 4;
                p2.FadeTime = 7;
                p2.LineWidth = 32;
                p2.ChainedParticle = p;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Projectile.DrawShadowTrails(Color.White, 0.3f, 0.3f / 7, 1, 7, 1, Projectile.scale, frameBox, 0);

            Projectile.QuickDraw(frameBox, Color.White, 0);

            return false;
        }
    }

    public class PiscesMinionDust : ModDust
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override void SetStaticDefaults()
        {
            UpdateType = DustID.Clentaminator_Cyan;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(Main.rand.Next(3), Main.rand.Next(2), 3, 2);
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.frame, dust.position - Main.screenPosition, dust.color, dust.rotation, dust.scale);

            return false;
        }
    }
}
