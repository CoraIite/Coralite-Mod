using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class RuneSong : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public int useCount;
        public int oldCombo;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 226;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 4f;

            Item.holdStyle = ItemHoldStyleID.HoldGolfClub;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = RarityType<LegendaryRarity>();
            Item.shoot = ProjectileType<RuneSoneSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.expert = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.5f, 0, player.Center);
            if (Main.myPlayer == player.whoAmI)
            {
                int combo = Main.rand.Next(2);
                if (combo == oldCombo)
                {
                    useCount++;
                    if (useCount > 1)
                    {
                        useCount = 0;
                        combo = combo switch
                        {
                            0 => 1,
                            _ => 0,
                        };

                    }
                }
                Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                    type, (int)(damage*1.2f), knockback, player.whoAmI, combo);

                oldCombo = combo;
            }

            return false;
        }

        //public override void HoldItem(Player player)
        //{
        //    player.statLifeMax2 += 62;
        //}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Damage", null);
            if (tooltip != null)
            {
                bool addLeadingSpace = Item.DamageType is not VanillaDamageClass;
                string tip = (addLeadingSpace ? " " : "") + Item.DamageType.DisplayName;

                tooltip.Text = string.Concat(((int)(Item.damage * 0.903f)).ToString()
                    ,"-", ((int)(Item.damage * 1.098f)).ToString(),tip);
            }
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

    /// <summary>
    /// 由于在物品里改这个完全不生效所以单独开了个mp来改
    /// </summary>
    public class RuneSongPlayer : ModPlayer
    {
        public override void PreUpdateBuffs()
        {
            if (Player.HeldItem.type == ItemType<RuneSong>())
                Player.statLifeMax2 += 62;
            else if(Player.HeldItem.type == ItemType<BrokenHandle>())
                Player.statLifeMax2 += 31;
        }
    }

    public class RuneSoneSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + "RuneSong";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public RuneSoneSlash() : base(0.785f, trailLength: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2s");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "RuneSongGradient");
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
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 80;
            trailTopWidth = 0;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
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

            Projectile.extraUpdates = 4;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥
                    startAngle = 1.6f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = 4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 44;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    break;
                case 1://下挥，圆
                    startAngle = -1.6f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = -4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 44;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    break;
            }

            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle -recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), 1.2f, 1.6f);

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;
            float scale = 1f;
            if (Owner.HeldItem.type == ItemType<RuneSong>())
                scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
            Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle - recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.6f);
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale>0.8f)
            {
                Projectile.scale *= 0.999f;
            }
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.immortal && target.life < Owner.statLife && Main.rand.NextBool(15, 100))
                target.Kill();

            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                if (!target.immortal)
                    Owner.Heal(3);
                if (Main.netMode == NetmodeID.Server)
                    return;

                float strength = 2;
                //float baseScale = 1;

                Helper.PlayPitched("CoreKeeper/swordLegendaryImpact", 0.5f, 0, Projectile.Center);

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;
                //if (VisualEffectSystem.HitEffect_Lightning)
                //{
                //    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                //        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                //    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                //    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                //             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                //    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                //    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.2f, 0.2f);
                //}

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(1f, 5f), newColor: Color.Cyan*0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(5f, 10f),newColor:Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        byte hue = (byte)(Main.rand.NextFloat(0.45f, 0.65f) * 255f);

                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                        {
                            PositionInWorld = pos,
                            MovementVector = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(4f, 16f),
                            UniqueInfoPiece = hue
                        });
                    }
                }
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
            if (oldRotate != null)
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
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

}
