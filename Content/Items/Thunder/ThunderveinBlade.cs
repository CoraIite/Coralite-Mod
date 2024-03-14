using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinBlade : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 55;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<ThunderveinBladeSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 2)
                    useCount = 0;

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                        if (player.altFunctionUse == 2)
                            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), (int)(damage * 1.35f), knockback, player.whoAmI, useCount);
                        else
                            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, useCount);

                        break;
                }
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre)
        {
            return true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

    }

    public class ThunderveinBladeSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinBlade";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ThunderveinBladeSlash() : base(new Vector2(48, 46).ToRotation() - 0.08f, trailLength: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail3");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.ThunderItems + "ThunderveinBladeGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            WarpTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = -8;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 56 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 5;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 1.6f;
                    totalAngle = 3.8f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 88;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = 0.4f;
                    break;
                case 1://下挥，圆
                    startAngle = 2.2f;
                    totalAngle = 3.4f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 88;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.4f;
                    break;
                case 2://上挥 较圆
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    minTime = 24;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 95 + 24;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    break;
            }

            ExtraInit();
            base.Initializer();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = 0.7f;
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            Slasher();
            Projectile.scale = Helper.Lerp(Projectile.scale, Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), 1.2f, 1.7f), 0.1f);
            if ((int)Timer == minTime)
            {
                switch (Combo)
                {
                    default:
                    case 0:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 1:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 2:
                        Helper.PlayPitched("Misc/Slash", 0.4f, 0.4f, Owner.Center);

                        break;
                }
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Owner.HeldItem.type == ItemType<ThunderveinBlade>())
                scale = Owner.GetAdjustedItemScale(Owner.HeldItem);

            switch (Combo)
            {
                default:
                case 0:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.3f);
                    break;
                case 1:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime), 1.1f, 1.4f);
                    break;
                case 2:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1.8f);
                    break;
            }
            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 50) + 200;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Projectile.scale *= 0.99f;
            if (alpha > 20)
                alpha -= 5;
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                Owner.immuneTime += 10;
                if (Main.netMode == NetmodeID.Server)
                    return;

                float strength = 2;
                float baseScale = 1;

                if (Combo == 5 || Combo == 6)
                {
                    strength = 7;
                    baseScale = 3;
                }

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;
                if (VisualEffectSystem.HitEffect_Lightning)
                {
                    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.2f, 0.2f);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.t_Slime, dir * Main.rand.NextFloat(1f, 5f), 150, new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.ShimmerSpark, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings()
                    {
                        PositionInWorld = pos,
                        MovementVector = RotateVec2 * Main.rand.NextFloat(2f, 4f),
                    });
                //for (int i = 0; i < 3; i++)
                //{
                //    Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                //    Vector2 position = pos + Main.rand.NextVector2Circular(8, 8);
                //    for (int j = 0; j < 8; j++)
                //    {
                //        dust = Dust.NewDustPerfect(position, DustID.YellowTorch, dir.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * (j * 1.5f), 70, Scale: 3f - j * 0.3f);
                //        dust.noGravity = true;
                //    }
                //}
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / count;
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["SimpleGradientTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(1, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
