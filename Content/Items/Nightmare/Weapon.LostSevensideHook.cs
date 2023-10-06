using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class LostSevensideHook : BaseSilkKnifeItem
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<LostSevensideSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(2, 0, 0, 0));
            Item.SetWeaponValues(276, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

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
                        }
                        return false;
                    }

                    //生成弹幕
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<LostSevensideChain>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                    return false;
                }

                if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)//射出特殊弹幕
                {

                    cp.nightmareEnergy--;
                }
                else //生成弹幕
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);
                    combo++;
                    if (combo > 3)
                        combo = 0;
                }

            }

            return false;
        }
    }

    public class LostSevensideSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "LostSevensideHookProj";

        public static Asset<Texture2D> ChainTex;
        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ref float Combo => ref Projectile.ai[0];

        public int alpha;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ChainTex = Request<Texture2D>(AssetDirectory.NightmareItems + "LostSevensideChain");
            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail3");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "LostSevensideGradient");

        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            ChainTex = null;
            trailTexture = null;
            WarpTexture = null;
            GradientTexture = null;
        }

        public LostSevensideSlash() : base(1.57f,36) { }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefs()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 16;
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

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 2;
            Projectile.scale = 0.9f;

            alpha = 0;

            SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
            switch ((int)Combo)
            {
                default:
                case 0:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = 2f;
                    totalAngle = 4.9f;
                    distanceToOwner = 40;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 1:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = -2f;
                    totalAngle = -4.9f;
                    distanceToOwner = 40;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 2:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = 2f;
                    totalAngle = 4.9f;
                    distanceToOwner = 80;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    Projectile.scale = 0.9f;
                    break;
                case 3:
                    maxTime = Owner.itemTimeMax * 3;
                    startAngle = -3f;
                    totalAngle = -12;
                    distanceToOwner = 80;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
                case 4:
                    maxTime = Owner.itemTimeMax * 3;
                    startAngle = 3f;
                    totalAngle = 12;
                    distanceToOwner = 90;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
            }
            base.Initializer();
        }

        protected override void OnSlash()
        {
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height)/2, DustID.VilePowder,
                   dir * Main.rand.NextFloat(0.5f, 2f));
            dust.noGravity = true;
            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 200) + 50;

            switch ((int)Combo)
            {
                default:
                case 0:
                case 1:
                    Projectile.scale = Helper.EllipticalEase(2f - 4.9f * Smoother.Smoother(timer, maxTime - minTime), 0.9f, 1.1f);
                    if (timer < 24)
                        distanceToOwner = Helper.EllipticalEase(2f - 4.9f * Smoother.Smoother(timer, maxTime - minTime), 40, 80);
                    else
                        distanceToOwner -= 1.4f;

                    break;
                case 2:
                    Projectile.scale = Helper.EllipticalEase(2f - 4.9f * Smoother.Smoother(timer, maxTime - minTime), 0.9f, 1.1f);
                    if (timer > 16)
                        distanceToOwner -= 1.4f;

                    break;
                case 3:
                    if ((int)Timer == 28)
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                    if (timer < 24)
                    {
                        distanceToOwner = Helper.EllipticalEase(3f - 12f * Smoother.Smoother(timer, maxTime - minTime), 80, 110);
                    }
                    else
                        distanceToOwner -= 1.6f;
                    break;
                case 4:
                    if ((int)Timer == 28)
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                    if (timer < 24)
                    {
                        distanceToOwner = Helper.EllipticalEase(3f - 12f * Smoother.Smoother(timer, maxTime - minTime), 90, 120);
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
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, 2, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }
                
                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = RotateVec2.RotatedBy(-1.57f * Math.Sign(totalAngle));

                    Helper.SpawnDirDustJet(target.Center, () => RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 5,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustType<NightmarePetal>(), newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: false,extraRandRot:0.2f);

                    for (int i = 0; i < 6; i++)
                    {
                       Dust d= Dust.NewDustPerfect(target.Center, DustID.VilePowder, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(6f, 12f),
                           newColor:NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 2f));
                           d.noGravity=true;
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

            //Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            //绘制影子拖尾
            //Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 8, 1, 8, 1, 1.57f, -1);

            //绘制自己
            //Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 1.57f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(),
                                                lightColor, Projectile.rotation + extraRot, origin, Projectile.scale+0.2f, CheckEffect(), 0f);
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 8; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                lightColor * (0.5f - i * 0.5f/8), Projectile.oldRot[i] + extraRot, origin, Projectile.scale, effect, 0);
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

                Effect effect = Filters.Scene["SimpleGradientTrail"].GetShader().Shader;

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
                    //Main.graphics.GraphicsDevice.DrawUserPrimitives(1, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }   
    }

    public class LostSevensideChain : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.NightmareItems + "LostSevensideHookProj";

        public LostSevensideChain() : base(32*16, 48, 24, 16)
        {
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 48;
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
                Owner.immuneTime = 30;
                Owner.immune = true;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero), ProjectileType<LostSevensideSpurt>(), Projectile.damage * 3, 2, Owner.whoAmI, 0, ai2: 34);
            }

            if ((int)Timer < 8)
            {
                Owner.velocity *= 0;
                Owner.Center = Vector2.Lerp(Owner.Center, Projectile.Center, 0.25f);

                if (Timer > 0)
                {
                    Vector2 center = Owner.Center + Main.rand.NextVector2CircularEdge(64, 64) + (Projectile.rotation + MathHelper.Pi + (Timer % 2 == 0 ? 0.45f : -0.45f) + Main.rand.NextFloat(-0.25f, 0.25f)).ToRotationVector2() * 140;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, (Owner.Center - center).SafeNormalize(Vector2.Zero) * 28, ProjectileType<LostSevensideSpurt>(), Projectile.damage, 2, Owner.whoAmI, 1, -(Timer * 3 + 14), 22);
                }
            }
            else
            {
                //将玩家继续射出
                SoundEngine.PlaySound(CoraliteSoundID.Bloody_NPCHit9, Projectile.Center);
                Helper.PlayPitched("Misc/BloodySlash2", 0.4f, -0.2f, Projectile.Center);
                Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                var modifier = new PunchCameraModifier(Owner.position, dir, 14, 8f, 6, 1000f);
                Main.instance.CameraModifiers.Add(modifier);
                Owner.velocity = new Vector2(Math.Sign(Owner.Center.X - Projectile.Center.X) * 4, -3);
                //生成新的挥舞弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<LostSevensideSlash>(), Projectile.damage*3, 2, Owner.whoAmI, 4);
                if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                {
                    cp.nightmareEnergy += 2;
                    if (cp.nightmareEnergy>7)
                        cp.nightmareEnergy = 7;
                }
                Projectile.Kill();
            }

            Timer++;
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

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 origin = mainTex.Size() / 2;
            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 1.57f, origin, Projectile.scale+0.2f, 0, 0);

            if (HookState == (int)AIStates.rolling)
            {
                Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

                for (int i = 1; i < 8; i += 1)
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    lightColor * (0.5f - i * 0.5f / 8), Projectile.oldRot[i] + 1.57f, origin, Projectile.scale, 0, 0);
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
        public override string Texture => AssetDirectory.OtherProjectiles + "SpurtTrail2";

        public ref float State => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float TrailWidth => ref Projectile.ai[2];

        public Player Owner => Main.player[Projectile.owner];

        private Trail trail;

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

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;
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
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), WidthFunction, ColorFunction);

            if (Timer > 0)
            {
                Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2, DustID.PlatinumCoin,
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
                trail.Positions = Projectile.oldPos;
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

            Effect effect = Filters.Scene["AlphaGradientTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(TextureAssets.Projectile[Projectile.type].Value);
            effect.Parameters["gradientTexture"].SetValue(LostSevensideSlash.GradientTexture.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawWarp()
        {
            if (Timer < 0)
                return;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float w = 1f;
            Vector2 up = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Vector2 down = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = 1f - i / 15f;
                Vector2 Center = Projectile.oldPos[i];
                float r = Projectile.rotation % 6.18f;
                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                float width = WidthFunction(factor) * 0.75f;
                Vector2 Top = Center + up * width;
                Vector2 Bottom = Center + down * width;

                bars.Add(new CustomVertexInfo(Top, new Color(dir, w, 0f, 1f), new Vector3(factor, 0f, w)));
                bars.Add(new CustomVertexInfo(Bottom, new Color(dir, w, 0f, 1f), new Vector3(factor, 1f, w)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.TransformationMatrix;

            Effect effect = Filters.Scene["KEx"].GetShader().Shader;

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

    public class LostVine
    {

    }
}
