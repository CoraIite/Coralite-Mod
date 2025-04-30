using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class RuneSong : ModItem, IBuffHeldItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public int useCount;
        public int oldCombo;
        private int holdItemCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 226;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = RarityType<LegendaryRarity>();
            Item.shoot = ProjectileType<RuneSongSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.expert = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        //原作中有的效果，手持时会发出一点光粒子
        public override void HoldItem(Player player)
        {
            Lighting.AddLight(player.Center, new Vector3(0.1f, 0.1f, 0.5f));
            if (holdItemCount > 30)
            {
                holdItemCount = 0;
                Vector2 center = player.Center + new Vector2(0f, player.height * -0.1f);
                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                //float distance = 0.8f + Main.rand.NextFloat() * 0.2f;
                Vector2 velocity = new(0f, (-Main.rand.NextFloat() * 0.3f) - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction, DustID.SilverFlame, velocity, newColor: new Color(150, 150, 150));
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }

            holdItemCount++;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                        type, (int)(damage * 1f), knockback, player.whoAmI, 2, -1);
                    return false;
                }

                int combo = 0;
                if (combo == oldCombo)
                {
                    if (Main.rand.NextBool())
                        useCount++;
                    if (useCount > 3)
                    {
                        useCount = 0;
                        combo = 1;
                    }
                }

                Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.7f, Main.rand.NextFloat(0.2f, 0.3f), player.Center);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                    type, (int)(damage * 1.6f), knockback, player.whoAmI, combo, -1);

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
                    , "-", ((int)(Item.damage * 1.098f)).ToString(), tip);
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RuneParchment>()
                .AddIngredient<ChippedBlade>()
                .AddIngredient<BrokenHandle>()
                .AddIngredient<ClearGemstone>()
                .AddIngredient<AncientGemstone>(10)
                .AddIngredient(ItemID.IronBar, 50)
                .AddCondition(CoraliteConditions.UseRuneParchment)
                .Register();
        }

        public void UpdateBuffHeldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.LifeMaxModifyer.Flat += 62;
        }
    }

    public class RuneSongSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + "RuneSong";

        public ref float Combo => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];

        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public RuneSongSlash() : base(0.785f, trailCount: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;
        private float channelTimer;
        private float channelAlpha;
        private float channelShineAlpha;
        private float channelCount;

        public const int ChannelTimeMax = 90 * 4;

        public SlotId soundSlot;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "RuneSongGradient");
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
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
            Projectile.hide = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            if (Combo > 2)
            {
                return 85 * Projectile.scale;
            }
            return 65 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 4;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥
                    startAngle = 1.6f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = 4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 44;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    ExtraInit();

                    break;
                case 1://下挥，圆
                    startAngle = -1.6f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = -4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 44;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    ExtraInit();

                    break;
                case 2:
                    startAngle = 2f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = 4.7f + Main.rand.NextFloat(-0.2f, 0.2f);
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 54;
                    minTime = 5;
                    Projectile.scale = 0.8f;

                    soundSlot = Helper.PlayPitched("CoreKeeper/windupSwordUnsheathe", 0.7f, 0, Owner.Center);
                    ExtraInit();
                    break;
                case 3:
                    Projectile.hide = false;
                    startAngle = 0f;
                    totalAngle = 30.5f;
                    maxTime = 90 * 4;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 20;
                    Projectile.localNPCHitCooldown = 60;
                    Projectile.scale = 1.3f;

                    break;
                case 4:
                    Projectile.hide = false;
                    startAngle = 3.14f;
                    totalAngle = 30.5f;
                    maxTime = 90 * 4;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 20;
                    Projectile.localNPCHitCooldown = 60;
                    Projectile.scale = 1.3f;

                    break;
            }

            if (Combo != 2)
            {
                base.InitializeSwing();
                return;
            }

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                //totalAngle *= OwnerDirection;
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

        private void ExtraInit()
        {
            extraScaleAngle = Main.rand.NextFloat(-0.4f, 0.4f);
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.2f, 1.7f);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
            if (Combo < 3)
                base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            if (Combo != 2)
                return;

            if (DownRight)
            {
                channelCount++;
                Timer = 1;
                _Rotation = GetStartAngle() - (DirSign * startAngle);
                Slasher();
                if (channelTimer < ChannelTimeMax)
                {
                    channelAlpha = channelTimer / (ChannelTimeMax + 300);
                    channelTimer++;
                    startAngle += 0.4f / ChannelTimeMax;
                    Projectile.scale += 0.3f / ChannelTimeMax;
                    if (channelTimer == ChannelTimeMax)
                    {
                        channelShineAlpha = 1;
                        Helper.PlayPitched("CoreKeeper/sword", 0.5f, 0.1f, Owner.Center);
                    }
                }
                else
                {
                    channelAlpha = 0;
                    if (channelShineAlpha > 0)
                        channelShineAlpha -= 0.01f;

                    if (channelCount % 120 == 0)
                        channelShineAlpha = 1;
                }
            }
            else
            {
                if (channelTimer >= ChannelTimeMax)
                {
                    Timer = minTime + 1;

                    _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                    totalAngle *= DirSign;

                    //Helper.PlayPitched("Misc/Slash", 0.4f, 0f, Owner.Center);
                    Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.7f, 0, Owner.Center);
                    //射弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center
                        , (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 8
                        , ProjectileType<RuneSoneRightProj>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack, Projectile.owner);
                    InitializeCaches();
                }
                else
                {
                    Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.7f, Main.rand.NextFloat(0.2f, 0.3f), Owner.Center);

                    if (SoundEngine.TryGetActiveSound(soundSlot, out var result))
                        result.Stop();
                    Timer = 0;
                    Combo = Main.rand.Next(2);
                    InitializeSwing();
                }
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Combo > 2)
            {
                if (alpha < 255)
                    alpha += 2;
                if (timer % 30 == 0)
                    onHitTimer = 0;
                int nextcount = 4;
                if (timer > 70 * 4)
                {
                    Projectile.scale -= 0.005f;
                    nextcount = 12;
                }

                if (Main.rand.NextBool(nextcount))
                {
                    Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                    int a = Main.rand.Next(5);
                    int type = a switch
                    {
                        0 => DustType<Runes>(),
                        _ => DustID.AncientLight
                    };
                    float scale2 = a switch
                    {
                        0 => Main.rand.NextFloat(0.5f, 1.2f),
                        _ => 1.1f
                    };

                    Dust dust = Dust.NewDustPerfect(Top + (RotateVec2 * Main.rand.Next(-45, 5)), type,
                           dir * Main.rand.NextFloat(0.5f, 3f), newColor: Coralite.IcicleCyan, Scale: scale2);
                    dust.noGravity = true;
                }
            }
            else
            {
                alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 100) + 100;
            }
            if (Item.type == ItemType<RuneSong>())
            {
                scale = Owner.GetAdjustedItemScale(Item);
                scale = (1.5f * scale) - 0.5f;
                if (scale > 3f)
                    scale = 3f;
            }
            else
                Projectile.Kill();

            if (Combo < 3)
                Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime)), 1.2f, 1.7f);
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale > 0.8f)
            {
                Projectile.scale *= 0.999f;
            }
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void AIAfter()
        {
            if (Combo < 3)
                base.AIAfter();
            else
            {
                Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
                Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

                if (useShadowTrail || useSlashTrail)
                    UpdateCaches();
            }
        }

        protected override Vector2 OwnerCenter()
        {
            if (Main.projectile.IndexInRange((int)OwnerIndex))
            {
                Projectile p = Main.projectile[(int)OwnerIndex];
                if (p.active && p.type == ProjectileType<RuneSoneRightProj>())
                    return p.Center;

                Projectile.Kill();
                return Vector2.Zero;
            }

            //Projectile.Kill();
            return base.OwnerCenter();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool TrueMelee = Combo < 3;
            if (TrueMelee && !target.immortal && target.life < Owner.statLife && Main.rand.NextBool(15, 100))
                target.Kill();

            Projectile.damage = (int)(Projectile.damage * 0.8f);

            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                if (TrueMelee && !target.immortal && !target.SpawnedFromStatue)
                    Owner.Heal(3);
                if (VaultUtils.isServer)
                    return;

                float strength = 3;
                if (Combo > 2)
                    strength = 2;
                //float baseScale = 1;

                Helper.PlayPitched("CoreKeeper/swordLegendaryImpact", 0.5f, 0, Projectile.Center);

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 4, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);
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
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(1f, 5f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
                        dust = Dust.NewDustPerfect(pos, DustID.AncientLight, dir * Main.rand.NextFloat(1f, 10f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                {
                    int start = Main.rand.Next(6);
                    for (int i = 0; i < 6; i++)
                    {
                        float rot = ((start + i) % 6 * 0.2f) - (0.2f * 3);
                        byte hue = (byte)(Main.rand.NextFloat(0.45f, 0.65f) * 255f);
                        Vector2 vel = RotateVec2.RotatedBy(Main.rand.NextFloat(rot - 0.15f, rot + 0.15f)) * Main.rand.NextFloat(4f * i, 5 + (i * 0.9f));
                        for (int j = 0; j < 4; j++)
                            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                            {
                                PositionInWorld = pos,
                                MovementVector = vel * (1 - (0.1f * j)),
                                UniqueInfoPiece = hue
                            });
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        byte hue = (byte)(Main.rand.NextFloat(0.45f, 0.65f) * 255f);

                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                        {
                            PositionInWorld = pos,
                            MovementVector = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(2f, 8f),
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
            if (oldRotate == null)
                return;
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
                    Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBlurSmall.Value);
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Combo > 2)
            {
                return;
            }

            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            if (Combo == 2 && Timer < minTime)
            {
                Texture2D highlightTex = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "RuneSongHighlight").Value;
                Texture2D mainTex2 = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "CraftUI").Value;
                DrawBar(mainTex2, Color.White);
                mainTex2 = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "CraftUIHighlight").Value;

                if (channelTimer < ChannelTimeMax)
                {
                    base.DrawSelf(highlightTex, origin, lightColor * channelAlpha, extraRot);
                    DrawBar(mainTex2, Color.White * channelAlpha);
                }
                else
                {
                    base.DrawSelf(highlightTex, origin, lightColor * channelShineAlpha, extraRot);

                    Vector2 pos = Owner.Center + new Vector2(0, 200) - Main.screenPosition;
                    var frameBox = mainTex2.Frame(1, 2, 0, 0);
                    Vector2 origin2 = frameBox.Size() / 2;

                    Main.spriteBatch.Draw(mainTex2, pos, frameBox, Color.White * channelShineAlpha, 0, origin2, 1, 0, 0);
                }
            }
        }

        private void DrawBar(Texture2D mainTex, Color c)
        {
            Vector2 pos = Owner.Center + new Vector2(0, 200) - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            Vector2 origin2 = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, c, 0, origin2, 1, 0, 0);

            frameBox = mainTex.Frame(1, 2, 0, 1);
            int width = frameBox.Width;
            frameBox.Width = 2 + (int)(width * channelTimer / ChannelTimeMax);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, c, 0, origin2, 1, 0, 0);
        }
    }

    public class RuneSoneRightProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private bool span;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 90;
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public void Initialize()
        {
            var source2 = Projectile.GetSource_FromAI();
            var Owner = Main.player[Projectile.owner];

            Projectile.NewProjectile(source2, Owner.Center, Vector2.Zero, ProjectileType<RuneSongSlash>()
                , Projectile.damage, Projectile.knockBack, Projectile.owner, 3, Projectile.whoAmI);
            Projectile.NewProjectile(source2, Owner.Center, Vector2.Zero, ProjectileType<RuneSongSlash>()
                , Projectile.damage, Projectile.knockBack, Projectile.owner, 4, Projectile.whoAmI);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            if (Projectile.timeLeft < 50)
            {
                Projectile.velocity *= 0.9f;
            }
            //if (Main.rand.NextBool(4))
            //{
            //    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(120, 120), DustType<Runes>(),
            //        new Vector2(0, -Main.rand.NextFloat(2f, 5f))+Projectile.velocity, Scale: Main.rand.NextFloat(0.8f, 1f));
            //}
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class Runes : ModDust
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(6, 1, Main.rand.Next(6), 0);
        }

        public override bool Update(Dust dust)
        {
            if (dust.fadeIn < 8)
                dust.rotation = dust.fadeIn / 8f;
            else
            {
                dust.rotation = (16 - dust.fadeIn) / 8f;
                if (dust.fadeIn > 16)
                {
                    dust.active = false;
                }
            }

            dust.position += dust.velocity;
            dust.fadeIn++;
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D mainTex = Texture2D.Value;
            var frameBox = dust.frame;
            var origin = frameBox.Size() / 2;
            Vector2 pos = dust.position - Main.screenPosition;
            Color c = dust.color * dust.rotation * 0.3f;

            float r = MathHelper.PiOver4;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(mainTex, pos + (r.ToRotationVector2() * 2), frameBox, c, 0, origin, dust.scale, 0, 0);
                r += MathHelper.PiOver2;
            }
            Main.EntitySpriteDraw(mainTex, pos, frameBox, Color.White * dust.rotation, 0, origin, dust.scale, 0, 0);

            return false;
        }
    }
}
