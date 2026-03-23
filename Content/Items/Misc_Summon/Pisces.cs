using Coralite.Content.Items.Misc_Melee;
using Coralite.Content.Items.ShieldPlus;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Summon
{
    public class Pisces : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

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
            Item.shootSpeed = 16;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse==2)
            {
                return false;
            }

            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI,Main.rand.Next(2));

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

        public PiscesSwing() : base(0.785f, 26) { }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
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
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 75;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.4f);
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                    break;
                case 1:
                    startAngle = 2.4f;
                    totalAngle = 3.5f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 75;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 18;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.4f);
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);

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

            if (Main.rand.NextBool(3) || timer % 3 == 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Projectile.height * 0.4f) + Main.rand.NextVector2Circular(12, 12)
                    , DustID.Clentaminator_Cyan, RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 2f)
                    , Scale: Main.rand.NextFloat(0.5f, 1f));

                d.noGravity = true;
            }

            if (timer % (Projectile.MaxUpdates ) == 0&&timer<maxTime*0.35f)
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
            Projectile.damage = (int)(Projectile.damage * 0.85f);

            if (onHitTimer != 1 || !VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
            Vector2 pos = Bottom + (RotateVec2 * offset);
            pos = Vector2.SmoothStep(pos, target.Center, 0.4f);

            float rot;

            Color c = new(50, 200, 150, 150);
            for (int i = -1; i < 2; i += 2)
            {
                rot = _Rotation + (Main.rand.NextFloat(0.7f, 1.4f) * i);

                for (int j = 0; j < 2; j++)
                {
                    LightShotParticle.Spawn(pos, c, rot + Main.rand.NextFloat(-0.2f, 0.2f)
                        , new Vector2(Main.rand.NextFloat(0.1f, 0.5f)
                        , 0.02f));
                    LightShotParticle.Spawn(pos, Color.DarkSeaGreen, rot + Main.rand.NextFloat(-0.2f, 0.2f)
                        , new Vector2(Main.rand.NextFloat(0.1f, 0.3f)
                        , 0.01f));

                    rot += MathHelper.Pi;
                }
            }

            for (int i = 0; i < 4; i++)
                LightTrailParticle_NoPrimitive.Spawn(pos, Helper.NextVec2Dir(2f, 3f), c, Main.rand.NextFloat(0.1f, 0.15f));
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
}
