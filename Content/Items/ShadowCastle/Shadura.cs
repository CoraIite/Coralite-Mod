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
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ShadowCastle
{
    /// <summary>
    /// 连段0:左键下批，右键上挥
    /// 连段1：左键横挥，右键射幻影剑，可以按住射4次，之后强制终止连招
    /// 连段2：左键下挥2，右键上挥2
    /// 连段3：侧挥
    /// 左键攻击后摇长但是伤害高，右键伤害低但是攻速快
    /// </summary>
    public class Shadura : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems+Name;

        public int shadowShootCount;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Rapier;
            //Item.shoot = ProjectileType<ShadowChainSwing>();
            Item.DamageType = DamageClass.Summon;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.SetWeaponValues(40, 4, 0);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override void HoldItem(Player player)
        {
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            //switch (combo)
            //{
            //    default:
            //    case 0:
            //        {
            //            combo++;
            //            if (player.altFunctionUse == 2)
            //            {

            //            }
            //            else
            //            {

            //            }
            //        }
            //        break;
            //    case 1:
            //        break;
            //    case 2:
            //        combo++;
            //        break;
            //    case 3:
            //        {
            //            combo = 0;
            //        }
            //        break;
            //}


            return false;
        }
    }

    public class ShaduraSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.GelItems + "EmperorSabre";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ShaduraSlash() : base(new Vector2(52, 56).ToRotation() - 0.1f, trailLength: 48) { }

        public int delay;
        public int alpha;

        public enum ComboType
        {
            /// <summary>
            /// 连段0左键下劈
            /// </summary>
            c0_Left,
            /// <summary>
            /// 连段0右键上挥
            /// </summary>
            c0_Right,
            /// <summary>
            /// 连段1左键横挥
            /// </summary>
            c1_Left,
            /// <summary>
            /// 连段1右键射幻影剑
            /// </summary>
            c1_Right_ShaodwShoot,
             /// <summary>
             /// 连段2左键下劈2
             /// </summary>
            c2_Left,
            /// <summary>
            /// 连段2右键上挥2
            /// </summary>
            c2_Right,
            /// <summary>
            /// 最后的终结技
            /// </summary>
            c3,
        }

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
                case (int )ComboType.c0_Left:
                    break;
                case (int)ComboType.c0_Right:
                    break;
                case (int)ComboType.c1_Left:
                    break;
                case (int)ComboType.c1_Right_ShaodwShoot:
                    break;
                case (int)ComboType.c2_Left:
                    break;
                case (int)ComboType.c2_Right:
                    break;
                case (int)ComboType.c3:
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

                Effect effect = Filters.Scene["StarsTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
                effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);

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
