using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ItemTransform;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
            Item.damage = 40;
            Item.useTime = Item.useAnimation = 16;

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

    public abstract class NightsEdgeREModed : NightsEdgeRE0
    {
        public override string Texture => AssetDirectory.VanillaRework + Name;
    }

    public class NightsEdgeRE1 : NightsEdgeREModed { }

    public class NightsEdgeRE2 : NightsEdgeREModed { }

    public class NightsEdgeRE3 : NightsEdgeREModed { }

    public class NightsEdgeRE4 : NightsEdgeREModed { }

    public class NightsEdgeRE5 : NightsEdgeREModed { }

    public class NightsEdgeRE6 : NightsEdgeREModed { }

    public class NightsEdgeRE7 : NightsEdgeREModed { }

    public class NightsEdgeRE8 : NightsEdgeREModed { }

    public class NightsEdgeRE9 : NightsEdgeREModed { }

    public class NightsEdgeREa0 : NightsEdgeREModed { }

    public class NightsEdgeREa1 : NightsEdgeREModed { }

    public class NightsEdgeREa2 : NightsEdgeREModed { }

    public class NightsEdgeREa3 : NightsEdgeREModed { }

    [VaultLoaden(AssetDirectory.VanillaRework)]
    public class NightsEdgeRESlash() : BaseSwingProj_ScaledItem(trailCount: 30), IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        private bool canTeleport = false;

        public static ATex NightsColor0 { get; private set; }
        public static ATex NightsColor1 { get; private set; }
        public static ATex NightsColor2 { get; private set; }
        public static ATex NightsColor3 { get; private set; }
        public static ATex NightsColor4 { get; private set; }
        public static ATex NightsColor5 { get; private set; }
        public static ATex NightsColor6 { get; private set; }
        public static ATex NightsColor7 { get; private set; }
        public static ATex NightsColor8 { get; private set; }
        public static ATex NightsColor9 { get; private set; }
        public static ATex NightsColora0 { get; private set; }
        public static ATex NightsColora1 { get; private set; }
        public static ATex NightsColora3 { get; private set; }

        public static ATex NightsColorB0 { get; private set; }
        public static ATex NightsColorB1 { get; private set; }
        public static ATex NightsColorB2 { get; private set; }
        public static ATex NightsColorB3 { get; private set; }
        public static ATex NightsColorB4 { get; private set; }
        public static ATex NightsColorB5 { get; private set; }
        public static ATex NightsColorB6 { get; private set; }
        public static ATex NightsColorB7 { get; private set; }
        public static ATex NightsColorB8 { get; private set; }
        public static ATex NightsColorB9 { get; private set; }
        public static ATex NightsColorBa0 { get; private set; }
        public static ATex NightsColorBa1 { get; private set; }
        public static ATex NightsColorBa3 { get; private set; }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 60 * Projectile.scale;
        }

        public override Texture2D GetGradient()
        {
            return GetGradientNew((int)ItemType);
        }

        public static Texture2D GetGradientNew(int ItemType)
        {
            if (ItemType == ItemType<NightsEdgeRE1>())
                return NightsColor1.Value;
            else if (ItemType == ItemType<NightsEdgeRE2>())
                return NightsColor2.Value;
            else if (ItemType == ItemType<NightsEdgeRE3>())
                return NightsColor3.Value;
            else if (ItemType == ItemType<NightsEdgeRE4>())
                return NightsColor4.Value;
            else if (ItemType == ItemType<NightsEdgeRE5>())
                return NightsColor5.Value;
            else if (ItemType == ItemType<NightsEdgeRE6>())
                return NightsColor6.Value;
            else if (ItemType == ItemType<NightsEdgeRE7>())
                return NightsColor7.Value;
            else if (ItemType == ItemType<NightsEdgeRE8>())
                return NightsColor8.Value;
            else if (ItemType == ItemType<NightsEdgeRE9>())
                return NightsColor9.Value;
            else if (ItemType == ItemType<NightsEdgeREa0>())
                return NightsColora0.Value;
            else if (ItemType == ItemType<NightsEdgeREa1>())
                return NightsColora1.Value;
            else if (ItemType == ItemType<NightsEdgeREa3>())
                return NightsColora3.Value;

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
            else if (ItemType == ItemType<NightsEdgeRE5>())
                return NightsColorB5.Value;
            else if (ItemType == ItemType<NightsEdgeRE6>())
                return NightsColorB6.Value;
            else if (ItemType == ItemType<NightsEdgeRE7>())
                return NightsColorB7.Value;
            else if (ItemType == ItemType<NightsEdgeRE8>())
                return NightsColorB8.Value;
            else if (ItemType == ItemType<NightsEdgeRE9>())
                return NightsColorB9.Value;
            else if (ItemType == ItemType<NightsEdgeREa0>())
                return NightsColorBa0.Value;
            else if (ItemType == ItemType<NightsEdgeREa1>())
                return NightsColorBa1.Value;
            else if (ItemType == ItemType<NightsEdgeREa3>())
                return NightsColorBa3.Value;

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
            int exDamage = (int)(Projectile.damage * 0.7f);

            switch (Combo)
            {
                default:
                    Projectile.Kill();
                    return;
                case 0:
                    {
                        SetTimes(12, (int)(Owner.itemTimeMax * 0.8f), 2, 5);
                        SetAngles(0.8f, 5.2f, 1.9f);
                        SetScaleValues(1.5f, 1.05f, DirSign * 0.2f);

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
                        SetAngles(-2.0f, -4.6f, 0.7f);
                        SetScaleValues(1.7f, 1.1f, -DirSign * 0.5f);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.HeavySmootherInstance;

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
                        SetTimes(26, (int)(Owner.itemTimeMax * 1.1f), 20, 19);
                        SetAngles(3.6f, 5f, -1f);
                        SetScaleValues(1.7f, 1.55f, DirSign * 0.3f);
                        onHitFreeze = (byte)(6 * Projectile.MaxUpdates);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.HeavySmootherInstance;
                    }
                    break;

                case 6:
                    {
                        SetTimes(20, 20, 20, 16);
                        SetAngles(0.8f, 0.17f, 1.9f);
                        SetScaleValues(1.4f, 0.9f, 0);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.NoSmootherInstance;
                    }
                    break;

                case 8:
                    {
                        SetTimes(10, (int)(Owner.itemTimeMax * 1.1f), 20, 7);
                        SetAngles(3.6f, 5f, -1f);
                        SetScaleValues(1.7f, 1.55f, DirSign * 0.3f);
                        onHitFreeze = (byte)(6 * Projectile.MaxUpdates);

                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        Smoother = Coralite.Instance.HeavySmootherInstance;
                    }
                    break;
            }

            base.InitializeSwing();
        }

        public override void OnBeforeOver()
        {
            switch (Combo)
            {
                default:
                    Helper.PlayPitched(CoraliteSoundID.Swing_Item1, OwnerCenter());
                    break;
                case 4:
                case 8:
                    Helper.PlayPitched(CoraliteSoundID.Swing_Item1, OwnerCenter());
                    Helper.PlayPitched(AssetDirectory.Sounds.Misc + "Slash", 0.3f, -0.1f, OwnerCenter());

                    Helper.PlayPitched(AssetDirectory.Sounds.Misc + "HeavySwing2", 0.4f, 0.5f, OwnerCenter());

                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Top, UnitToMouseV, 15, 2, 5, 1000));
                    }

                    break;
                case 6:
                    {
                        Helper.PlayPitched(CoraliteSoundID.Swing_Item1, OwnerCenter());

                        Follow = Projectile.NewProjectileFromThis<NightsEdgeREController>(Owner.Center, UnitToMouseV * 14, 0, 0);
                    }
                    break;
            }
        }

        protected override void OnSlash()
        {
            alpha = 150;
            SwingDusts(totalAngle, RotateVec2, Top, alpha);
            int currTime = (int)Timer - minTime;
            Owner.direction = recordOwnerDirection;

            switch (Combo)
            {
                default:
                    base.OnSlash();
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
                                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, OwnerCenter());
                                Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, Projectile.damage, Projectile.knockBack, ItemType, -1, 4);
                                Projectile.StartAttack();
                            }
                        }

                        base.OnSlash();
                    }
                    break;
                case 4:
                case 8:
                    {
                        if (currTime == 1)
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, (int)(Projectile.damage * 1.15f), Projectile.knockBack, ItemType, -1, 5);
                        }
                        else if (currTime == (int)((maxTime - minTime) * 0.3f))
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, (int)(Projectile.damage * 1.15f), Projectile.knockBack, ItemType, -1, 6);
                        }
                        else if (currTime == (int)((maxTime - minTime) * 0.6f))
                        {
                            Projectile.NewProjectileFromThis<NightsEdgeREExSlash>(OwnerCenter(), Vector2.Zero, (int)(Projectile.damage * 1.15f), Projectile.knockBack, ItemType, -1, 7);
                        }

                        base.OnSlash();
                    }
                    break;
                case 6:
                    {
                        if (!Follow.GetProjectileOwner(out Projectile p, Projectile.Kill))
                            return;

                        if (Timer > maxTime - 3)
                            Timer = maxTime - 3;

                        _Rotation += totalAngle * (1 - 0.8f * Helper.X2Ease((float)currTime / (maxTime - minTime)));
                        Slasher();

                        if (p.ai[1] % (10 * p.MaxUpdates) == 0 && Timer % Projectile.MaxUpdates == 0)
                        {
                            Projectile.StartAttack();
                        }

                        if (Timer % (8 * Projectile.MaxUpdates) == 0)
                        {
                            Helper.PlayPitched(CoraliteSoundID.Swing2_Item7, Projectile.Center);
                        }

                        if (p.ai[1] > 20 * p.MaxUpdates)
                        {
                            canTeleport = true;
                        }

                        if (p.ai[0] > 0)
                        {
                            Timer = maxTime + 1;
                        }

                        if (canTeleport && DownRight)//瞬移斩击
                        {
                            Vector2 dir = (p.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                            Vector2 targetP = p.Center;
                            targetP.X -= dir.X * 16;

                            Vector2 SelfP = Owner.Center;

                            Owner.Teleport(targetP, TeleportationStyleID.MysticFrog);

                            var particle = PRTLoader.NewParticle<NightsEdgeTeleportParticle>(Owner.Center, Vector2.Zero, Color.White);

                            particle.StartPos = SelfP;

                            int direction = -MathF.Sign(dir.X);
                            if ((direction == 1 && Owner.controlRight) || (direction == -1 && Owner.controlLeft))
                                Owner.velocity.X = direction * 4;
                            Owner.velocity.Y = -6.5f;


                            Owner.AddImmuneTime(ImmunityCooldownID.General, 40);
                            Owner.immune = true;

                            p.Kill();
                            Projectile.Kill();
                            Projectile.NewProjectileFromThis<NightsEdgeRESlash>(Owner.Center, Vector2.Zero, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, ItemType, -1, 8);
                        }
                    }
                    break;
            }

            SetScale();
        }

        protected override void AfterSlash()
        {
            if (Combo == 6)
            {
                if (!Follow.GetProjectileOwner(out Projectile p, Projectile.Kill))
                    return;

                alpha = 0;

                if (Timer > maxTime + (int)(Delay * 0.4f))
                    Timer = maxTime + (int)(Delay * 0.4f) + 1;

                _Rotation += totalAngle * 0.4f;

                distanceToOwner = Helper.Lerp(4, -Projectile.height / 2, (Timer - maxTime) / (Delay * 0.4f));
            }
            base.AfterSlash();
        }

        public override void RestartSlash()
        {
            Projectile.damage = Owner.GetWeaponDamage(Owner.HeldItem);

            switch (Combo)
            {
                default:
                    base.RestartSlash();
                    break;
                case 3:
                    {
                        if (DownRight)
                        {
                            Combo = 6;
                            Timer = 0;
                            onHitTimer = 0;
                            recordScaleInn = Projectile.scale;
                            Projectile.StartAttack();
                            InitializeSwing();
                        }
                        else
                            base.RestartSlash();
                    }
                    break;
            }
        }

        public static void SwingDusts(float totalAngle, Vector2 RotateVec2, Vector2 top, int alpha)
        {
            if (alpha > 0 && Main.rand.NextBool(5))
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                int a = Main.rand.Next(3);

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
                    _ => Main.rand.NextFloat(1f, 2f),
                };

                Dust dust = Dust.NewDustPerfect(top + (RotateVec2 * Main.rand.Next(-45, 5)), type,
                       dir * Main.rand.NextFloat(0.5f, 6f), alpha2, Scale: scale2);
                dust.noGravity = true;
            }
        }

        protected override void AIAfter()
        {
            if (Combo == 6)
            {
                Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
                Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制
                if (Timer <= minTime)
                    Owner.itemRotation = _Rotation + (Owner.direction > 0 ? 0 : MathHelper.Pi);

                if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
                    UpdateCaches();
            }
            else
                base.AIAfter();
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.damage > 10)
                Projectile.damage = (int)(Projectile.damage * 0.9f);

            if (Combo == 6)
            {
                if (!Follow.GetProjectileOwner(out Projectile p, Projectile.Kill))
                    return;

                p.velocity *= 0.2f;
                if (p.ai[1] > 10)
                    canTeleport = true;
            }

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

        protected override Vector2 OwnerCenter()
        {
            if (Combo == 6 && Timer > minTime && Follow.GetProjectileOwner(out Projectile p))
            {
                return p.Center;
            }

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Combo == 6 && Timer > maxTime)
                lightColor *= 1 - Helper.X2Ease((Timer - maxTime) / (Delay * 0.4f));
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            Color drawColor = new(150, 130, 255, 0);
            Color shineColor = new(100, 80, 200, 255);

            switch (Combo)
            {
                default:
                    {
                        if (Timer < minTime || Timer >= maxTime)
                            return;

                        float f = (Timer - minTime) / (maxTime - minTime);
                        int i = (int)(trailCount * 0.1f);
                        Vector2 pos = GetCenter(i) + (oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.9f + trailTopWidth + oldDistanceToOwner[i])) - Main.screenPosition;
                        float rot = MathHelper.PiOver4;

                        Helper.DrawPrettyStarSparkle(f, 0, pos, drawColor, shineColor, f, 0, 0.3f, 0.7f, 1, rot, Vector2.One * 1.75f, new Vector2(1, 1.2f));
                    }
                    break;
                case 4:
                case 8:
                    {
                        if (Timer > minTime)
                            return;

                        float f = Helper.Clamp(Timer / beforeTime, 0, 1);
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
            if (Timer < maxTime && oldRotate != null)
                WarpDrawer(0.75f, warpStrength: 0.15f);
        }

        public override Color AdditiveColor(float f)
        {
            return Color.White * 0.8f * Utils.Remap(alpha, 0, 150, 0, 1);
        }
    }

    public class NightsEdgeREExSlash() : BaseSwingProj_ScaledItem(trailCount: 30), IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public bool useOffset = true;
        public Vector2 offset;

        protected override float ControlTrailBottomWidth(float factor)
        {
            return trailBottomWidth * Projectile.scale;
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
                        SetTimes(15, (int)(Owner.itemTimeMax * 2f));
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
                        SetTimes(1, (int)(Owner.itemTimeMax * 1.5f));
                        SetAngles(-3.8f, -6.6f, 0.001f);
                        SetScaleValues(1.6f, 1.1f, -DirSign * 0.25f);

                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        trailBottomWidth = 60;
                        offset = UnitToMouseV * 20;
                    }
                    break;
                case 6:
                    {
                        SetTimes(1, (int)(Owner.itemTimeMax * 1.5f));
                        SetAngles(3.6f, 6.6f, 0.001f);
                        SetScaleValues(1.6f, 1.1f, DirSign * 0.25f);

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
                    Helper.PlayPitched(CoraliteSoundID.Swing_DD2_DarkMageAttack, Owner.Center, volume: 0.3f, pitch: -0.3f);
                    break;
                case 6:
                    Helper.PlayPitched(CoraliteSoundID.Swing_DD2_DarkMageAttack, Owner.Center, volume: 0.3f, pitch: 0.5f);
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

            alpha = -250 + (int)(400 * MathF.Sin(f * MathHelper.PiOver2));
            if (alpha < 0)
                alpha = 0;

            switch (Combo)
            {
                default:
                    break;
                case 2:
                case 7:
                    if (currentTime == (maxTime - minTime) / 2)
                    {
                        Projectile.StartAttack();
                    }
                    break;
            }

            NightsEdgeRESlash.SwingDusts(totalAngle, RotateVec2, Top, alpha);
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
                Projectile.damage = (int)(Projectile.damage * 0.90f);

            NightsEdgeRESlash.HitDusts(target, this, false);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Timer <= minTime || oldLength == null)
            {
                return;
            }

            float f = alpha / 150f;
            int i = (int)(trailCount * 0.3f);
            Vector2 pos = GetCenter(i) + (oldRotate[i].ToRotationVector2() * (oldLength[i] * 0.9f + trailTopWidth + oldDistanceToOwner[i]));
            Color drawColor = new(150, 130, 255, 0);
            Color shineColor = new(100, 80, 200, 255);

            Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0, 1f, 1f, 2, MathHelper.PiOver4, Vector2.One * 2.5f, new Vector2(1, 1.2f));

            i = (int)(trailCount * 0.4f);
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
            effect.Parameters["gradientTexture"].SetValue(NightsEdgeRESlash.GetGradientNew((int)ItemType));
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

        public override Texture2D GetGradient()
        {
            return null;
        }
    }

    public class NightsEdgeREController : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 32;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://平平无奇向前飞
                    {
                        Projectile.ShimmerReflect();
                        Timer++;
                        if (Timer > 20 * Projectile.MaxUpdates)
                        {
                            Projectile.velocity *= 0.95f;
                            if (Timer > 30 * Projectile.MaxUpdates)
                            {
                                State = 1;
                                Timer = 0;
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        Timer++;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.Center = Vector2.SmoothStep(Projectile.Center, Main.player[Projectile.owner].Center, Timer / (20f * Projectile.MaxUpdates));

                        if (Timer > 20 * Projectile.MaxUpdates)
                        {
                            Projectile.Center = Main.player[Projectile.owner].Center;
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                default:
                case 0://平平无奇向前飞
                    Projectile.velocity *= 0.95f;
                    break;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class NightsEdgeTeleportParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        public Vector2 StartPos;

        public override void SetProperty()
        {
            base.SetProperty();
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity > 24)
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            //绘制连线
            float f = Opacity / 24;
            Vector2 pos = Position;

            Color drawColor = new(150, 130, 255, 0);
            Color shineColor = new(100, 80, 200, 255);

            //绘制连线
            Helper.DrawPrettyLine(1, 0, (Position + StartPos) / 2 - Main.screenPosition, drawColor, shineColor, f, 0, 0.5f, 0.5f, 1, (StartPos - pos).ToRotation(), (StartPos - pos).Length() / (TextureAssets.Extra[ExtrasID.SharpTears].Width() * 0.5f), new Vector2(2, 1.2f));

            //绘制尾部
            for (int i = 0; i < 2; i++)
            {
                Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0.3f, 0.6f, 0.6f, 1, MathHelper.PiOver4, Vector2.One * 3.5f, new Vector2(1.4f, 1.4f));

                Helper.DrawPrettyStarSparkle(f, 0, Vector2.SmoothStep(pos, StartPos, 0.16f) - Main.screenPosition, drawColor, shineColor, f, 0.3f, 0.6f, 0.6f, 1, MathHelper.PiOver4, Vector2.One * 2.5f, new Vector2(1.4f, 1.4f));
            }

            //绘制头部
            pos = StartPos;
            for (int i = 0; i < 3; i++)
            {
                Helper.DrawPrettyStarSparkle(f, 0, pos - Main.screenPosition, drawColor, shineColor, f, 0f, 0.3f, 0.3f, 0.7f, MathHelper.PiOver4, Vector2.One * 6.5f, new Vector2(1, 2f));

                Helper.DrawPrettyStarSparkle(f, 0, Vector2.SmoothStep(pos, Position, 0.16f) - Main.screenPosition, drawColor, shineColor, f, 0f, 0.3f, 0.3f, 0.7f, MathHelper.PiOver4, Vector2.One * 4.5f, new Vector2(1, 2f));
            }

            return false;
        }
    }
}
