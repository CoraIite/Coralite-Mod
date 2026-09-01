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
            Item.damage = 65;
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
    }

    public class NightsEdgeRE1: NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework+Name;

        public override void SetStaticDefaults()
        {
            ItemTransformSystem.RegisterToTransformGroup(Type, ItemID.NightsEdge);
        }
    }

    public class NightsEdgeRE2 : NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework + Name;

        public override void SetStaticDefaults()
        {
            ItemTransformSystem.RegisterToTransformGroup(Type, ItemID.NightsEdge);
        }
    }

    [VaultLoaden(AssetDirectory.VanillaRework)]
    public class NightsEdgeRESlash() : BaseSwingProj_ScaledItem(trailCount: 30),IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex NightsColor0 { get; private set; }
        public static ATex NightsColor1 { get; private set; }
        public static ATex NightsColor2 { get; private set; }
        public static ATex NightsColorB0 { get; private set; }
        public static ATex NightsColorB1 { get; private set; }
        public static ATex NightsColorB2 { get; private set; }

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

            return NightsColor0.Value;
        }

        public static Texture2D GetBackGradient(float ItemType)
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsColorB1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsColorB2.Value;

            return NightsColorB0.Value;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            onHitFreeze = 6;
            useSlashTrail = true;

            Projectile.hide = true;

            distanceToOwner = 4;
        }

        protected override void InitializeSwing()
        {
            InitDirection();
            Projectile.extraUpdates = 6;

            switch (Combo)
            {
                default:
                    Projectile.Kill();
                    return;
                case 0:
                    {
                        SetTimes(12 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax * 0.8f) * Projectile.MaxUpdates, Projectile.MaxUpdates * 5, 5 * Projectile.MaxUpdates);
                        startAngle = 0.8f;
                        BeforeAngle = 1.9f;
                        extraScaleAngle =DirSign* 0.2f;
                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        totalAngle = 5.2f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.4f;
                        yScale = 1f;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 0);
                    }
                    break;
                case 1:
                    {
                        SetTimes(7 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax * 0.8f) * Projectile.MaxUpdates, Projectile.MaxUpdates * 10, 4 * Projectile.MaxUpdates);

                        startAngle = 2.6f;
                        BeforeAngle = 0.1f;
                        extraScaleAngle = -DirSign * 0.2f;
                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        totalAngle = 5.2f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.4f;
                        yScale = 1f;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 1);
                    }
                    break;
                case 2:
                    {
                        SetTimes(11 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax * 0.8f) * Projectile.MaxUpdates, Projectile.MaxUpdates * 15, 5 * Projectile.MaxUpdates);

                        startAngle = -2.6f;
                        BeforeAngle = 0.1f;
                        extraScaleAngle = -DirSign * 0.4f;
                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        totalAngle = -5.2f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.5f;
                        yScale = 1f;

                        Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 2);
                    }
                    break;
            }

            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            alpha = 150;
            SwingDusts(totalAngle, RotateVec2, Top,alpha);
            base.OnSlash();
            SetScale();
        }

        public static void SwingDusts(float totalAngle, Vector2 RotateVec2, Vector2 top, int alpha)
        {
            if (alpha > 0 && Main.rand.NextBool(7))
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                HitDusts(target);
            }
        }

        public static void HitDusts(NPC target)
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

        public Vector2 offset;

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 60 * Projectile.scale;
        }

        public override Texture2D GetGradient()
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsEdgeRESlash.NightsColor1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsEdgeRESlash.NightsColor2.Value;

            return NightsEdgeRESlash.NightsColor0.Value;
        }

        protected override void AIBefore()
        {
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            useSlashTrail = true;

            Projectile.localNPCHitCooldown = -1;
            distanceToOwner = 4;
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
                        SetTimes(15 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax * 1.3f) * Projectile.MaxUpdates);

                        startAngle = -3.8f;
                        totalAngle = -6.6f;
                        BeforeAngle = 0.001f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.5f;
                        yScale = 1.2f;
                        extraScaleAngle = -DirSign * 0.2f;

                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 1:
                    {
                        SetTimes(13 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax) * Projectile.MaxUpdates);

                        startAngle = 3.6f;
                        totalAngle = 6.6f;
                        BeforeAngle = 0.001f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.5f;
                        yScale = 1.4f;
                        extraScaleAngle = DirSign * 0.2f;

                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 2:
                    {
                        SetTimes(15 * Projectile.MaxUpdates, (int)(Owner.itemTimeMax * 2f) * Projectile.MaxUpdates);

                        startAngle = 3.6f;
                        totalAngle = 12.6f;
                        BeforeAngle = 0.001f;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.5f;
                        yScale = 1.4f;
                        extraScaleAngle = DirSign * 0.2f;

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

        protected override void OnSlash()
        {
            offset += recordStartAngle.ToRotationVector2() * 0.7f;

            float currentTime = Timer - minTime;
            float f = currentTime / (maxTime - minTime);
            if (f <= 0.5f)
                f = Helper.SqrtEase(f / 0.5f);
            else
                f = 1 - Helper.X2Ease((f - 0.5f) / 0.5f);

            alpha = -300 + (int)(450 * MathF.Sin(f * MathHelper.PiOver2));
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                NightsEdgeRESlash.HitDusts(target);
            }
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
