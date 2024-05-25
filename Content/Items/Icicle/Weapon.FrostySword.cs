using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    //0下挥=》1横挥=》2下挥=》3上挥=》4反横挥
    //右键使用蓄力攻击，蓄力伤害根据左键命中次数决定
    public class FrostySword : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        private int useCount;
        public int leftHitCount;

        public const int LeftHitMax = 12;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 60;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<FrostySwordSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<FrostySwordRightSlash>(), (int)(damage * 0.75f), knockback, player.whoAmI, leftHitCount);
                    useCount = 0;
                    if (leftHitCount >= LeftHitMax)
                        leftHitCount = 0;
                    return false;
                }

                switch (useCount)
                {
                    default:
                    case 0:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<FrostySwordSlash>(), (int)(damage * 0.8f), knockback, player.whoAmI, useCount);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<FrostySwordSlash2>(), (int)(damage * 0.8f), knockback, player.whoAmI, useCount);
                        Helper.PlayPitched("Misc/Slash", 0.4f, 0f, player.Center);
                        break;
                    case 2:
                    case 3:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<FrostySwordSlash>(), (int)(damage * 1.2f), knockback, player.whoAmI, useCount);
                        break;
                    case 4:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<FrostySwordSlash2>(), (int)(damage * 1.2f), knockback, player.whoAmI, useCount);
                        Helper.PlayPitched("Misc/Slash", 0.4f, 0f, player.Center);
                        break;
                }
            }

            useCount++;
            if (useCount > 4)
                useCount = 0;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostCrystal>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FrostySwordSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.IcicleItems + "FrostySword";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public FrostySwordSlash() : base(MathHelper.PiOver4, trailLength: 48) { }

        public int delay;
        public int alpha;

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 100;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail3");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.IcicleItems + "FrostSwordGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            WarpTexture = null;
            GradientTexture = null;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 85 * Projectile.scale;
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
                case 2: //重下挥
                    startAngle = 1.2f;
                    totalAngle = 5.6f;
                    minTime = 66;
                    onHitFreeze = 20;
                    maxTime = (int)(Owner.itemTimeMax) + 70;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 28;
                    Projectile.scale = 0.8f;
                    break;
                case 3://上挥 较圆
                    startAngle = -1.6f;
                    totalAngle = -4.6f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 15;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 12;
                    break;
            }

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Owner.Center, Coralite.Instance.IcicleCyan.ToVector3());
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            if (Combo == 2)
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
            FrostDustsOnSlash(Top, RotateVec2, totalAngle);

            switch ((int)Combo)
            {
                default:
                case 0:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1.2f);
                    break;
                case 2:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
                    Projectile.scale = Helper.EllipticalEase(3f - 5.6f * Smoother.Smoother(timer, maxTime - minTime), 1.1f, 1.6f);
                    break;
                case 3:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.6f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.1f, 1.6f);
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

                float strength = 1;
                float baseScale = 4;

                if (Combo == 2)
                {
                    strength = 4;
                    baseScale = 7;
                }

                FrostDustsOnHit(Projectile, Top, Bottom, RotateVec2, totalAngle, _Rotation, strength, baseScale);
                if (Owner.HeldItem.ModItem is FrostySword fs)
                {
                    if (fs.leftHitCount < FrostySword.LeftHitMax && target.type != NPCID.TargetDummy)
                    {
                        fs.leftHitCount++;
                        IceStarLight.Spawn(Top + RotateVec2 * 70, RotateVec2.RotatedBy(Main.rand.NextFromList(1, -1) * MathHelper.PiOver2) * Main.rand.Next(10, 14), Main.rand.NextFloat(0.6f, 1f), () => Owner.Center, 14);
                        if (fs.leftHitCount >= FrostySword.LeftHitMax)
                            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Owner.Center);
                    }
                }
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.5f);
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

                var topColor = Color.Lerp(new Color(0, 0, 0, alpha), new Color(0, 0, 0, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(0, 0, 0, alpha), new Color(0, 0, 0, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
                DrawFrostTrail(bars, originalState);
        }

        public static void FrostDustsOnSlash(Vector2 Top, Vector2 RotateVec2, float totalAngle)
        {
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            Dust dust = Dust.NewDustPerfect(Top - 24 * RotateVec2 + Main.rand.NextVector2Circular(30, 30), DustID.ApprenticeStorm,
                   dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
            dust.noGravity = true;

            if (Main.rand.NextBool(4))
            {
                dust = Dust.NewDustPerfect(Top - 14 * RotateVec2 + Main.rand.NextVector2Circular(10, 10), DustID.ApprenticeStorm,
                       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                Particle.NewParticle(Top - RotateVec2 * 16 + Main.rand.NextVector2Circular(32, 32), dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1f, 2f),
                    CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.15f, 0.4f));
            }

        }

        public static void FrostDustsOnHit(Projectile Projectile, Vector2 Top, Vector2 Bottom, Vector2 RotateVec2, float totalAngle, float _Rotation, float strength, float baseScale)
        {
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
                float Rot = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
                dust = Dust.NewDustPerfect(pos, DustType<FrostStrikeDust>(),
                    Scale: Main.rand.NextFloat(baseScale, baseScale * 1.2f));
                dust.rotation = Rot;

                for (int i = -1; i < 2; i += 2)
                {
                    dust = Dust.NewDustPerfect(pos, DustType<FrostStrikeDust>(),
                             Scale: Main.rand.NextFloat(baseScale * 0.3f, baseScale * 0.4f));
                    dust.rotation = Rot + i * 0.5f;
                }
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int j = 0; j < 6; j++)
                {
                    Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                    dust = Dust.NewDustPerfect(pos, DustID.ApprenticeStorm, dir * Main.rand.NextFloat(1f, 5f), Scale: Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                    dust = Dust.NewDustPerfect(pos, DustID.Frost, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1.5f, 2f));
                    dust.noGravity = true;
                }
            }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                Particle p = Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<Strike>(), new Color(129, 216, 243), Main.rand.NextFloat(0.75f, 1.2f));
                p.Rotation = _Rotation + 2.2f;
            }
        }

        public static void DrawFrostTrail(List<VertexPositionColorTexture> bars, RasterizerState originalState)
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
            }

            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }

    public class FrostySwordSlash2 : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.IcicleItems + "FrostySword2";

        public FrostySwordSlash2() : base(0, 48) { }

        public ref float Combo => ref Projectile.ai[0];

        public int alpha;
        public int delay;

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 36;
            Projectile.width = 40;
            Projectile.height = 100;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 4;
            trailTopWidth = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 95 * Projectile.scale;
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
                case 1:
                    startAngle = 1.4f;
                    totalAngle = 4.3f;
                    minTime = 12;
                    Projectile.scale = 0.8f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 24;
                    delay = 24;
                    Projectile.scale = 0.7f;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
                case 4:
                    startAngle = -2.8f;
                    totalAngle = -5.6f;
                    minTime = 0;
                    maxTime = Owner.itemTimeMax + 24;
                    delay = 18;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = 0.9f;
                    break;
            }

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Owner.Center, Coralite.Instance.IcicleCyan.ToVector3());
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            FrostySwordSlash.FrostDustsOnSlash(Top, RotateVec2, totalAngle);

            switch (Combo)
            {
                default:
                case 1:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 200) + 50;
                    Projectile.scale = Helper.EllipticalEase(-1.4f + 4.3f * Smoother.Smoother(timer, maxTime - minTime), 0.6f, 1.5f);
                    //Main.NewText(Projectile.scale);
                    break;
                case 4:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 200) + 50;
                    Projectile.scale = Helper.EllipticalEase(2.8f - 5.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 2.5f);
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

                float strength = 1;
                float baseScale = 4;

                if (Combo == 4)
                {
                    strength = 4;
                    baseScale = 7;
                }

                FrostySwordSlash.FrostDustsOnHit(Projectile, Top, Bottom, RotateVec2, totalAngle, _Rotation, strength, baseScale);

                if (Owner.HeldItem.ModItem is FrostySword fs)
                {
                    if (fs.leftHitCount < FrostySword.LeftHitMax && target.type != NPCID.TargetDummy)
                    {
                        fs.leftHitCount++;
                        IceStarLight.Spawn(Top + RotateVec2 * 70, RotateVec2.RotatedBy(Main.rand.NextFromList(1, -1) * MathHelper.PiOver2) * Main.rand.Next(10, 14), Main.rand.NextFloat(0.6f, 1f), () => Owner.Center, 14);
                        if (fs.leftHitCount >= FrostySword.LeftHitMax)
                            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Owner.Center);
                    }
                }
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.5f);
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
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.3f + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
                FrostySwordSlash.DrawFrostTrail(bars, originalState);
        }
    }

    public class FrostySwordRightSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.IcicleItems + "FrostySword";

        public ref float Count => ref Projectile.ai[0];
        private int howmany;
        public int alpha;

        public FrostySwordRightSlash() : base(MathHelper.PiOver4, trailLength: 48) { }

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 100;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;

            startAngle = 1.2f;
            totalAngle = 5.6f;
            minTime = 46;
            onHitFreeze = 8;
            maxTime = (int)(Owner.itemTimeMax * 0.8f) + 60;
            Smoother = Coralite.Instance.NoSmootherInstance;
            Projectile.scale = 0.8f;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 95 * Projectile.scale;
        }

        protected override void BeforeSlash()
        {
            Projectile.scale = MathHelper.Lerp(0.8f, 1f, Timer / minTime);

            if (Timer % 8 == 0 && Count > FrostySword.LeftHitMax - 1 && howmany < Count)
            {
                howmany += 3;
                IceStarLight.Spawn(Owner.Center, RotateVec2.RotatedBy((howmany % 4 == 0 ? -1 : 1) * MathHelper.PiOver2) * Main.rand.Next(10, 14), Main.rand.NextFloat(0.6f, 1f), () => Top, 14);
            }

            if (Timer < 30)
                startAngle += 0.05f;
            _Rotation = GetStartAngle() - OwnerDirection * startAngle;
            Slasher();
            if ((int)Timer == minTime)
            {
                _Rotation = startAngle = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                totalAngle *= OwnerDirection;

                //Helper.PlayPitched("Misc/Slash", 0.4f, 0f, Owner.Center);
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Projectile.Center);
                //射弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 15, ProjectileType<FrostySwordProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Count);
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            FrostySwordSlash.FrostDustsOnSlash(Top, RotateVec2, totalAngle);

            alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
            Projectile.scale = Helper.EllipticalEase(2.8f - 5.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1f);
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;
            _Rotation -= totalAngle * 0.001f;
            Slasher();
            if (Timer > maxTime + 24)
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

                FrostySwordSlash.FrostDustsOnHit(Projectile, Top, Bottom, RotateVec2, totalAngle, _Rotation, 2, 5);
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.5f);
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
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.3f + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
                FrostySwordSlash.DrawFrostTrail(bars, originalState);
        }
    }

    /// <summary>
    /// 使用ai0传入命中次数
    /// </summary>
    public class FrostySwordProj : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "IcicleMagicBall";

        //private Trail trail;
        //BasicEffect effect;

        //public FrostySwordProj()
        //{
        //    Main.QueueMainThreadAction(() =>
        //    {
        //        effect = new BasicEffect(Main.instance.GraphicsDevice);
        //        effect.VertexColorEnabled = true;
        //        effect.TextureEnabled = true;
        //        effect.Texture = Request<Texture2D>(AssetDirectory.IcicleProjectiles + "IceBurst", AssetRequestMode.ImmediateLoad).Value;
        //    });
        //}

        public ref float Count => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        //public override void OnSpawn(IEntitySource source)
        //{
        //    Projectile.oldPos = new Vector2[18];
        //    for (int i = 0; i < 18; i++)
        //        Projectile.oldPos[i] = Projectile.Center;
        //}

        public override void OnKill(int timeLeft)
        {
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);

            if (Main.myPlayer == Projectile.owner && Count > FrostySword.LeftHitMax - 1)
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, rot.ToRotationVector2() * 10, ProjectileType<FrostySwordBurst>(), (int)(Projectile.damage * 4f), Projectile.knockBack, Projectile.owner);
                    rot += MathHelper.TwoPi / 6;
                }
                Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, Projectile.Center);
            }

            for (int i = 0; i < 6; i++)
            {
                Helper.SpawnDirDustJet(Projectile.Center, () => rot.ToRotationVector2(), 1, 8, (i) => i * 0.6f, DustID.ApprenticeStorm, Scale: Main.rand.NextFloat(1.5f, 2f), noGravity: true);
                rot += MathHelper.TwoPi / 6;
            }
        }

        public override void AI()
        {
            if (Alpha < 1)
            {
                Alpha += 0.05f;
                if (Alpha > 1)
                    Alpha = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            //trail ??= new Trail(Main.instance.GraphicsDevice, 18, new TriangularTip(8), factor =>
            //{
            //    if (factor > 0.7f)
            //        return Helper.Lerp(18, 6, (factor - 0.7f) / 0.3f);

            //    return Helper.Lerp(6, 18, factor / 0.7f);
            //}, factor => Color.Lerp(Color.Transparent, Color.White, factor.X) * Alpha);

            //for (int i = 0; i < 17; i++)
            //    Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            //Projectile.oldPos[17] = Projectile.Center + Projectile.velocity;
            //trail.Positions = Projectile.oldPos;

            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.FrostStaff,
                 dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
            dust.noGravity = true;

            if (Main.rand.NextBool(3))
            {
                dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.ApprenticeStorm,
                       dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1f, 2f),
                    CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.15f, 0.4f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Projectile.DrawShadowTrails(new Color(138, 225, 249), 0.4f, 0.03f, 1, 12, 2, new Vector2(1, 1.4f), 0.02f, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, new Vector2(1, 2f), 0, 0);
            return false;
        }

        //public void DrawPrimitives()
        //{
        //    if (effect == null)
        //        return;

        //    Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
        //    Matrix view = Main.GameViewMatrix.TransformationMatrix;
        //    Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

        //    effect.World = world;
        //    effect.View = view;
        //    effect.Projection = projection;

        //    trail?.Render(effect);

        //}
    }

    public class FrostySwordBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[0];
        private bool onSpawn = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            //Projectile.usesIDStaticNPCImmunity = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        //public override bool ShouldUpdatePosition() => false;

        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    if (State <2)
        //        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.scale * 256, 60, ref Projectile.localAI[2]);

        //    return false;
        //}

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.alpha = 255;
            //Projectile.scale = 0.2f;
        }

        public override void AI()
        {
            //if (SecondaryRot < 0.1f)
            //    SecondaryRot += 0.02f;
            //if (ThirdRot < 0.2f)
            //    ThirdRot += 0.04f;
            Timer++;
            if (onSpawn)
            {
                onSpawn = false;
                Alpha = 0.5f;
            }
            //生成粒子
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.ApprenticeStorm, Projectile.velocity * Main.rand.NextFloat(0.4f, 0.6f), Scale: Main.rand.NextFloat(1, 2));
                dust.noGravity = true;
            }

            Color c = Main.rand.Next(2) switch
            {
                0 => new Color(220, 255, 255),
                1 => new Color(129, 216, 243),
                _ => new Color(0, 28, 59)
            };

            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustType<FrostStar>(),
                  Projectile.velocity * Main.rand.NextFloat(0.4f, 0.6f), newColor: c, Scale: Main.rand.NextFloat(1, 2));
            d.rotation = Projectile.rotation + Main.rand.Next(-3, 4) * MathHelper.Pi / 4;
            if (Timer > 10)
            {
                Alpha -= 0.02f;
                if (Timer > 20)
                    Projectile.Kill();
            }
        }

        //public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        //{
        //    //绘制5个光束
        //    Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
        //    Vector2 baseCemter = Projectile.Center - Main.screenPosition;
        //    Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height);
        //    Vector2 scale = new Vector2(1, Projectile.scale);
        //    float baseRot = Projectile.rotation + MathHelper.PiOver2;

        //    //绘制本体
        //    Main.spriteBatch.Draw(mainTex, baseCemter, null, new Color(240, 255, 255, Projectile.alpha), baseRot, origin, scale, 0, 0);

        //    //绘制两边次级的深蓝色
        //    for (int i = -1; i < 2; i += 2)
        //    {
        //        float trueRot = baseRot + SecondaryRot * i;
        //        Main.spriteBatch.Draw(mainTex, baseCemter + Projectile.scale * Projectile.rotation.ToRotationVector2() * 64, null,
        //            new Color(138, 225, 249, Projectile.alpha), trueRot, origin, scale * 0.6f, 0, 0);
        //    }

        //    //绘制第三级的冰蓝色
        //    for (int i = -1; i < 2; i += 2)
        //    {
        //        float trueRot = baseRot + ThirdRot * i;
        //        Main.spriteBatch.Draw(mainTex, baseCemter + Projectile.scale * Projectile.rotation.ToRotationVector2() * 10, null,
        //            new Color(0, 28, 59, Projectile.alpha), trueRot, origin, scale * 0.6f, 0, 0);
        //    }
        //}

        public override bool PreDraw(ref Color lightColor)
        {
            Helper.DrawPrettyLine(1, SpriteEffects.None, Projectile.Center - Main.screenPosition,
                new Color(0, 28, 59, 0), Color.White * 0.8f, Alpha, 0, 0.5f, 0.5f, 0, Projectile.rotation, 3, Vector2.One);
            return false;
        }
    }

    public class FrostStar : ModDust
    {
        public override string Texture => AssetDirectory.Blank;

        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn > 24)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Helper.DrawPrettyStarSparkle(1f, 0, dust.position - Main.screenPosition, new Color(40, 40, 40, 40), dust.color,
                dust.fadeIn / 24, 0, 0.5f, 0.5f, 1, dust.rotation, new Vector2(dust.scale, dust.scale), Vector2.One);
            return false;
        }
    }
}
