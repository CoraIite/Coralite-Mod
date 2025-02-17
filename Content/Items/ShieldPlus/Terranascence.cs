using Coralite.Content.Items.Glistent;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
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
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.ShieldPlus
{
    /*
     * 左键常规挥舞，2段从上至下，之后突刺，此次突刺会增加自然能量，自然能量上限为3
     * 盾冲和完美格挡将消耗自然能量
     * 
     * 右键防御，自带Parry
     * Parry完成后，按下按键
     *     - 左键使用强化挥舞，带拖尾
     * 
     * 特殊攻击键丢出飞盾，并在短时间内拥有移动突刺的能力
     *     - 突刺时命中飞盾后将飞盾弹开，飞盾将绕圈旋转一段时间后返回，同时获得1个自然能量
     *     - 命中后一小段时间内左键进行强化突刺，右键进行斩击
     *     - 每次丢出飞盾后仅能使用1次突刺
     *     - 
     * 
     * 双击使用盾冲，盾冲后短时间内可以使用冲刺
     */
    public class Terranascence()
        : BaseShieldPlusWeapon<TerranascenceGuard>(Item.sellPrice(0, 1), ItemRarityID.Green, AssetDirectory.ShieldPlusItems)
        , IFlyingShieldAccessory
    {
        public override int FSProjType => ModContent.ProjectileType<TerranascenceFSProj>();

        private int combo;

        public int energy;

        /// <summary>
        /// 在丢出飞盾后将此值设置为一定值<br></br>
        /// 为0时表示无法进行突刺
        /// </summary>
        public int PowerfulLeft;
        /// <summary>
        /// 左键突刺，右键强化斩击
        /// </summary>
        public int PowerfulRight;
        /// <summary>
        /// 强化突刺，右键强化斩击
        /// </summary>
        public int PowerfulAtt2;
        /// <summary>
        /// 左键普攻时是否有刀光
        /// </summary>
        public bool SwordLightNormalAttack;

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<TerranascenceSwing>();
            Item.knockBack = 4f;
            Item.shootSpeed = 12;
            Item.damage = 26;

            Item.UseSound = null;
            Item.useTurn = false;
            Item.autoReuse = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (PowerfulLeft > 0)
                PowerfulLeft--;
            if (PowerfulRight > 0)
                PowerfulRight--;
            if (PowerfulAtt2 > 0)
                PowerfulAtt2--;
        }

        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            Item.stack++;
            if (SwordLightNormalAttack)
                SwordLightNormalAttack = false;
            else
                SwordLightNormalAttack = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);

            if (player.ownedProjectileCounts[ModContent.ProjectileType<TerranascenceTag>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<TerranascenceTag>()
                    , 0, 0, player.whoAmI);
            }
        }

        #region 攻击相关

        public override void LeftAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (PowerfulAtt2 > 0)
            {
                PowerfulAtt2 = 0;
                PowerfulLeft = 0;
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TerranascenceSpurt>()
                    , (int)(damage * 3.35f), knockback, player.whoAmI, 1, 1);
                return;
            }

            if (PowerfulLeft > 0)
            {
                PowerfulLeft = 0;
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TerranascenceSpurt>()
                    , (int)(damage * 1.75f), knockback, player.whoAmI, 1);
                return;
            }

            if (combo == 2)
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TerranascenceSpurt>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);

            combo++;
            if (combo > 2)
            {
                combo = 0;
                GetEnergy();
            }
        }

        public override void ShootFlyingShield(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            damage = (int)(damage * 0.75f);
            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);
            PowerfulLeft = 30;
            base.ShootFlyingShield(player, source, velocity, type, damage, knockback);
        }

        public override void RightShoot(Player player, IEntitySource source, int damage)
        {
            if (PowerfulRight > 0)
            {
                PowerfulRight = 0;
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, Item.shoot, (int)(damage * 3.5f), 6, player.whoAmI, 2);
                return;
            }
            damage = (int)(damage * 0.7f);
            base.RightShoot(player, source, damage);
        }

        #endregion

        #region 冲刺

        public override bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp))
            {
                CheckGuardProj(Player.GetModPlayer<CoralitePlayer>(), Player);

                if (cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                    && flyingShieldGuard.CanDash())
                {
                    SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Player.Center);

                    flyingShieldGuard.TurnToDashing(this, 16, dashDirection, 12f);

                    Player.GetModPlayer<CoralitePlayer>().DashTimer = 75;
                    Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;

                    return true;
                }
            }

            return false;
        }

        public override void CheckGuardProj(CoralitePlayer cp, Player Player)
        {
            if (cp.TryGetFlyingShieldGuardProj(out _))
                return;

            base.RightShoot(Player, Player.GetSource_ItemUse(Item), Player.GetWeaponDamage(Item));

            var projectile = Main.projectile.FirstOrDefault(p => p.active && p.owner == Player.whoAmI && p.ModProjectile is BaseFlyingShieldGuard);
            if (projectile != null)
            {
                projectile.ai[0] = (int)BaseFlyingShieldGuard.GuardState.Guarding;
                (projectile.ModProjectile as BaseFlyingShieldGuard).CompletelyHeldUpShield = true;
                cp.FlyingShieldGuardIndex = projectile.whoAmI;
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            Rectangle rect = projectile.Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.hostile || proj.whoAmI == projectile.Projectile.whoAmI || proj.type != ModContent.ProjectileType<TerranascenceFSProj>())
                    continue;

                if (projectile.Projectile.Colliding(rect, proj.getRect()))
                {
                    (proj.ModProjectile as TerranascenceFSProj).SpurtBy(projectile.Owner.velocity.RotateByRandom(-0.3f, 0.3f));
                    if (ConsumeEnergy())
                    {
                        PowerfulAtt2 = 30;
                        PowerfulRight = 30;
                    }

                    projectile.Timer = 0;
                    projectile.Owner.velocity.X = -Math.Sign(projectile.Owner.velocity.X) * 9;
                    projectile.Owner.velocity.Y = -3f;
                    return;
                }
            }

            projectile.OnGuard_DamageReduce(projectile.damageReduce);

            if (projectile.Timer > projectile.dashTime / 4)
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ConsumeEnergy())
                PowerfulLeft = 30;
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            projectile.OnGuard();
            projectile.OnGuardNPC(target.whoAmI);

            projectile.Timer = 0;
            projectile.Owner.velocity.X = -Math.Sign(projectile.Owner.velocity.X) * 9;
            projectile.Owner.velocity.Y = -3f;
        }

        #endregion

        #region 帮助方法
        /// <summary>
        /// 消耗能量，如果消耗失败则返回<see langword="false"/>
        /// </summary>
        /// <returns></returns>
        public bool ConsumeEnergy()
        {
            if (energy > 0)
            {
                energy--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得能量，最大3
        /// </summary>
        public void GetEnergy()
        {
            energy++;
            energy = Math.Clamp(energy, 0, 3);
        }

        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 35)
                .AddIngredient<GlistentBar>(20)
                .AddTile(TileID.LivingLoom)
                .Register();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("effect", SwordLightNormalAttack);
        }

        public override void LoadData(TagCompound tag)
        {
            SwordLightNormalAttack = tag.GetBool("effect");
        }
    }

    public class TerranascenceFSProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "TerranascenceShield";

        public int StayTime = 80;
        public int RollingTime;
        public float rot = 0.2f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 26;
        }

        public override void SetOtherValues()
        {
            flyingTime = 4;
            backTime = 12;
            backSpeed = 14;
            trailCachesLength = 6;
            trailWidth = 20 / 2;
            maxJump++;
        }

        public override void Shooting()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (RollingTime > 0)
            {
                RollingTime--;
                Projectile.velocity = Projectile.velocity.RotatedBy(rot);
                if (RollingTime < 1)
                    TurnToBack();

                return;
            }

            Chasing();
            if (firstShoot && Timer >= flyingTime - 2)
            {
                Projectile.tileCollide = false;
                if (Timer == flyingTime - 2)
                {
                    firstShoot = false;
                    Projectile.tileCollide = recordTileCollide;
                }
            }

            Timer--;
            if (Timer < 0)
            {
                if (StayTime > 0)
                    State = (int)FlyingShieldStates.JustHited;
                else
                    TurnToBack();
            }
        }

        public override void OnJustHited()
        {
            extraRotation += 0.6f;

            if (StayTime > 0)
            {
                StayTime--;
                if (Main.rand.NextBool(8))
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Green, Helper.NextVec2Dir(2, 4)
                        , Scale: Main.rand.NextFloat(0.2f, 0.6f));
                    d.noGravity = true;
                }
                Projectile.velocity *= 0.92f;
                return;
            }

            jumpCount++;
            if (jumpCount > maxJump)
                TurnToBack();
            else
                JumpInNpcs();

            UpdateShieldAccessory(accessory => accessory.OnJustHited(this));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            rot *= -1;

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -1;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -1f;

            if (RollingTime > 0)
                return false;
            State = (int)FlyingShieldStates.JustHited;
            UpdateShieldAccessory(accessory => accessory.OnTileCollide(this));
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = -Projectile.velocity.RotateByRandom(-0.2f, 0.2f);
            if (RollingTime > 0)
                return;

            if (State != (int)FlyingShieldStates.Backing)
                State = (int)FlyingShieldStates.JustHited;
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(this, target, hit, damageDone));
        }

        public void SpurtBy(Vector2 velocity)
        {
            Projectile.damage = (int)(Owner.GetWeaponDamage(Item) * 0.75f);
            StayTime = 0;
            RollingTime = 3 * 60;
            State = (int)FlyingShieldStates.Shooting;
            Projectile.velocity = velocity;
            Helper.PlayPitched("TheLegendOfZelda/Guard_Wood_Wood_" + Main.rand.Next(4), 0.5f, 0f, Projectile.Center);
        }

        public override void OnShootDusts()
        {
            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(4f, DustID.PureSpray, Main.rand.NextFloat(0.2f, 0.6f)
                    , Scale: Main.rand.NextFloat(0.5f, 0.8f));
            }
            extraRotation += 0.6f;
        }

        public override void OnBackDusts()
        {
            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(4f, DustID.PureSpray, Main.rand.NextFloat(0.2f, 0.6f)
                    , Scale: Main.rand.NextFloat(0.5f, 0.8f));
            }
            extraRotation += 0.6f;
            backSpeed += 0.2f;
        }

        public override Color GetColor(float factor)
        {
            return new Color(60, 200, 150) * factor;
        }
    }

    public class TerranascenceGuard : BaseFlyingShieldPlusGuard
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "TerranascenceShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            scalePercent = 2f;
            distanceAdder = 2;
            delayTime = 10;
            parryTime = 6;
        }

        public override void LimitFields()
        {
            base.LimitFields();
        }

        public override void OnParry()
        {
            if (Item.ModItem is Terranascence terranascence
                && Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (terranascence.ConsumeEnergy())
                {
                    terranascence.PowerfulRight = 30;
                }

                if (cp.parryTime < 100)
                {
                    Owner.immuneTime = 30;
                    Owner.immune = true;
                }

                if (cp.parryTime < 250)
                    cp.parryTime += 80;
            }

            SpawnWoodDust();
            Helper.PlayPitched("TheLegendOfZelda/Guard_Wood_Wood_" + Main.rand.Next(4), 0.5f, 0f, Projectile.Center);
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 2;
            Helper.PlayPitched("TheLegendOfZelda/Guard_Wood_Wood_" + Main.rand.Next(4), 0.5f, 0f, Projectile.Center);
            SpawnWoodDust();
        }

        public void SpawnWoodDust()
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), DustID.WoodFurniture,
                    (Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(2, 6)
                    , Scale: 1);

                d.noGravity = true;
            }
        }

        public override float GetWidth()
        {
            return Projectile.width * 0.5f / Projectile.scale;
        }
    }

    /// <summary>
    /// 指示物弹幕，最多在玩家背后出现3个水晶
    /// </summary>
    public class TerranascenceTag : BaseHeldProj
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + Name;

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
            if (Item.type != ModContent.ItemType<Terranascence>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = Owner.Center;
            if (Item.ModItem is Terranascence terranascence)
            {
                int count = terranascence.energy;

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
            if (Item.ModItem is Terranascence terranascence)
            {
                Texture2D mainTex = Projectile.GetTexture();

                var origin = new Vector2(0, mainTex.Height);
                var pos = Projectile.Center - Main.screenPosition + new Vector2(-Owner.direction * 20, -8);
                int howMany = terranascence.energy;
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

    public class TerranascenceSwing : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "TerranascenceSword";

        public ref float Combo => ref Projectile.ai[0];

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public int delay;
        public int alpha;

        public static Asset<Texture2D> GradientTexture;

        public TerranascenceSwing() : base(0.785f, 16) { }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = ModContent.Request<Texture2D>(AssetDirectory.ShieldPlusItems + "TerranascenceGradient");
        }

        public override void Unload()
        {
            GradientTexture = null;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 40;
            Projectile.height = 60;
            trailTopWidth = 0;
            distanceToOwner = 12;
            onHitFreeze = 8;
            Projectile.hide = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 45 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0:
                    startAngle = 1.7f;
                    totalAngle = 4f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 30;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 14;
                    extraScaleAngle = -0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1f, 1.5f);
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                    break;
                case 1:
                    startAngle = 2.4f;
                    totalAngle = 3.5f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 30;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    delay = 18;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1f, 1.5f);
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);

                    break;
                case 2:
                    startAngle = -2f;
                    totalAngle = -4.9f;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 40;
                    delay = 24;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    useSlashTrail = true;
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.3f, 1.7f);
                    Helper.PlayPitched("Misc/LongSwordSwing2", 0.3f, 0, Projectile.Center);
                    onHitFreeze = 16;
                    break;
            }

            if (Item.ModItem is Terranascence terranascence && terranascence.SwordLightNormalAttack)
                useSlashTrail = true;

            base.InitializeSwing();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.25f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Main.rand.NextBool(3) || timer % 3 == 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Projectile.height * 0.4f) + Main.rand.NextVector2Circular(12, 12)
                    , DustID.PureSpray, RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 2f)
                    , Scale: Main.rand.NextFloat(0.5f, 1f));

                d.noGravity = true;
            }

            if (alpha < 255)
                alpha += 8;

            if (Item.type == ModContent.ItemType<Terranascence>())
                scale = Owner.GetAdjustedItemScale(Item);
            else
                Projectile.Kill();

            float angle = recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime));

            Projectile.scale = Combo switch
            {
                0 => scale * Helper.EllipticalEase(angle, 1f, 1.5f),
                1 => scale * Helper.EllipticalEase(angle, 1f, 1.5f),
                _ => scale * Helper.EllipticalEase(angle, 1.3f, 1.7f),
            };

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (DownRight)
            {
                Projectile.Kill();
                Owner.itemAnimation = Owner.itemTime = 0;
                return;
            }
            if (alpha > 20)
                alpha -= 10;

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Timer < minTime || (Timer > maxTime && Combo < 3))
                return false;

            if (target.noTileCollide || target.friendly || Projectile.hostile)
                return null;

            if (Collision.CanHit(Owner, target))
                return null;

            return false;
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);

            if (onHitTimer != 1 || !VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
            Vector2 pos = Bottom + (RotateVec2 * offset);
            pos = Vector2.SmoothStep(pos, target.Center, 0.4f);

            float rot;

            Color c = new(50, 200, 150, 150);
            for (int i = -1; i < 2; i += 2)
            {
                rot = _Rotation + (Main.rand.NextFloat(0.7f, 1.4f) * i);

                for (int j = 0; j < 2; j++)
                {
                    LightShotParticle.Spawn(pos, c, rot + Main.rand.NextFloat(-0.2f, 0.2f)
                        , new Vector2(Main.rand.NextFloat(0.1f, 0.5f)
                        , 0.02f));
                    LightShotParticle.Spawn(pos, Color.DarkSeaGreen, rot + Main.rand.NextFloat(-0.2f, 0.2f)
                        , new Vector2(Main.rand.NextFloat(0.1f, 0.3f)
                        , 0.01f));

                    rot += MathHelper.Pi;
                }
            }

            for (int i = 0; i < 4; i++)
                LightTrailParticle_NoPrimitive.Spawn(pos, Helper.NextVec2Dir(2f, 3f), c, Main.rand.NextFloat(0.1f, 0.15f));

            if (Combo == 2)
            {
                Helper.PlayPitched("Misc/LongSwordSwing2", 0.4f, -0.4f, Projectile.Center);
                Helper.PlayPitched("Misc/BloodySlash", 0.4f, 0.5f, Projectile.Center);
                if (VisualEffectSystem.HitEffect_ScreenShaking)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 10, 8, 10, 1000));
            }
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
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

                var c = new Color(255, 255, 255, Helper.Lerp(alpha, 0, 1 - factor));
                bars.Add(new(Top.Vec3(), c, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), c, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split.Value);
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

    public class TerranascenceSpurt : BaseSwingProj
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "TerranascenceSword";

        /// <summary>
        /// 会冲刺
        /// </summary>
        public bool Powerful => Projectile.ai[0] > 0;
        /// <summary>
        /// 会冲刺并且更加强力
        /// </summary>
        public bool Powerful2 => Projectile.ai[1] > 0;

        public static Asset<Texture2D> EXTex;

        public float alpha;
        private int delay;

        public float distanceToOwner2;
        public float yScale;

        public bool canDrawBaseEffect;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            EXTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail");
        }

        public override void Unload()
        {
            EXTex = null;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 40;
            Projectile.height = 70;
            distanceToOwner = -45;
            distanceToOwner2 = -15;
            onHitFreeze = 0;
            Projectile.hide = true;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            startAngle = 0.001f;
            totalAngle = 0.002f;
            minTime = 0;
            Smoother = Coralite.Instance.HeavySmootherInstance;

            if (Powerful)
            {
                SpawnCircle();
                canDrawBaseEffect = true;
                delay = 8;
                if (Powerful2)
                {
                    maxTime = 12;
                    Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.2f, Projectile.Center);
                    Helper.PlayPitched("Misc/EnergyBurst", 0.4f, 0.2f, Projectile.Center);
                    Main.instance.CameraModifiers.Add(new MoveModifyer(8, 15));
                }
                else
                {
                    maxTime = 10;
                    Helper.PlayPitched("Misc/FlowSwing2", 0.4f, 0.2f, Projectile.Center);
                }
            }
            else
            {
                if (Item.ModItem is Terranascence terranascence && terranascence.SwordLightNormalAttack)
                    canDrawBaseEffect = true;

                maxTime = 8;
                delay = 10;
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
            }
            base.InitializeSwing();
        }

        public void SpawnCircle()
        {
            float angle = GetStartAngle();
            Vector2 dir = angle.ToRotationVector2();
            Color c1 = Color.LightSeaGreen;
            WindCircle.Spawn(Projectile.Center + (dir * 60), -dir * 4, angle, c1, 0.75f, 0.95f, new Vector2(1.5f, 0.8f));
            WindCircle.Spawn(Projectile.Center + (dir * 70), -dir * 3, angle, c1, 0.75f, 0.5f, new Vector2(1.5f, 0.8f));
            c1 = Color.DarkSeaGreen;
            WindCircle.Spawn(Projectile.Center + (dir * 50), -dir * 3, angle, c1, 0.5f, 1.4f, new Vector2(2, 1f));
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.25f);
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            distanceToOwner = -45 + (Smoother.Smoother((int)Timer, maxTime) * 58);
            distanceToOwner2 = -15 + (Coralite.Instance.SqrtSmoother.Smoother((int)Timer, maxTime) * 60);
            yScale = Coralite.Instance.SqrtSmoother.Smoother((int)Timer, maxTime);

            if (alpha < 1)
            {
                alpha += 0.3f;
                if (alpha > 1)
                    alpha = 1;
            }

            if (Powerful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d2 = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Projectile.height / 2) + Main.rand.NextVector2Circular(6, 6)
                        , DustID.PureSpray, RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 2f)
                        , Scale: Main.rand.NextFloat(0.5f, 1f));

                    d2.noGravity = true;
                }

                Vector2 velocity = RotateVec2 * 15;
                if (Powerful2)
                {
                    velocity = RotateVec2 * 22;
                    if (Timer > 2)
                        PRTLoader.NewParticle<SpeedLine>(Projectile.Center, -RotateVec2, Color.LightSeaGreen, 0.6f);

                    Dust d = Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(0, 32), DustID.Firework_Green,
                        -RotateVec2 * Main.rand.NextFloat(4, 6), Scale: Main.rand.NextFloat(0.2f, 0.8f));
                    d.noGravity = true;
                }
                else
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(0, 32), DustID.GreenTorch,
                        -RotateVec2 * Main.rand.NextFloat(2, 4));
                    d.noGravity = true;
                    if (CheckCollideWithShield(velocity))
                        return;
                }
                Owner.velocity = velocity;
                if (Timer == maxTime)
                    Owner.velocity *= 0.5f;
            }

            base.OnSlash();
        }

        public bool CheckCollideWithShield(Vector2 velocity)
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.hostile || proj.whoAmI == Projectile.whoAmI || proj.type != ModContent.ProjectileType<TerranascenceFSProj>())
                    continue;

                if (Projectile.Colliding(rect, proj.getRect()))
                {
                    (proj.ModProjectile as TerranascenceFSProj).SpurtBy(velocity.RotateByRandom(-0.3f, 0.3f));
                    Timer = maxTime + 1;
                    Owner.velocity *= -0.3f;
                    if (Item.ModItem is Terranascence terranascence)
                    {
                        terranascence.PowerfulAtt2 = 30;
                        terranascence.PowerfulRight = 30;
                        terranascence.GetEnergy();
                    }
                    return true;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            if (VisualEffectSystem.HitEffect_Lightning)
                PRTLoader.NewParticle<SpeedLine>(Projectile.Center + (RotateVec2 * Projectile.height / 2), RotateVec2 * 5, Color.LightSeaGreen, 0.6f);

            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Main.rand.NextFloat(0, Projectile.height)),
                        DustID.GreenTorch, RotateVec2.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(4f, 8f)
                        , Scale: Main.rand.NextFloat(1, 1.5f));
                    d.noGravity = true;
                }

            if (Powerful)
            {
                if (VisualEffectSystem.HitEffect_SpecialParticles)
                    WindCircle.Spawn(Owner.Center - (RotateVec2 * 20), -Owner.velocity * 0.4f, RotateVec2.ToRotation(), Color.DarkSeaGreen, 0.75f, 1f, new Vector2(1.75f, 0.7f));
                if (!Powerful2)
                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, RotateVec2, 12, 8, 8, 1000));

                Timer = maxTime + 1;
                Owner.velocity *= -0.3f;

                if (VisualEffectSystem.HitEffect_Lightning)
                    for (int i = 0; i < 4; i++)
                        PRTLoader.NewParticle<SpeedLine>(Projectile.Center + (i * RotateVec2 * 24), RotateVec2 * 5, Color.LightSeaGreen, 0.6f - (i * 0.1f));
            }
        }

        protected override void AfterSlash()
        {
            distanceToOwner = 13 - (23 * Smoother.Smoother((int)Timer - maxTime, delay));
            yScale = 1 - Smoother.Smoother((int)Timer - maxTime, delay);
            alpha = 1 - Smoother.Smoother((int)Timer - maxTime, delay);
            distanceToOwner2 += 7;
            if (DownRight)
            {
                Projectile.Kill();
                Owner.itemAnimation = Owner.itemTime = 0;
                return;
            }

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor * alpha, extraRot);
            base.DrawSelf(mainTex, origin, new Color(100, 100, 100, 0) * alpha, extraRot);

            Texture2D ex = EXTex.Value;

            Color c = Color.LightSeaGreen;
            c.A = 0;
            c *= 0.75f;
            c *= alpha;

            Color c3 = Color.DarkSeaGreen;
            c3.A = 0;
            c3 *= 0.75f;
            c3 *= alpha;

            Vector2 pos = OwnerCenter() - Main.screenPosition + (RotateVec2 * distanceToOwner2);
            if (canDrawBaseEffect)
            {
                Main.spriteBatch.Draw(ex, pos, null,
                                        c3, Projectile.rotation, ex.Size() / 2, 0.5f * new Vector2(1, yScale), 0, 0f);
                Main.spriteBatch.Draw(ex, pos, null,
                                        c, Projectile.rotation, ex.Size() / 2, 0.5f * new Vector2(1, 0.5f * yScale), 0, 0f);
            }

            if (Powerful)
            {
                c *= 0.7f;
                //Main.spriteBatch.Draw(ex, pos, null,
                //        c, Projectile.rotation, ex.Size() / 2, 0.7f * new Vector2(1, 0.5f * yScale), 0, 0f);
                Color c2 = Color.White;
                c2.A = 0;
                c2 *= alpha;
                Main.spriteBatch.Draw(ex, pos, null,
                                        c3, Projectile.rotation, ex.Size() / 2, new Vector2(0.7f, 0.6f * yScale), 0, 0f);
                Main.spriteBatch.Draw(ex, pos - (RotateVec2 * distanceToOwner2 * 1.5f), null,
                                        c, Projectile.rotation, ex.Size() / 2, new Vector2(1f, 0.6f * yScale), 0, 0f);
                Main.spriteBatch.Draw(ex, pos - (RotateVec2 * distanceToOwner2 / 2), null,
                                        c2 * 0.5f, Projectile.rotation, ex.Size() / 2, 0.9f * new Vector2(1f, 0.5f * yScale), 0, 0f);
            }
        }
    }
}