using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
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
    public class SolarTwinkle : BaseFlyingShieldAccessory, IFlyingShieldAccessory, IDashable
    {
        public SolarTwinkle() : base(ItemRarityID.Cyan, Item.sellPrice(0, 4, 50, 0))
        {
        }

        public float Priority => IDashable.AccessoryDashHigh + 5;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 170;
        }

        public bool isDashing;
        public int SpawnEXProjCount;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
                cp.AddDash(this);
            }

            if (isDashing)
            {
                player.noKnockback = true;
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            for (int i = 0; i < 2; i++)
            {
                int type = Utils.SelectRandom(Main.rand, 6, 259, 158);
                projectile.Projectile.SpawnTrailDust(type, Main.rand.NextFloat(0.3f, 0.5f),
                    Scale: Main.rand.NextFloat(2f, 2.5f));
            }

            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SpawnEXProjCount < 3)
            {
                int damage = projectile.Owner.GetWeaponDamage(Item);
                int num4 = projectile.Projectile.NewProjectileFromThis(projectile.Projectile.Center, Vector2.Zero, 608, damage, 15f);
                Main.projectile[num4].netUpdate = true;
                Main.projectile[num4].Kill();

                SpawnEXProjCount++;
            }
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            projectile.Owner.velocity *= 0.7f;
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
                SoundEngine.PlaySound(CoraliteSoundID.FireShoot_DD2_BetsyFireballShot, Player.Center);
                int damage = Player.GetWeaponDamage(Item);
                int combo = Main.rand.Next(4);
                float startAngle = combo switch
                {
                    0 => -2.6f,
                    1 => 2.6f,
                    2 => -1.6f,
                    _ => 1.6f,
                };

                if (DashDir < 0)
                    startAngle += MathHelper.Pi;

                flyingShieldGuard.TurnToDashing(this, 22, dashDirection, 15f);

                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero,
                    ProjectileType<SolarTwinkleSlash>(), damage, 4, Player.whoAmI, startAngle, combo, DashDir);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
                SpawnEXProjCount = 0;
                isDashing = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ForbiddenLamp>()
                .AddIngredient<PiezoArmorPanel>()
                .AddIngredient(ItemID.FragmentSolar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    /// <summary>
    /// ai0输入初始角度
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.FlyingShieldAccessories)]
    public class SolarTwinkleSlash : BaseSwingProj, IDrawWarp, IDrawAdditive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "HorizontalLight";

        public ref float StartRot => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];

        public SolarTwinkleSlash() : base(0f, 16) { }

        [AutoLoadTexture(Path = AssetDirectory.OtherProjectiles, Name = "WarpTex")]
        public static ATex WarpTexture { get; private set; }
        [AutoLoadTexture(Name = "SolarTwinkleGradient")]
        public static ATex GradientTexture { get; private set; }
        [AutoLoadTexture(Name = "SolarTwinkleProj")]
        public static ATex HeldTex { get; private set; }

        public int delay;
        public int alpha;

        public Vector2 scale;
        private float scaleOffset = 0.35f;

        public override void SetSwingProperty()
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

        protected override void InitializeSwing()
        {
            scale = new Vector2(1f, 1.75f);
            Projectile.extraUpdates = 2;
            startAngle = 0f;
            maxTime = 45;

            switch (Combo)
            {
                default:
                case 0:
                    totalAngle = 5f * Projectile.ai[2];
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
                case 1:
                    totalAngle = -5f * Projectile.ai[2];
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
                case 2:
                    totalAngle = 4.3f * Projectile.ai[2];
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
                case 3:
                    totalAngle = -4.3f * Projectile.ai[2];
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
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
            Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 1.2f, 0.8f));
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (Main.rand.NextBool(2))//生成粒子
            {
                int type = Utils.SelectRandom(Main.rand, 6, 259, 158);
                int num142 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 2.5f * Projectile.direction, -2.5f);
                Main.dust[num142].alpha = 200;
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
                scale = Vector2.Lerp(scale, new Vector2(2f, 2.5f), 0.05f);
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

            Projectile.NewProjectileFromThis(target.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion
                , (int)(Projectile.damage * 0.5f), 10f, 0f, 0.85f + (Main.rand.NextFloat() * 1.15f));

            for (int i = 0; i < 3; i++)
            {
                int type = Utils.SelectRandom(Main.rand, 6, 259, 158);
                int num142 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 2.5f * Projectile.direction, -2.5f);
                Main.dust[num142].alpha = 200;
                Dust dust2 = Main.dust[num142];
                //dust2.velocity *= 2.4f;
                dust2.scale += Main.rand.NextFloat();
            }

            if (VaultUtils.isServer)
                return;

            Dust dust;
            float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
            Vector2 pos = Bottom + (RotateVec2 * offset);
            if (VisualEffectSystem.HitEffect_Lightning)
            {
                byte hue = (byte)(0.1f * 255f);

                for (int i = 0; i < 6; i++)
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
                    dust = Dust.NewDustPerfect(pos, DustID.SolarFlare, dir * Main.rand.NextFloat(2f, 6f), Scale: Main.rand.NextFloat(1.5f, 2f));
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

            for (int i = -3; i < 3; i++)
            {
                spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition + (RotateVec2 * i * 12), mainTex.Frame(),
                  new Color(232, 108, 26, 200),
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
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatVMirror.Value);
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

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = HeldTex.Value;
            Vector2 origin = new(mainTex.Width / 2, mainTex.Height / 2);

            int dir = Math.Sign(totalAngle);
            float extraRot = DirSign < 0 ? MathHelper.Pi : 0;
            extraRot += DirSign == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            if (canDrawSelf)
                Main.spriteBatch.Draw(mainTex, OwnerCenter() + (RotateVec2 * 30) - Main.screenPosition, mainTex.Frame(),
                                                    Color.White, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);
        }
    }
}
