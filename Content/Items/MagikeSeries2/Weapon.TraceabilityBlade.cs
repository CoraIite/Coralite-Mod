using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.DamageClasses;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class TraceabilityBlade : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();

        public int energy;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(75, 5, 4);
            Item.DamageType = MagikeDamage.Instance;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);
            Item.shoot = ModContent.ProjectileType<TraceabilityBladeSwing>();
            Item.shootSpeed = 20;
            Item.useTime = Item.useAnimation = 30; 

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TraceabilityBladeTag>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<TraceabilityBladeTag>()
                    , 0, 0, player.whoAmI);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 &&energy>0)
            {
                energy--;
                Projectile.NewProjectile(source, position/*+(Main.MouseWorld-player.MountedCenter).SafeNormalize(Vector2.Zero)*32*/, velocity, ModContent.ProjectileType<TraceabilityBladeRollingTrail>(), damage * 2, 0, player.whoAmI);

                Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

                return false;
            }

            int p = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TraceabilityBladeController>(), 0, 0, player.whoAmI);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, p, 0);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    public class TraceabilityBladeSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        [VaultLoaden("{@classPath}" + "TraceabilityBladeSPattack")]
        public static ATex SPattackTex { get; set; }

        [VaultLoaden("{@classPath}" + "TraceabilityBladeGradient")]
        public static ATex GradientTexture { get; set; }

        public TraceabilityBladeSwing() : base(-MathHelper.PiOver2 - 0.45f, trailCount: 50) { }

        public int delay;
        public int alpha;

        public int direction;
        public float dir;
        public float offsetLength;
        public float maxLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 60;
            Projectile.height = 95;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
            Projectile.localNPCHitCooldown = 45;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            //if (OwnerIndex.GetProjectileOwner(out Projectile proj, Projectile.Kill))
            //{
            //    dir = Projectile.velocity.ToRotation();//(Projectile.Center - npc.Center).ToRotation();
            //    maxLength = Vector2.Distance(Main.player[proj.target].Center, proj.Center) + 140;

            //    if (maxLength > 480)
            //        maxLength = 480;
            //}
            Projectile.extraUpdates = 2;
            alpha = 0;
            minTime = 0;
            Smoother = Coralite.Instance.BezierEaseSmoother;
            distanceToOwner = -Projectile.height / 2;
            Projectile.InitOldPosCache(50);

            switch (State)
            {
                default:
                case 0://左键挥舞
                    {
                        direction = Owner.direction;
                        maxTime = int.MaxValue;
                        trailTopWidth = 8;

                        base.InitializeSwing();
                    }
                    break;
                case 1://特殊攻击
                case 2:
                case 3:
                    {
                        trailTopWidth = 0;

                        minTime = (int)(Projectile.MaxUpdates * State) * 20;

                        maxTime = 60 + minTime;
                        //int dir = Main.rand.NextFromList(-1, 1);

                        startAngle = 1.5f;//*dir;
                        totalAngle = 8f;

                        delay = 10;

                        Projectile.velocity *= 0f;
                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            _Rotation = startAngle = GetStartAngle() - (startAngle);//设定起始角度
                            Projectile.netUpdate = true;
                            onStart = false;
                            netSendBasicValues = true;
                            init = false;
                        }

                        Slasher();
                        Smoother.ReCalculate(maxTime - minTime);

                        if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
                        {
                            oldRotate ??= new float[trailCount];
                            oldDistanceToOwner ??= new float[trailCount];
                            oldLength ??= new float[trailCount];
                            InitializeCaches();
                        }

                    }
                    break;
            }
        }

        protected override float GetStartAngle()
        {
            switch (State)
            {
                default:
                case 0:
                    break;
                case 1:
                case 2:
                case 3:
                    {
                        if (!OwnerIndex.GetProjectileOwner(out Projectile proj, Projectile.Kill))
                            return base.GetStartAngle();

                        return proj.velocity.ToRotation();
                    }
            }

            return base.GetStartAngle();
        }

        protected override void AIBefore()
        {
            if (State==0)
                Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 1f);
            else
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            if (!OwnerIndex.GetProjectileOwner(out Projectile proj, Projectile.Kill))
            {
                return;
            }

            if (alpha < 255)
            {
                alpha += 10;
                if (alpha > 255)
                {
                    alpha = 255;
                }
            }
            //int timer = (int)Timer - minTime;

            //if (timer % 30 == 0)
            //    onHitTimer = 0;

            //alpha = (int)(Helper.SinEase(timer, maxTime) * 255);
            //if (timer < maxTime / 2)
            //    offsetLength = Helper.SqrtEase(timer, maxTime / 2) * maxLength;
            //else
            //    offsetLength = Helper.SinEase(timer, maxTime) * maxLength;

            switch (State)
            {
                default:
                case 0:
                    {
                        float f = (1 - Helper.SqrtEase(Helper.Clamp(proj.velocity.Length() / 30, 0, 1)));
                        _Rotation += (0.05f + f * 0.3f) * direction;
                        Slasher();
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    base.OnSlash();
                    break;
            }
        }

        protected override void BeforeSlash()
        {
            base.BeforeSlash();
            if (State != 0)
            {
                Projectile.Center = OwnerCenter();
            }
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 20;
            if (Projectile.scale > 0.8f)
                Projectile.scale *= 0.999f;

            Slasher();
            if (Timer > maxTime + delay)
            {
                if (OwnerIndex.GetProjectileOwner(out Projectile proj))
                    proj.Kill();

                Projectile.Kill();
            }
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (useShadowTrail || useSlashTrail)
            {
                UpdateCaches();
                UpdateOldPosCaches();
            }
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float rot = (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f);

            Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, 0.5f);

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos2 = pos + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 1f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos2, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            var p = PRTLoader.NewParticle<TraceabilityBladeParticle>(pos, rot.ToRotationVector2() * Main.rand.NextFloat(1, 3), Coralite.CrystallinePurple, Main.rand.NextFloat(0.7f, 1f));
            var p2 = PRTLoader.NewParticle<TraceabilityBladeEffect>(pos, rot.ToRotationVector2() * Main.rand.NextFloat(1, 3), Color.White, 0.7f);
            
            p.Rotation = rot;
            p2.Rotation = rot;

            Projectile.damage = (int)(Projectile.damage * 0.95f);

            switch (State)
            {
                default:
                    break;
                case 0:
                    {
                        if (OwnerIndex.GetProjectileOwner(out Projectile proj, Projectile.Kill) && proj.ai[1]<30)
                        {
                            proj.velocity *= 0.5f;
                            if (proj.ai[1]<20)
                            {
                                proj.ai[1] = 20;
                            }
                        }

                        if (Item.ModItem is TraceabilityBlade blade && blade.energy < 3)
                        {
                            blade.energy++;
                        }
                    }
                    break;
            }
        }

        public void UpdateOldPosCaches()
        {
            for (int i = oldRotate.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Top;
        }

        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetProjectileOwner(out Projectile proj, Projectile.Kill))
            {
                return proj.Center;
            }

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            //base.DrawSelf(SPattackTex.Value, origin, Color.White * (alpha / 255f), extraRot);
            float a = alpha / 255f;
            if (State == 0)
            {
                base.DrawSelf(mainTex, origin, lightColor* a, extraRot);
            }
            else
            {
                Texture2D tex = SPattackTex.Value;
                Rectangle rect = tex.Frame(2, 1, 0, 0);

                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect,
                                                    lightColor * 0.6f * a, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);

                rect = tex.Frame(2, 1, 1, 0);

                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect,
                                                    lightColor * a, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);
            }
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);
            Vector2 Center = OwnerCenter();

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);

                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center /*+ (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]))*/
                    - (oldRotate[i].ToRotationVector2() * (ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);
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
    }

    public class TraceabilityBladeController : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Targetr => ref Projectile.ai[2];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 16;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            if (State==1)
            {
                return Timer >= 0;
            }

            return base.ShouldUpdatePosition();
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://转一转然后回归玩家
                    {
                        if (Timer < 20)
                        {
                            break;
                        }

                        if (Timer < 20 + 10)
                        {
                            Projectile.velocity *= 0.9f;
                            break;
                        }

                        float realTime = Timer - 20 - 10;

                        Vector2 dir = Owner.Center - Projectile.Center;
                        Projectile.velocity += dir.SafeNormalize(Vector2.Zero) * (1f + realTime * 0.02f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length(), Helper.Clamp(realTime / 20f, 0, 1) * 0.5f);
                        Projectile.tileCollide = false;

                        if (dir.LengthSquared() < Projectile.velocity.LengthSquared())
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
                case 1:
                    {
                        Projectile.tileCollide = false;

                        if (Timer<0)
                        {
                            if (Targetr.GetNPCOwner(out NPC target,Projectile.Kill))
                            {
                                if (Projectile.localAI[0] == 0)
                                {
                                    Projectile.localAI[0] = 1;
                                    Vector2 dir = Projectile.Center- target.Center;
                                    Projectile.localAI[1] = dir.X;
                                    Projectile.localAI[2] = dir.Y;
                                }

                                Projectile.Center = target.Center + new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
                            }
                        }
                    }
                    break;
            }

            Timer++;
        }
    }

    public class TraceabilityBladeRollingTrail : BaseHeldProj
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + nameof(TraceabilityBladeSwing);

        public ref float Timer => ref Projectile.ai[0];
        public ref float FadeoutFactor => ref Projectile.ai[1];
        public ref float MaxtimeLeft => ref Projectile.ai[2];

        private Vector2 dir;

        public override void SetStaticDefaults()
        {
            Helper.QuickTrailSets(Type, Helper.TrailingMode.RecordAll, 20);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - dir * 20, Projectile.Center + dir * 80, 40, ref a);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                dir = UnitToMouseV;
            }

            SetHeld();
            Owner.itemTime = Owner.itemAnimation = 2;

            const float dashTime = 25;
            const float totalTime = 45;

            float fadein = Utils.Remap(Timer, 0, 5, 0, 1f);
            float fadeout = Utils.Remap(Timer, dashTime, totalTime, 1f, 0f);

            Projectile.Opacity = fadein * fadeout;
            FadeoutFactor = fadeout;
            Projectile.rotation = dir.ToRotation();

            Timer++;
            Projectile.velocity = dir * fadein * fadeout * 12;
            if (Timer < dashTime)
            {
                Owner.Center = Projectile.Center - dir * 32 * fadein;
                Owner.velocity = dir * 12;
            }
            else
            {
                Vector2 pos = Owner.MountedCenter + dir * 32;
                Projectile.Center = pos;
                if (Timer > totalTime)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer < 25)
            {
                Timer = 25;
            }

            Projectile.tileCollide = false;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float rot = (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f);

            Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, 0.5f);

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos2 = pos + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 1f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos2, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            var p = PRTLoader.NewParticle<TraceabilityBladeParticle>(pos, rot.ToRotationVector2() * Main.rand.NextFloat(1, 3), Coralite.CrystallinePurple, Main.rand.NextFloat(0.8f, 1f));
            var p3 = PRTLoader.NewParticle<TraceabilityBladeEffect>(pos, rot.ToRotationVector2() * Main.rand.NextFloat(1, 3), Color.White, 0.7f);

            p.Rotation = rot;
            p3.Rotation = rot;


            if (Timer < 25)
            {
                Timer = 25;
                Projectile.tileCollide = false;
                Owner.velocity.X = -MathF.Sign(dir.X) * Math.Clamp(MathF.Abs(dir.X), 0.4f, 1f) * 8;
                Owner.velocity.Y = -3;

                Owner.AddImmuneTime(ImmunityCooldownID.General, 25);
                Owner.immune = true;

                if (MagikeHelper.TryCosumeMagike(10, Item, Owner))
                {
                    float rot2 = Main.rand.NextFloat(MathHelper.TwoPi);
                    int damage = (int)(Projectile.damage * 1.35f);

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = rot2.ToRotationVector2();
                        Vector2 pos3 = target.Center + dir * 200;

                        int p2 = Projectile.NewProjectileFromThis<TraceabilityBladeController>(pos3, -dir * 13, 0, 0, 1, -(i + 1) * 20, target.whoAmI);

                        Projectile.NewProjectileFromThis<TraceabilityBladeSwing>(pos3, Vector2.Zero, damage, 0, p2, i + 1);

                        rot2 += MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.4f, 0.4f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Helper.DrawPrettyLine(Projectile.Opacity, 0, Projectile.Center + dir * 24 - Main.screenPosition, Color.White * 0.5f, Coralite.CrystallinePurple * 0.5f, FadeoutFactor, 1.1f, 1, 0.9f, 0, Projectile.rotation, 3, new Vector2(2, 1));

            Projectile.QuickDraw(lightColor, -MathHelper.PiOver2 - 0.45f);

            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int j = -1; j < 2; j += 2)
            {
                Vector2 lastTrailPos = Vector2.Zero;
                float innerRot = 0;
                float mult = 24;
                float maxRadius = 32f;
                float minRadius = 6f;
                int total = (int)(Projectile.oldPos.Length * mult - mult);
                Vector2 scale = new Vector2(0.4f, 0.2f) * 0.25f * Projectile.scale;
                for (int i = 0; i < total - 1; i++)
                {
                    var roundI = (int)(i / mult);
                    if (Projectile.oldPos[roundI] == Vector2.Zero || Projectile.oldPos[roundI + 1] == Vector2.Zero)
                        continue;

                    float factor = 1 - (float)i / total;
                    float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                    float radius = Utils.Remap(factor, 0, 1, minRadius, maxRadius);
                    Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[roundI], Projectile.oldPos[roundI + 1], lerpFactor) + Projectile.Size / 2;
                    float oldrot = MathHelper.Lerp(Projectile.oldRot[roundI], Projectile.oldRot[roundI + 1], lerpFactor);
                    float phase = (float)(-i * 0.025f - Projectile.timeLeft * 0.35f + Main.timeForVisualEffects * (0.04f + j * 0f));
                    float phaseoffset = phase + (j > 0 ? MathHelper.Pi : 0);
                    float fake3dAlpha = phaseoffset % MathHelper.TwoPi < MathHelper.Pi ? Utils.Remap(MathF.Abs(MathF.Cos(phaseoffset)), 0f, 1f, 0f, 1f) : 1f;
                    float y = MathF.Cos(phase) * j;

                    Vector2 dir = (oldrot + MathHelper.PiOver2).ToRotationVector2() * y;

                    float fadein = Utils.Remap(factor, 0.7f, 1f, 1f, 0f);
                    float fadeinFactor = MathHelper.Lerp(1f, fadein, FadeoutFactor);
                    Vector2 trailPos = oldpos + dir * radius * fadeinFactor;
                    var normalDir = lastTrailPos - trailPos;
                    innerRot -= 0.018f;
                    lastTrailPos = trailPos;


                    if (i == 0)
                        continue;

                    float alpha = factor * Projectile.Opacity * fake3dAlpha;
                    Color drawColor = j < 0 ? Coralite.CrystallinePurple : new Color(134, 156, 255);
                    Main.spriteBatch.Draw(star, trailPos - Main.screenPosition, null, drawColor with { A = 0 } * alpha, normalDir.ToRotation() + MathHelper.PiOver2, star.Size() / 2, scale, 0, 0);
                }
            }

            return false;
        }
    }

    public class TraceabilityBladeTag : BaseHeldProj
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float RecordCount => ref Projectile.ai[0];

        private float[] scales = new float[3];
        private float[] alphas = new float[3];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            if (Item.type != ModContent.ItemType<TraceabilityBlade>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = Owner.Center;
            if (Item.ModItem is TraceabilityBlade blade)
            {
                int count = blade.energy;

                for (int i = 0; i < 3; i++)
                {
                    if (i < count)
                    {
                        if (scales[i] < 1)
                        {
                            scales[i] += 0.08f;
                            if (scales[i] > 1)
                                scales[i] = 1;
                        }
                        if (alphas[i] < 1)
                        {
                            alphas[i] += 0.08f;
                            if (alphas[i] > 1)
                                alphas[i] = 1;
                        }
                    }
                    else
                    {
                        scales[i] = 0;
                        alphas[i] = 0;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Item.ModItem is TraceabilityBlade blade)
            {
                Texture2D mainTex = Projectile.GetTextureValue();

                var origin = new Vector2(0, mainTex.Height);
                var pos = Projectile.Center - Main.screenPosition + new Vector2(-Owner.direction * 20, -8);
                int howMany = blade.energy;
                float rotation = -0.785f - (Owner.direction * 0.5f);
                float scale = 1;
                for (int i = 0; i < howMany; i++)
                {
                    float scale2 = scales[i] * scale;
                    Vector2 offset = ((i * MathHelper.TwoPi / 3) + Main.GlobalTimeWrappedHourly).ToRotationVector2() * 4;
                    Main.spriteBatch.Draw(mainTex, pos + offset, null, lightColor * alphas[i], rotation, origin, scale2, 0, 0);
                    Main.spriteBatch.Draw(mainTex, pos + offset, null, Color.White * 0.3f * alphas[i], rotation, origin, scale2, 0, 0);

                    pos += new Vector2(Owner.direction * 4, 8);
                    rotation -= Owner.direction * 0.6f;
                    scale *= 0.8f;
                }
            }

            return false;
        }
    }

    public class TraceabilityBladeParticle : Particle
    {
        public override string Texture => AssetDirectory.Sparkles + "ShotLineSPA";

        public Vector2 scale;

        public override void AI()
        {
            Opacity++;
            if (Opacity <= 6)
            {
                scale = Vector2.Lerp(Vector2.Zero, new Vector2(0.5f, 1f), Opacity / 6f);
            }
            else if (Opacity <= 6 + 7)
            {
                scale = Vector2.Lerp(new Vector2(0.5f, 1f), new Vector2(1, 0), (Opacity - 6) / 7f);
            }
            else
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, scale * Scale, 0, 0);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color.White with { A = 0 }, Rotation, TexValue.Size() / 2, scale * Scale * 0.6f, 0, 0);

            return false;
        }
    }

    public class TraceabilityBladeEffect() : BaseFrameParticle(1, 5, 3,randRot:false)
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override Color GetColor()
        {
            return Color.White;
        }
    }
}
