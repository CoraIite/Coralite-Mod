using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.SlashBladeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ShadowCastle
{
    /// <summary>
    /// 连段0:左键下批，右键上挥
    /// 连段1：左键横挥，右键射幻影剑，可以按住射4次，之后强制终止连招
    /// 连段2：左键下挥2，右键上挥2
    /// 连段3：左键侧下挥，右键横挥
    /// 左键攻击后摇长但是伤害高，右键伤害低但是攻速快
    /// </summary>
    public class Shadura : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public int shadowShootCount;

        public ComboManager comboManager;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Muramasa;
            ItemID.Sets.ShimmerTransformToItem[ItemID.Muramasa] = Type;
        }

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<ShaduraSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.SetWeaponValues(48, 4, 0);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override void HoldItem(Player player)
        {
            comboManager?.UpdateDelay(player);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (comboManager == null)
                InitCombos();

            comboManager.Shoot(player, source, damage, knockback);
            return false;
        }

        public void InitCombos()
        {
            int type = ProjectileType<ShaduraSlash>();
            comboManager = new ComboManager(30)
                .AddCombo((int)ComboManager.ControlType.Left, 0, type, 1.2f)
                .AddCombo((int)ComboManager.ControlType.Left, 1, type, 1f)
                .AddCombo((int)ComboManager.ControlType.Left, 2, type, 1.4f)
                .AddCombo((int)ComboManager.ControlType.Left, 3, type, 2f, ComboData.ResetCombo)

                .AddCombo((int)ComboManager.ControlType.Right, 0, type, 0.5f)
                .AddCombo((int)ComboManager.ControlType.Right, 1, type, 0.6f)
                .AddCombo((int)ComboManager.ControlType.Right, 2, type, 0.8f)
                .AddCombo((int)ComboManager.ControlType.Right, 3, type, 1.1f, ComboData.ResetCombo)

                .AddCombo((int)ComboManager.ControlType.Right_Down, 3, type, 0.9f, ShootShadowSword)
                .AddCombo((int)ComboManager.ControlType.Right_Up, 3, type, 0.9f, ShootShadowSword);
        }

        public void ShootShadowSword(Projectile p, ref int c)
        {
            shadowShootCount++;
            if (shadowShootCount > 2)
            {
                shadowShootCount = 0;
                c = 0;
            }
        }

        public override bool MeleePrefix() => true;
        public override bool AllowPrefix(int pre) => true;
    }

    public class ShaduraSlash : BaseSwingProj/*, IDrawWarp*/
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + "Shadura";

        public ref float ControlType => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];

        public static Asset<Texture2D> GradientTexture;

        public ShaduraSlash() : base(new Vector2(52, 56).ToRotation(), trailCount: 48) { }

        public int delay;
        public int alpha;

        public float minScale;
        public float maxScale;
        public float extraScaleAngle;
        private float recordStartAngle;
        private float recordTotalAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowCastleItems + "ShaduraGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GradientTexture = null;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 64;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 4;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 60 * Projectile.scale;
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : MathHelper.Pi;

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 4;
            alpha = 0;
            switch (ControlType)
            {
                default:
                case (int)ComboManager.ControlType.Left:
                    switch (Combo)
                    {
                        default:
                        case 0://下劈
                            startAngle = 1.6f;
                            totalAngle = 3.6f;
                            minTime = 32;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 54 + 5;
                            Smoother = Coralite.Instance.NoSmootherInstance;
                            delay = 12;
                            extraScaleAngle = 0;
                            minScale = 1.2f;
                            maxScale = 1.2f;

                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);

                            break;
                        case 1://横挥
                            startAngle = 1.8f;
                            totalAngle = 6f;
                            minTime = 32;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 66 + 6;
                            Smoother = Coralite.Instance.NoSmootherInstance;
                            delay = 8;
                            extraScaleAngle = 0;
                            minScale = 0.8f;
                            maxScale = 1.6f;

                            Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);
                            break;
                        case 2://下挥2
                            startAngle = 1.9f;
                            totalAngle = 4.2f;
                            //minTime = 12;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 44;
                            Smoother = Coralite.Instance.NoSmootherInstance;
                            delay = 18;
                            extraScaleAngle = 0;
                            minScale = 1.2f;
                            maxScale = 1.4f;

                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                            break;
                        case 3://侧下挥
                            startAngle = 1.7f;
                            totalAngle = 5.6f;
                            minTime = 44;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 70 + 14;
                            Smoother = Coralite.Instance.BezierEaseSmoother;
                            delay = 18;
                            extraScaleAngle = 0.6f;
                            minScale = 1.4f;
                            maxScale = 1.8f;

                            Helper.PlayPitched("Misc/SwingWave", 0.4f, 0f, Owner.Center);
                            break;
                    }
                    break;
                case (int)ComboManager.ControlType.Right:
                    switch (Combo)
                    {
                        default:
                        case 0://上挑
                            startAngle = -1.8f;
                            totalAngle = -2.8f;
                            //minTime = 8;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 30;
                            Smoother = Coralite.Instance.BezierEaseSmoother;
                            delay = 4;
                            extraScaleAngle = 0.7f;
                            minScale = 1.2f;
                            maxScale = 1.5f;

                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                            break;
                        case 1://下挥
                            startAngle = 1.6f;
                            totalAngle = 4.5f;
                            //minTime = 12;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 40;
                            Smoother = Coralite.Instance.BezierEaseSmoother;
                            delay = 8;
                            extraScaleAngle = 0.4f;
                            minScale = 1.1f;
                            maxScale = 1.5f;

                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);

                            break;
                        case 2://下挥2
                            startAngle = 2.2f;
                            totalAngle = 5f;
                            //minTime = 32;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 38;
                            Smoother = Coralite.Instance.NoSmootherInstance;
                            delay = 8;
                            extraScaleAngle = -0.2f;
                            minScale = 1.1f;
                            maxScale = 1.6f;

                            Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);

                            break;
                        case 3://反横挥
                            startAngle = -2.2f;
                            totalAngle = -4.6f;
                            //minTime = 18;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 52;
                            Smoother = Coralite.Instance.BezierEaseSmoother;
                            delay = 20;
                            extraScaleAngle = -0.5f;
                            minScale = 1.4f;
                            maxScale = 1.8f;

                            Helper.PlayPitched("Misc/SwingWave", 0.4f, 0f, Owner.Center);

                            break;
                    }
                    break;
                case (int)ComboManager.ControlType.Right_Up://射幻影剑
                case (int)ComboManager.ControlType.Right_Down:
                    switch (Combo)
                    {
                        default:
                            Projectile.Kill();
                            break;
                        case 3:
                            useSlashTrail = false;
                            startAngle = -2.7f;
                            totalAngle = 0.01f;
                            minTime = 16;
                            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 24;
                            Smoother = Coralite.Instance.BezierEaseSmoother;
                            delay = 16;
                            extraScaleAngle = 0f;
                            minScale = 1f;
                            maxScale = 1f;

                            distanceToOwner = -30;

                            Projectile.NewProjectileFromThis(OwnerCenter() + new Vector2(0, -16), Vector2.Zero, ProjectileType<ShaduraShoot>(),
                                Projectile.damage, Projectile.knockBack);

                            Helper.PlayPitched(CoraliteSoundID.SpiritFlame_Item117, Owner.Center, pitch: -0.5f);
                            break;
                    }
                    break;
            }

            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), minScale, maxScale);

            base.InitializeSwing();
            //extraScaleAngle *= Math.Sign(totalAngle);
        }

        public override bool? CanDamage()
        {
            if ((ControlType == (int)ComboManager.ControlType.Right_Down ||
                ControlType == (int)ComboManager.ControlType.Right_Up) && Combo == 3)
                return false;

            return base.CanDamage();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Owner.Center, new Vector3(0.8f, 0.3f, 0.8f));

            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            if (ControlType == (int)ComboManager.ControlType.Left && Timer < 36)
            {
                float a = Math.Sign(totalAngle) * 0.03f;
                startAngle -= a;
                recordStartAngle += 0.03f;
            }

            _Rotation = startAngle;
            Slasher();
            if ((int)Timer == minTime)
            {

                if (useSlashTrail)
                    InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Item.type == ItemType<Shadura>())
                scale = Owner.GetAdjustedItemScale(Item);
            else
                Projectile.Kill();

            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;

            Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime)), minScale, maxScale);

            if ((ControlType == (int)ComboManager.ControlType.Right_Down ||
                ControlType == (int)ComboManager.ControlType.Right_Up) && Combo == 3)
            {
                float halfTime = (maxTime - minTime) / 2;
                if (timer < halfTime)
                    distanceToOwner -= 32f / halfTime;
                else
                    distanceToOwner += 32f / halfTime;
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
                if (VaultUtils.isServer)
                    return;

                float strength = 2;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);
                if (VisualEffectSystem.HitEffect_Lightning)
                {
                    Vector2 dir = (_Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2();
                    for (int i = 0; i < 6; i++)
                    {
                        byte hue = (byte)(Main.rand.NextFloat(0.65f, 0.75f) * 255f);
                        Vector2 vel = dir * (i + 0.1f) * 1.5f;
                        for (int j = 0; j < 3; j++)
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                            {
                                PositionInWorld = pos + (dir * j * 2) + (dir * i * Main.rand.NextFloat(6f, 12f)),
                                MovementVector = vel * (1 - (0.1f * j)),
                                UniqueInfoPiece = hue
                            });
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        byte hue = (byte)(Main.rand.NextFloat(0.65f, 0.75f) * 255f);
                        Vector2 vel = -dir * (i + 0.1f) * 1.5f;
                        for (int j = 0; j < 3; j++)
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                            {
                                PositionInWorld = pos - (dir * j * 2) - (dir * i * Main.rand.NextFloat(6f, 12f)),
                                MovementVector = vel * (1 - (0.1f * j)),
                                UniqueInfoPiece = hue
                            });
                    }
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                {
                    int start = Main.rand.Next(2);
                    for (int i = 0; i < 2; i++)
                    {
                        float rot = ((start + i) % 2 * 0.4f) - (0.4f * 2);
                        byte hue = (byte)(Main.rand.NextFloat(0.65f, 0.85f) * 255f);
                        Vector2 vel = RotateVec2.RotatedBy(Main.rand.NextFloat(rot - 0.15f, rot + 0.15f)) * Main.rand.NextFloat(6f * i, 8 + (i * 0.9f));
                        for (int j = 0; j < 4; j++)
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                            {
                                PositionInWorld = pos,
                                MovementVector = vel * (1 - (0.1f * j)),
                                UniqueInfoPiece = hue
                            });
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        byte hue = (byte)(Main.rand.NextFloat(0.65f, 0.85f) * 255f);

                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                        {
                            PositionInWorld = pos,
                            MovementVector = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(2f, 8f),
                            UniqueInfoPiece = hue
                        });
                    }
                }
            }
        }

        //public void DrawWarp()
        //{
        //    WarpDrawer(0.75f);
        //}

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

                Effect effect = Filters.Scene["StarsTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBright.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
                effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                effect.Parameters["uExchange"].SetValue(0.87f + (0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly)));

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

    public class ShaduraShoot : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + "Shadura";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.Shadowflame, 0, 0, Scale: Main.rand.NextFloat(1f, 1.3f));
                d.noGravity = true;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.localAI[0] < 30)
            {
                Projectile.localAI[0]++;
                Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center + new Vector2(owner.direction * 16, -32), 0.2f);
                Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                Dust d = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.Shadowflame, 0, 0, Scale: Main.rand.NextFloat(1f, 1.3f));
                d.noGravity = true;
            }
            else if (Projectile.localAI[0] == 30)
            {
                Projectile.localAI[0]++;
                Projectile.tileCollide = true;
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 12;
            }
            else
            {
                Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.1f, 0.5f));

                Projectile.SpawnTrailDust(DustID.Shadowflame, Main.rand.NextFloat(0.3f, 0.5f)
                    , Scale: Main.rand.NextFloat(1f, 1.3f), noGravity: Main.rand.NextBool(3));
            }

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
                Projectile.SpawnTrailDust(DustID.Shadowflame, Main.rand.NextFloat(0.1f, 0.7f)
                    , Main.rand.NextFloat(-0.45f, 0.45f), Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D maintex = Projectile.GetTexture();
            Texture2D flowTex = Request<Texture2D>(AssetDirectory.ShadowCastleItems + "ShaduraHighlight").Value;

            Vector2 origin = maintex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + 0.785f;

            Color c = Color.Purple;
            c.A = 100;
            //Projectile.DrawShadowTrails(Color.MediumPurple, 0.5f, 0.5f / 10, 1, 10, 1, 1.57f, 1.1f);
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 8; i++)
                Main.spriteBatch.Draw(flowTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                c * (0.2f - (i * 0.2f / 8)), Projectile.oldRot[i] + 0.785f, origin, 1.15f, 0, 0);
            lightColor.A = 150;
            Main.spriteBatch.Draw(maintex, pos, null, lightColor, rot, origin, 1, 0, 0);

            return false;
        }
    }
}
