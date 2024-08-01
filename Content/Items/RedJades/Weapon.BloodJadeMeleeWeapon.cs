using Coralite.Content.Bosses.ModReinforce.Bloodiancie;
using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
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

namespace Coralite.Content.Items.RedJades
{
    public class BloodJadeMeleeWeapon : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 60;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileType<BloodJadeSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<BloodJadeParry>(), damage, knockback, player.whoAmI);
                    return false;
                }

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                    case 2:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, useCount);
                        break;
                    case 3:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.15f), knockback, player.whoAmI, useCount);
                        break;
                }
            }

            useCount++;
            if (useCount > 3)
                useCount = 0;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodJade>(20)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class BloodJadeSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.RedJadeItems + "BloodJadeMeleeWeapon";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public BloodJadeSlash() : base(0.785f, trailCount: 48) { }

        public int delay;
        public int alpha;
        public bool canShoot = true;
        public float StartRot;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.RedJadeItems + "BloodJadeMeleeGradient");
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
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hide = true;
            trailTopWidth = -6;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 50 * Projectile.scale;
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
                case 1://下挥，圆
                    startAngle = 1.6f;
                    totalAngle = 3.2f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 24;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 0.9f;
                    break;
                case 2://下挥 较圆
                    startAngle = 1.2f;
                    totalAngle = 4.2f;
                    onHitFreeze = 12;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 38;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    Projectile.scale = 0.8f;
                    break;
                case 3://重上挥
                case 4:
                    startAngle = -2.6f;
                    totalAngle = -4.6f;
                    onHitFreeze = 12;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 38;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
            }

            StartRot = GetStartAngle();
            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
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

            if (canShoot && timer > (maxTime - minTime) / 2)
            {
                if (Owner.HasBuff<BloodJadeBuff>())
                {
                    var source = Projectile.GetSource_FromAI();
                    Vector2 dir = StartRot.ToRotationVector2();
                    switch ((int)Combo)
                    {
                        default:
                            break;
                        case 0:
                        case 1:
                            Projectile.NewProjectile(source, Owner.Center, dir * 9, ProjectileType<BloodJadeStrike>(),
                                (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0);
                            break;
                        case 2:
                            Projectile.NewProjectile(source, Owner.Center, dir * 8, ProjectileType<BloodJadeStrike>(),
                                (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 1);
                            break;
                        case 3:
                            Projectile.NewProjectile(source, Owner.Center, dir * 7, ProjectileType<BloodJadeStrike>(),
                                (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 2);
                            break;
                    }
                }
                canShoot = false;
            }

            switch ((int)Combo)
            {
                default:
                case 0:
                case 1:
                    alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    //Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 0.8f, 1.2f);
                    break;
                case 2:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 140) + 100;
                    Projectile.scale = Helper.EllipticalEase(1.2f - 4.2f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.4f);
                    break;
                case 3:
                case 4:
                    alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
                    Projectile.scale = Helper.EllipticalEase(2.6f + 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1.4f, 1.8f);
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
                if (Main.netMode == NetmodeID.Server)
                    return;

                float baseScale = 1;

                if (Combo == 3)
                {
                    baseScale = 3;
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;
                if (VisualEffectSystem.HitEffect_Lightning)
                {
                    dust = Dust.NewDustPerfect(pos, DustType<BloodJadeStrikeDust>(),
                        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                    dust = Dust.NewDustPerfect(pos, DustType<BloodJadeStrikeDust>(),
                             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.8f, 0.8f);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.GemRuby, dir * Main.rand.NextFloat(1f, 5f), 150, Scale: Main.rand.NextFloat(1f, 1.2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.GemRuby, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1f, 1.2f));
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

            for (int i = 0; i < count; i++)
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

    public class BloodJadeParry : BaseHeldProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "BloodJadeMeleeWeapon";

        public int Timer;
        public float alpha;
        public float distanceToOwner;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 80;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.direction = Owner.direction;

            do
            {
                if (Timer < 12)
                {
                    alpha += 1 / 12f;
                    distanceToOwner += 44 / 12f;
                    Projectile.scale = Helper.Lerp(1, 1.3f, Timer / 12f);
                    Projectile.rotation = 1.57f + Helper.Lerp(Owner.direction * 1.8f, -Owner.direction * 0.3f, Timer / 12f);
                    break;
                }

                if (Timer == 12)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.Zenith_Item169, Projectile.Center);
                    Owner.immuneTime = 12;
                    Owner.immune = true;
                }

                if (Timer < 20)
                {
                    Rectangle rect = Projectile.getRect();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                            continue;

                        if (proj.Colliding(proj.getRect(), rect))
                        {
                            Parry(33);
                            break;
                        }
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (!npc.active || npc.friendly || npc.immortal)
                            continue;

                        if (Projectile.Colliding(rect, npc.getRect()))
                        {
                            Parry(20);
                            break;
                        }
                    }

                    break;
                }

                if (Timer < 20 + 40)
                {
                    alpha -= 1 / 40f;
                    if (distanceToOwner > 0)
                        distanceToOwner -= 32 / 25f;
                    Projectile.scale = Helper.Lerp(1.3f, 1f, (Timer - 24) / 40f);
                    Projectile.rotation += -Owner.direction * 0.16f;
                    break;
                }

                Projectile.Kill();

            } while (false);

            Projectile.Center = Owner.Center + new Vector2(Projectile.direction * distanceToOwner, 0);
            Timer++;
        }

        public void Parry(int immuneTime)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, immuneTime);
                    Owner.immune = true;
                }

                if (cp.parryTime < 5)
                {
                    var source = Projectile.GetSource_FromAI();

                    Projectile.NewProjectile(source, Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 14, ProjectileType<BloodJadeWave>(),
                        (int)(Projectile.damage * 1.35f), Projectile.knockBack, Projectile.owner, 3);

                    Owner.AddBuff(BuffType<BloodJadeBuff>(), 60 * 6);
                }

                if (cp.parryTime < 280)
                    cp.parryTime += 100;
            }

            Particle.NewParticle(Projectile.Center, Vector2.Zero,
                CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.RedJadeRed, 1.5f);
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<BloodJadeSlash>(),
                (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 4);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Color c = lightColor * alpha;
            Color c2 = Coralite.RedJadeRed;
            c2.A = 150;
            c2 *= 0.6f * alpha;
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2();
                Main.spriteBatch.Draw(mainTex, Projectile.Center + offset * 4 - Main.screenPosition, null, c2, Projectile.rotation + 0.785f,
                    mainTex.Size() / 2, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, c, Projectile.rotation + 0.785f,
                mainTex.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class BloodJadeStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + "RedJadeStrike";

        public ref float ExplosionType => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 45;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.04f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 0.7f);
                    dust.noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                var source = Projectile.GetSource_FromThis();
                switch (Projectile.ai[0])
                {
                    default:
                    case 0:
                        Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, ProjectileType<BloodJade_BigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        break;
                }
                return;
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(3, 3), 0, default, Main.rand.NextFloat(1f, 1.3f));
                    dust.noGravity = true;
                }
        }
    }

    public class BloodJadeStrikeDust : BaseStrikeDust
    {
        public BloodJadeStrikeDust() : base(new Color(255, 192, 192), new Color(130, 13, 70), 16) { }

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, 1f, 0.3f, 0.3f);
            float factor = dust.fadeIn / 16f;
            if (factor < 0.5f)
            {
                dust.scale += 0.4f;
                dust.rotation += 0.03f;

            }
            else
                dust.scale -= 0.15f;

            dust.fadeIn++;
            if (dust.fadeIn > 16)
                dust.active = false;

            return false;
        }
    }

    public class BloodJade_BigBoom : Bloodiancie_BigBoom
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 320;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 center = Projectile.Center;
            Helper.PlayPitched("RedJade/RedJadeBoom", 1f, -1f, center);

            Color red = new Color(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                for (int i = 0; i < 5; i++)
                {
                    Particle.NewParticle(center, Helper.NextVec2Dir(38, 40), type, red, Main.rand.NextFloat(0.15f, 0.2f));
                }
                for (int i = 0; i < 10; i++)
                {
                    Particle.NewParticle(center, Helper.NextVec2Dir(24, 30), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                    Particle.NewParticle(center, Helper.NextVec2Dir(24, 30), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                    Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, Helper.NextVec2Dir(6, 10), Scale: Main.rand.NextFloat(2f, 2.4f));
                    dust.noGravity = true;
                }
            }

            RedExplosionParticle.Spawn(center, 1.4f, Coralite.RedJadeRed);
            RedGlowParticle.Spawn(center, 1.3f, Coralite.RedJadeRed, 0.4f);
            RedGlowParticle.Spawn(center, 1.3f, Coralite.RedJadeRed, 0.4f);

            if (VisualEffectSystem.HitEffect_ScreenShaking)
            {
                var modifier = new PunchCameraModifier(center, Helper.NextVec2Dir(), 10, 8f, 14, 1000f);
                Main.instance.CameraModifiers.Add(modifier);
            }
        }
    }

    public class BloodJadeWave : BloodWave
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;

            Projectile.scale = 1.6f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if ((int)Projectile.ai[0] % 6 == 0)
            {
                int type = Main.rand.NextFromList(ProjectileType<RedJadeBoom>(), ProjectileType<RedJadeBigBoom>());

                Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center + new Vector2(Main.rand.NextFloat(-16, 16), Main.rand.NextFloat(-80, 80)), Vector2.Zero, type,
                    Projectile.damage / 2, 8, Projectile.owner);
            }

            if (Projectile.ai[0] > 40)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                    Vector2.Zero, ProjectileType<BloodJade_BigBoom>(), Projectile.damage, 8f);
        }
    }

    public class BloodJadeBuff : ModBuff
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
    }
}
