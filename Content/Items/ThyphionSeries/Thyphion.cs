using Coralite.Content.Items.Thunder;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Coralite.Content.Items.ThyphionSeries.ThyphionPhantomArrow;
using static Terraria.ModLoader.ModContent;
#pragma warning disable CS8524 // switch 表达式不会处理其输入类型的某些值(它不是穷举)，这包括未命名的枚举值。

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Thyphion : ModItem, IDashable
    {
        public static Color ThyphionColor1 = new Color(66, 152, 201);
        public static Color ThyphionColor2 = new Color(85, 198, 207);
        public static Color ThyphionColor3 = new Color(103, 244, 194);

        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public int shootCount;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(235, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 20, 19f, true);

            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 35);

            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            int shootThyphionArrow = 0;
            switch (shootCount)
            {
                default://普普通通连射
                case 0:
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

                    Helper.PlayPitched("Misc/Do", 0.7f, 0, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 1:
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<ThyphionTagProj>()
                        , damage, knockback, player.whoAmI, 1, type, velocity.Length());

                    Helper.PlayPitched("Misc/Do", 0.7f, 4 / 13f, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 2:
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<ThyphionTagProj>()
                        , damage, knockback, player.whoAmI, 2, type, velocity.Length());

                    Helper.PlayPitched("Misc/Do", 0.7f, 7 / 13f, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 3://射出贯穿箭
                    {
                        Helper.PlayPitched("Misc/Do", 0.7f, 1f, player.Center);
                        Helper.PlayPitched("Misc/Arrow", 0.4f, 0.1f, player.Center);
                        Helper.PlayPitched("Misc/EnergyBurst", 0.2f, 0.3f, player.Center);
                        Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, player.Center, volume: 0.4f);
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ThyphionArrow>(), damage, knockback, player.whoAmI, 0, 0, 1);
                        shootThyphionArrow = 1;
                    }
                    break;
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero
                , ProjectileType<ThyphionHeldProj>(), damage, knockback, player.whoAmI, rot, 0, shootThyphionArrow);

            shootCount++;

            if (shootCount > 3)
                shootCount = 0;

            return false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 17;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 100;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immune = true;
            Player.AddImmuneTime(ImmunityCooldownID.General, 20);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<ThyphionHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThyphionHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 20);
            }

            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = new StatModifier(1 + (damage.Additive - 1) / 10, 1);
        }
    }

    public class ThyphionTagProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.localAI[0];
        public ref float State => ref Projectile.ai[0];
        public ref float ArrowType => ref Projectile.ai[1];
        public ref float Speed => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.HeldItem.type != ItemType<Thyphion>())
                Projectile.Kill();

            switch (State)
            {
                default:
                case 1:
                    {
                        if (Timer < Owner.itemTimeMax / 3 - 1 && Timer % (Owner.itemTimeMax / 9) == 0)
                        {
                            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);

                            Projectile.NewProjectileFromThis(Owner.Center + Main.rand.NextVector2Circular(20, 20), dir * Speed, (int)ArrowType,
                                Projectile.damage, Projectile.knockBack);
                        }
                    }
                    break;
                case 2:
                    {
                        if (Timer < Owner.itemTimeMax / 2 - 1 && Timer % (Owner.itemTimeMax / 10) == 0)
                        {
                            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);

                            Projectile.NewProjectileFromThis(Owner.Center + Main.rand.NextVector2Circular(20, 20), dir * Speed, (int)ArrowType,
                                Projectile.damage, Projectile.knockBack);
                        }
                    }
                    break;
            }

            Timer++;
            if (Timer > Owner.itemTimeMax)
                Projectile.Kill();
        }

        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor) => false;
    }

    [AutoLoadTexture(Path =AssetDirectory.ThyphionSeriesItems)]
    public class ThyphionHeldProj : BaseDashBow, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Thyphion";

        public ref float Timer => ref Projectile.localAI[0];

        public float ReleaseTimer;
        public ref float Release => ref Projectile.localAI[1];

        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset = 0;

        public bool ShowArrow = false;

        [AutoLoadTexture(Name = "Thyphion_glow")]
        public static ATex GlowTex { get; private set; }

        public int projectile;

        public override int GetItemType()
            => ItemType<Thyphion>();

        public override Vector2 GetOffset()
            => new(34 + handOffset, -12);

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }
        public override void SetCenter()
        {
            base.SetCenter();
        }
        public override void NormalShootAI()
        {
            //借用AI2来判断射出的是不是穿透箭
            if (Projectile.ai[2] == 1)
            {
                if (Timer == 0)
                {
                    handOffset = -10;

                    Vector2 ShootDir = Projectile.ai[0].ToRotationVector2();
                    Color lightCyen = new Color(Thyphion.ThyphionColor3.R, Thyphion.ThyphionColor3.G, Thyphion.ThyphionColor3.B, 0);
                    WindCircle.Spawn(Projectile.Center + (ShootDir * 40), -ShootDir * 2 + Owner.velocity, Rotation, lightCyen, 0.6f, 1.3f, new Vector2(1.4f, 1f));
                    Color skyBlue = Thyphion.ThyphionColor1;
                    WindCircle.Spawn(Projectile.Center + (ShootDir * 30), -ShootDir * 3 + Owner.velocity, Rotation, skyBlue, 0.2f, 1.3f, new Vector2(1.6f, 1.3f));

                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                    if (Timer < 10)
                        Rotation -= Owner.direction * 0.02f;
                    else
                        Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);

                    handOffset = MathHelper.Lerp(handOffset, 0, 0.13f);
                }

            }
            Timer++;
            base.NormalShootAI();
        }

        public override void DashAttackAI()
        {
            do
            {
                //冲刺
                if (Timer < DashTime + 2)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 dir = (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One);
                        Owner.itemTime = Owner.itemAnimation = 2;
                        Rotation = Rotation.AngleLerp(dir.ToRotation(), 0.15f);
                        Owner.direction = Math.Sign(dir.X);
                    }
                    if (Timer % 4 == 0)
                    {
                        int bowIndex = Main.rand.Next(1, (int)BowType.End);

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ProjectileType<ThyphionShadowBow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, bowIndex);
                    }

                    Owner.itemTime = Owner.itemAnimation = 2;
                    ShowArrow = true;
                    if (Timer == DashTime)
                    {
                        Owner.velocity = new Vector2(Owner.velocity.X * 0.5f, Owner.velocity.Y);
                    }
                    break;
                }
                else if (Timer == DashTime + 2)
                {
                    Vector2 dir = Rotation.ToRotationVector2();
                    projectile = Projectile.NewProjectileFromThis(Owner.Center, dir * 12f, ProjectileType<ThyphionArrow>(), Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, 0, 0, 2);
                }

                //右键瞄准/发射
                if (DownLeft && ReleaseTimer == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        float lerpSpeed = DownLeft ? 0.15f : 1f;
                        Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), lerpSpeed);
                    }
                    Projectile.timeLeft = 2;
                    if (TryGetArrowProjectile(projectile, out var thyphionArrow))
                    {
                        Vector2 dir = Rotation.ToRotationVector2();
                        thyphionArrow.Hold(Owner.Center, dir * 12f);
                    }
                    LockOwnerItemTime();
                }
                else
                {
                    if (ReleaseTimer == 0)
                    {
                        Vector2 dir = Rotation.ToRotationVector2();

                        SpwanShootParticle(dir);
                        ShowArrow = false;

                        handOffset = -30;

                        if (VisualEffectSystem.HitEffect_ScreenShaking)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));
                    }
                    if (ReleaseTimer >= 20)
                    {
                        Projectile.Kill();
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        if (ReleaseTimer < 10)
                            Rotation -= Owner.direction * 0.05f;
                        else
                            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                    }

                    handOffset = MathHelper.Lerp(handOffset, 0, 0.07f);
                    ReleaseTimer++;

                    LockOwnerItemTime();
                    Projectile.timeLeft = 2;
                }
            } while (false);

            Timer++;
            Projectile.rotation = Rotation;
            base.DashAttackAI();
        }

        private void SpwanShootParticle(Vector2 ShootDir)
        {
            Color lightCyen = new Color(Thyphion.ThyphionColor3.R, Thyphion.ThyphionColor3.G, Thyphion.ThyphionColor3.B, 0);
            WindCircle.Spawn(Projectile.Center + (ShootDir * 20), -ShootDir * 2 + Owner.velocity, Rotation, lightCyen, 0.6f, 1.3f, new Vector2(1.4f, 1f));
            WindCircle.Spawn(Projectile.Center + (ShootDir * 20), -ShootDir * 2 + Owner.velocity, Rotation, lightCyen, 0.3f, 1f, new Vector2(1.2f, 1f));

            Color skyBlue = Thyphion.ThyphionColor1;
            WindCircle.Spawn(Projectile.Center + (ShootDir * 15), -ShootDir * 3 + Owner.velocity, Rotation, skyBlue, 0.2f, 1.3f, new Vector2(1.6f, 1.3f));

            Color lightBlue = new Color(Thyphion.ThyphionColor2.R, Thyphion.ThyphionColor2.G, Thyphion.ThyphionColor2.B, 150);
            Tornado.Spawn(Projectile.Center + (ShootDir * 20), ShootDir * -3 + Owner.velocity, lightBlue, 15, ShootDir.ToRotation(), Main.rand.NextFloat(0.7f, 0.5f));
            Tornado.Spawn(Projectile.Center + (ShootDir * 20), ShootDir * -3 + Owner.velocity, lightBlue, 15, ShootDir.ToRotation(), Main.rand.NextFloat(0.7f, 0.5f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            if (Special == 1)
            {
                if (ShowArrow)
                {
                    float value = Math.Clamp(Timer / DashTime, 0, 1);
                    Vector2 dir2 = (Rotation).ToRotationVector2();
                    ThyphionArrow.ArrowSelfDraw(Owner.MountedCenter + dir2 * 40, 0.8f * value, Projectile.rotation.ToRotationVector2(), value);
                }
            }
            else
            {

            }

            return false;
        }

        public void DrawPrimitives()
        {

        }

        private bool TryGetArrowProjectile(int index, out ThyphionArrow thyphionArrow)
        {
            Projectile p = Main.projectile[index];
            if (p != null && p.type == ProjectileType<ThyphionArrow>())
            {
                thyphionArrow = p.ModProjectile as ThyphionArrow;
                return true;
            }

            thyphionArrow = null;
            return false;
        }
    }

    public class ThyphionArrow : ModProjectile, IDrawPrimitive
    {
        public int SurrroundTrailCount = 20;
        public override string Texture => AssetDirectory.Blank;

        public bool CanDoDamage = true;

        private bool hasInit = false;
        private ref float Timer => ref Projectile.ai[0];
        private ref float StartSpeed => ref Projectile.ai[1];

        private ArrowType arrowType => (ArrowType)Projectile.ai[2];

        public enum ArrowType
        {
            //幻影弓使用
            Normal,
            //左键第四下射出
            PlayerShoot,
            //冲刺攻击
            DashAttack
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            return CanDoDamage;
        }

        public void Init()
        {
            hasInit = true;
            StartSpeed = Projectile.velocity.Length();
            StartSpeed = MathHelper.Clamp(StartSpeed, 5, 7);
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One) * StartSpeed;
            switch (arrowType)
            {
                case ArrowType.Normal:
                    Projectile.scale = 0.7f;
                    break;
                case ArrowType.PlayerShoot:
                    Projectile.scale = 1f;
                    break;
                case ArrowType.DashAttack:
                    Projectile.scale = 1.3f;
                    break;
            }
            Projectile.InitOldPosCache(20 + 4);

        }

        public override void AI()
        {
            if (!hasInit)
            {
                Init();
            }
            if (!IsHoldBy())
            {
                ArrowSpeedUpdate();
                ArrowLight();
                ArrowDust();
            }
            Timer++;
            base.AI();
        }

        private void ArrowSpeedUpdate()
        {
            if (Timer < 20 && !IsHoldBy())
            {
                Projectile.velocity = Projectile.velocity * 1.05f;
            }

        }

        private void ArrowLight()
        {
            Lighting.AddLight(Projectile.Center, Thyphion.ThyphionColor3.ToVector3());
        }

        private void ArrowDust()
        {
            int dustWidth = arrowType switch
            {
                ArrowType.Normal => 5,
                ArrowType.PlayerShoot => 10,
                ArrowType.DashAttack => 15,
            };
            int speedLineWidth = arrowType switch
            {
                ArrowType.Normal => 6,
                ArrowType.PlayerShoot => 10,
                ArrowType.DashAttack => 13,
            };
            int flareFrequnce = arrowType switch
            {
                ArrowType.Normal => 6,
                ArrowType.PlayerShoot => 5,
                ArrowType.DashAttack => 3
            };
            int platinumFrequnce = arrowType switch
            {
                ArrowType.Normal => 8,
                ArrowType.PlayerShoot => 6,
                ArrowType.DashAttack => 4
            };

            int speedLineFrequnce = arrowType switch
            {
                ArrowType.Normal => 4,
                ArrowType.PlayerShoot => 3,
                ArrowType.DashAttack => 2
            };

            if (Main.rand.NextBool(flareFrequnce))
            {
                Dust flare = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(dustWidth, dustWidth), DustID.BlueFlare
                    , Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f), 200, Scale: Main.rand.NextFloat(1, 1.4f));
                flare.noGravity = true;
                flare.color = GetDustRandomColor(1, 1, 1, 200);
            }
            if (Main.rand.NextBool(platinumFrequnce))
            {
                Dust Platinum = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(dustWidth, dustWidth), DustID.PlatinumCoin
                    , Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f), 255, Scale: Main.rand.NextFloat(1, 1.4f));
                Platinum.noGravity = true;
                Platinum.color = GetDustRandomColor(4, 2, 1, 100);
            }
            if (Timer % speedLineFrequnce == 0 && Main.rand.NextBool())
            {
                PRTLoader.NewParticle<SpeedLine>(Projectile.Center + Main.rand.NextVector2Circular(speedLineWidth, speedLineWidth)
                    , Projectile.velocity * Main.rand.NextFloat(0.6f, 1f) * 1.5f, GetDustRandomColor(4, 2, 1, 255), Main.rand.NextFloat(0.1f, 0.3f));
            }
        }

        private void ArrowHitDust(Vector2 HitPosition)
        {
            int dustWidth = arrowType switch
            {
                ArrowType.Normal => 5,
                ArrowType.PlayerShoot => 10,
                ArrowType.DashAttack => 15,
            };
            float dustSpeedMul = arrowType switch
            {
                ArrowType.Normal => 1,
                ArrowType.PlayerShoot => 1.4f,
                ArrowType.DashAttack => 1.7f,
            };

            for (int i = 0; i < 7; i++)
            {
                int reverseVel = Main.rand.NextBool(4) ? 1 : -1;
                float velRot = Main.rand.NextFloat(-0.1f, 0.1f);
                Dust flare = Dust.NewDustPerfect(HitPosition + Main.rand.NextVector2Circular(dustWidth, dustWidth), DustID.BlueFlare
                            , Projectile.velocity.RotatedBy(velRot) * Main.rand.NextFloat(0.3f, 0.5f) * reverseVel * dustSpeedMul, 200, Scale: Main.rand.NextFloat(1, 1.4f));
                flare.noGravity = true;
                flare.color = GetDustRandomColor(1, 1, 1, 200);
            }
            for (int i = 0; i < 3; i++)
            {
                int reverseVel = Main.rand.NextBool(4) ? 1 : -1;
                float velRot = Main.rand.NextFloat(-0.2f, 0.2f);
                Dust platinum = Dust.NewDustPerfect(HitPosition + Main.rand.NextVector2Circular(dustWidth, dustWidth), DustID.PlatinumCoin
                            , Projectile.velocity.RotatedBy(velRot) * Main.rand.NextFloat(0.3f, 0.5f) * reverseVel * dustSpeedMul, 200, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                platinum.noGravity = true;
                platinum.color = GetDustRandomColor(3, 1, 1, 200);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (IsHoldBy())
                return false;
            ArrowSelfDraw(Projectile.Center, Projectile.scale, Projectile.velocity, 1);
            return false;
        }

        public static void ArrowSelfDraw(Vector2 center, float scale, Vector2 arrowDir, float alpha)
        {
            Texture2D texture = TextureAssets.Extra[98].Value;
            Vector2 ArrowSize = new Vector2(1, 7) * 0.3f * scale;
            Vector2 InnerSize = ArrowSize;
            Vector2 SecondLayerSize = ArrowSize * 1.5f;
            Vector2 OuterSize = ArrowSize * 3 * new Vector2(0.7f, 1);

            Vector2 Center = center - Main.screenPosition;

            Color SkyBlue = Thyphion.ThyphionColor1 * 0.5f * alpha;
            Main.spriteBatch.Draw(texture, Center - arrowDir.SafeNormalize(Vector2.One) * 40 * scale, null, SkyBlue, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, OuterSize, 0, 0);

            Color lightBlue = new Color(Thyphion.ThyphionColor2.R, Thyphion.ThyphionColor2.G, Thyphion.ThyphionColor2.B, 200) * alpha;
            Main.spriteBatch.Draw(texture, Center - arrowDir.SafeNormalize(Vector2.One) * 10 * scale, null, lightBlue, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, SecondLayerSize, 0, 0);
            Main.spriteBatch.Draw(texture, Center - arrowDir.SafeNormalize(Vector2.One) * 25 * scale, null, lightBlue, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, SecondLayerSize, 0, 0);

            Color lightCyen = new Color(Thyphion.ThyphionColor3.R, Thyphion.ThyphionColor3.G, Thyphion.ThyphionColor3.B, 70) * alpha;
            Main.spriteBatch.Draw(texture, Center + arrowDir.SafeNormalize(Vector2.One) * 20 * scale, null, lightCyen, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, InnerSize * 0.5f, 0, 0);
            Main.spriteBatch.Draw(texture, Center, null, lightCyen, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, InnerSize, 0, 0);
            Main.spriteBatch.Draw(texture, Center - arrowDir.SafeNormalize(Vector2.One) * 5 * scale, null, lightCyen, arrowDir.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, InnerSize, 0, 0);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }

        //粒子加顶点的设计？
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Rectangle npcHitBox = target.Hitbox;
            Vector2 DustSpwanPos = Vector2.Clamp(Projectile.Center, npcHitBox.TopLeft(), npcHitBox.BottomRight());

            ArrowHitDust(DustSpwanPos);

            base.OnHitNPC(target, hit, damageDone);
        }

        public Color GetDustRandomColor(int lightWeight = 1, int normalWeight = 1, int darkWeight = 1, int alpha = 255)
        {
            float random = Main.rand.NextFloat(lightWeight + normalWeight + darkWeight);
            if (random <= lightWeight)
            {
                return new Color(Thyphion.ThyphionColor3.R, Thyphion.ThyphionColor3.G, Thyphion.ThyphionColor3.B, alpha);
            }
            else if (random <= normalWeight)
            {
                return new Color(Thyphion.ThyphionColor2.R, Thyphion.ThyphionColor2.G, Thyphion.ThyphionColor2.B, alpha);
            }
            else
            {
                return new Color(Thyphion.ThyphionColor1.R, Thyphion.ThyphionColor1.G, Thyphion.ThyphionColor1.B, alpha);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return !IsHoldBy();
        }

        public bool IsHoldBy()
        {
            return Timer <= 3;
        }

        public void Hold(Vector2 pos, Vector2 vel)
        {
            Projectile.Center = pos;
            Projectile.velocity = vel;
            Timer = 0;
            Projectile.timeLeft = 100;
        }

        public void DrawPrimitives()
        {

        }
    }

    public class ThyphionShadowBow : ModProjectile
    {
        private Player PhantomPlayer;

        private int BowType => (int)Projectile.ai[0];

        private ArrowData arrowData => GetArrowData((BowType)BowType);

        private Vector2 MouseDir => (Main.MouseWorld - Projectile.Center);

        public override string Texture => AssetDirectory.Blank;

        private bool hasInit = false;

        //记录玩家信息
        Vector2 PlayerVelocity;
        int PlayerWingFrame;
        float Rotation;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.timeLeft = 40;
        }

        public override void AI()
        {
            if (!hasInit)
            {
                Init();
            }

            Vector2 MouseDir = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.One);
            if (Projectile.timeLeft == 24)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, MouseDir.SafeNormalize(Vector2.One) * 5, ModContent.ProjectileType<ThyphionArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, 0);

            if (Projectile.timeLeft < 20)
                Projectile.Opacity = Projectile.timeLeft / 20f;
            else
            {
                Rotation = MathHelper.Lerp(Rotation, MouseDir.ToRotation(), 0.07f);

            }

            base.AI();
        }

        public void Init()
        {
            hasInit = true;
            Main.instance.LoadItem(arrowData.ItemType);
            InitPhantomPlayer();
        }

        private void InitPhantomPlayer()
        {
            Player Owner = Main.player[Projectile.owner];
            PhantomPlayer = new Player();
            PhantomPlayer.CopyVisuals(Owner);
            PlayerVelocity = Owner.velocity;
            PlayerWingFrame = Owner.wingFrame;
            Rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPhantomPlayer();
            if (PhantomPlayer is null)
                return false;
            Vector2 ArmPosition = PhantomPlayer.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Rotation - MathHelper.PiOver2);
            ArmPosition.Y += PhantomPlayer.gfxOffY;

            Texture2D texture = TextureAssets.Item[arrowData.ItemType].Value;
            Vector2 offest = arrowData.Offest.RotatedBy(Rotation);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offest, null, Color.White * Projectile.Opacity, Rotation, texture.Size() / 2f, 1, 0, 0);
            return false;
        }

        private void DrawPhantomPlayer()
        {
            if (PhantomPlayer == null)
                return;

            Vector2 MouseDir = (Projectile.Center - Main.MouseWorld).SafeNormalize(Vector2.One);
            Player Owner = Main.player[Projectile.owner];

            PhantomPlayer.ResetEffects();
            PhantomPlayer.ResetVisibleAccessories();
            PhantomPlayer.UpdateDyes();
            PhantomPlayer.DisplayDollUpdate();
            PhantomPlayer.UpdateSocialShadow();
            PhantomPlayer.itemAnimationMax = 60;
            PhantomPlayer.itemAnimation = 1;

            PhantomPlayer.velocity = PlayerVelocity.SafeNormalize(Vector2.One);
            PhantomPlayer.wingFrame = PlayerWingFrame;

            PhantomPlayer.itemRotation = Rotation;
            int dir = (PhantomPlayer).itemRotation.ToRotationVector2().X > 0 ? 1 : -1;
            PhantomPlayer.direction = dir;

            Player.CompositeArmStretchAmount compositeArmStretchAmount = Projectile.timeLeft switch
            {
                <= 25 => Player.CompositeArmStretchAmount.ThreeQuarters,
                > 25 and <= 30 => Player.CompositeArmStretchAmount.Quarter,
                > 30 and <= 35 => Player.CompositeArmStretchAmount.None,
                > 35 and <= 40 => Player.CompositeArmStretchAmount.Full,
                _ => Player.CompositeArmStretchAmount.Full
            };

            PhantomPlayer.SetCompositeArmFront(true, compositeArmStretchAmount, PhantomPlayer.itemRotation - MathHelper.PiOver2);
            PhantomPlayer.PlayerFrame();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            PhantomPlayer.Center = Projectile.Center;
            Main.PlayerRenderer.DrawPlayer(Main.Camera, PhantomPlayer, PhantomPlayer.position, 0f, PhantomPlayer.fullRotationOrigin, 1 - Projectile.Opacity);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
    }

    public class ThyphionPhantomArrow
    {
        public enum BowType
        {
            /// <summary> 木弓 </summary>
            WoodenBow = 0,
            /// <summary> 灰烬木弓 </summary>
            AshWoodBow,
            /// <summary> 云杉木弓 </summary>
            BorealWoodBow,
            /// <summary> 棕榈木弓 </summary>
            PalmWoodBow,
            /// <summary> 红木弓 </summary>
            RichMahoganyBow,
            /// <summary> 乌木弓 </summary>
            EbonwoodBow,
            /// <summary> 暗影木弓 </summary>
            ShadewoodBow,
            /// <summary> 珍珠木弓 </summary>
            PearlwoodBow,

            /// <summary> 乱流 </summary>
            Turbulence,

            /// <summary> 震波 </summary>
            SeismicWave,

            /// <summary> 冰雹 </summary>
            Hail,
            /// <summary> 冰之弓 </summary>
            IceBow,
            /// <summary> 冰封世界 </summary>
            Glaciate,

            /// <summary> 晚霞 </summary>
            AfterGlow,
            /// <summary> 熔火之怒 </summary>
            MoltenFury,
            /// <summary> 地狱蝙蝠弓 </summary>
            HellwingBow,
            /// <summary> 旭日 </summary>
            RadiantSun,

            /// <summary> 暗影焰弓 </summary>
            ShadowFlameBow,
            /// <summary> 猩红弓 </summary>
            TendonBow,
            /// <summary> 腐化弓 </summary>
            DemonBow,
            /// <summary> 暗月 </summary>
            FullMoon,

            /// <summary> 日月同辉 </summary>
            Solunar,

            /// <summary> 遥远青空 </summary>
            FarAwaySky,
            /// <summary> 环彩弧 </summary>
            HorizonArc,

            /// <summary> 颤弦弓 </summary>
            TremblingBow,
            /// <summary> 电浆弓 </summary>
            PlasmaBow,
            /// <summary> 逆闪电 </summary>
            ReversedFlash,

            /// <summary> 极光 </summary>
            Aurora,
            End
        }

        public struct ArrowData
        {
            public int ItemType;

            public Color ArrowColor;

            public Vector2 Offest;
        }

        public static ArrowData GetArrowData(BowType type)
        {
            switch (type)
            {
                case BowType.WoodenBow:
                    return new ArrowData()
                    {
                        ItemType = 39,
                        Offest = new(8, 0)
                    };
                case BowType.AshWoodBow:
                    return new ArrowData()
                    {
                        ItemType = 5282,
                        Offest = new(8, 0)
                    };
                case BowType.BorealWoodBow:
                    return new ArrowData()
                    {
                        ItemType = 2747,
                        Offest = new(8, 0)
                    };
                case BowType.PalmWoodBow:
                    return new ArrowData()
                    {
                        ItemType = 2515,
                        Offest = new(8, 0)
                    };
                case BowType.RichMahoganyBow:
                    return new ArrowData()
                    {
                        ItemType = 658,
                        Offest = new(8, 0)
                    };
                case BowType.EbonwoodBow:
                    return new ArrowData()
                    {
                        ItemType = 655,
                        Offest = new(8, 0)
                    };
                case BowType.ShadewoodBow:
                    return new ArrowData()
                    {
                        ItemType = 923,
                        Offest = new(8, 0)
                    };
                case BowType.PearlwoodBow:
                    return new ArrowData()
                    {
                        ItemType = 661,
                        Offest = new(8, 0)
                    };
                case BowType.AfterGlow:
                    return new ArrowData()
                    {
                        ItemType = ModContent.ItemType<Afterglow>(),
                        Offest = new(8, 0)
                    };
                case BowType.FarAwaySky:
                    return new ArrowData()
                    {
                        ItemType = ModContent.ItemType<FarAwaySky>(),
                        Offest = new(8, 0)
                    };
                case BowType.Hail:
                    return new ArrowData()
                    {
                        ItemType = 39,
                        Offest = new(8, 0)
                    };
                case BowType.MoltenFury:
                    return new ArrowData()
                    {
                        ItemType = 120,
                        Offest = new(8, 0)
                    };
                case BowType.HellwingBow:
                    return new ArrowData()
                    {
                        ItemType = 3019,
                        Offest = new(9, 0)
                    };
                case BowType.RadiantSun:
                    return new ArrowData()
                    {
                        ItemType = ModContent.ItemType<RadiantSun>(),
                        Offest = new(14, 0)
                    };
                case BowType.ReversedFlash:
                    return new ArrowData()
                    {
                        ItemType = ModContent.ItemType<ReverseFlash>(),
                        Offest = new(12, 0)
                    };
                case BowType.End:
                    return new ArrowData()
                    {
                        ItemType = 39,
                    };
                default:
                    break;
            }
            return default;
        }
    }
}
