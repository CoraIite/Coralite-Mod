using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
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

namespace Coralite.Content.Items.Thunder
{
    public class ThunderDagger : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.damage = 78;
            Item.useTime = Item.useAnimation = 21;
            Item.knockBack = 7;
            Item.crit = 12;
            Item.mana = 10;
            Item.shootSpeed = 3;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<LightningBall_Friendly>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.autoReuse = true;
        }

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
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderDaggerSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderDaggerSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, useCount);
                        break;
                }
            }

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            useCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(4)
                .AddIngredient<InsulationCortex>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderDaggerSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderDagger";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ThunderDaggerSlash() : base(new Vector2(38, 40).ToRotation() - 0.08f, trailCount: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.ThunderItems + "ThunderveinBladeGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarpTexture = null;
            GradientTexture = null;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hide = true;
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

        protected override float GetStartAngle()
        {
            if (Combo == 3)
                return Owner.velocity.ToRotation();
            return base.GetStartAngle();
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Combo < 3)
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
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 82;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = 0.4f;
                    break;
                case 1://下挥，圆
                    startAngle = 2.2f;
                    totalAngle = 3.4f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 82;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.4f;
                    break;
                case 2://上挥 较圆
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    minTime = 24;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 90 + 24;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    break;
            }

            ExtraInit();
            base.InitializeSwing();
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
            Projectile.scale = Helper.Lerp(Projectile.scale, Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.2f, 1.7f), 0.1f);
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

            if (Item.type == ItemType<ThunderveinBlade>())
                scale = Owner.GetAdjustedItemScale(Item);

            if (timer % 7 == 0 && timer < maxTime * 0.5f)
            {
                Vector2 pos = Projectile.Center + (RotateVec2 * Main.rand.NextFloat(-10, 40) * Projectile.scale);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.05f, 0.2f));
                }
            }

            switch (Combo)
            {
                default:
                case 0:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.3f);
                    break;
                case 1:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1.1f, 1.4f);
                    break;
                case 2:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 0.8f, 1.8f);
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
                if (VaultUtils.isServer)
                    return;

                float strength = 2;
                float baseScale = 1;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);
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
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
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

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = Filters.Scene["SimpleGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class LightningBall_Friendly : BaseThunderProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "LightingBall";

        public ThunderTrail[] circles;
        public ThunderTrail[] trails;

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 80;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.2f;
        }

        public float GetAlpha2(float factor)
        {
            return ThunderAlpha * (1 - factor);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (circles == null)
            {
                circles = new ThunderTrail[5];
                trails = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBodyF");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i < 2)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);

                    circles[i].PartitionPointCount = 3;
                    circles[i].UseNonOrAdd = true;
                    circles[i].SetRange((0, 4));
                    circles[i].SetExpandWidth(2);
                }

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Yellow, GetAlpha2);
                    trails[i].UseNonOrAdd = true;
                    trails[i].SetRange((0, 6));
                    trails[i].SetExpandWidth(4);
                }

                float cacheLength = Projectile.velocity.Length() / 2;
                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        Vector2[] vec = new Vector2[(int)cacheLength];
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 5);
                        Vector2 dir = -Projectile.velocity;
                        vec[0] = basePos;

                        for (int i = 1; i < (int)cacheLength; i++)
                        {
                            vec[i] = basePos + (dir * i);
                        }

                        trail.BasePositions = vec;
                        trail.RandomThunder();
                    }
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(15, 30);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 40)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in trails)
                trail.UpdateTrail(Projectile.velocity);

            if (Timer % 5 == 0)
            {
                float cacheLength = Projectile.velocity.Length() * 0.55f;

                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                        {
                            Vector2[] vec = new Vector2[(int)cacheLength];
                            Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 10);
                            Vector2 dir = -Projectile.velocity * 0.55f;
                            vec[0] = basePos;

                            for (int i = 1; i < (int)cacheLength; i++)
                            {
                                vec[i] = basePos + (dir * i);
                            }

                            trail.BasePositions = vec;
                            trail.RandomThunder();
                        }
                    }
                }
            }

            if (Timer % 4 == 0)
            {
                ElectricParticle_Follow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(25, 25),
                    () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * angle)).ToRotationVector2()
                                * width);
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }

            Timer++;
            if (ThunderAlpha < 1)
            {
                ThunderWidth = 8;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            if (Timer > 30)
            {
                Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                    newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.3f));
                ThunderWidth = Main.rand.NextFloat(8, 12);

                if (Projectile.velocity.Length() < 20)
                    Projectile.velocity *= 1.05f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            PRTLoader.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<LightningParticle>(), Scale: 2.5f);

            float baseRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 4; i++)
            {
                PRTLoader.NewParticle(Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                    , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Color.White, 0f);

            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (trails != null)
                foreach (var trail in trails)
                    trail.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D exTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightFog").Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = ThunderveinDragon.ThunderveinYellowAlpha;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }
}
