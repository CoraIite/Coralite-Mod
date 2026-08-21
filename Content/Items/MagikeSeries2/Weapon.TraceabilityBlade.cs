using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.DamageClasses;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 5, 4);
            Item.DamageType = MagikeDamage.Instance;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);
            Item.shoot = ModContent.ProjectileType<TraceabilityBladeSwing>();
            Item.shootSpeed = 20;
            Item.useTime = Item.useAnimation = 20; 

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Swing2_Item7;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse==2&&MagikeHelper.TryCosumeMagike(10,Item,player))
            {
                Projectile.NewProjectile(source, position/*+(Main.MouseWorld-player.MountedCenter).SafeNormalize(Vector2.Zero)*32*/, velocity, ModContent.ProjectileType<TraceabilityBladeRollingTrail>(), damage*2, 0, player.whoAmI);

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

        public TraceabilityBladeSwing() : base(-MathHelper.PiOver2 - 0.45f, trailCount: 62) { }

        public int delay;
        public int alpha;

        public float dir;
        public float offsetLength;
        public float maxLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
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

            switch (State)
            {
                default:
                case 0://左键挥舞
                    {
                        maxTime = int.MaxValue;
                        Projectile.localNPCHitCooldown = 60;
                        Projectile.InitOldPosCache(62);
                    }
                    break;
                case 1://特殊攻击
                    {
                        maxTime = 90;
                        startAngle = -2f;
                        totalAngle = 4f;
                    }
                    break;
            }

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
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
                        _Rotation += 0.05f + f * 0.3f;
                        Slasher();
                    }
                    break;
                case 1:
                    base.OnSlash();
                    break;
            }

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
            switch (State)
            {
                default:
                case 0:
                    {

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

            if (State == 0)
            {
                base.DrawSelf(mainTex, origin, lightColor, extraRot);
            }
            else
            {
                Texture2D tex = SPattackTex.Value;
                Rectangle rect = tex.Frame(2, 1, 0, 0);

                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect,
                                                    lightColor*0.7f, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);

                rect = tex.Frame(2, 1, 1, 0);

                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect,
                                                    Color.White, Projectile.rotation + extraRot, origin, Projectile.scale, CheckEffect(), 0f);
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
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length(), Helper.Clamp(realTime/20f,0,1)*0.5f);


                        if (dir.LengthSquared() < Projectile.velocity.LengthSquared())
                        {
                            Projectile.Kill();
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
            Projectile.width = Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;

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
                Owner.Center = Projectile.Center - dir * 32*fadein;
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
            if (Timer<25)
            {
                Timer = 25;
            }

            Projectile.tileCollide = false;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Helper.DrawPrettyLine(Projectile.Opacity, 0, Projectile.Center+dir*24-Main.screenPosition, Color.White * 0.5f, Coralite.CrystallinePurple*0.5f, FadeoutFactor, 1.1f, 1, 0.9f, 0, Projectile.rotation, 3, new Vector2(2,1));

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
                    Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[roundI], Projectile.oldPos[roundI + 1], lerpFactor)+Projectile.Size/2;
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

}
