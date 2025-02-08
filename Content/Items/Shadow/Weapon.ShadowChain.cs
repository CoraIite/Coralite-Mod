using Coralite.Content.Items.Misc_Melee;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

namespace Coralite.Content.Items.Shadow
{
    public class ShadowChain : BaseSilkKnifeItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<ShadowChainSwing>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.SetWeaponValues(30, 4, 0);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<ShadowChainSpecial>()))
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
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ShadowChainSpecial>(), (int)(damage * 1.4f), knockback, player.whoAmI);
                    return false;
                }

                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, -1, combo);
                combo++;
                if (combo > 3)
                    combo = 0;
            }

            return false;
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IronSilkKnief>()
                .AddIngredient<ShadowCrystal>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    /// <summary>
    /// 使用ai0传入自身index，传入-1为玩家<br></br>
    /// 使用ai1传入combo，0：突刺，1：下挥，2：上挥，3：下挥<br></br>
    /// 如果ai0决定了不是玩家的话，那么使用ai2传入初始角度
    /// </summary>
    public class ShadowChainSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];
        public ref float OverrideStartAngle => ref Projectile.ai[2];

        public static Asset<Texture2D> handleTex;
        public static Asset<Texture2D> ChainTex;
        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public int alpha;
        public float selfExtraRot;
        public int delay;

        public ShadowChainSwing() : base(trailCount: 36)
        {

        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            handleTex = Request<Texture2D>(AssetDirectory.ShadowItems + "ShadowChainHandle");
            ChainTex = Request<Texture2D>(AssetDirectory.ShadowItems + "ShadowChainChain");
            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowItems + "ShadowChainGradient");
        }

        public override void SetSwingProperty()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.hide = true;
            minTime = 0;
            onHitFreeze = 6;
            trailTopWidth = 4;
            trailBottomWidth = 0;
            useShadowTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 50 * Projectile.scale;
        }

        protected override Vector2 OwnerCenter()
        {
            if (Main.projectile.IndexInRange((int)OwnerIndex))
            {
                Projectile p = Main.projectile[(int)OwnerIndex];
                if (p.active && p.type == ProjectileType<ShadowChainShadows>())
                    return p.Center;

                Projectile.Kill();
                return Vector2.Zero;
            }

            //Projectile.Kill();
            return base.OwnerCenter();
        }

        protected override float GetStartAngle()
        {
            if (Main.projectile.IndexInRange((int)OwnerIndex))
                return OverrideStartAngle;

            return base.GetStartAngle();
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 2;
            Projectile.scale = 0.9f;

            alpha = 0;
            spriteRotation = 1.57f;
            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
            switch ((int)Combo)
            {
                default:
                case 0://戳
                    spriteRotation = 0f;
                    maxTime = Owner.itemTimeMax + 26;
                    startAngle = -0.01f;
                    totalAngle = 0.02f;
                    distanceToOwner = 8;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 1://下挥
                    maxTime = Owner.itemTimeMax + 35;
                    startAngle = 2f;
                    totalAngle = 4.9f;
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 40, 80);
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 1f, 1.25f);
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    break;
                case 2://上挥
                    maxTime = Owner.itemTimeMax + 25;
                    startAngle = -2.5f;
                    totalAngle = -4.9f;
                    distanceToOwner = 80;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 60, 70);
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(0, maxTime - minTime)), 1f, 1.25f);
                    break;
                case 3://下挥
                    useSlashTrail = true;
                    maxTime = Owner.itemTimeMax + 14;
                    startAngle = 3f;
                    totalAngle = 5.8f;
                    distanceToOwner = Helper.EllipticalEase(3f - (5.8f * Smoother.Smoother(0, maxTime - minTime)), 80, 100);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    Projectile.scale = 1.2f;
                    delay = 16;
                    break;
                case 4://斜着
                    useSlashTrail = true;
                    maxTime = Owner.itemTimeMax + 16;
                    int reverse = Main.rand.NextFromList(-1, 1);
                    startAngle = 2.6f * reverse;
                    totalAngle = 5.2f * reverse;
                    distanceToOwner = Helper.EllipticalEase(2.6f - (5.2f * Smoother.Smoother(0, maxTime - minTime)), 30, 60);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    delay = 16;
                    break;
            }

            if (!Main.projectile.IndexInRange((int)OwnerIndex))//防止无限套娃
            {
                bool hasSpecialProj = false;
                NPC nPC = null;
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ProjectileType<ShadowChainSpecial>() && (int)proj.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit)
                    {
                        proj.ai[0]++;
                        nPC = Main.npc[(int)proj.ai[1]];
                        proj.netUpdate = true;
                        hasSpecialProj = true;
                        break;
                    }
                }

                if (hasSpecialProj)
                {
                    //生成影子弹幕
                    var source = Projectile.GetSource_FromAI();
                    float maxLength = Main.rand.NextFloat(300, 400);
                    float xxxxxxxFactor = Main.rand.NextFloat(0.9f, 1f);
                    Vector2 pos = nPC.Center + (Helper.NextVec2Dir() * maxLength / 2);
                    Vector2 vel = (nPC.Center - pos).SafeNormalize(Vector2.Zero) * maxLength / ((Owner.itemTimeMax + 16) * xxxxxxxFactor);
                    int index = Projectile.NewProjectile(source, pos, vel, ProjectileType<ShadowChainShadows>(), 1, 0, Projectile.owner);
                    Projectile.NewProjectile(source, pos, Vector2.Zero, ProjectileType<ShadowChainSwing>()
                        , Projectile.damage, 0, Projectile.owner, index, 4, vel.ToRotation());
                }
            }

            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            int type = Main.rand.Next(2) switch
            {
                0 => DustID.VilePowder,
                _ => DustID.Shadowflame,
            };
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), type,
                   dir * Main.rand.NextFloat(0.5f, 2f));
            dust.noGravity = true;
            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 50) + 200;
            int halfTime = (maxTime - minTime) / 2;

            switch ((int)Combo)
            {
                default:
                case 0:
                    if (timer < halfTime)
                        distanceToOwner += 220f / halfTime;
                    else
                        distanceToOwner -= 220f / halfTime;

                    break;
                case 1:
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.25f);
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 40, 80);

                    break;
                case 2:
                    Projectile.scale = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.25f);
                    distanceToOwner = Helper.EllipticalEase(2f - (4.9f * Smoother.Smoother(timer, maxTime - minTime)), 60, 70);

                    break;
                case 3:
                    if (timer < halfTime)
                    {
                        distanceToOwner = Helper.EllipticalEase(3f - (5.8f * Smoother.Smoother(timer, maxTime - minTime)), 80, 100);
                    }
                    else
                        distanceToOwner -= 1.6f;
                    break;
                case 4:
                    distanceToOwner = Helper.EllipticalEase(2.6f - (5.2f * Smoother.Smoother(timer, maxTime - minTime)), 30, 60);
                    break;
            }

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (Timer > minTime + maxTime + delay)
                Projectile.Kill();

            Slasher();
            if (distanceToOwner > 8)
                distanceToOwner -= 3;
            if (alpha > 0)
                alpha -= 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);

                if ((int)Combo == 0)
                {
                    //将目标向玩家拖拽
                    target.velocity += target.knockBackResist * -RotateVec2 * 16;
                }

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, 1, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = RotateVec2.RotatedBy(-1.57f * Math.Sign(totalAngle));

                    Helper.SpawnDirDustJet(target.Center, () => RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 5,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustID.Shadowflame, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: true, extraRandRot: 0.2f);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust d = Dust.NewDustPerfect(target.Center, DustID.VilePowder, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(6f, 12f),
                             Scale: Main.rand.NextFloat(1f, 2f));
                        d.noGravity = true;
                    }
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings()
                    {
                        PositionInWorld = pos,
                        MovementVector = RotateVec2 * Main.rand.NextFloat(2f, 4f),
                    });
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer > minTime)
                DrawSlashTrail();

            //把手的帧图
            Texture2D handleTex = ChainTex.Value;

            var handleOrigin = handleTex.Size() / 2;
            Vector2 ownerCenter = OwnerCenter();
            Vector2 handPos = ownerCenter + (RotateVec2 * 8);

            Main.spriteBatch.Draw(handleTex, handPos, null, lightColor, Projectile.rotation, handleOrigin, Projectile.scale, 0, 0);
            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - handPos).Length();   //链条长度

            Vector2 startPos = handPos - Main.screenPosition;//起始点

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
                                                lightColor, Projectile.rotation + extraRot, origin, Projectile.scale + 0.3f, CheckEffect(), 0f);
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 8; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                lightColor * (0.5f - (i * 0.5f / 8)), Projectile.oldRot[i] + extraRot, origin, Projectile.scale, effect, 0);
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

            Vector2 Center = GetCenter(1);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
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
                    Effect effect = Filters.Scene["StarsTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBright.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
                    effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                    effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                    effect.Parameters["uExchange"].SetValue(0.87f + (0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly)));

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
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

    public class ShadowChainSpecial : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.ShadowItems + "ShadowChainSwing";

        public ref float SpecialAttackCount => ref Projectile.ai[0];

        public ShadowChainSpecial() : base(35 * 16, 48, 28, 18) { backSpeed = 48; }

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

        public override void OnHookedToNPC()
        {
            base.OnHookedToNPC();
            if (SpecialAttackCount > 3)
                HookState = (int)AIStates.drag;
        }

        public override void Dragging()
        {
            if (Timer < 8)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.15f);
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                Projectile.Kill();
            }
            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState < 3 && (int)HookState > -1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShadowflameApparition_NPCDeath55, Projectile.Center);
                Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One);
                float length = (Owner.Center - Projectile.Center).Length();
                for (int i = 0; i < length; i += 12)
                {
                    int type = Main.rand.Next(2) switch
                    {
                        0 => DustID.VilePowder,
                        _ => DustID.Shadowflame,
                    };
                    Dust d = Dust.NewDustPerfect(Projectile.Center + (direction * i) + Main.rand.NextVector2Circular(4, 4)
                          , type, Vector2.UnitX.RotatedByRandom(3.14) * Main.rand.NextFloat(0, 2f), Scale: Main.rand.NextFloat(1f, 1.3f));
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = ShadowChainSwing.ChainTex.Value;

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
            float exRot = 0f;
            if (HookState == (int)AIStates.rolling)
            {
                exRot = MathHelper.PiOver2;
                Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

                for (int i = 1; i < 7; i += 1)
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    lightColor * (0.4f - (i * 0.4f / 7)), Projectile.oldRot[i] + 1.57f, origin, Projectile.scale, 0, 0);

            }
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + exRot, origin, Projectile.scale + 0.2f, 0, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }

    public class ShadowChainShadows : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.ai[2];

        public int maxTime;

        public override void SetDefaults()
        {
            Projectile.width = 16 * 2;
            Projectile.height = 16 * 3;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            maxTime = Main.player[Projectile.owner].itemTimeMax + 20;
            maxTime /= 3;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            do
            {
                if (Timer == 0)
                {
                    Alpha = 1;
                }
                if (Timer < 12)
                {
                    Alpha -= 0.99f / 12;
                    break;
                }

                if (Timer > maxTime - 12 && Timer < maxTime)
                {
                    Alpha += 0.99f / 12;
                }

                if (Timer > maxTime + (16 / 3))
                {
                    Projectile.velocity *= 0.95f;
                    Projectile.Kill();
                }
            } while (false);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.position, 0f, owner.fullRotationOrigin, Alpha);

            return false;
        }
    }
}
