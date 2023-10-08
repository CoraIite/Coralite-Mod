using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    /// <summary>
    /// 下挥=》上挥=》扔出后在身边转一圈=》向前剪一下<br></br>
    /// 右键向前剪一下，如果拥有满层噩梦能量那么就释放噩梦之咬
    /// </summary>
    public class DreamShears : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 23;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<DreamShearsSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(286, 4, 8);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (player.altFunctionUse == 2)
                {
                    //生成弹幕
                    if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 7)//射出特殊弹幕
                    {
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<LostVine>(), (int)(damage * 3.5f), knockback,
                            player.whoAmI, -1, (0.2f + cp.nightmareEnergy * 0.1f) * (cp.nightmareEnergy % 2 == 0 ? -1 : 1), player.itemTimeMax * 1.5f);
                        cp.nightmareEnergy = 0;
                    }
                    else
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsSlash>(), (int)(damage * 1.4f), knockback, player.whoAmI);
                    return false;
                }

                // 生成弹幕
                switch (combo)
                {
                    default:
                    case 0:
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage * 2, knockback, player.whoAmI, combo);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }

                if (player.TryGetModPlayer(out CoralitePlayer cp2)&&cp2.nightmareEnergy<7)//获得能量
                    cp2.nightmareEnergy++;
                combo++;
                if (combo > 1)
                    combo = 0;
            }

            return false;
        }
    }

    public class DreamShearsSlash: BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "DreamShears";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> GradientTexture;

        public DreamShearsSlash() : base(0.785f, trailLength: 26) { }

        public int alpha;
        public int delay = 24;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2a");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "DreamShearsGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 90;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 16;
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
                    startAngle = 2.4f;
                    totalAngle = 4.6f;
                    maxTime = Owner.itemTimeMax* 2;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(0, maxTime - minTime), 1f, 1.6f);
                    break;
                case 1://下挥，圆
                    startAngle = -2.4f;
                    totalAngle = -3.8f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    Projectile.scale = Helper.EllipticalEase(2.3f - 4.6f * Smoother.Smoother(0, maxTime - minTime), 1f, 1.6f);
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
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            Dust dust = Dust.NewDustPerfect((Top+Projectile.Center)/2 + Main.rand.NextVector2Circular(50, 50), DustID.RedMoss,
                   dir * Main.rand.NextFloat(0.5f, 2f));
            dust.noGravity = true;

            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.BezierEaseSmoother.Smoother(timer, maxTime - minTime) * 200) + 50;

            switch ((int)Combo)
            {
                default:
                case 0:
                    Projectile.scale = Helper.EllipticalEase(2.4f - 4.6f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.6f);
                    break;
                case 1:
                    Projectile.scale = Helper.EllipticalEase(2.4f - 3.8f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.6f);
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

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 center = target.Center +  (Projectile.rotation  + (Timer % 2 == 0 ? 1.57f : -1.57f) + Main.rand.NextFloat(-0.25f, 0.25f)).ToRotationVector2() * 140;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, (target.Center - center).SafeNormalize(Vector2.Zero) * 28, ProjectileType<DreamShearsSpurt>(), Projectile.damage, 2, Owner.whoAmI, 1, 0, 16);
                }

                float strength = 3;
                float baseScale = 1;

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
                    //dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                    //    Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                    //dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                    //dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                    //         Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                    //float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                    //dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.2f, 0.2f);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    //for (int j = 0; j < 8; j++)
                    //{
                    //    Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                    //    dust = Dust.NewDustPerfect(pos, DustID.t_Slime, dir * Main.rand.NextFloat(1f, 5f), 150, new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 2f));
                    //    dust.noGravity = true;
                    //}

                    //for (int i = 0; i < 12; i++)
                    //{
                    //    Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                    //    dust = Dust.NewDustPerfect(pos, DustID.ShimmerSpark, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1.5f, 2f));
                    //    dust.noGravity = true;
                    //}
                }

                //if (VisualEffectSystem.HitEffect_SpecialParticles)
                //    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Keybrand, new ParticleOrchestraSettings()
                //    {
                //        PositionInWorld = pos,
                //        MovementVector = RotateVec2 * Main.rand.NextFloat(2f, 4f),
                //    });
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

                Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

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

    /// <summary>
    /// 使用ai0传入状态，为0时跟踪玩家，为1时向前戳出
    /// </summary>
    public class DreamShearsSpurt : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "SpurtTrail2";

        public ref float State => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float TrailWidth => ref Projectile.ai[2];

        public Player Owner => Main.player[Projectile.owner];

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override bool ShouldUpdatePosition() => Timer >= 0;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > 0)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), WidthFunction, ColorFunction);

            if (Timer > 0)
            {
                Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2, DustID.RedTorch,
                    Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(0.5f, 3f));
                dust.noGravity = true;

                switch (State)
                {
                    case 0:
                        {
                            if (Timer < 8)
                            {
                                Projectile.Center = Owner.Center;
                                Alpha += 1 / 8f;
                            }
                            else if (Timer == 8)
                            {
                                Projectile.velocity *= 12;
                            }
                            else if (Timer > 12)
                            {
                                Projectile.velocity *= 0.96f;
                                Alpha -= 0.1f;
                                if (Alpha < 0)
                                    Projectile.Kill();
                            }
                        }
                        break;
                    default:
                    case 1:
                        {
                            if (Timer < 8)
                            {
                                if (Alpha < 1)
                                    Alpha += 1 / 2f;
                                break;
                            }

                            Projectile.velocity *= 0.7f;
                            Alpha -= 0.15f;
                            if (Alpha < 0)
                                Projectile.Kill();
                        }
                        break;
                }

                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer < 12)
                {
                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);
                }
                else
                {
                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
                }
                trail.Positions = Projectile.oldPos;
            }

            Timer++;
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.3f)
                return Helper.Lerp(0, TrailWidth, factor / 0.3f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.3f) / 0.7f);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null || Timer < 0)
                return;

            Effect effect = Filters.Scene["AlphaGradientTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(TextureAssets.Projectile[Projectile.type].Value);
            effect.Parameters["gradientTexture"].SetValue(DreamShearsSlash.GradientTexture.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawWarp()
        {
            if (Timer < 0)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float w = 1f;
            Vector2 up = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 down = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - i / 15f;
                Vector2 Center = Projectile.oldPos[i];
                float r = Projectile.rotation % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                float width = WidthFunction(factor) * 0.75f;
                Vector2 Top = Center + up * width;
                Vector2 Bottom = Center + down * width;

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = FrostySwordSlash.WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}
