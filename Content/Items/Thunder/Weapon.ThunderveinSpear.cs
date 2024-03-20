using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.ModPlayers;
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

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinSpear : ModItem, IDashable
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
            Item.shoot = ProjectileType<ThunderveinSpearSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 6)
                    useCount = 0;

                switch (useCount)
                {
                    default:
                    case 0://挥舞一下
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), damage, knockback, player.whoAmI, 0);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4 ://戳
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSpurt>(), damage, knockback, player.whoAmI);
                        break;
                    case 5://挥舞
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, 1);
                        break;
                    case 6://转圈
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, 2);
                        break;
                }
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<ZapCrystal>(2)
            //    .AddTile(TileID.MythrilAnvil)
            //    .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        DashDir = (int)dashDirection;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = new Vector2(DashDir * 30, Player.velocity.Y);
            Player.direction = DashDir;
            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Player.Center);

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is ThunderveinBladeSlash)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                int damage = Player.GetWeaponDamage(Player.HeldItem);
                //Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                //    damage, Player.HeldItem.knockBack, Player.whoAmI, 3);
                //Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                //    damage, Player.HeldItem.knockBack, Player.whoAmI, 10, DashDir);
            }

            return true;
        }
    }

    public class ThunderveinSpearSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinSpearProj";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ThunderveinSpearSlash() : base(new Vector2(92, 96).ToRotation() - 0.05f, trailLength: 48) { }

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
            Projectile.height = 135;
            trailTopWidth = 0;
            distanceToOwner = -68;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 70 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner && Combo < 3)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 5;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 2f;
                    totalAngle = 4.2f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 62;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 44;
                    extraScaleAngle = -0.4f;
                    break;
                case 1://上挥 较圆
                    startAngle = -2.2f;
                    totalAngle = -4.8f;
                    minTime = 5;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 70 + 5;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    extraScaleAngle = 0.4f;

                    break;
                case 2://上挥 较圆
                    startAngle = 2.6f;
                    totalAngle = -8f;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 80 + 5;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
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
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            Slasher();
            Projectile.scale = Helper.Lerp(Projectile.scale, Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), 1f, 1.2f), 0.1f);
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
                        Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Owner.Center);
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

            if (timer % 5 == 0 && timer < maxTime * 0.7f)
            {
                Vector2 pos = Projectile.Center + RotateVec2 * Main.rand.NextFloat(20, 70) * Projectile.scale;
                if (Main.rand.NextBool())
                {
                    Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
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
                        recordStartAngle + extraScaleAngle - recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime), 1.1f, 1.3f);
                    distanceToOwner = -68 + 40 * Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime);

                    break;
                case 1:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.5f);
                    distanceToOwner = -68 + 40 * Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime);

                    break;
                case 2:
                    Projectile.scale = scale * 1.3f;
                    distanceToOwner = -68 + 40*Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime);
                    break;
            }
            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 100) + 150;
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

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
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
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class ThunderveinSpearSpurt : BaseSwingProj
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinSpearProj";

        public ThunderveinSpearSpurt() : base(new Vector2(48, 46).ToRotation() - 0.08f, trailLength: 10) { }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 135;
            trailTopWidth = -8;
            distanceToOwner = -68;
            minTime = 0;
            onHitFreeze = 12;
            useShadowTrail = true;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
            startAngle = Main.rand.NextFloat(-0.2f, 0.2f);
            totalAngle = 0.01f;

            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 20 + 5;
            Smoother = Coralite.Instance.BezierEaseSmoother;

            Projectile.extraUpdates = 3;

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            float factor = Timer / maxTime;
            if (Timer % 6 == 0)
            {
                Vector2 pos = Projectile.Center + RotateVec2 * Main.rand.NextFloat(20, 70) * Projectile.scale;
                if (Main.rand.NextBool())
                {
                    Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.05f, 0.2f));
                }
            }

            if (factor < 0.5f)
                distanceToOwner = -68 + 80 * Smoother.Smoother(factor * 2);
            else
                distanceToOwner = -68 + 80 * Smoother.Smoother((1 - factor) * 2);

            base.OnSlash();
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

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 10; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                ThunderveinDragon.ThunderveinYellowAlpha * (0.5f - i * 0.5f / 10), Projectile.oldRot[i] + extraRot, origin, Projectile.scale*1.1f, effect, 0);
        }
    }
}
