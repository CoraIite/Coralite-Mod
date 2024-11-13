using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class Lullaby : ModItem
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<LullabySlash>();
            Item.DamageType = DamageClass.Magic;
            Item.rare = RarityType<FantasyRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(188, 4, 4);
            Item.mana = 36;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

        public override bool MagicPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                // 生成弹幕
                //damage = (int)(damage * 1f);
                switch (combo)
                {
                    default:
                    case 0:
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<LullabySlash2>(), damage, knockback, player.whoAmI);
                        break;
                }

                combo++;
                if (combo > 2)
                    combo = 0;
            }

            return false;
        }

    }

    public class LullabySlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "Lullaby";

        public ref float Combo => ref Projectile.ai[0];
        public int alpha;

        public LullabySlash() : base(MathF.Atan(46f / 52f), trailCount: 34) { }

        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "LullabyGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarpTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 30;
            Projectile.height = 70;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 8;
            useSlashTrail = true;
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
                case 0:
                    startAngle = 1.2f;
                    totalAngle = 5.6f;
                    minTime = 46;
                    onHitFreeze = 8;
                    maxTime = (int)(Owner.itemTimeMax * 3f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 1f;

                    break;
                case 1:
                    startAngle = -1.2f;
                    totalAngle = -4.6f;
                    minTime = 36;
                    onHitFreeze = 8;
                    maxTime = (int)(Owner.itemTimeMax * 3f);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 1f;

                    break;
            }

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度
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

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 75 * Projectile.scale;
        }

        protected override void BeforeSlash()
        {
            Projectile.scale = MathHelper.Lerp(0.8f, 1f, Timer / minTime);

            if (Timer < 30)
                startAngle += Math.Sign(startAngle) * 0.05f;
            _Rotation = GetStartAngle() - (DirSign * startAngle);
            Slasher();
            if ((int)Timer == minTime)
            {
                _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                totalAngle *= DirSign;

                //Helper.PlayPitched("Misc/Slash", 0.4f, 0f, Owner.Center);
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Projectile.Center);
                //射弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                    (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 14, ProjectileType<LullabyBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: -1);
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            switch (Combo)
            {
                default:
                case 0:
                    Projectile.scale = Helper.EllipticalEase(2.8f - (5.6f * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.2f);

                    break;
                case 1:
                    Projectile.scale = Helper.EllipticalEase(2.8f - (4.6f * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.2f);

                    break;
            }

            alpha = (int)(Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 80) + 160;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;
            _Rotation -= totalAngle * 0.001f;
            Slasher();
            if (Timer > maxTime + 24)
                Projectile.Kill();
        }

        public void DrawWarp()
        {
            WarpDrawer(0.5f);
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
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
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

    public class LullabySlash2 : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "Lullaby";

        public LullabySlash2() : base(MathF.Atan(46f / 52f), trailCount: 34) { }

        public int alpha;
        public int delay = 48;

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 90;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 75 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;

            startAngle = -1.57f;
            totalAngle = 6.282f;
            maxTime = Owner.itemTimeMax * 4;
            Smoother = Coralite.Instance.BezierEaseSmoother;

            base.Initializer();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 3.141f : 0f;

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            //Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            //Dust dust = Dust.NewDustPerfect((Top + Projectile.Center) / 2 + Main.rand.NextVector2Circular(50, 50), DustID.RedMoss,
            //       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
            //dust.noGravity = true;

            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.BezierEaseSmoother.Smoother(timer, maxTime - minTime) * 200) + 50;

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;

            if (Timer < maxTime + 24)
            {
                distanceToOwner -= 0.2f;
            }
            else
            {
                distanceToOwner = Helper.Lerp(distanceToOwner, 32, 0.2f);
                if (Timer == maxTime + 34)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, Owner.Center);
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                            (i * MathHelper.TwoPi / 7).ToRotationVector2() * 8, ProjectileType<LullabyBall>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner, ai2: i);
                    }
                }
            }

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public void DrawWarp()
        {
            WarpDrawer(0.75f);
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

                Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
                effect.Parameters["gradientTexture"].SetValue(LullabySlash.GradientTexture.Value);

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

    /// <summary>
    /// 使用ai2传入颜色
    /// </summary>
    public class LullabyBall : ModProjectile, IDrawPrimitive/*, IDrawNonPremultiplied*/, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "FantasyBall";

        private Trail trail;
        public Color DrawColor;
        public bool init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            if (init)
            {
                if (Projectile.ai[2] >= 0 && Projectile.ai[2] < 7)
                    DrawColor = NightmarePlantera.phantomColors[(int)Projectile.ai[2]];
                else
                    DrawColor = FantasyGod.shineColor;

                init = false;
            }

            if (Projectile.timeLeft < 865)
            {
                bool flag2 = false;
                float readyTime = 865f;
                float value = 0.075f;
                float value2 = 0.125f;
                float num4 = 30f;

                if (Projectile.timeLeft > 20)
                    flag2 = true;

                int num7 = (int)Projectile.ai[0];
                if (Main.npc.IndexInRange(num7) && !Main.npc[num7].CanBeChasedBy(this))
                {
                    num7 = -1;
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }

                if (num7 == -1)
                {
                    int num8 = Projectile.FindTargetWithLineOfSight();
                    if (num8 != -1)
                    {
                        Projectile.ai[0] = num8;
                        Projectile.netUpdate = true;
                    }
                }

                if (flag2)
                {
                    int num9 = (int)Projectile.ai[0];
                    Vector2 value3 = Projectile.velocity;

                    if (Main.npc.IndexInRange(num9))
                    {
                        if (Projectile.timeLeft < 10)
                            Projectile.timeLeft = 10;

                        NPC nPC = Main.npc[num9];
                        value3 = Projectile.DirectionTo(nPC.Center) * num4;
                    }
                    else
                    {
                        Projectile.timeLeft--;
                    }

                    float amount = MathHelper.Lerp(value, value2, Utils.GetLerpValue(readyTime, 30f, Projectile.timeLeft, clamped: true));
                    Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, amount);
                    Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, clamped: true));
                }

                //Projectile.Opacity = Utils.GetLerpValue(240f, 220f, Projectile.timeLeft, clamped: true);
            }

            Projectile.rotation += 0.25f;
            Projectile.rotation = Projectile.velocity.ToRotation() + ((float)Math.PI / 2f);

            if (Projectile.timeLeft > 894)
            {
                for (int i = 0; i < 16; i++)
                    Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[15], Projectile.Center + Projectile.velocity, i / 16f);
            }
            else
            {
                for (int i = 0; i < 15; i++)
                    Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
            }

            if (Main.rand.NextBool(6))
            {
                int i = Main.rand.NextFromList(-1, 1);
                int type = Main.rand.NextFromList(DustID.PlatinumCoin, DustID.GoldCoin);
                Vector2 dir = new(i, 0);
                Dust.NewDustPerfect(Projectile.Center, type, dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.5f, 2), Scale: Main.rand.NextFloat(1, 1.5f));

            }

            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), factor => Helper.Lerp(0, 8, factor)
            , factor =>
            {
                if (factor.X < 0.7f)
                {
                    return Color.Lerp(new Color(0, 0, 0, 0), DrawColor, factor.X / 0.7f);
                }

                return Color.Lerp(DrawColor, FantasyGod.shineColor, (factor.X - 0.7f) / 0.3f);
            });

            trail.Positions = Projectile.oldPos;
        }

        public override void OnKill(int timeLeft)
        {
            Color color = DrawColor;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 vector6 = Projectile.oldPos[i];
                    if (vector6 == Vector2.Zero)
                        break;

                    Vector2 pos = Projectile.oldPos[i];
                    float num28 = MathHelper.Lerp(0.3f, 1f, Utils.GetLerpValue(Projectile.oldPos.Length, 0f, i, clamped: true));
                    int howMany = Main.rand.Next(0, 3);
                    for (float j = 0f; j < howMany; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(32, 32), DustID.RainbowMk2, null, 0, color);
                        dust.velocity *= Main.rand.NextFloat();
                        dust.noGravity = true;
                        dust.scale = 0.9f + (Main.rand.NextFloat() * 1.2f);
                        dust.fadeIn = Main.rand.NextFloat() * 1.2f * num28;
                        dust.scale *= num28;
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    Vector2 v = Helper.NextVec2Dir();
                    Dust.NewDustPerfect(Projectile.Center, DustType<NightmareStar>(), v * Main.rand.NextFloat(0.5f, 3), newColor: DrawColor, Scale: Main.rand.NextFloat(1.2f, 1.8f));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6f);
            int num45 = Projectile.FindTargetWithLineOfSight();
            if (num45 != -1)
            {
                Projectile.ai[0] = num45;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Vector2 scale = new Vector2(0.5f);
            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White, FantasyGod.shineColor,
            //    0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, new Vector2(1.5f, 2.5f), Vector2.One);

            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White, FantasyGod.shineColor,
            //    0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, new Vector2(1f, 2.25f), Vector2.One * 2);

            //float exRot = Main.GlobalTimeWrappedHourly * 2;
            //for (int i = 0; i < 4; i++)
            //{
            //    ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos + (exRot + i * MathHelper.PiOver2).ToRotationVector2() * 18, Color.White, DrawColor * 0.8f,
            //        0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, scale, Vector2.One);
            //}
            return false;
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["FantasyTentacle"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
            effect.Parameters["sampleTexture"].SetValue(NightmarePlantera.tentacleTex.Value);
            effect.Parameters["extraTexture"].SetValue(NightmareSpike.FlowTex.Value);
            effect.Parameters["flowAlpha"].SetValue(0.5f);
            effect.Parameters["warpAmount"].SetValue(3);

            trail?.Render(effect);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float rot = Projectile.rotation;
            Color shineColor = new(252, 233, 194, 255);
            //中心的闪光

            Texture2D lightTex = BaseNightmareSparkle.MainLight.Value;
            var origin = lightTex.Size() / 2;

            Color c = shineColor;
            //c.A = 0;
            var scale = new Vector2(1f, 2.25f) * 0.15f;
            Main.spriteBatch.Draw(lightTex, pos, null, c, rot, origin, scale, 0, 0);
            Main.spriteBatch.Draw(lightTex, pos, null, c * 0.5f, rot, origin, scale, 0, 0);

            Main.spriteBatch.Draw(lightTex, pos, null, c, rot + 1.57f, origin, scale * 0.5f, 0, 0);
            Main.spriteBatch.Draw(lightTex, pos, null, c * 0.5f, rot + 1.57f, origin, scale * 0.5f, 0, 0);

            Texture2D flowTex = BaseNightmareSparkle.Flow.Value;
            origin = flowTex.Size() / 2;

            Color shineC = shineColor * 0.75f;
            //shineC.A = 0;

            var scale2 = scale.X * 0.55f;
            Main.spriteBatch.Draw(flowTex, pos, null, shineC, rot + Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.3f, Projectile.rotation - Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);

            Vector2 secondScale = scale * 0.4f;
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 offsetPos = pos + (rot.ToRotationVector2() * i * 12);
                float rot3 = rot - (i * 0.3f);
                Main.spriteBatch.Draw(lightTex, offsetPos, null, c, rot3, origin, secondScale, 0, 0);
                //spriteBatch.Draw(lightTex, offsetPos, null, c, rot3, origin, secondScale, 0, 0);

                //ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + new Vector2(i * 16, 0), Color.White, shineColor * 0.6f,
                //    0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.4f, Vector2.One);
            }

            //周围一圈小星星
            for (int i = 0; i < 7; i++)
            {
                float rot2 = (Main.GlobalTimeWrappedHourly * 2) + (i * MathHelper.TwoPi / 7);
                Vector2 dir = rot2.ToRotationVector2();
                dir = pos + (dir * (18 + (factor * 4)));
                rot2 += 1.57f;
                Color phantomC = DrawColor;
                //phantomC.A = 0;
                Main.spriteBatch.Draw(lightTex, dir, null, phantomC, rot2, origin, 0.4f * 0.2f, 0, 0);
            }
        }

        //public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        //{
        //    Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;

        //    spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, DrawColor, 0, mainTex.Size() / 2, 0.15f, 0, 0);
        //}

    }
}
