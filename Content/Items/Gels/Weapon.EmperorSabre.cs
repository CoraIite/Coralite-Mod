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
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    /// <summary>
    /// 下挥=>下挥=》横挥=》上挥=》反横挥=》重下挥=》重上挥=》指挥
    /// </summary>
    public class EmperorSabre : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 30;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.shoot = ProjectileType<EmperorSabreSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.expert = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 6)
                {
                    useCount = 0;
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSummon>(), (int)(damage * 1.35f), knockback, player.whoAmI);

                    return false;
                }

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                    case 3:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 5:
                    case 6: //和上面一样，只是增加了伤害
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSlash>(), (int)(damage * 1.35f), knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                    case 4:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EmperorSabreSlash2>(), damage, knockback, player.whoAmI, useCount);
                        Helper.PlayPitched("Misc/Slash", 0.4f, 0f, player.Center);
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

    public class EmperorSabreSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public EmperorSabreSlash() : base(new Vector2(56, 64).ToRotation() - 0.1f, trailLength: 48) { }

        public int delay;
        public int alpha;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail3");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.GelItems + "EmperorSabreGradient");
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
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 1.6f;
                    totalAngle = 4.8f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 24;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 0.9f;
                    delay = 24;
                    break;
                case 1://下挥，圆
                    startAngle = 2.2f;
                    totalAngle = 3.4f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 12;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 44;
                    break;
                case 3://上挥 较圆
                    startAngle = -1.6f;
                    totalAngle = -4.6f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 15;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 12;
                    break;
                case 5: //重下挥
                    startAngle = 1.2f;
                    totalAngle = 5.2f;
                    minTime = 66;
                    onHitFreeze = 20;
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 70;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 38;
                    Projectile.scale = 0.8f;
                    break;
                case 6://重上挥
                    startAngle = -1.8f;
                    totalAngle = -5f;
                    minTime = 26;
                    onHitFreeze = 20;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 36;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 38;
                    break;
            }

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            if (Combo == 5)
            {
                Projectile.scale = MathHelper.Lerp(0.8f, 1.2f, Timer / minTime);
            }
            if (Timer < 36)
                startAngle -= Math.Sign(totalAngle) * 0.05f;
            _Rotation = startAngle;
            Slasher();
            if ((int)Timer == minTime)
            {
                Helper.PlayPitched("Misc/Slash", 0.4f, 0f, Owner.Center);
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            switch ((int)Combo)
            {
                default:
                case 0:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1.2f);
                    break;
                case 1:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    break;
                case 3:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1f);
                    break;
                case 5:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.4f, 1.6f);
                    break;
                case 6:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.6f, 1.4f);
                    break;
            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
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

    public class EmperorSabreSlash2 : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre2";

        public EmperorSabreSlash2() : base(0, 48) { }

        public ref float Combo => ref Projectile.ai[0];

        public int alpha;

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 36;
            Projectile.width = 60;
            Projectile.height = 80;
            distanceToOwner = 2;
            minTime = 0;
            onHitFreeze = 4;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 75 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 2:
                    startAngle = 1.4f;
                    totalAngle = 4.5f;
                    minTime = 12;
                    Projectile.scale = 0.8f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 24;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
                case 4:
                    startAngle = -2.8f;
                    totalAngle = -5.6f;
                    minTime = 0;
                    maxTime = Owner.itemTimeMax + 24;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = 0.9f;
                    break;
            }

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            switch (Combo)
            {
                default:
                case 2:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.4f - 4f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 2.2f);
                    break;
                case 4:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.8f - 5.6f * Smoother.Smoother(timer, maxTime - minTime), 0.9f, 2.6f);
                    break;
            }

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;
            Slasher();
            if (Timer > maxTime + 12)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            float originWidth = totalAngle > 0 ? 0 : mainTex.Width;
            Main.spriteBatch.Draw(mainTex, Bottom - Main.screenPosition, mainTex.Frame(),
                                                lightColor, Projectile.rotation + extraRot, new Vector2(originWidth, origin.Y), new Vector2(Projectile.scale, 1), CheckEffect(), 0f);
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

                if (Combo == 4)
                {
                    strength = 4;
                    baseScale = 2;
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
                //float trailWidth = ControlTrailBottomWidth(factor) + trailTopWidth;

                //Vector2 current = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i]);  //当前的拖尾中心点
                //Vector2 next;   //下一个点
                //if (i == oldRotate.Length - 1)
                //    next=current+current- Center + oldRotate[i -1].ToRotationVector2() * (oldLength[i - 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i - 1]);
                //else
                //    next = Center + oldRotate[i + 1].ToRotationVector2() * (oldLength[i + 1] + trailTopWidth - trailWidth / 2 + oldDistanceToOwner[i + 1]);

                //Vector2 normalToNext = (next - current).SafeNormalize(Vector2.Zero);    
                //Vector2 normalPerp = normalToNext.RotatedBy(MathHelper.PiOver2);  //法线
                //trailWidth /= 2;
                //Vector2 Top = current + (normalPerp * trailWidth);
                //Vector2 Bottom = current- (normalPerp * trailWidth);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.3f + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                //var w = Helper.Lerp(0.5f, 0.05f, factor);
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
                effect.Parameters["sampleTexture"].SetValue(EmperorSabreSlash.trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(EmperorSabreSlash.GradientTexture.Value);

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

    public class EmperorSabreSummon : BaseSwingProj
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre";

        public EmperorSabreSummon() : base(new Vector2(56, 64).ToRotation() - 0.1f) { }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
        }

        protected override float GetStartAngle()
        {
            return (Owner.direction > 0 ? 0 : MathHelper.Pi);
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 2;
            startAngle = 1.4f;
            totalAngle = 1.5f;
            minTime = 26;
            Projectile.scale = 0.9f;
            maxTime = (int)(Owner.itemTimeMax * 0.8f) + 24;
            Smoother = Coralite.Instance.SqrtSmoother;

            base.Initializer();
            Projectile.rotation = _Rotation + MathHelper.Pi;
        }

        protected override void BeforeSlash()
        {
            startAngle -= Math.Sign(totalAngle) * 0.03f;
            _Rotation = startAngle;
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = Owner.Center + RotateVec2 * (Projectile.scale * Projectile.height / 2 + distanceToOwner);
            Projectile.rotation = Projectile.rotation.AngleTowards(_Rotation, 0.1f);
        }

        protected override void OnSlash()
        {
            Projectile.scale += 0.02f;
            if ((int)Timer == maxTime && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One) * 8,
                    ProjectileType<GelSpawner>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + 34)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
    }

    public class GelSpawner : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.SpawnTrailDust(DustID.t_Slime, Main.rand.NextFloat(0.1f, 0.3f),
                Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void OnKill(int timeLeft)
        {
            SpawnGelChaser();
        }

        private void SpawnGelChaser()
        {
            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, Projectile.Center);
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            int damage = (int)(Projectile.damage * 0.8f);

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + dir * Main.rand.NextFloat(60, 80),
                    dir * Main.rand.NextFloat(2, 4), ProjectileType<GelChaser>(), damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.Center.X, ai2: Projectile.Center.Y);
                rot += Main.rand.NextFloat(MathHelper.PiOver2 - 0.3f, MathHelper.PiOver2 + 0.3f);
            }

            if (VisualEffectSystem.HitEffect_Dusts)
                Helper.SpawnRandomDustJet(Projectile.Center, 6, 6, (j) => j * 2f, DustID.t_Slime,
                    150, new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1.5f, 2f));
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>
    /// 使用ai1,ai2传入目标位置
    /// </summary>
    public class GelChaser : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SmallGelBall";

        protected float Scale;

        protected ref float State => ref Projectile.ai[0];
        protected Vector2 Center
        {
            get => new Vector2(Projectile.ai[1], Projectile.ai[2]);
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                      Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.2f, 0.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.SpawnTrailDust(DustID.t_Slime, Main.rand.NextFloat(0.1f, 0.3f),
                Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));

            switch ((int)State)
            {
                default:
                case 0:
                    Scale += 0.05f;//膨胀小动画
                    if (Scale > 1)
                    {
                        Scale = 1f;
                        State = 1;
                    }

                    Projectile.rotation += 0.1f;
                    Projectile.velocity *= 0.99f;
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
                    break;
                case 1:
                    Projectile.velocity = (Center - Projectile.Center).SafeNormalize(Vector2.One) * 10;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    State++;
                    break;
                case 2:
                    Projectile.localAI[2]++;
                    if (Projectile.localAI[2] > 120)
                        Projectile.Kill();

                    if (Vector2.Distance(Projectile.Center, Center) < 16)
                    {
                        State++;
                        Projectile.velocity *= 0;
                        Projectile.localAI[2] = 0;
                    }
                    break;
                case 3:
                    Projectile.localAI[2]++;
                    if (Projectile.localAI[2] > 4)
                        Projectile.Kill();
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target) => State > 1 && !target.friendly;

        public override void OnKill(int timeLeft)
        {
            //生成粒子
            if (VisualEffectSystem.HitEffect_Dusts)
                Helper.SpawnDirDustJet(Projectile.Center, () => -Projectile.velocity.SafeNormalize(Vector2.Zero), 6, 6, (j) => j * 1f, DustID.t_Slime,
                    150, new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.5f), extraRandRot: 0.4f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            var pos = Projectile.Center - Main.screenPosition;
            Color color = lightColor * Scale;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Scale, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(50, 152 + (int)(100 * factor), 225);
            color *= 0.75f;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.3f, 0.03f, 1, 8, 2, Scale, frameBox);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Scale, 0, 0);

            return false;
        }

    }
}
