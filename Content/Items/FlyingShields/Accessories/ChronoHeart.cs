using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class ChronoHeart : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public ChronoHeart() : base(ItemRarityID.Red, Item.sellPrice(0, 10, 0, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 240;
        }

        public bool isDashing;
        public bool hit;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }

            if (isDashing)
            {
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            for (int i = 0; i < 2; i++)
                projectile.Projectile.SpawnTrailDust(DustID.PureSpray, Main.rand.NextFloat(0.2f, 0.5f));

            byte hue = (byte)(0.4f * 255f);
            Vector2 pos = projectile.Projectile.Center;

            Vector2 velocity = projectile.Owner.velocity.SafeNormalize(Vector2.Zero);

            int direction = Math.Sign(velocity.X);
            for (int i = -1; i < 2; i += 2)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = pos + ((projectile.Projectile.rotation + (i * 1.57f)).ToRotationVector2() * 17),
                    MovementVector = velocity.RotatedBy(-i * direction * projectile.Owner.direction * 0.2f) * 6,
                    UniqueInfoPiece = hue
                });
            }

            if (projectile.Timer > projectile.dashTime / 4)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            hit = true;

            if (projectile.parryTime > 0 && projectile.Timer > projectile.dashTime - (projectile.parryTime * 1.5f))
            {
                projectile.OnParry();
                projectile.UpdateShieldAccessory(accessory => accessory.OnParry(projectile));
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
            }
            else
            {
                projectile.OnGuard_DamageReduce(projectile.damageReduce);
                projectile.OnGuard();
                projectile.OnGuardNPC(target.whoAmI);
            }

            projectile.Timer = 0;
            projectile.Owner.velocity.X = -Math.Sign(projectile.Owner.velocity.X) * 6;
            projectile.Owner.velocity.Y = -3f;
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            if (!hit)
                projectile.Owner.velocity.X *= 0.05f;
            isDashing = false;
        }


        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        DashDir = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Player.Center);
                int damage = Player.GetWeaponDamage(Item);
                float startAngle = Main.rand.Next(12) * MathHelper.TwoPi / 12;

                flyingShieldGuard.TurnToDashing(this, 12, dashDirection, 35f);
                Main.instance.CameraModifiers.Add(new MoveModifyer(10, 50));

                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero,
                    ProjectileType<ChronoHeartSlash>(), damage, 4, Player.whoAmI, startAngle, 0, DashDir);

                startAngle = Main.rand.Next(12) * MathHelper.TwoPi / 12;
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero,
                    ProjectileType<ChronoHeartSlash>(), damage, 4, Player.whoAmI, startAngle, 1, DashDir);

                Main.time -= 50;
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
                hit = false;
                isDashing = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OldClockwork>()
                .AddIngredient<SolarPanel>()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    /// <summary>
    /// ai0输入初始角度
    /// </summary>
    public class ChronoHeartSlash : BaseSwingProj, IDrawWarp, IDrawAdditive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        public ref float StartRot => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];

        public ChronoHeartSlash() : base(0f, 12) { }

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public int delay;
        public int alpha;

        public Vector2 scale;
        private float scaleOffset = 0.35f;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "SpurtTrail");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.FlyingShieldAccessories + "ChronoHeartGradient");
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 20;
            distanceToOwner = 40;
            trailBottomWidth = 80;
            minTime = 0;
            useSlashTrail = true;
        }

        public override bool? CanDamage()
        {
            if (Timer > maxTime)
            {
                return false;
            }
            return base.CanDamage();
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 2;
            startAngle = 0f;
            maxTime = 70;

            Smoother = Coralite.Instance.BezierEaseSmoother;

            if (Combo == 0)
            {
                scale = new Vector2(1f, 1.1f);
                totalAngle = -MathHelper.TwoPi * Projectile.ai[2];
            }
            else
            {
                distanceToOwner = 20;
                scale = new Vector2(0.5f, 0.7f);
                totalAngle = -MathHelper.TwoPi / 12 * Projectile.ai[2];
            }

            delay = 30;
            alpha = 0;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = startAngle = GetStartAngle() - (Projectile.ai[2] * startAngle);//设定起始角度
                totalAngle *= Projectile.ai[2];
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailCount];
                oldDistanceToOwner = new float[trailCount];
                oldLength = new float[trailCount];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        protected override float GetStartAngle()
        {
            return StartRot;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return Projectile.height;
        }

        protected override void AIBefore()
        {
            base.AIBefore();
            Lighting.AddLight(Projectile.Center, new Vector3(0.8f, 1.2f, 1.2f));
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (Main.rand.NextBool(2))//生成粒子
            {
                int num142 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Terragrim
                    , 2.5f * Projectile.direction, -2.5f);
                Dust dust2 = Main.dust[num142];
                //dust2.velocity *= 2.4f;
                dust2.scale += Main.rand.NextFloat();
                Main.dust[num142].noGravity = true;
            }

            if (timer % 20 == 0)
                Helper.PlayPitched(CoraliteSoundID.LaserSwing_Item15, Projectile.Center, volume: 1);

            if (timer < minTime + 8)
            {
                alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer - minTime, 8) * 255);
            }
            else
                alpha = 255;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;
            Slasher();
            if (Timer < maxTime + (delay / 2))
                scale = Vector2.Lerp(scale, new Vector2(2f, 2f), 0.05f);
            else if (Timer < maxTime + delay)
                scale = Vector2.Lerp(scale, Vector2.Zero, 0.1f);
            else
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Timer > maxTime)
                return;
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
            }

            if (Main.netMode == NetmodeID.Server)
                return;

            Dust dust;
            float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
            Vector2 pos = Bottom + (RotateVec2 * offset);
            if (VisualEffectSystem.HitEffect_Lightning)
            {
                byte hue = (byte)(0.4f * 255f);

                for (int i = 0; i < 3; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                    {
                        PositionInWorld = pos,
                        MovementVector = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(6f, 18f),
                        UniqueInfoPiece = hue
                    });
                }
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                    dust = Dust.NewDustPerfect(pos, DustID.Terragrim, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1.5f, 2f));
                    dust.noGravity = true;
                }
            }
        }

        public override void PostDraw(Color lightColor) { }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = mainTex.Size() / 2;
            Vector2 scale2 = scale * scaleOffset;
            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                    Color.White, Projectile.rotation, origin, scale2, 0, 0f);

            for (int i = -2; i < 2; i++)
            {
                spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition + (RotateVec2 * i * 12), mainTex.Frame(),
                  new Color(176, 203, 146, 200),
                   Projectile.rotation, origin, scale2 * 1.5f, 0, 0f);
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(1);
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
    }
}
