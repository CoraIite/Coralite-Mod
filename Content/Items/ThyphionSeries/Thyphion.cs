using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
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
using static Terraria.ModLoader.ModContent;
#pragma warning disable CS8524 // switch 表达式不会处理其输入类型的某些值(它不是穷举)，这包括未命名的枚举值。

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Thyphion : ModItem, IDashable
    {
        public static Color ThyphionColor1 = new Color(66, 152, 201);
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
                        Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, player.Center, volume: 0.4f);
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<ThyphionArrow>(), damage, knockback, player.whoAmI, ai0: -1, ai2: 1);
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

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 23;
            Player.immune = true;
            Player.AddImmuneTime(ImmunityCooldownID.General, 23);

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
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, newVelocity, ProjectileType<ThyphionHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 23);
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Solunar>()
                .AddIngredient<HorizonArc>()
                .AddIngredient<SeismicWave>()
                .AddIngredient<PlasmaBow>()
                .AddIngredient<ReverseFlash>()
                //.AddIngredient<>()//海市蜃楼
                .AddIngredient<Glaciate>()
                .AddIngredient(ItemID.Tsunami)
                .AddIngredient(ItemID.FairyQueenRangedItem)
                .AddIngredient<Aurora>()
                .AddIngredient<QueensWreath>()
                .AddTile<AncientFurnaceTile>()
                .Register();
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
            if (Item.type != ItemType<Thyphion>())
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

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class ThyphionHeldProj : BaseDashBow/*, IDrawPrimitive*/
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

        public override void InitializeDashBow()
        {
            RecordAngle = Rotation;
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
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                    if (Timer < 10)
                        Rotation -= Owner.direction * 0.02f;
                    else
                        Rotation = Rotation.AngleLerp(ToMouseA, 0.15f);

                    handOffset = MathHelper.Lerp(handOffset, 0, 0.13f);
                }

            }
            Timer++;
            base.NormalShootAI();
        }

        //冲刺
        public override void DashAttackAI()
        {
            do
            {
                if (Timer < DashTime + 2)
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Vector2 dir = (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One);
                        Owner.itemTime = Owner.itemAnimation = 2;
                        Rotation = Rotation.AngleLerp(dir.ToRotation(), 0.15f);
                        Owner.direction = Math.Sign(dir.X);
                    }
                    if (Timer % 4 == 0)
                    {
                        int bowIndex = Main.rand.Next(1, (int)ThyphionShadowBow.BowType.End);

                        int dir = ((int)Timer / 4) % 4;
                        dir = dir switch
                        {
                            0 => -1,
                            2 => 1,
                            _ => 0
                        };

                        float velicityDir = Projectile.velocity.ToRotation() + MathHelper.Pi + dir * MathF.Sign(Projectile.velocity.X) * 0.3f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, velicityDir.ToRotationVector2() * 10, ProjectileType<ThyphionShadowBow>()
                            , Owner.GetDamageWithAmmo(Item), Projectile.knockBack, Projectile.owner, bowIndex, ToMouseA, MathF.Sign(ToMouse.X));
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
                    projectile = Projectile.NewProjectileFromThis<ThyphionArrow>(Owner.Center, dir * 18
                        , 1000, Projectile.knockBack, ai0: -1, ai2: 2);
                }

                if (DownLeft && ReleaseTimer == 0)
                {
                    if (Projectile.IsOwnedByLocalPlayer())
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

                    Owner.itemTime = Owner.itemAnimation = 2;
                }
                else
                {
                    if (ReleaseTimer == 0)
                    {
                        Vector2 dir = Rotation.ToRotationVector2();

                        SpwanShootParticle(dir);
                        ShowArrow = false;

                        handOffset = -30;

                        if (TryGetArrowProjectile(projectile, out var thyphionArrow))
                            thyphionArrow.canDamage = true;

                        Helper.PlayPitched("Misc/Do", 0.7f, 1f, Owner.Center);
                        Helper.PlayPitched("Misc/Arrow", 0.4f, -0.2f, Owner.Center);
                        Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, Owner.Center);

                        if (VisualEffectSystem.HitEffect_ScreenShaking)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));
                    }
                    if (ReleaseTimer >= 20)
                    {
                        Projectile.Kill();
                    }

                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        if (ReleaseTimer < 10)
                            Rotation -= Owner.direction * 0.05f;
                        else
                            Rotation = Rotation.AngleLerp(ToMouseA, 0.15f);
                    }

                    handOffset = MathHelper.Lerp(handOffset, 0, 0.07f);
                    ReleaseTimer++;

                    Owner.itemTime = Owner.itemAnimation = 2;
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

            Tornado.Spawn(Projectile.Center + (ShootDir * 20), ShootDir * -3 + Owner.velocity, lightCyen, 15, ShootDir.ToRotation(), Main.rand.NextFloat(0.7f, 0.5f));
            Tornado.Spawn(Projectile.Center + (ShootDir * 20), ShootDir * -3 + Owner.velocity, lightCyen, 15, ShootDir.ToRotation(), Main.rand.NextFloat(0.7f, 0.5f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            if (ShowArrow)
            {
                float value = Math.Clamp(Timer / DashTime, 0, 1);
                Vector2 dir2 = (Rotation).ToRotationVector2();
                ThyphionArrow.ArrowSelfDraw(Owner.MountedCenter + dir2 * 40, 0.8f * value, Projectile.rotation.ToRotationVector2(), value, Thyphion.ThyphionColor3, 1);
            }
            return false;
        }

        //public void DrawPrimitives()
        //{

        //}

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

    public class ThyphionArrow : ModProjectile/*, IDrawPrimitive*/
    {
        public int SurrroundTrailCount = 20;
        public override string Texture => AssetDirectory.Blank;

        private bool hasInit = false;

        private ref float ArrowItemId => ref Projectile.ai[0];

        private ref float Timer => ref Projectile.localAI[0];
        private ref float StartSpeed => ref Projectile.ai[1];

        private ArrowType arrowType => (ArrowType)Projectile.ai[2];

        public Color ArrowColor;

        public bool canDamage = true;

        public override bool? CanDamage()
        {
            if (canDamage)
                return null;
            return false;
        }

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
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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

            if (ArrowItemId != -1)
            {
                var data = ThyphionShadowBow.GetArrowData((ThyphionShadowBow.BowType)ArrowItemId);
                ArrowColor = data.ArrowColor;
            }
            else
                ArrowColor = Thyphion.ThyphionColor1;

            Projectile.InitOldPosCache(12);
            Projectile.InitOldRotCache(12);
        }

        public override void AI()
        {
            if (!hasInit)
                Init();

            if (!IsHoldBy())
            {
                ArrowSpeedUpdate();
                ArrowLight();
                ArrowDust();

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();

            Timer++;
        }

        private void ArrowSpeedUpdate()
        {
            if (Timer < 20 && !IsHoldBy())
                Projectile.velocity *= 1.05f;
        }

        private void ArrowLight()
        {
            Lighting.AddLight(Projectile.Center, Thyphion.ThyphionColor3.ToVector3());
        }

        #region 弓箭粒子

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

        #endregion

        //粒子加顶点的设计？
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Rectangle npcHitBox = target.Hitbox;
            Vector2 DustSpwanPos = Vector2.Clamp(Projectile.Center, npcHitBox.TopLeft(), npcHitBox.BottomRight());

            ArrowHitDust(DustSpwanPos);
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
                return new Color(Thyphion.ThyphionColor3.R, Thyphion.ThyphionColor3.G, Thyphion.ThyphionColor3.B, alpha);
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
            Projectile.timeLeft = 200;
            canDamage = false;
        }

        //public void DrawPrimitives()
        //{

        //}

        public override bool PreDraw(ref Color lightColor)
        {
            if (IsHoldBy())
                return false;

            float colorFactor;

            if (Timer < 15)
                colorFactor = 0;
            else
                colorFactor = Timer > 45 ? 1 : ((Timer - 15) / 45);

            Texture2D flowTex = TextureAssets.Extra[98].Value;
            Vector2 ArrowSize = new Vector2(1, 7) * 0.3f * Projectile.scale;

            Vector2 origin = flowTex.Size() / 2f;
            Color SkyBlue = Color.Lerp(ArrowColor, Thyphion.ThyphionColor3, colorFactor);

            int count = Projectile.oldPos.Length;
            for (int i = 0; i < count; i++)
            {
                Main.spriteBatch.Draw(flowTex, Projectile.oldPos[i] - Main.screenPosition, null, SkyBlue * ((float)i / count)
                    , Projectile.oldRot[i] + MathHelper.PiOver2, origin, ArrowSize, 0, 0);
            }

            ArrowSelfDraw(Projectile.Center, Projectile.scale, Projectile.velocity.SafeNormalize(Vector2.Zero), 1
                , ArrowColor, colorFactor);

            return false;
        }

        public static void ArrowSelfDraw(Vector2 center, float scale, Vector2 arrowDir, float alpha, Color startColor, float colorFactor)
        {
            Texture2D arrowTex = CoraliteAssets.Trail.ArrowSPA.Value;
            Vector2 Center = center - Main.screenPosition;
            Vector2 origin = arrowTex.Size() / 2;
            float rotation = arrowDir.ToRotation();

            Color SkyBlue = Color.Lerp(startColor, Thyphion.ThyphionColor1, colorFactor) * 0.5f * alpha;
            Color lightCyen = Color.Lerp(startColor, Thyphion.ThyphionColor3, colorFactor) * alpha;
            lightCyen.A = 70;

            Main.spriteBatch.Draw(arrowTex, Center - arrowDir * 40 * scale, null, SkyBlue, rotation, origin, scale * new Vector2(0.9f, 0.75f), 0, 0);
            Main.spriteBatch.Draw(arrowTex, Center - arrowDir * 40 * scale, null, lightCyen, rotation, origin, scale * new Vector2(0.6f, 0.5f), 0, 0);

            rotation += MathHelper.PiOver2;

            Texture2D flowTex = TextureAssets.Extra[98].Value;
            Vector2 ArrowSize = new Vector2(1, 7) * 0.3f * scale;
            Vector2 InnerSize = ArrowSize;
            Vector2 SecondLayerSize = ArrowSize * 1.5f;
            Vector2 OuterSize = ArrowSize * 3 * new Vector2(0.7f, 1);

            origin = flowTex.Size() / 2f;

            Main.spriteBatch.Draw(flowTex, Center - arrowDir * 40 * scale, null, SkyBlue, rotation, origin, OuterSize, 0, 0);

            Main.spriteBatch.Draw(flowTex, Center - arrowDir * 10 * scale, null, SkyBlue, rotation, origin, SecondLayerSize, 0, 0);

            Main.spriteBatch.Draw(flowTex, Center + arrowDir * 20 * scale, null, lightCyen, rotation, origin, InnerSize * 0.5f, 0, 0);
            Main.spriteBatch.Draw(flowTex, Center, null, lightCyen, rotation, origin, InnerSize, 0, 0);
            Main.spriteBatch.Draw(flowTex, Center - arrowDir * 5 * scale, null, lightCyen, rotation, origin, InnerSize, 0, 0);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }

    public class ThyphionShadowBow : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        private Player PhantomPlayer;

        private int BowItemType => (int)Projectile.ai[0];
        private int StartAngle => (int)Projectile.ai[1];
        private int Direction => (int)Projectile.ai[2];

        private ArrowData Data => GetArrowData((BowType)BowItemType);

        private bool hasInit = false;

        //记录玩家信息
        private int PlayerWingFrame;
        private float Rotation { get; set; }

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
                Init();

            Projectile.velocity *= 0.95f;
            int time = 40 - Projectile.timeLeft;

            Vector2 MouseDir = (InMousePos - Projectile.Center).SafeNormalize(Vector2.One);
            if (Projectile.timeLeft == 24)
            {
                Projectile.NewProjectileFromThis<ThyphionArrow>(Projectile.Center, MouseDir.SafeNormalize(Vector2.One) * 18
                    , 1000, Projectile.knockBack, BowItemType);

                Helper.PlayPitched(CoraliteSoundID.Bow_Item5, Projectile.Center);
            }

            if (time > 16)
            {
                if (time > 30)
                {
                    Projectile.Opacity = 1 - (time - 30f) / 10f;
                }
            }
            else
            {
                Projectile.Opacity = time / 16f;
                Rotation += Direction * MathHelper.TwoPi / 16;
                if (time == 16)
                {
                    Rotation = MouseDir.ToRotation();
                }
            }
        }

        public void Init()
        {
            hasInit = true;
            Main.instance.LoadItem(Data.ItemType);
            Rotation = StartAngle;
            InitPhantomPlayer();
        }

        private void InitPhantomPlayer()
        {
            Player Owner = Main.player[Projectile.owner];
            PhantomPlayer = new Player();
            PhantomPlayer.CopyVisuals(Owner);
            PlayerWingFrame = Owner.wingFrame;
            Rotation = (InMousePos - Projectile.Center).ToRotation();
            Projectile.netUpdate = true;
        }

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

            /// <summary> 海啸 </summary>
            Tsunami,

            /// <summary> 日暮 </summary>
            Eventide,

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
                        ItemType = ItemID.WoodenBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.Brown
                    };
                case BowType.AshWoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.AshWoodBow,
                        Offest = new(8, 0),
                        ArrowColor = new Color(167, 137, 141)
                    };
                case BowType.BorealWoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.BorealWoodBow,
                        Offest = new(8, 0),
                        ArrowColor = new Color(107, 86, 71)
                    };
                case BowType.PalmWoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.PalmWoodBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.Brown
                    };
                case BowType.RichMahoganyBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.RichMahoganyBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.DarkRed
                    };
                case BowType.EbonwoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.EbonwoodBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.DarkMagenta
                    };
                case BowType.ShadewoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.ShadewoodBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.DarkMagenta
                    };
                case BowType.PearlwoodBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.PearlwoodBow,
                        Offest = new(8, 0),
                        ArrowColor = Color.SaddleBrown
                    };
                case BowType.AfterGlow:
                    return new ArrowData()
                    {
                        ItemType = ItemType<Afterglow>(),
                        Offest = new(8, 0),
                        ArrowColor = Color.Red
                    };
                case BowType.FarAwaySky:
                    return new ArrowData()
                    {
                        ItemType = ItemType<FarAwaySky>(),
                        Offest = new(8, 0),
                        ArrowColor = Color.SkyBlue
                    };
                case BowType.Hail:
                    return new ArrowData()
                    {
                        ItemType = ItemType<IcicleBow>(),
                        Offest = new(8, 0),
                        ArrowColor = Coralite.IcicleCyan
                    };
                case BowType.MoltenFury:
                    return new ArrowData()
                    {
                        ItemType = ItemID.MoltenFury,
                        Offest = new(8, 0),
                        ArrowColor = Color.OrangeRed
                    };
                case BowType.HellwingBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.HellwingBow,
                        Offest = new(9, 0),
                        ArrowColor = Color.OrangeRed
                    };
                case BowType.RadiantSun:
                    return new ArrowData()
                    {
                        ItemType = ItemType<RadiantSun>(),
                        Offest = new(14, 0),
                        ArrowColor = Color.Goldenrod
                    };
                case BowType.ReversedFlash:
                    return new ArrowData()
                    {
                        ItemType = ItemType<ReverseFlash>(),
                        Offest = new(12, 0),
                        ArrowColor = Coralite.ThunderveinYellow
                    };
                case BowType.Turbulence:
                    return new ArrowData()
                    {
                        ItemType = ItemType<Turbulence>(),
                        Offest = new(12, 0),
                        ArrowColor = Color.DeepSkyBlue
                    };
                case BowType.SeismicWave:
                    return new ArrowData()
                    {
                        ItemType = ItemType<SeismicWave>(),
                        Offest = new(12, 0),
                        ArrowColor = Color.Gray
                    };
                case BowType.IceBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.IceBow,
                        Offest = new(12, 0),
                        ArrowColor = new Color(97, 200, 225)
                    };
                case BowType.Glaciate:
                    return new ArrowData()
                    {
                        ItemType = ItemType<Glaciate>(),
                        Offest = new(12, 0),
                        ArrowColor = Coralite.IcicleCyan
                    };
                case BowType.ShadowFlameBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.ShadowFlameBow,
                        Offest = new(12, 0),
                        ArrowColor = Color.Gray
                    };
                case BowType.TendonBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.TendonBow,
                        Offest = new(12, 0),
                        ArrowColor = Color.Crimson
                    };
                case BowType.DemonBow:
                    return new ArrowData()
                    {
                        ItemType = ItemID.DemonBow,
                        Offest = new(12, 0),
                        ArrowColor = new Color(87, 74, 166)
                    };
                case BowType.FullMoon:
                    return new ArrowData()
                    {
                        ItemType = ItemType<FullMoon>(),
                        Offest = new(12, 0),
                        ArrowColor = Color.MediumPurple
                    };
                case BowType.Solunar:
                    return new ArrowData()
                    {
                        ItemType = ItemType<Solunar>(),
                        Offest = new(12, 0),
                        ArrowColor = new Color(244, 225, 174)
                    };
                case BowType.HorizonArc:
                    return new ArrowData()
                    {
                        ItemType = ItemType<HorizonArc>(),
                        Offest = new(12, 0),
                        ArrowColor = Color.White
                    };
                case BowType.TremblingBow:
                    return new ArrowData()
                    {
                        ItemType = ItemType<TremblingBow>(),
                        Offest = new(12, 0),
                        ArrowColor = new Color(56, 235, 252)
                    };
                case BowType.PlasmaBow:
                    return new ArrowData()
                    {
                        ItemType = ItemType<PlasmaBow>(),
                        Offest = new(12, 0),
                        ArrowColor = new Color(56, 235, 252)
                    };
                case BowType.Aurora:
                    return new ArrowData()
                    {
                        ItemType = ItemType<Aurora>(),
                        Offest = new(12, 0),
                        ArrowColor = new Color(181, 255, 249)
                    };
                case BowType.Tsunami:
                    return new ArrowData()
                    {
                        ItemType = ItemID.Tsunami,
                        Offest = new(12, 0),
                        ArrowColor = Color.LightCyan
                    };
                case BowType.Eventide:
                    return new ArrowData()
                    {
                        ItemType = ItemID.FairyQueenRangedItem,
                        Offest = new(12, 0),
                        ArrowColor = Main.DiscoColor
                    };
                default:
                    break;
            }

            return default;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (PhantomPlayer is null)
                return false;

            DrawPhantomPlayer();

            Vector2 ArmPosition = PhantomPlayer.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Rotation - MathHelper.PiOver2);
            ArmPosition.Y += PhantomPlayer.gfxOffY;

            Texture2D texture = TextureAssets.Item[Data.ItemType].Value;
            Vector2 offest = Data.Offest.RotatedBy(Rotation);
            Vector2 center = Projectile.Center - Main.screenPosition + offest;

            Vector2 origin = texture.Size() / 2f;
            SpriteEffects effects = PhantomPlayer.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color color = Data.ArrowColor * 0.3f * Coralite.Instance.SqrtSmoother.Smoother(Projectile.Opacity);
            float length = (2 + (1 - Projectile.Opacity) * 36);
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, center + (Projectile.Opacity * 1f + i * MathHelper.TwoPi / 4).ToRotationVector2() * length, null, color,
                    Rotation, origin, 1.1f, effects, 0);
            }

            Main.spriteBatch.Draw(texture, center, null, lightColor * Projectile.Opacity, Rotation, origin, 1, effects, 0);
            return false;
        }

        private void DrawPhantomPlayer()
        {
            PhantomPlayer.ResetEffects();
            PhantomPlayer.ResetVisibleAccessories();
            PhantomPlayer.UpdateDyes();
            PhantomPlayer.DisplayDollUpdate();
            PhantomPlayer.UpdateSocialShadow();
            PhantomPlayer.itemAnimationMax = 60;
            PhantomPlayer.itemAnimation = 1;
            PhantomPlayer.direction = MathF.Sign(InMousePos.X - Projectile.Center.X);

            PhantomPlayer.wingFrame = PlayerWingFrame;

            PhantomPlayer.itemRotation = Rotation;

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

            PhantomPlayer.Center = Projectile.Center;
            Main.PlayerRenderer.DrawPlayer(Main.Camera, PhantomPlayer, PhantomPlayer.position, 0f, PhantomPlayer.fullRotationOrigin, 1 - Projectile.Opacity);
        }
    }
}
