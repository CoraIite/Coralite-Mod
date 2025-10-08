using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class LostSevensideHook : BaseSilkKnifeItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<LostSevensideSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(266, 4, 8);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, Item);

                if (player.altFunctionUse == 2)
                {
                    foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<LostSevensideChain>()))
                    {
                        if ((int)proj.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit)
                        {
                            for (int i = 0; i < proj.localNPCImmunity.Length; i++)
                                proj.localNPCImmunity[i] = 0;

                            proj.ai[2] = (int)BaseSilkKnifeSpecialProj.AIStates.drag;
                            proj.netUpdate = true;
                            Main.instance.CameraModifiers.Add(new MoveModifyer(10, 50));
                        }
                        return false;
                    }

                    //生成弹幕
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<LostSevensideChain>(), (int)(damage * 1.4f), knockback, player.whoAmI);
                    return false;
                }

                if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)//射出特殊弹幕
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<LostVine>(), (int)(damage * 4.5f), knockback,
                        player.whoAmI, -1, (0.2f + (cp.nightmareEnergy * 0.1f)) * (cp.nightmareEnergy % 2 == 0 ? -1 : 1), player.itemTimeMax * 1.5f);
                    cp.nightmareEnergy--;
                }
                else //生成弹幕
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.5f), knockback, player.whoAmI, combo);
                    combo++;
                    if (combo > 3)
                        combo = 0;
                }
            }

            return false;
        }

        public override bool MeleePrefix() => true;
    }

    public class LostSevensideSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "LostSevensideHookProj";

        public static Asset<Texture2D> ChainTex;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ref float Combo => ref Projectile.ai[0];

        public int alpha;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ChainTex = Request<Texture2D>(AssetDirectory.NightmareItems + "LostSevensideChain");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "LostSevensideGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            ChainTex = null;
            WarpTexture = null;
            GradientTexture = null;
        }

        public LostSevensideSlash() : base(1.57f, 36) { }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAllAndFollowPlayer, 8);
        }

        public override void SetSwingProperty()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 16;
            Projectile.hide = true;
            minTime = 0;
            onHitFreeze = 6;
            useSlashTrail = true;
            trailTopWidth = -8;
            trailBottomWidth = 0;
            useShadowTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 2;
            Projectile.scale = 0.9f;

            alpha = 0;

            SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
            switch ((int)Combo)
            {
                default:
                case 0:
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 20;
                    startAngle = 2f;
                    totalAngle = 4.9f;
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 40, 80);
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.1f);
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 1:
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 20;
                    startAngle = -2f;
                    totalAngle = -4.9f;
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 40, 80);
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.1f);
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 2:
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 20;
                    startAngle = 2f;
                    totalAngle = 4.9f;
                    distanceToOwner = 80;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.1f);
                    break;
                case 3:
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 22 + 15;
                    startAngle = -3f;
                    totalAngle = -12;
                    distanceToOwner = Helper.EllipticalEase(3f - (12f * Smoother.Smoother(0, maxTime - minTime)), 80, 110);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
                case 4:
                    maxTime = (int)(Owner.itemTimeMax * 0.8f) + 22 + 15;
                    startAngle = 3f;
                    totalAngle = 12;
                    distanceToOwner = Helper.EllipticalEase(3f - (12f * Smoother.Smoother(0, maxTime - minTime)), 90, 120);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
            }
            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            Dust dust = Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Circular(Projectile.width, Projectile.height) / 2), DustID.VilePowder,
                   dir * Main.rand.NextFloat(0.5f, 2f));
            dust.noGravity = true;
            int timer = (int)Timer - minTime;
            alpha = (int)(Helper.SinEase(timer, maxTime - minTime) * 200) + 50;

            switch ((int)Combo)
            {
                default:
                case 0:
                case 1:
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 0.9f, 1.1f);
                    if (timer < 24)
                        distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 40, 80);
                    else
                        distanceToOwner -= 0.6f;

                    break;
                case 2:
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 0.9f, 1.1f);
                    if (timer > 16)
                        distanceToOwner -= 1.4f;

                    break;
                case 3:
                    if ((int)Timer == 28)
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                    if (timer < 24)
                    {
                        distanceToOwner = Helper.EllipticalEase(3f - (12f * Smoother.Smoother(timer, maxTime - minTime)), 80, 110);
                    }
                    else
                        distanceToOwner -= 1.6f;
                    break;
                case 4:
                    if ((int)Timer == 28)
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                    if (timer < 24)
                    {
                        distanceToOwner = Helper.EllipticalEase(3f - (12f * Smoother.Smoother(timer, maxTime - minTime)), 90, 120);
                    }
                    else
                        distanceToOwner -= 1.8f;
                    break;
            }

            base.OnSlash();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, 2, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = RotateVec2.RotatedBy(-1.57f * Math.Sign(totalAngle));

                    Helper.SpawnDirDustJet(target.Center, () => RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 5,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustType<NightmarePetal>(), newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: false, extraRandRot: 0.2f);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust d = Dust.NewDustPerfect(target.Center, DustID.VilePowder, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(6f, 12f),
                            newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 2f));
                        d.noGravity = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer > minTime)
                DrawSlashTrail();

            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            //Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                                lightColor, Projectile.rotation + extraRot, origin, Projectile.scale + 0.2f, CheckEffect(), 0f);
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 8; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                lightColor * (1f - (i * 1f / 8)), Projectile.oldRot[i] + extraRot, origin, Projectile.scale, effect, 0);
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
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
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("SimpleGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        //Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class LostSevensideChain : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.NightmareItems + "LostSevensideHookProj";

        public LostSevensideChain() : base(42 * 16, 48, 32, 18) { backSpeed = 48; }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAllAndFollowPlayer, 7);
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void Dragging()
        {
            if ((int)Timer == 0)
            {
                Owner.itemTime = 2;
                Owner.immuneTime = 45;
                Owner.immune = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero), ProjectileType<LostSevensideSpurt>(), Projectile.damage * 4, 2, Owner.whoAmI, 0, ai2: 34);
            }

            if ((int)Timer < 8)
            {
                Owner.velocity *= 0;
                Owner.Center = Vector2.Lerp(Owner.Center, Projectile.Center, 0.25f);

                if (Timer > 0)
                {
                    Vector2 center = Owner.Center + Main.rand.NextVector2CircularEdge(64, 64) + ((Projectile.rotation + MathHelper.Pi + (Timer % 2 == 0 ? 0.45f : -0.45f) + Main.rand.NextFloat(-0.25f, 0.25f)).ToRotationVector2() * 140);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, (Owner.Center - center).SafeNormalize(Vector2.Zero) * 28, ProjectileType<LostSevensideSpurt>(), Projectile.damage, 2, Owner.whoAmI, 1, -((Timer * 3) + 14), 22);
                }
            }
            else
            {
                //将玩家继续射出
                canDamage = true;
                SoundEngine.PlaySound(CoraliteSoundID.Bloody_NPCHit9, Projectile.Center);
                Helper.PlayPitched("Misc/BloodySlash2", 0.4f, -0.2f, Projectile.Center);
                Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                var modifier = new PunchCameraModifier(Owner.position, dir, 14, 8f, 6, 1000f);
                Main.instance.CameraModifiers.Add(modifier);
                Owner.velocity = new Vector2(Math.Sign(Owner.Center.X - Projectile.Center.X) * 8, -3);
                //生成新的挥舞弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<LostSevensideSlash>(), Projectile.damage * 3, 2, Owner.whoAmI, 4);
                if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                    cp.GetNightmareEnergy(2);
                Projectile.Kill();
            }

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState < 3 && (int)HookState > -1)
            {
                //生成血液粒子和钩子gore
                SoundEngine.PlaySound(CoraliteSoundID.ShadowflameApparition_NPCDeath55, Projectile.Center);
                Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One);
                float length = (Owner.Center - Projectile.Center).Length();
                for (int i = 0; i < length; i += 12)
                {
                    Color c = Main.rand.Next(0, 2) switch
                    {
                        0 => new Color(255, 209, 252),
                        _ => new Color(218, 205, 232),
                    };
                    Dust.NewDustPerfect(Projectile.Center + (direction * i) + Main.rand.NextVector2Circular(4, 4), DustType<NightmarePetal>(), Vector2.UnitX.RotatedByRandom(3.14) * Main.rand.NextFloat(0, 2f), newColor: c, Scale: Main.rand.NextFloat(1f, 1.3f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = LostSevensideSlash.ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin2 = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin2, 0, 0);

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2;
            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 1.57f, origin, Projectile.scale + 0.2f, 0, 0);

            if (HookState == (int)AIStates.rolling)
            {
                Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

                for (int i = 1; i < 7; i += 1)
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    lightColor * (0.4f - (i * 0.4f / 7)), Projectile.oldRot[i] + 1.57f, origin, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入状态，为0时跟踪玩家，为1时向前戳出
    /// </summary>
    public class LostSevensideSpurt : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.Trails + "SlashFlatBlurVMirror";

        public ref float State => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float TrailWidth => ref Projectile.ai[2];

        public Player Owner => Main.player[Projectile.owner];

        private Trail trail;
        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public void Initialize()
        {
            Projectile.InitOldPosCache(16);
        }

        public override bool ShouldUpdatePosition() => Timer >= 0;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > 0)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new EmptyMeshGenerator(), WidthFunction, ColorFunction);

            if (Timer > 0)
            {
                Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2), DustID.PlatinumCoin,
                    Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(0.5f, 3f));
                dust.noGravity = true;

                switch (State)
                {
                    case 0:
                        {
                            if (Timer < 8)
                            {
                                Projectile.Center = Owner.Center;
                                Alpha += 1 / 8f;
                            }
                            else if (Timer == 8)
                            {
                                Projectile.velocity *= 12;
                            }
                            else if (Timer > 12)
                            {
                                Projectile.velocity *= 0.96f;
                                Alpha -= 0.1f;
                                if (Alpha < 0)
                                    Projectile.Kill();
                            }
                        }
                        break;
                    default:
                    case 1:
                        {
                            if (Timer < 10)
                            {
                                if (Alpha < 1)
                                    Alpha += 1 / 8f;
                                break;
                            }

                            Projectile.velocity *= 0.9f;
                            Alpha -= 0.15f;
                            if (Alpha < 0)
                                Projectile.Kill();
                        }
                        break;
                }

                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer < 12)
                {
                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);
                }
                else
                {
                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
                }
                trail.TrailPositions = Projectile.oldPos;
            }

            Timer++;
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.3f)
                return Helper.Lerp(0, TrailWidth, factor / 0.3f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.3f) / 0.7f);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null || Timer < 0)
                return;

            Effect effect = ShaderLoader.GetShader("AlphaGradientTrail");

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(LostSevensideSlash.GradientTexture.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.DrawTrail(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawWarp()
        {
            if (Timer < 0)
                return;

            List<ColoredVertex> bars = new();

            float w = 1f;
            Vector2 up = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 down = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - (i / 15f);
                Vector2 Center = Projectile.oldPos[i];
                float r = Projectile.rotation % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                float width = WidthFunction(factor) * 0.75f;
                Vector2 Top = Center + (up * width);
                Vector2 Bottom = Center + (down * width);

                bars.Add(new ColoredVertex(Top, new Color(dir, 0.25f, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new ColoredVertex(Bottom, new Color(dir, 0.25f, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = ShaderLoader.GetShader("KEx");

            effect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = FrostySwordSlash.WarpTexture.Value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.CurrentTechnique.Passes[0].Apply();
            if (bars.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    /// <summary>
    /// 使用ai0传入颜色，0紫色，-1红色<br></br>
    /// 使用ai1传入额外的追踪的角度<br></br>
    /// 使用ai2传入蓄力时间
    /// </summary>
    public class LostVine : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "VineSpike";

        private RotateTentacle tentacle;

        private Player Owner => Main.player[Projectile.owner];

        public float State;
        public ref float ColorState => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float ChannelTime => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public float alpha;
        private bool init = true;
        private Color tentacleColor = new(80, 86, 102);
        private Vector2 rotateVec2;
        public SpriteEffects effect;

        public static Color pink = new(233, 184, 230);

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.width = 142;
            Projectile.height = 56;
            Projectile.scale = 0.8f;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == 1)
            {
                float a = 0;
                Vector2 dir = rotateVec2 * Projectile.width / 2;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    Projectile.Center - dir, Projectile.Center + dir, 20, ref a);
            }

            return false;
        }

        public override void AI()
        {
            if (init)
            {
                //if (ColorState == 0)
                //    tentacleColor = NightmarePlantera.lightPurple;
                //else if (ColorState == -1)
                //    tentacleColor = NightmarePlantera.nightmareRed;
                //else
                //    tentacleColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Projectile.rotation = Projectile.velocity.ToRotation();
                effect = Main.rand.Next(0, 2) switch
                {
                    0 => SpriteEffects.None,
                    _ => SpriteEffects.FlipVertically
                };
                init = false;
            }

            tentacle ??= new RotateTentacle(30, factor =>
            {
                return Color.Lerp(tentacleColor * alpha, Color.Transparent, factor);
            }, factor =>
            {
                if (factor > 0.6f)
                    return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

                return Helper.Lerp(0, 25, factor / 0.6f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, Owner.Center, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(Owner.Center, Projectile.Center) / 30, 0.5f);

            switch ((int)State)
            {
                default:
                case 0: //伸出后向后蓄力并瞄准玩家
                    {
                        Owner.itemTime = Owner.itemAnimation = 2;
                        if (alpha < 1)
                        {
                            alpha += 1 / ChannelTime;
                            if (alpha > 1)
                                alpha = 1;
                        }

                        float factor = Timer / ChannelTime;
                        Vector2 center = Main.MouseWorld;
                        Vector2 dir = center - Projectile.Center + ((Angle + (Owner.Center - center).ToRotation()).ToRotationVector2() * Helper.Lerp(200, 650, factor));

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 45;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.65f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.55f);
                        Projectile.rotation = Projectile.rotation.AngleTowards((center - Projectile.Center).ToRotation(), 0.35f);

                        if (Timer > ChannelTime)
                        {
                            Timer = 0;
                            State++;
                            Projectile.velocity = rotateVec2 * 64;
                            Helper.PlayPitched("Misc/Spike", 0.8f, -0.4f, Projectile.Center);
                            var modifyer = new PunchCameraModifier(Projectile.Center, rotateVec2, 10, 6, 12, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        //for (int i = 0; i < 2; i++)
                        //{
                        Color c = Main.rand.Next(0, 1) switch
                        {
                            0 => tentacleColor,
                            _ => tentacleColor * 2f,
                        };
                        PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.2f),
                            CoraliteContent.ParticleType<SpeedLine>(), c, Main.rand.NextFloat(0.3f, 0.5f));
                        //}
                        if (Main.rand.NextBool())
                        {
                            Color c2 = Main.rand.Next(0, 1) switch
                            {
                                0 => tentacleColor,
                                _ => tentacleColor * 2f,
                            };
                            PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f),
                                CoraliteContent.ParticleType<SpeedLine>(), c2, Main.rand.NextFloat(0.3f, 0.5f));
                        }

                        if (Timer > 15)
                        {
                            State++;
                            Timer = 0;
                            Projectile.velocity *= 0;
                            alpha -= 0.2f;
                        }
                    }
                    break;
                case 2://收回并小幅度摇摆
                    {
                        if (alpha > 0)
                        {
                            alpha -= 0.015f;
                            if (alpha < 0)
                            {
                                alpha = 0;
                                Projectile.Kill();
                            }
                        }
                        float velLength = Projectile.velocity.Length();
                        if (velLength < 44)
                            velLength += 0.75f;
                        Vector2 dir = Owner.Center - Projectile.Center;
                        Vector2 dir2 = dir.SafeNormalize(Vector2.Zero);
                        Projectile.velocity = dir2 * velLength;
                        Projectile.rotation = dir2.ToRotation() + MathHelper.Pi + (0.35f * MathF.Sin(Timer * 0.2f));

                        if (dir.Length() < 50 || Timer > 180)
                            Projectile.Kill();
                    }
                    break;
            }

            rotateVec2 = Projectile.rotation.ToRotationVector2();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 selforigin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;

            tentacle?.DrawTentacle(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            Color c = tentacleColor * alpha;

            Texture2D leavesTex = VineSpike.LeavesTex.Value;
            selforigin = leavesTex.Size() / 2;
            for (int i = 2; i < 26; i += 2)
            {
                Vector2 pos2 = tentacle.points[i] - Main.screenPosition;
                float rotation = tentacle.rotates[i];
                float scale = tentacle.widthFunc(i / 26f) * 2.5f / leavesTex.Height;
                Main.spriteBatch.Draw(leavesTex, pos2, null, c * (0.75f - (i * 0.75f / 26)), rotation, selforigin, scale, effect, 0);
            }

            c = NightmarePlantera.nightmareRed * alpha;

            for (int i = 0; i < 7; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter, null,
                c * (0.4f - (i * 0.4f / 7)), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale * (1 + (i * 0.05f)), effect, 0);

            c = pink * alpha;

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale, effect, 0);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 selforigin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Color c = pink;
            c.A = (byte)(c.A * alpha);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale, effect, 0);
            c.A = (byte)(c.A * 0.4f);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale * 1.35f, effect, 0);
        }
    }
}
