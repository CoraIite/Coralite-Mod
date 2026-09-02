using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ItemTransform;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.VanillaRework
{
    public class NightsEdgeRE0 : ModItem
    {
        public override string Texture => AssetDirectory.Vanilla + "Item_273";

        public override void SetStaticDefaults()
        {
            ItemTransformSystem.RegisterToTransformGroup(Type, ItemID.NightsEdge);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.NightsEdge);
            Item.UseSound = null;
            Item.damage = 37;
            Item.useTime = Item.useAnimation = 15;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileType<NightsEdgeRESlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, Item.type);
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }

    public class NightsEdgeRE1: NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework+Name;
    }

    public class NightsEdgeRE2 : NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework + Name;
    }

    public class NightsEdgeRE3 : NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework + Name;
    }

    public class NightsEdgeRE4 : NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework + Name;
    }

    [VaultLoaden(AssetDirectory.VanillaRework)]
    public class NightsEdgeRESlash() : BaseSwingProj_ScaledItem(trailCount: 30),IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex NightsColor0 { get; private set; }
        public static ATex NightsColor1 { get; private set; }
        public static ATex NightsColor2 { get; private set; }
        public static ATex NightsColor3 { get; private set; }
        public static ATex NightsColor4 { get; private set; }
        public static ATex NightsColorB0 { get; private set; }
        public static ATex NightsColorB1 { get; private set; }
        public static ATex NightsColorB2 { get; private set; }
        public static ATex NightsColorB3 { get; private set; }
        public static ATex NightsColorB4 { get; private set; }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        public override Texture2D GetGradient()
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsColor1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsColor2.Value;
            else if (ItemType == ItemType<NightsEdgeRE3>())
                return NightsColor3.Value;
            else if (ItemType == ItemType<NightsEdgeRE4>())
                return NightsColor4.Value;

            return NightsColor0.Value;
        }

        public static Texture2D GetBackGradient(float ItemType)
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsColorB1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsColorB2.Value;
            else if (ItemType == ItemType<NightsEdgeRE3>())
                return NightsColorB3.Value;
            else if (ItemType == ItemType<NightsEdgeRE4>())
                return NightsColorB4.Value;

            return NightsColorB0.Value;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            useSlashTrail = true;

            Projectile.hide = true;

            distanceToOwner = 4;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void InitializeSwing()
        {
            Projectile.extraUpdates = 6;
            onHitFreeze = (byte)(3 * Projectile.MaxUpdates);
            int exDamage = (int)(Projectile.damage * 0.8f);

            switch (Combo)
            {
                default:
                    Projectile.Kill();
                    return;
                case 0:
                    {
                        SetTimes(12, (int)(Owner.itemTimeMax * 0.8f), 2, 5);
                        SetAngles(0.8f, 5.2f, 1.9f);
                        SetScaleValues(1.5f,1.05f, DirSign * 0.2f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.BezierEaseSmoother;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, exDamage, Projectile.knockBack, ItemType, -1, 0);
                    }
                    break;
                case 1:
                    {
                        SetTimes(10, (int)(Owner.itemTimeMax * 0.8f), 2, 6);
                        SetAngles(3.6f, 4.2f, -1f);
                        SetScaleValues(1.5f, 1.05f, -DirSign * 0.2f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.BezierEaseSmoother;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, exDamage, Projectile.knockBack, ItemType, -1, 1);
                    }
                    break;
                case 2:
                    {
                        SetTimes(18, (int)(Owner.itemTimeMax * 0.8f), 5, 12);
                        SetAngles(-2.0f, -4.2f, 0.7f);
                        SetScaleValues(1.6f, 1.1f, -DirSign * 0.4f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.BezierEaseSmoother;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, exDamage, Projectile.knockBack, ItemType, -1, 2);
                    }
                    break;
                case 3:
                    {
                        SetTimes(26, (int)(Owner.itemTimeMax * 1.8f), 5, 20);
                        SetAngles(1.9f, 5.2f + MathHelper.TwoPi, 0.8f);
                        SetScaleValues(1.6f, 0.85f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                    }
                    break;
                case 4:
                    {
                        SetTimes(28, (int)(Owner.itemTimeMax*1.1f), 20, 19);
                        SetAngles(3.6f, 5f, -1f);
                        SetScaleValues(1.7f, 1.55f, DirSign * 0.3f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.HeavySmootherInstance;
                    }
                    break;
            }

            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            alpha = 150;
            SwingDusts(totalAngle, RotateVec2, Top,alpha);
            int currTime = (int)Timer - minTime;
            Owner.direction = recordOwnerDirection;

            switch (Combo)
            {
                default:
                    break;
                case 3:
                    {
                        if (currTime == 1)
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 3);
                        }
                        else if (currTime > (maxTime - minTime) * 0.35f && currTime < (maxTime - minTime) * 0.65f)
                        {
                            Owner.direction = recordOwnerDirection * -1;
                            if (currTime == (maxTime - minTime) / 2)
                            {
                                Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 4);
                                Projectile.StartAttack();
                            }
                        }
                    }
                    break;
                case 4:
                    {
                        if (currTime == 1)
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 5);
                        }
                        else if (currTime == (int)((maxTime - minTime) * 0.3f))
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 6);
                        }
                        else if (currTime == (int)((maxTime - minTime) * 0.6f))
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 7);
                        }
                    }
                    break;
            }

            base.OnSlash();
            SetScale();
        }

        public static void SwingDusts(float totalAngle, Vector2 RotateVec2, Vector2 top, int alpha)
        {
            if (alpha > 0 && Main.rand.NextBool(5))
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                int a = Main.rand.Next(5);

                int alpha2 = a switch
                {
                    0 => 150,
                    _ => 100
                };
                int type = a switch
                {
                    0 => DustID.Demonite,
                    _ => DustID.Shadowflame
                };
                float scale2 = a switch
                {
                    0 => Main.rand.NextFloat(1f, 2f),
                    _ => Main.rand.NextFloat(1f, 1.5f),
                };

                Dust dust = Dust.NewDustPerfect(top + (RotateVec2 * Main.rand.Next(-45, 5)), type,
                       dir * Main.rand.NextFloat(0.5f, 3f), alpha2, Scale: scale2);
                dust.noGravity = true;
            }
        }

        public override void OnBeforeOver()
        {
            Helper.PlayPitched(CoraliteSoundID.Swing_Item1, OwnerCenter());

            switch (Combo)
            {
                default:
                    break;
                case 4:
                    if ( VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Top, UnitToMouseV, 20, 2, 5, 1000));
                    }

                    break;
            }
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.damage > 10)
                Projectile.damage = (int)(Projectile.damage * 0.95f);
            HitDusts(target, this, onHitTimer == 1);
        }

        public static void HitDusts(NPC target, BaseSwingProj proj, bool hasScreeenShake)
        {
            Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(target.width / 2, target.height / 2);
            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings()
                {
                    PositionInWorld = pos
                });
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(target.Center, DustID.Shadowflame, Helper.NextVec2Dir(2, 6), Scale: Main.rand.NextFloat(1, 1.5f));
                    d.noGravity = true;
                }
            }

            if (hasScreeenShake && VisualEffectSystem.HitEffect_ScreenShaking)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(proj.Top, proj.RotateVec2, 3, 3, 5, 1000));
            }
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            Color drawColor = new(150, 130, 255, 0);
            Color shineColor = new(100, 80, 200, 255);

            switch (Combo)
            {
                default:
                    {
                        if (Timer < minTime||Timer>=maxTime)
                            return;

                        float f = (Timer-minTime) / (maxTime-minTime);
                        int i = (int)(trailCount * 0.1f);
                        Vector2 pos = GetCenter(i) + (oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.9f + trailTopWidth + oldDistanceToOwner[i]))-Main.screenPosition;
                        float rot = MathHelper.PiOver4 ;

                        Helper.DrawPrettyStarSparkle(f, 0, pos, drawColor, shineColor, f, 0, 0.3f, 0.7f, 1, rot, Vector2.One * 1.75f, new Vector2(1, 1.2f));
                    }
                    break;
                case 4:
                    {
                        if (Timer >minTime)
                            return;

                        float f =Helper.Clamp( Timer / beforeTime,0,1);
                        Vector2 pos = GetCenter(0) + RotateVec2 * f * Projectile.height * Projectile.scale;
                        float rot = MathHelper.PiOver4 + f * MathHelper.TwoPi;

                        Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0, 0.5f, 0.9f, 1, rot, Vector2.One * 2.5f, new Vector2(1, 1.2f));

                        pos -= RotateVec2 * Projectile.scale * 16;
                        Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0, 0.5f, 0.9f, 1, rot, Vector2.One * 1.5f, new Vector2(1, 1.2f));
                    }
                    break;
            }
        }

        public override Effect ApplyBottomColorShader()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split2.Value);
            effect.Parameters["gradientTexture"].SetValue(GetBackGradient(ItemType));
            return effect;
        }

        public override Effect ApplyHighlightColor()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
            effect.Parameters["gradientTexture"].SetValue(GetGradient());
            return effect;
        }

        //public override void ApplyHighlight(List<ColoredVertex> bars2)
        //{
        //    //for (int i = 0; i < 2; i++)
        //    base.ApplyHighlight(bars2);
        //}

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f,warpStrength:0.15f);
        }


        public override Color AdditiveColor(float f)
        {
            return Color.White * 0.8f * Utils.Remap(alpha, 0, 150, 0, 1);
        }
    }

    public class NightsEdgeREExSlash() : BaseSwingProj_ScaledItem(trailCount: 30), IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public bool useOffset=true;
        public Vector2 offset;

        protected override float ControlTrailBottomWidth(float factor)
        {
            return trailBottomWidth * Projectile.scale;
        }

        public override Texture2D GetGradient()
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsEdgeRESlash.NightsColor1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsEdgeRESlash.NightsColor2.Value;
            else if (ItemType == ItemType<NightsEdgeRE3>())
                return NightsEdgeRESlash.NightsColor3.Value;
            else if (ItemType == ItemType<NightsEdgeRE4>())
                return NightsEdgeRESlash.NightsColor4.Value;

            return NightsEdgeRESlash.NightsColor0.Value;
        }

        protected override void AIBefore()
        {
        }

        public override void SetSwingProperty()
        {
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            useSlashTrail = true;

            Projectile.localNPCHitCooldown = -1;
            distanceToOwner = 4;
            useTurnOnStart = false;
        }

        protected override void InitializeSwing()
        {
            Projectile.extraUpdates = 6;

            switch (Combo)
            {
                default:
                    Projectile.Kill();
                    return;
                case 0:
                    {
                        SetTimes(15, (int)(Owner.itemTimeMax * 1.3f));
                        SetAngles(-3.8f, -6.6f, 0.001f);
                        SetScaleValues(1.5f, 1.2f, -DirSign * 0.2f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 1:
                    {
                        SetTimes(13, (int)(Owner.itemTimeMax));
                        SetAngles(3.6f, 6.6f, 0.001f);
                        SetScaleValues(1.5f, 1.4f, DirSign * 0.2f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 2:
                    {
                        SetTimes(15 , (int)(Owner.itemTimeMax * 2f));
                        SetAngles(3.6f, 12.6f, 0.001f);
                        SetScaleValues(1.5f, 1.45f, -DirSign * 0.1f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 3:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax * 1.2f));
                        SetAngles(3.6f, 6.6f, 0.001f);
                        SetScaleValues(1.4f, 0.7f, -DirSign * 0.25f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        distanceToOwner = 70;
                        trailBottomWidth = 100;

                        offset = UnitToMouseV * 20;

                        useOffset = false;
                    }
                    break;
                case 4:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax * 1.2f));
                        SetAngles(-3.6f, -6.6f, 0.001f);
                        SetScaleValues(1.4f, 0.7f, DirSign * 0.25f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        distanceToOwner = 70;
                        trailBottomWidth = 100;

                        offset = UnitToMouseV * 20;

                        useOffset = false;
                    }
                    break;
                case 5:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax * 1.3f));
                        SetAngles(-3.8f, -6.6f, 0.001f);
                        SetScaleValues(1.5f, 1.1f, -DirSign * 0.2f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 6:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax));
                        SetAngles(3.6f, 6.6f, 0.001f);
                        SetScaleValues(1.5f, 1.1f, DirSign * 0.2f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 7:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax * 2f));
                        SetAngles(3.6f, 12.6f, 0.001f);
                        SetScaleValues(1.5f, 1.45f, -DirSign * 0.1f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;

            }

            base.InitializeSwing();
        }

        protected override Vector2 OwnerCenter()
        {
            return base.OwnerCenter() + offset;
        }

        protected override void BeforeSlash()
        {
            InitScale();
            offset = UnitToMouseV * 20;

            base.BeforeSlash();
        }

        public override void OnBeforeOver()
        {
            Projectile.DamageType = DamageClass.Melee;

            switch (Combo)
            {
                default:
                    break;
                case 5:
                case 7:
                    Helper.PlayPitched(CoraliteSoundID.Swing_DD2_DarkMageAttack, Owner.Center, pitch: -0.3f);
                    break;
                case 6:
                    Helper.PlayPitched(CoraliteSoundID.Swing_DD2_DarkMageAttack, Owner.Center, pitch: 0.5f);
                    break;
            }
        }

        protected override void OnSlash()
        {
            if (useOffset)
            offset += recordStartAngle.ToRotationVector2() * 0.7f * Owner.GetAttackSpeed(DamageClass.Melee);

            float currentTime = Timer - minTime;
            float f = currentTime / (maxTime - minTime);
            if (f <= 0.5f)
                f = Helper.SqrtEase(f / 0.5f);
            else
                f = 1 - Helper.X2Ease((f - 0.5f) / 0.5f);

            alpha = -400 + (int)(550 * MathF.Sin(f * MathHelper.PiOver2));
            if (alpha < 0)
                alpha = 0;

            switch (Combo)
            {
                default:
                    break;
                case 2:
                    if (currentTime == (maxTime - minTime) / 2)
                    {
                        Projectile.StartAttack();
                    }
                    break;
            }

            NightsEdgeRESlash.SwingDusts(totalAngle, RotateVec2, Top,alpha);
            base.OnSlash();
            SetScale();
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + Delay)
            {
                Projectile.Kill();
            }
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
                UpdateCaches();
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.damage > 10)
                Projectile.damage = (int)(Projectile.damage * 0.95f);

            NightsEdgeRESlash.HitDusts(target, this, false);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Timer <= minTime || oldLength == null)
            {
                return;
            }

            float f = alpha / 150f;
            int i = (int)(trailCount*0.3f);
            Vector2 pos = GetCenter(i) + (oldRotate[i].ToRotationVector2() * (oldLength[i]*0.9f + trailTopWidth + oldDistanceToOwner[i]));
            Color drawColor = new(150, 130, 255, 0);
            Color shineColor = new(100, 80, 200, 255);

            Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0, 1f, 1f, 2, MathHelper.PiOver4, Vector2.One * 2.5f, new Vector2(1, 1.2f));

            i =  (int)(trailCount * 0.4f);
            pos = GetCenter(i) + (oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.9f + trailTopWidth + oldDistanceToOwner[i]));
            Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0, 1f, 1f, 2, MathHelper.PiOver4, Vector2.One * 1.5f, new Vector2(1, 1.2f));
        }

        public override Effect ApplyBottomColorShader()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.ClawSlashConnect.Value);
            effect.Parameters["gradientTexture"].SetValue(NightsEdgeRESlash.GetBackGradient(ItemType));
            return effect;
        }

        public override Effect ApplyHighlightColor()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.ClawSlashConnect.Value);
            effect.Parameters["gradientTexture"].SetValue(GetGradient());
            return effect;
        }

        public override Color AdditiveColor(float f)
        {
            return Color.White * 0.8f * Utils.Remap(alpha, 0, 150, 0, 1);
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f, warpStrength: 0.15f);
        }
    }
}
