using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class PoisonousSickle : ModItem, IEquipHeldItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public int useCount;
        public int oldCombo;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 194;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = RarityType<EpicRarity>();
            Item.shoot = ProjectileType<PoisonousSickleSlash>();

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
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                        type, (int)(damage * 1f), knockback, player.whoAmI, 2);
                    return false;
                }

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

                Helper.PlayPitched("CoreKeeper/swordAttack", 0.7f, Main.rand.NextFloat(0f, 0.1f), player.Center);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                    type, (int)(damage * 1.4f), knockback, player.whoAmI, combo);

                oldCombo = combo;
            }

            return false;
        }

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

        public void UpdateEquipHeldItem(Player player)
        {
            player.GetModPlayer<CoralitePlayer>().critDamageBonus += 0.18f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 99)
                .AddIngredient(ItemID.MythrilBar, 20)
                .AddIngredient(ItemID.VialofVenom, 20)
                .AddIngredient(ItemID.SoulofFright)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Gel, 99)
                .AddIngredient(ItemID.OrichalcumBar, 20)
                .AddIngredient(ItemID.VialofVenom, 20)
                .AddIngredient(ItemID.SoulofFright)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class PoisonousSickleSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + "PoisonousSickle";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public PoisonousSickleSlash() : base(MathF.Atan2(50, 62), trailLength: 32) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;
        private float channelTimer;
        private float channelAlpha;
        private float channelShineAlpha;
        private float channelCount;

        public const int ChannelTimeMax = 60 * 4;

        public SlotId soundSlot;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2s");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "PoisonousSickleGradient");
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
            Projectile.height = 70;
            trailTopWidth = 0;
            distanceToOwner = 12;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 60 * Projectile.scale;
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
                    totalAngle = 4.4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 54;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    minTime = 20;
                    delay = 16;
                    ExtraInit();

                    break;
                case 1://下挥，圆
                    startAngle = -1.6f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = -4.4f + Main.rand.NextFloat(-0.2f, 0.2f);
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 54;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    minTime = 20;
                    delay = 16;
                    ExtraInit();

                    break;
                case 2:
                    startAngle = 2f + Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = 5f + Main.rand.NextFloat(-0.2f, 0.2f);
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 54;
                    minTime = 5;
                    Projectile.scale = 0.8f;

                    soundSlot = Helper.PlayPitched("CoreKeeper/windupSwordUnsheathe", 0.7f, 0, Owner.Center);
                    ExtraInit();
                    break;
            }

            //if (Combo != 2)
            //{
            //    base.Initializer();
            //    return;
            //}

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                //if (Combo < 2)
                    totalAngle *= OwnerDirection;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
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
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), 1.5f, 1.7f);
        }

        protected override void BeforeSlash()
        {
            if (Combo != 2)
            {
                startAngle += Math.Sign(startAngle) * 0.03f;

                _Rotation = GetStartAngle() - OwnerDirection * startAngle;
                Slasher();

                if (Timer == minTime)
                {
                    _Rotation = startAngle = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                    //totalAngle *= OwnerDirection;
                    InitializeCaches();
                }
                return;
            }

            if (Main.mouseRight)
            {
                channelCount++;
                Timer = 1;
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;
                totalAngle =recordTotalAngle* OwnerDirection;

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
                        //Helper.PlayPitched("CoreKeeper/sword", 0.5f, 0.1f, Owner.Center);
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

                    _Rotation = startAngle = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                    //totalAngle *= OwnerDirection;

                    //Helper.PlayPitched("Misc/Slash", 0.4f, 0f, Owner.Center);
                    Helper.PlayPitched("CoreKeeper/swooshStrong", 0.9f, 0.2f, Owner.Center);
                    Projectile.damage = (int)(Projectile.damage * 4.5f);
                    //射弹幕
                    //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center
                    //    , (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 8
                    //    , ProjectileType<RuneSoneRightProj>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack, Projectile.owner);
                    //InitializeCaches();
                }
                else
                {
                    Helper.PlayPitched("CoreKeeper/swordAttack", 0.7f, Main.rand.NextFloat(0.2f, 0.3f), Owner.Center);

                    if (SoundEngine.TryGetActiveSound(soundSlot, out var result))
                        result.Stop();
                    Timer = 0;
                    Combo = Main.rand.Next(2);
                    Initializer();
                }
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;
            alpha = (int)(Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 250);

            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            int a = Main.rand.Next(5);
            int type = a switch
            {
                0 => DustID.DemonTorch,
                _ => DustID.UnholyWater
            };
            float scale2 = a switch
            {
                0 => Main.rand.NextFloat(1f, 2f),
                _ => 1.1f
            };

            Dust dust = Dust.NewDustPerfect(Top + RotateVec2 * Main.rand.Next(-45, 5), type,
                   dir * Main.rand.NextFloat(0.5f, 3f), Scale: scale2);
            dust.noGravity = true;

            if (Owner.HeldItem.type == ItemType<PoisonousSickle>())
            {
                scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
                scale = 1.2f * scale - 0.2f;
                if (scale > 3f)
                    scale = 3f;
            }
            else
                Projectile.Kill();

            Projectile.scale = Combo switch
            {
                2 => scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime), 2.4f, 2.6f),
                _ => scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime), 1.5f, 1.7f),
            };
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool addbuff = Main.rand.NextBool(28, 100);
            if (addbuff)
            {
                target.AddBuff(BuffType<IvyPoison>(), 60 * 10);
                target.AddBuff(BuffID.Venom, 60 * 5);
            }
            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                int type = addbuff ? DustType<PoisonExplosion>() : DustType<PoisonImpact>();
                Dust.NewDustPerfect(target.Bottom+Main.rand.NextVector2Circular(16,16), type,
                    Scale: target.width);
                //dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                //dust = Dust.NewDustPerfect(pos, type,
                //         Scale: Main.rand.NextFloat(baseScale * 0.5f, baseScale * 0.8f));
                //dust.rotation = _Rotation - MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);
            }

            Projectile.damage = (int)(Projectile.damage * 0.8f);

            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                if (Main.netMode == NetmodeID.Server)
                    return;

                float strength = 3;
                if (Combo > 2)
                    strength = 2;

                Helper.PlayPitched("CoreKeeper/swordImpact" + Main.rand.Next(2), 0.5f, 0, Projectile.Center);

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 4, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                         Dust.NewDustPerfect(pos, DustID.UnholyWater, dir * Main.rand.NextFloat(1f, 3f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
                        //dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
                         Dust.NewDustPerfect(pos, DustID.Water_Corruption, dir * Main.rand.NextFloat(1f, 4f), newColor: Color.Cyan * 0.5f, Scale: Main.rand.NextFloat(1.5f, 2f));
                        //dust.noGravity = true;
                    }
                }
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            if (Combo == 2 && Timer < minTime)
            {
                Texture2D highlightTex = Request<Texture2D>(AssetDirectory.CoreKeeperItems + "PoisonousSickleHighlight").Value;
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

    public class PoisonExplosion : ModDust
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(11, 1, 0, 0);
            float width = 32; 
            dust.scale =  width/ dust.scale;
            if (dust.scale > 6)
                dust.scale = 6;
        }

        public override bool Update(Dust dust)
        {
            if (dust.fadeIn % 4 == 0)
            {
                dust.frame.X += Texture2D.Width() / 11;
                if (dust.frame.X >= Texture2D.Width())
                    dust.active = false;
            }
            dust.fadeIn++;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates()), dust.rotation, new Vector2(dust.frame.Width / 2, dust.frame.Height), dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class PoisonImpact : PoisonExplosion
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(6, 1, 0, 0);
            dust.scale = Texture2D.Width() / 6 / dust.scale;
            if (dust.scale > 6)
                dust.scale = 6;
        }

        public override bool Update(Dust dust)
        {
            if (dust.fadeIn % 4 == 0)
            {
                dust.frame.X += Texture2D.Width() / 6;
                if (dust.frame.X >= Texture2D.Width())
                    dust.active = false;
            }
            dust.fadeIn++;

            return false;
        }
    }
}
