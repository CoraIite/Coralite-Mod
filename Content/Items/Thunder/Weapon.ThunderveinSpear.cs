using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinSpear : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 63;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 2f;
            Item.crit = 8;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<ThunderveinSpearSlash>();

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
                    foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<ThunderveinSpearShoot>()))
                    {
                        if ((int)proj.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit)
                        {
                            for (int i = 0; i < proj.localNPCImmunity.Length; i++)
                                proj.localNPCImmunity[i] = 0;

                            proj.ai[2] = (int)BaseSilkKnifeSpecialProj.AIStates.drag;
                            Main.instance.CameraModifiers.Add(new MoveModifyer(10, 60));
                            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                                damage / 2, knockback, player.whoAmI, 10, (proj.Center - player.Center).ToRotation(), ai2: 5);

                            proj.netUpdate = true;
                        }
                        return false;
                    }

                    //生成弹幕
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearShoot>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                    return false;
                }

                if (useCount > 6)
                    useCount = 0;

                switch (useCount)
                {
                    default:
                    case 0://挥舞一下
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), damage, knockback, player.whoAmI, 0);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4://戳
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSpurt>(), damage, knockback, player.whoAmI);
                        break;
                    case 5://挥舞
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, 1);
                        break;
                    case 6://转圈
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinSpearSlash>(), (int)(damage * 1.25f), knockback, player.whoAmI, 2);
                        break;
                }
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(4)
                .AddIngredient<InsulationCortex>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ThunderveinSpearSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinSpearProj";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ThunderveinSpearSlash() : base(new Vector2(92, 96).ToRotation() - 0.05f, trailCount: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.ThunderItems + "ThunderveinBladeGradient");
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
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 135;
            Projectile.hide = true;
            trailTopWidth = 0;
            distanceToOwner = -68;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 70 * Projectile.scale;
        }

        protected override void Initializer()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Combo < 3)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 5;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 2f;
                    totalAngle = 4.2f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 62;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 44;
                    extraScaleAngle = -0.4f;
                    break;
                case 1://上挥 较圆
                    startAngle = -2.2f;
                    totalAngle = -4.8f;
                    minTime = 5;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 70 + 5;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    extraScaleAngle = 0.4f;

                    break;
                case 2://上挥 较圆
                    startAngle = 2.6f;
                    totalAngle = -8f;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 80 + 5;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 12;
                    break;
            }

            ExtraInit();
            base.Initializer();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            Slasher();
            Projectile.scale = Helper.Lerp(Projectile.scale, Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1f, 1.2f), 0.1f);
            if ((int)Timer == minTime)
            {
                switch (Combo)
                {
                    default:
                    case 0:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 1:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 2:
                        Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Owner.Center);
                        break;
                }
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Item.type == ItemType<ThunderveinBlade>())
                scale = Owner.GetAdjustedItemScale(Item);

            if (timer % 5 == 0 && timer < maxTime * 0.7f)
            {
                Vector2 pos = Projectile.Center + (RotateVec2 * Main.rand.NextFloat(20, 70) * Projectile.scale);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.05f, 0.2f));
                }
            }

            switch (Combo)
            {
                default:
                case 0:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1.1f, 1.3f);
                    distanceToOwner = -68 + (40 * Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime));

                    break;
                case 1:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1.2f, 1.5f);
                    distanceToOwner = -68 + (40 * Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime));

                    break;
                case 2:
                    Projectile.scale = scale * 1.3f;
                    distanceToOwner = -68 + (40 * Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime));
                    break;
            }
            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 100) + 150;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Projectile.scale *= 0.99f;
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
                if (VaultUtils.isServer)
                    return;

                float strength = 2;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
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
                    Effect effect = Filters.Scene["SimpleGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class ThunderveinSpearSpurt : BaseSwingProj
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinSpearProj";

        public ThunderveinSpearSpurt() : base(new Vector2(48, 46).ToRotation() - 0.08f, trailCount: 10) { }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAllAndFollowPlayer, 10);
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 135;
            Projectile.hide = true;
            trailTopWidth = -8;
            distanceToOwner = -68;
            minTime = 0;
            onHitFreeze = 12;
            useShadowTrail = true;
        }

        protected override void Initializer()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
            startAngle = Main.rand.NextFloat(-0.2f, 0.2f);
            totalAngle = 0.01f;

            maxTime = (int)(Owner.itemTimeMax * 0.4f) + 20 + 5;
            Smoother = Coralite.Instance.BezierEaseSmoother;

            Projectile.extraUpdates = 3;

            base.Initializer();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            float factor = Timer / maxTime;
            if (Timer % 6 == 0)
            {
                Vector2 pos = Projectile.Center + (RotateVec2 * Main.rand.NextFloat(20, 70) * Projectile.scale);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.05f, 0.2f));
                }
            }

            if (factor < 0.5f)
                distanceToOwner = -68 + (100 * Smoother.Smoother(factor * 2));
            else
                distanceToOwner = -68 + (100 * Smoother.Smoother((1 - factor) * 2));

            base.OnSlash();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer != 0)
            {
                return;
            }

            onHitTimer = 1;
            Owner.immuneTime += 10;
            if (VaultUtils.isServer)
            {
                return;
            }

            float strength = 2;

            if (VisualEffectSystem.HitEffect_ScreenShaking)
            {
                PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                Main.instance.CameraModifiers.Add(modifier);
            }

            Dust dust;
            float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
            Vector2 pos = Bottom + (RotateVec2 * offset);

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                    dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                    dust.noGravity = true;
                }

                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                    dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
                    dust.noGravity = true;
                }
            }
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 10; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                ThunderveinDragon.ThunderveinYellowAlpha * (0.5f - (i * 0.5f / 10)), Projectile.oldRot[i] + extraRot, origin, Projectile.scale * 1.1f, effect, 0);
        }
    }

    public class ThunderveinSpearShoot : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinSpearProj";

        public ThunderveinSpearShoot() : base(16 * 40, 40, 12, 50)
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
        }

        public override void RollingInHand()
        {
            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.Center = Owner.Center + (dir * 64);
            Projectile.velocity = dir * shootSpeed;
            Projectile.rotation = dir.ToRotation();
            HookState = (int)AIStates.shoot;
            Projectile.tileCollide = true;
            Projectile.netUpdate = true;
        }

        public override void Shoot()
        {
            if (Timer > shootTime)
            {
                Timer = 0;
                HookState = (int)AIStates.back;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            Timer++;

            Projectile.SpawnTrailDust(DustID.PortalBoltTrail, Main.rand.NextFloat(0.2f, 0.5f), newColor: ThunderveinDragon.ThunderveinYellowAlpha,
                Scale: Main.rand.NextFloat(1f, 1.5f));
        }

        public override void Dragging()
        {
            if ((int)Timer == 0)
            {
                Owner.immuneTime = 30;
                Owner.immune = true;
            }

            if ((int)Timer < 6 * 3)
            {
                Owner.velocity *= 0;
                Owner.Center = Vector2.Lerp(Owner.Center, Projectile.Center, 0.35f);
                if (Timer == 5 * 3)
                {
                    canDamage = true;
                }
            }
            else
            {
                //将玩家回弹
                SoundEngine.PlaySound(CoraliteSoundID.Thunder, Projectile.Center);
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Projectile.Center);
                Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                var modifier = new PunchCameraModifier(Owner.position, dir, 14, 8f, 6, 1000f);
                Main.instance.CameraModifiers.Add(modifier);
                Owner.velocity = new Vector2(Math.Sign(Owner.Center.X - Projectile.Center.X) * 4, -3);
                int index = Projectile.NewProjectileFromThis<ThunderFalling>(Projectile.Center + new Vector2(0, -500), Projectile.Center + new Vector2(0, 50),
                    Projectile.damage, Projectile.knockBack, 20, ai2: 60);
                Main.projectile[index].friendly = true;
                Main.projectile[index].hostile = false;
                Projectile.Kill();
            }

            Timer++;
        }

        public override void OnHookedToNPC()
        {
            if (Target < 0 || Target > Main.maxNPCs)
            {
                Projectile.Kill();
            }
            NPC npc = Main.npc[(int)Target];
            if (!npc.active || npc.dontTakeDamage || npc.Distance(Owner.Center) > onHookedLength || !Collision.CanHitLine(Owner.Center, 1, 1, npc.Center, 1, 1))
            {
                Projectile.Kill();
            }

            Projectile.Center = npc.Center + offset;
            Timer = 0;
        }

        public override void BackToOwner()
        {
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState != (int)AIStates.drag)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, Projectile.Center);
                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(50, 50), Vector2.Zero,
                        CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.4f, 0.6f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, MathHelper.PiOver4);
            return false;
        }
    }
}
