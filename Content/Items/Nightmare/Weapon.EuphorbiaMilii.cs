using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Content.NPCs.GlobalNPC;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    /// <summary>
    /// 刺刺=》下挥后再次向下挥=》上至下挥砍后下至上挥舞2圈=》在手里转一圈后向前刺出<br></br>
    /// 右键消耗能量释放特殊攻击
    /// </summary>
    public class EuphorbiaMilii : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<EuphorbiaMiliiProj>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(180, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, Item);

                if (player.altFunctionUse == 2)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<EuphorbiaMiliiRightClick>(), damage, knockback, player.whoAmI, 0);
                    return false;
                }

                // 生成弹幕
                switch (combo)
                {
                    default:
                    case 0:
                    case 1://刺
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);
                        Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One) * 22, ProjectileType<EuphorbiaSpike>(), (int)(damage * 1.5f), knockback, player.whoAmI);
                        break;
                    case 2://下挥*2
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.1f), knockback, player.whoAmI, combo);
                        break;
                    case 3://下挥+上挥*2
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.2f), knockback, player.whoAmI, combo);
                        break;
                    case 4://转圈刺
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.4f), knockback, player.whoAmI, combo);
                        break;
                }

                combo++;
                if (combo > 4)
                    combo = 0;
            }

            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入combo<br></br>
    /// 使用ai1传入颜色，0为紫色，1为红色
    /// </summary>
    public class EuphorbiaMiliiProj : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "EuphorbiaMiliiProj";

        public ref float Combo => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> GradientTexture;

        public EuphorbiaMiliiProj() : base(0.785f, trailCount: 36) { }

        public int alpha;
        public int delay;
        public int innerCombo;
        public float nextStartAngle;
        public float firstStartAngle;
        public float finalRotation;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "EuphorbiaMiliiGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 50;
            Projectile.height = (int)(118 * 1.414f);
            Projectile.hide = true;
            trailTopWidth = 2;
            minTime = 0;
            onHitFreeze = 8;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 50 * Projectile.scale;
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 3;
            alpha = 0;

            switch (Combo)
            {
                default:
                case 0: //刺1
                    startAngle = Main.rand.NextFloat(-0.2f, 0.2f);
                    totalAngle = 0.001f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27;
                    Smoother = Coralite.Instance.ReverseX2Smoother;
                    distanceToOwner = -Projectile.height / 2;
                    useSlashTrail = false;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Projectile.Center);
                    break;
                case 1://刺2
                    startAngle = Main.rand.NextFloat(-0.1f, 0.1f);
                    totalAngle = 0.001f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27;
                    Smoother = Coralite.Instance.ReverseX2Smoother;
                    distanceToOwner = -Projectile.height / 2;

                    useSlashTrail = false;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Projectile.Center);

                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    startAngle = 1.9f;
                    totalAngle = 4f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = -Projectile.height / 2;
                    nextStartAngle = GetStartAngle() - OwnerDirection * 2.4f;
                    delay = 20;
                    useTurnOnStart = false;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.2f, Projectile.Center);

                    break;
                case 2 when innerCombo == 1://下挥2 转圈并稍微伸出
                    {
                        startAngle = 2.4f;
                        totalAngle = 4.9f;
                        maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22 * 2;
                        Smoother = Coralite.Instance.HeavySmootherInstance;
                        distanceToOwner = -Projectile.height / 2;
                        delay = 0;
                        Helper.PlayPitched("Misc/FlowSwing2", 0.3f, 0.8f, Projectile.Center);
                    }
                    break;
                case 3 when innerCombo == 0://下挥 伸出，更类似于挥砍，不会挥到身体后方
                    {
                        startAngle = 3f;
                        totalAngle = 4.4f;
                        maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        distanceToOwner = 10 - Projectile.height / 2;
                        nextStartAngle = GetStartAngle() - OwnerDirection * -2.2f;

                        delay = 0;
                        useTurnOnStart = false;
                        Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.2f, Projectile.Center);
                    }
                    break;
                case 3 when innerCombo == 1://上挥，转圈
                    {
                        startAngle = -1.4f;
                        totalAngle = -4.4f;
                        maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22 * 2;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        //distanceToOwner = -Projectile.height / 2;
                        delay = 18;
                        Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Projectile.Center);
                    }
                    break;
                case 3 when innerCombo == 2://上挑
                    {
                        startAngle = -2.2f;
                        totalAngle = -4.4f;
                        maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                        Smoother = Coralite.Instance.HeavySmootherInstance;
                        //distanceToOwner = -Projectile.height / 2;
                        delay = 12;
                        Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.6f, Projectile.Center);
                    }
                    break;
                case 4 when innerCombo == 0://在手里转圈圈
                    startAngle = 0.001f;
                    totalAngle = -6.282f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = -Projectile.height / 2;
                    delay = 18;
                    Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.2f, Projectile.Center);

                    break;
                case 4 when innerCombo == 1://大力刺出
                    startAngle = 0.001f;
                    totalAngle = 0.001f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    distanceToOwner = -Projectile.height / 2;
                    delay = 20;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Projectile.Center);

                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        var modifier = new PunchCameraModifier(Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero), 10, 6, 10, 1000);
                        Main.instance.CameraModifiers.Add(modifier);
                    }

                    firstStartAngle = base.GetStartAngle();
                    useSlashTrail = false;
                    break;
                case 5://未完全蓄力的普通上挥
                    startAngle = -2.2f;
                    totalAngle = -4.85f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22 * 3;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    //distanceToOwner = -Projectile.height / 2;
                    delay = 18;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.2f, Projectile.Center);
                    break;
                case 6 when innerCombo == 0://普通上挥1
                    startAngle = -2.2f;
                    totalAngle = -4.85f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    nextStartAngle = GetStartAngle() - OwnerDirection * -2.2f;

                    //distanceToOwner = -Projectile.height / 2;
                    delay = 18;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.4f, Projectile.Center);
                    ColorState = 1;
                    break;
                case 6 when innerCombo == 1://普通上挥2
                    startAngle = -2.2f;
                    totalAngle = -4.4f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    //distanceToOwner = -Projectile.height / 2;
                    delay = 4;
                    Helper.PlayPitched("Misc/HeavySwing2", 0.4f, 0.6f, Projectile.Center);

                    break;
                case 6 when innerCombo == 2://普通突刺3
                    startAngle = 0.001f;
                    totalAngle = 0.001f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    distanceToOwner = -Projectile.height / 2;
                    delay = 20;
                    Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.4f, Projectile.Center);

                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        var modifier = new PunchCameraModifier(Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero), 10, 6, 10, 1000);
                        Main.instance.CameraModifiers.Add(modifier);
                    }

                    firstStartAngle = base.GetStartAngle();
                    useSlashTrail = false;

                    break;
                case 7 when innerCombo == 0://瞬移到鼠标位置并转圈圈放刺
                    Vector2 pointPoisition = default(Vector2);
                    pointPoisition.X = Main.mouseX + Main.screenPosition.X;
                    if (Owner.gravDir == 1f)
                        pointPoisition.Y = Main.mouseY + Main.screenPosition.Y - Owner.height;
                    else
                        pointPoisition.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY;

                    pointPoisition.X -= Owner.width / 2;
                    Owner.LimitPointToPlayerReachableArea(ref pointPoisition);
                    if (!(pointPoisition.X > 50f) || !(pointPoisition.X < (Main.maxTilesX * 16 - 50)) || !(pointPoisition.Y > 50f) || !(pointPoisition.Y < (Main.maxTilesY * 16 - 50)))
                        goto Over;
                    int num = (int)(pointPoisition.X / 16f);
                    int num2 = (int)(pointPoisition.Y / 16f);
                    if (Collision.SolidCollision(pointPoisition, Owner.width, Owner.height))
                        goto Over;

                    Owner.Teleport(pointPoisition, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Owner.whoAmI, pointPoisition.X, pointPoisition.Y, 1);

                    if (Owner.chaosState)
                    {
                        Owner.statLife -= Owner.statLifeMax2 / 7;
                        PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                        if (Main.rand.NextBool(2))
                            damageSource = PlayerDeathReason.ByOther(Owner.Male ? 14 : 15);

                        if (Owner.statLife <= 0)
                            Owner.KillMe(damageSource, 1.0, 0);

                        Owner.lifeRegenCount = 0;
                        Owner.lifeRegenTime = 0f;
                    }

                    Owner.AddBuff(88, 360);

                    for (int i = 0; i < 6; i++)
                    {
                        Color color = Main.rand.Next(0, 2) switch
                        {
                            0 => new Color(110, 68, 200),
                            _ => new Color(122, 110, 134)
                        };

                        Particle.NewParticle(Owner.Center + Main.rand.NextVector2Circular(16, 16), Helper.NextVec2Dir(2, 5f),
                            CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 dir2 = Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(Owner.Center + dir2 * Main.rand.Next(0, 32), DustType<NightmareStar>(),
                            dir2 * Main.rand.NextFloat(4f, 8f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(0.6f, 1.2f));
                        dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;

                        dir2 = Helper.NextVec2Dir();
                        Dust.NewDustPerfect(Owner.Center + dir2 * Main.rand.Next(0, 32), DustID.VilePowder,
                            dir2 * Main.rand.NextFloat(1f, 3f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
                    }

                    SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                    st.Pitch = -1;
                    SoundEngine.PlaySound(st, Owner.Center);

                    Owner.AddImmuneTime(ImmunityCooldownID.General, 60);
                    Owner.immune = true;
                Over:
                    startAngle = 0.001f;
                    totalAngle = -8f;
                    maxTime = (int)(Owner.itemTimeMax * 0.5f) + 27 + 22 * 2;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = -Projectile.height / 2;
                    delay = 18;
                    Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.2f, Projectile.Center);
                    ColorState = 1;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float angle = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 dir = (angle + i * MathHelper.TwoPi / 5).ToRotationVector2();
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center - dir * 35,
                                dir, ProjectileType<EuphorbiaSpecialSpike>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner, 30, 230);
                        }

                        angle += MathHelper.TwoPi / 10;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 dir = (angle + i * MathHelper.TwoPi / 5).ToRotationVector2();
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center - dir * 35,
                                dir, ProjectileType<EuphorbiaSpecialSpike>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner, 50, 280);
                        }
                    }

                    break;
            }

            base.Initializer();
            finalRotation = startAngle + totalAngle;
        }

        protected override float GetStartAngle()
        {
            if (innerCombo == 0)
            {
                firstStartAngle = base.GetStartAngle();
                return firstStartAngle;
            }

            return firstStartAngle;
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            if (Timer % 2 == 0)
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                Color c = (int)ColorState switch
                {
                    0 => NightmarePlantera.nightPurple,
                    _ => NightmarePlantera.nightmareRed
                };
                Dust dust = Dust.NewDustPerfect(Top + Main.rand.NextVector2Circular(25, 25), DustID.ViciousPowder,
                       dir * Main.rand.NextFloat(0.5f, 2f), 100, c, Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 175) + 75;

            switch ((int)Combo)
            {
                default:
                case 0:
                case 1:
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 180;
                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    distanceToOwner = -Projectile.height / 2 + Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 40;
                    Projectile.scale = 1 + Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 0.3f;
                    if (timer == Owner.itemTimeMax)
                    {
                        float start = nextStartAngle + OwnerDirection * 2.4f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, start.ToRotationVector2() * 22, ProjectileType<EuphorbiaSpike>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);

                        for (int i = -1; i < 3; i++)
                        {
                            if (i == 0)
                                continue;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.6f).ToRotationVector2() * 16, ProjectileType<EuphorbiaSmallSpike>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }
                    break;
                case 2 when innerCombo == 1://下挥2 转圈并稍微伸出
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 60;
                    //Projectile.scale = 1 + Smoother.Smoother(timer, maxTime - minTime) * 0.3f;
                    Projectile.scale = Helper.EllipticalEase(2.4f - 4.9f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.4f);
                    if (timer == Owner.itemTimeMax / 3)
                    {
                        float start = nextStartAngle + OwnerDirection * 2.4f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, start.ToRotationVector2() * 22, ProjectileType<EuphorbiaSpike>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);

                        for (int i = -1; i < 3; i++)
                        {
                            if (i == 0)
                                continue;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.6f).ToRotationVector2() * (17 + i), ProjectileType<EuphorbiaSmallSpike>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }

                    break;
                case 3 when innerCombo == 0://下挥 伸出，更类似于挥砍，不会挥到身体后方
                    distanceToOwner = 10 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 5;
                    if (timer == Owner.itemTimeMax)
                    {
                        float start = nextStartAngle + OwnerDirection * -2.2f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, start.ToRotationVector2() * 22, ProjectileType<EuphorbiaSpike>(), (int)(Projectile.damage * 1.75f), Projectile.knockBack, Projectile.owner);

                        for (int i = -2; i < 3; i++)
                        {
                            if (i == 0)
                                continue;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.5f).ToRotationVector2() * (17 + i), ProjectileType<EuphorbiaSmallSpike>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }

                    break;
                case 3 when innerCombo == 1://上挥，转圈
                    distanceToOwner = 15 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 10;
                    Projectile.scale = Helper.EllipticalEase(1.4f - 4.4f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.4f);
                    if (timer == Owner.itemTimeMax)
                    {
                        float start = nextStartAngle + OwnerDirection * -2.2f;

                        for (int i = -3; i < 3; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.7f).ToRotationVector2() * (17 + i * 0.5f), ProjectileType<EuphorbiaSmallSpike>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }
                    break;
                case 3 when innerCombo == 2://上挑
                    distanceToOwner = 25 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 40;
                    Projectile.scale = Helper.EllipticalEase(2.2f - 4.4f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.4f);
                    if (timer == Owner.itemTimeMax / 3)
                    {
                        float start = nextStartAngle + OwnerDirection * -2.2f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, start.ToRotationVector2() * 22, ProjectileType<EuphorbiaSpike>(), (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner);

                        for (int i = -2; i < 2; i++)
                        {
                            if (i == 0)
                                continue;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.8f).ToRotationVector2() * (17 + i), ProjectileType<EuphorbiaSmallSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    break;
                case 4 when innerCombo == 0://在手里转圈圈

                    break;
                case 4 when innerCombo == 1://大力刺出
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 180;
                    if (timer == Owner.itemTimeMax / 2)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, startAngle.ToRotationVector2() * 48, ProjectileType<EuphorbiaSpurt>(),
                            (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner, ai1: 28);
                    }
                    break;
                case 5://未完全蓄力的普通上挥
                    distanceToOwner = 25 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 20;
                    Projectile.scale = Helper.EllipticalEase(2.2f - 4.85f * Smoother.Smoother(timer, maxTime - minTime), 1.2f, 1.4f);
                    break;
                case 6 when innerCombo == 0://普通上挥1
                    distanceToOwner = 25 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 20;
                    Projectile.scale = Helper.EllipticalEase(2.2f - 4.85f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.2f);
                    if (timer == Owner.itemTimeMax)
                    {
                        float start = nextStartAngle + OwnerDirection * -2.2f;

                        for (int i = -3; i < 3; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.7f).ToRotationVector2() * (17 + i * 0.5f), ProjectileType<EuphorbiaSmallSpike>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                        }
                    }
                    break;
                case 6 when innerCombo == 1://普通上挥2
                    distanceToOwner = 25 - Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 20;
                    Projectile.scale = Helper.EllipticalEase(2.2f - 4.85f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.2f);
                    if (timer == Owner.itemTimeMax / 3)
                    {
                        float start = nextStartAngle + OwnerDirection * -2.2f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, start.ToRotationVector2() * 22, ProjectileType<EuphorbiaSpike>(), (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner);

                        for (int i = -2; i < 2; i++)
                        {
                            if (i == 0)
                                continue;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (start + i * 0.8f).ToRotationVector2() * (17 + i), ProjectileType<EuphorbiaSmallSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    break;
                case 6 when innerCombo == 2://普通突刺3
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 180;
                    if (timer == Owner.itemTimeMax / 2)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, startAngle.ToRotationVector2() * 48, ProjectileType<EuphorbiaSpurt>(),
                            (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner, ai1: 28);
                    }

                    break;
                case 7 when innerCombo == 0://瞬移到鼠标位置并转圈圈放刺

                    break;

            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;

            switch ((int)Combo)
            {
                default:
                    if (Timer > maxTime + delay)
                        Projectile.Kill();

                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    _Rotation = finalRotation.AngleLerp(nextStartAngle, (Timer - maxTime) / delay);
                    distanceToOwner = Helper.Lerp(distanceToOwner, -Projectile.height / 2, 0.15f);

                    if (Timer > maxTime + delay)
                    {
                        innerCombo++;
                        Timer = 0;
                        Initializer();
                    }
                    break;
                case 3 when innerCombo == 0:
                    innerCombo++;
                    Timer = 0;
                    Initializer();

                    break;
                case 6 when innerCombo == 0://普通上挥1
                case 3 when innerCombo == 1:
                    _Rotation = finalRotation.AngleLerp(nextStartAngle, (Timer - maxTime) / delay);
                    //distanceToOwner = Helper.Lerp(distanceToOwner, -Projectile.height / 2, 0.15f);

                    if (Timer > maxTime + delay)
                    {
                        innerCombo++;
                        Timer = 0;
                        Initializer();
                    }

                    break;
                case 3 when innerCombo == 2:
                    distanceToOwner = Helper.Lerp(distanceToOwner, -Projectile.height / 2, 0.05f);

                    if (Timer > maxTime + delay)
                        Projectile.Kill();

                    break;
                case 6 when innerCombo == 1://普通上挥2
                case 4 when innerCombo == 0:
                    _Rotation = finalRotation.AngleLerp(GetStartAngle(), (Timer - maxTime) / delay);

                    if (Timer > maxTime + delay)
                    {
                        innerCombo++;
                        Timer = 0;
                        Initializer();
                    }
                    break;
                case 4 when innerCombo == 1:
                case 5://未完全蓄力的普通上挥
                case 6 when innerCombo == 2://普通突刺3
                    distanceToOwner = Helper.Lerp(distanceToOwner, -Projectile.height / 2, 0.1f);

                    if (Timer > maxTime + delay)
                        Projectile.Kill();

                    break;
            }

            Slasher();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                Owner.immuneTime += 8;
                if (Main.netMode == NetmodeID.Server)
                    return;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, 3, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.RedTorch, dir * Main.rand.NextFloat(4f, 12f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            if (Combo != 0 && Combo != 1 && oldRotate != null)
                WarpDrawer(0.75f);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Rectangle frameBox = mainTex.Frame(1, 3, 0, (int)ColorState);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox,
                                                lightColor, Projectile.rotation + extraRot, frameBox.Size() / 2, Projectile.scale, CheckEffect(), 0f);
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
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
                    effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
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

    public class EuphorbiaMiliiRightClick : BaseSwingProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmareItems + "EuphorbiaMiliiProj";

        public ref float ExtraScale => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];

        public float ExtraAlpha;

        public float ShinyAlpha;
        public int ChannelTime;
        public int MaxChannelTime;

        public EuphorbiaMiliiRightClick() : base(MathHelper.PiOver4, trailCount: 48) { }

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 50;
            Projectile.height = (int)(118 * 1.414f);
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 4;
            onHitFreeze = 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 1;
            ExtraAlpha = 0;
            ExtraScale = 0.3f;
            startAngle = -2.2f;
            totalAngle = 0.001f;
            maxTime = 6;
            Smoother = Coralite.Instance.NoSmootherInstance;
            Projectile.scale = 0.8f;
            distanceToOwner = 20 - Projectile.height / 2;
            MaxChannelTime = 55 + Owner.itemTimeMax * 3;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
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

        protected override void BeforeSlash()
        {
            if (Main.mouseRight)
            {
                Timer = 2;

                _Rotation = GetStartAngle() - OwnerDirection * startAngle;
                Slasher();

                if (ChannelTime < MaxChannelTime / 2)
                {
                    if (ExtraAlpha < 1)
                    {
                        ExtraAlpha += 0.05f;
                        if (ExtraAlpha > 1)
                            ExtraAlpha = 1;
                    }
                    if (Projectile.scale < 1f)
                        Projectile.scale += 0.1f;

                    ExtraScale = Helper.Lerp(0.3f, 0, ChannelTime / (float)MaxChannelTime);
                    Projectile.scale = MathHelper.Lerp(0.8f, 1f, ChannelTime / (float)MaxChannelTime);
                    distanceToOwner = 20 - Projectile.height / 2 + Helper.Lerp(0, 20, ChannelTime / (float)MaxChannelTime);
                }
                else if (ChannelTime < MaxChannelTime)
                {
                    ExtraAlpha -= 1 / ((float)MaxChannelTime / 2);
                    ExtraScale = Helper.Lerp(0.3f, 0, ChannelTime / (float)MaxChannelTime);
                    Projectile.scale = MathHelper.Lerp(0.8f, 1f, ChannelTime / (float)MaxChannelTime);
                    distanceToOwner = 20 - Projectile.height / 2 + Helper.Lerp(0, 20, ChannelTime / (float)MaxChannelTime);
                }
                else if (ChannelTime == MaxChannelTime)
                {
                    ExtraAlpha = 0;
                    SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Owner.Center);
                    Projectile.scale = 1;
                    distanceToOwner = -Projectile.height / 2 + 40;
                    ColorState = 1;
                }
                else if (ChannelTime < MaxChannelTime + 20)
                {
                    ShinyAlpha += 0.05f;
                    if (ShinyAlpha > 1)
                        ShinyAlpha = 1;
                }
                else if (ChannelTime > MaxChannelTime + 20 + 45)
                {
                    if (ShinyAlpha > 0)
                    {
                        ShinyAlpha -= 0.05f;
                        if (ShinyAlpha < 0)
                            ShinyAlpha = 0;
                    }
                }
                else
                    ShinyAlpha = 0;

                ChannelTime++;
            }
            else
            {
                //检测当前梦魇光能
                if (ChannelTime >= MaxChannelTime)
                {
                    if (Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 7)//能量足够，生成瞬移
                    {
                        cp.nightmareEnergy -= 7;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<EuphorbiaMiliiProj>(),
                            (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner, 7);
                        Projectile.Kill();
                        return;
                    }
                    else//能量不够，普通挥舞
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<EuphorbiaMiliiProj>(),
                            (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 6);
                        Projectile.Kill();
                        return;
                    }
                }
                else//没蓄力好，更弱鸡的单词挥舞
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<EuphorbiaMiliiProj>(),
                       Projectile.damage, Projectile.knockBack, Projectile.owner, 5);
                    Projectile.Kill();
                    return;

                }

            }
        }

        protected override void OnSlash()
        {
            Projectile.Kill();
        }

        protected override void AfterSlash()
        {
            Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Rectangle frameBox = mainTex.Frame(1, 3, 0, (int)ColorState);
            Vector2 center = Projectile.Center - Main.screenPosition;
            origin = frameBox.Size() / 2;
            float rot = Projectile.rotation + extraRot;
            SpriteEffects effect = CheckEffect();

            Main.spriteBatch.Draw(mainTex, center, frameBox,
                                                lightColor, rot, origin, Projectile.scale, effect, 0f);

            if (ShinyAlpha > 0)
                Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 3, 0, 2),
                                                    Color.White * ShinyAlpha, rot, origin, Projectile.scale, effect, 0f);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (ExtraAlpha > 0)
            {
                Texture2D exTex = BlackHole.CircleTex.Value;
                Color c = Color.White;
                c.A = (byte)(c.A * ExtraAlpha);
                Main.spriteBatch.Draw(exTex, (Top + Projectile.Center) / 2 - Main.screenPosition, null,
                                                   c, ChannelTime * 0.1f, exTex.Size() / 2, ExtraScale, 0, 0f);
            }
        }
    }

    public class EuphorbiaSpike : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public ref float State => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[0];

        private Vector2 offset;
        private float alpha;
        private bool init = true;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 12;

            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (init)
            {
                alpha += 0.05f;
                if (alpha > 1)
                {
                    alpha = 1f;
                    init = false;
                }
            }

            switch (State)
            {
                default:
                    OnShoot();
                    break;
                case 1:
                    {
                        if (Target < 0 || Target > Main.maxNPCs)
                            Projectile.Kill();

                        NPC npc = Main.npc[(int)Target];
                        if (!npc.active || npc.dontTakeDamage)
                            Projectile.Kill();

                        Projectile.Center = npc.Center + offset;

                        if (Main.rand.NextBool(30))
                        {
                            Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                                    DustID.Corruption, -Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                            dust3.noGravity = true;
                        }

                        if (Projectile.timeLeft < 30)
                        {
                            alpha -= 1 / 32f;
                        }
                    }
                    break;
            }
        }

        public virtual void OnShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    DustID.VilePowder, Vector2.Zero, newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)State == 0)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffType<EuphorbiaPoison>(), 90, true);

            if ((int)State == 0)
            {
                Projectile.tileCollide = false;
                Projectile.timeLeft = 120;
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                State = 1;
                Projectile.netUpdate = true;
                alpha = 1;
                init = false;

            }
        }

        public override void OnKill(int timeLeft)
        {
            if (State == 0 && VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 direction = -Projectile.rotation.ToRotationVector2();

                Helper.SpawnDirDustJet(Projectile.Center, () => direction.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)), 2, 6, (i) => Main.rand.NextFloat(0.5f, 10f),
                    DustID.VilePowder, Scale: Main.rand.NextFloat(1f, 1.5f), noGravity: true, extraRandRot: 0.15f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = new Vector2(3 * mainTex.Width / 4, mainTex.Height / 4);
            float Rot = Projectile.rotation + 0.785f;

            if ((int)State == 0)//残影绘制
                Projectile.DrawShadowTrails(lightColor * alpha, 0.5f, 0.5f / 8f, 1, 8, 1, 0.785f, -1f);
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor * alpha, Rot, origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class EuphorbiaSmallSpike : EuphorbiaSpike
    {
        public int npcIndex;

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 12;

            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            //if (Main.rand.NextBool())
            //{
            //    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
            //        DustID.PlatinumCoin, Vector2.Zero, newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
            //    dust.noGravity = true;
            //}

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    DustID.VilePowder, Vector2.Zero, newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            bool flag2 = false;
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

                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, 0.15f);
                //Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, clamped: true));
            }
        }
    }

    public class EuphorbiaSpurt : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "SpurtTrail2";

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.ai[0];
        public ref float TrailWidth => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];

        private Trail trail;
        private bool hited = true;

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
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override bool ShouldUpdatePosition() => Timer >= 0;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > 0)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hited)
            {
                hited = false;
                if (Owner.TryGetModPlayer(out CoralitePlayer cp))//射出特殊弹幕
                    cp.GetNightmareEnergy(2);
            }
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 24, new NoTip(), WidthFunction, ColorFunction);

            if (Timer > 0)
            {
                Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
                if (Timer < 14)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2, DustType<NightmarePetal>(),
                        Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(0.5f, 3f), newColor: Color.MintCream);
                    dust.noGravity = true;
                }

                do
                {
                    if (Timer < 10)
                    {
                        if (Alpha < 1)
                            Alpha += 1 / 8f;
                        break;
                    }

                    Projectile.velocity *= 0.84f;
                    TrailWidth *= 0.9f;
                    Alpha -= 0.05f;
                    if (TrailWidth < 2)
                        Projectile.Kill();

                } while (false);

                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer < 12)
                {
                    Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;

                    for (int i = 0; i < 23; i++)
                        Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[23], i / 23f);
                }
                else
                {
                    for (int i = 0; i < 23; i++)
                        Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                    Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
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
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(EuphorbiaMiliiProj.GradientTexture.Value);
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
                float factor = 1f - i / 23f;
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

    /// <summary>
    /// 使用ai0传入蓄力时间,ai1传入刺出长度
    /// </summary>
    public class EuphorbiaSpecialSpike : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "ConfusionHole";

        private ref float ChannelTime => ref Projectile.ai[0];
        private ref float SpurtLength => ref Projectile.ai[1];

        private ref float State => ref Projectile.localAI[1];

        private float SelfScale;

        public ref Vector2 SpikeTop => ref Projectile.velocity;

        private bool Init = true;
        private float Timer;
        private float spikeWidth;

        public NightmareTentacle spike;
        public Color drawColor = NightmarePlantera.nightmareRed;

        public static Asset<Texture2D> SparkleTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SparkleTex = Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSparkle");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            SparkleTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 16;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)State != 1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.velocity);
        }

        public override void AI()
        {
            if (Init)
            {
                Vector2 center = Projectile.Center;
                Projectile.width = Projectile.height = (int)SpurtLength;
                Projectile.Center = center;

                Init = false;
                Projectile.rotation = Projectile.velocity.ToRotation();
                SpikeTop = center;
            }

            spike ??= new NightmareTentacle(30, factor =>
            {
                if (factor > 0.2f)
                    return Color.Lerp(drawColor, Color.White, (factor - 0.2f) / 0.8f);

                return Color.Lerp(Color.Transparent, drawColor, factor / 0.2f);
            }, factor =>
            {
                return Helper.Lerp(0, spikeWidth, factor);
            }, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);


            switch ((int)State)
            {
                default:
                case 0: //伸出一个小头
                    {
                        float factor = MathHelper.Clamp(Timer / (ChannelTime * 0.5f), 0, 1);
                        if (SelfScale < 0.5f)
                            SelfScale = Helper.Lerp(0, 0.5f, Timer / (ChannelTime * 0.25f));

                        if (Timer < 3)
                        {
                            SpikeTop += Projectile.rotation.ToRotationVector2() * 25;
                            spikeWidth += 3f;
                        }

                        if (Timer > ChannelTime)
                        {
                            State++;
                            Timer = 0;

                            Helper.PlayPitched("Misc/Spike", 0.5f, 0.4f, Projectile.Center);
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        if (Timer < 5)
                        {
                            float currentLength = Vector2.Distance(SpikeTop, Projectile.Center);
                            currentLength = Helper.Lerp(currentLength, SpurtLength, 0.7f);
                            SpikeTop = Projectile.Center + Projectile.rotation.ToRotationVector2() * currentLength;
                            spikeWidth += 4f;
                        }

                        if (Timer > 30)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://逐渐收回并减淡消失
                    {
                        SpikeTop = Vector2.Lerp(SpikeTop, Projectile.Center, 0.14f);
                        drawColor *= 0.9f;
                        SelfScale *= 0.9f;
                        if (drawColor.A < 10)
                            Projectile.Kill();
                    }
                    break;
            }

            spike.pos = Projectile.Center;
            spike.rotation = Projectile.rotation;
            spike.UpdateTentacle((SpikeTop - Projectile.Center).Length() / 30, i => MathF.Sin((i + Timer) * 0.314f) * 2);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            spike?.DrawTentacle(spike.perLength * 30 / 200);
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D mainTex = Projectile.GetTexture();
            Texture2D sparkleTex = SparkleTex.Value;
            Texture2D blackholeTex = BlackHole.BlackHoleTex.Value;

            float rot = Projectile.rotation + MathHelper.PiOver2;
            Color c = Color.White;
            c.A = 200;
            spriteBatch.Draw(mainTex, pos, null, c, rot, mainTex.Size() / 2, new Vector2(0.5f, SelfScale), 0, 0);
            spriteBatch.Draw(blackholeTex, pos, null, c, rot + MathHelper.PiOver2, blackholeTex.Size() / 2, new Vector2(SelfScale, 0.5f) * 0.3f, 0, 0);

            var sparkleFrame = sparkleTex.Frame(1, 2, 0, 1);
            var sparkleOrigin = sparkleFrame.Size() / 2;
            spriteBatch.Draw(sparkleTex, pos, sparkleFrame, c, Main.GlobalTimeWrappedHourly * 0.5f, sparkleOrigin, SelfScale / 3 + Main.rand.NextFloat(0, 0.02f), 0, 0);
        }
    }

    public class EuphorbiaPoison : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC gnpc))
            {
                gnpc.EuphorbiaPoison = true;
            }

            Dust dust6 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.VilePowder, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1.5f);
            dust6.noGravity = true;
            dust6.velocity *= 2.8f;
            dust6.velocity.Y -= 0.5f;
            if (Main.rand.NextBool(4))
            {
                dust6.scale *= 0.5f;
            }
        }
    }
}
