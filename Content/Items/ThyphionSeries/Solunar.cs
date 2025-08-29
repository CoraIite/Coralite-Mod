using Coralite.Content.Items.Materials;
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
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Solunar : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public bool SpecialAttack;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(70, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 21, 14f);

            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 10);

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            int sp = 0;

            if (SpecialAttack)
            {
                SpecialAttack = false;
                Helper.PlayPitched(CoraliteSoundID.TerraBlade_Item60, player.Center, pitch: 0.4f);

                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(source, player.Center + dir.RotatedBy(i * MathHelper.PiOver2), velocity
                        , ProjectileType<SolunarArrow>(), (int)(damage * 2.5f), knockback, player.whoAmI, i + 1, dir.X, dir.Y);
                }

                sp = 2;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(source, player.Center, velocity.RotateByRandom(-0.1f, 0.1f), type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<SolunarHeldProj>(), damage, knockback, player.whoAmI, rot, sp);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RadiantSun>()
                .AddIngredient<FullMoon>()
                .AddIngredient<AncientCore>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            float dashDirection2;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        dashDirection2 = 0;
                        break;
                    }
                case CoralitePlayer.DashUp:
                case CoralitePlayer.DashDown:
                    dashDirection = 0;
                    dashDirection2 = DashDir == CoralitePlayer.DashDown ? 1 : -1;
                    break;
                default:
                    return false;
            }

            if (dashDirection != 0)
            {
                newVelocity = new Vector2(dashDirection, 0);

                //float angle = (Main.MouseWorld - Player.Center).ToRotation();
                //const float angleLimit = 0.2f;

                //if ((angle > -MathHelper.PiOver2 - angleLimit && angle < -MathHelper.PiOver2 + angleLimit)
                //    || (angle > MathHelper.PiOver2 - angleLimit && angle < MathHelper.PiOver2 + angleLimit))
                //{
                //    dashDirection = Math.Sign(Main.MouseWorld.X - Player.Center.X);
                //    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
                //}
            }
            else
            {
                newVelocity = new Vector2(0, dashDirection2);
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 85;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 22;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;
            Player.AddImmuneTime(ImmunityCooldownID.General, 20);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(10, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<SolunarHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, newVelocity, ProjectileType<SolunarHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 22);
            }

            return true;
        }
    }

    [VaultLoaden(AssetDirectory.ThyphionSeriesItems)]
    public class SolunarHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(Solunar);

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;
        public int SPTimer;
        private MoonDustSpawner dustSpawner;

        [VaultLoaden("{@classPath}" + "Solunar_Glow")]
        public static ATex Glow { get; private set; }

        private struct MoonDustSpawner
        {
            public Vector2 pos;

            public void Spawn(float timer)
            {
                if (timer < 11 && timer % 2 == 0)
                {
                    float length = timer / 10 * 32;
                    Dust.NewDustPerfect(pos + Helper.NextVec2Dir() * length, DustType<FullMoonCircle>(), Vector2.Zero
                         , newColor: new Color(237, 214, 148), Scale: 1f);

                    //for (int i = -1; i < 2; i+=2)
                    //{
                    //   Dust.NewDustPerfect(pos + dir.RotatedBy(i * (0.2f + (2.5f - 0.2f) * timer / 10)) * 48, DustType<FullMoonCircle>(), Vector2.Zero
                    //        , newColor: new Color(237, 214, 148),Scale:1f);
                    //}
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override int GetItemType()
            => ItemType<Solunar>();

        public override void InitializeDashBow()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺

        public override void DashAttackAI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;

            switch (DashState)
            {
                default:
                case 0://朝向目标方向冲刺，并检测碰撞
                    if (Timer < DashTime + 2)
                    {

                        if (Projectile.velocity.X != 0)
                        {
                            Rotation = RecordAngle;
                            Vector2 dir = Rotation.ToRotationVector2();

                            Owner.velocity = dir * 14;
                            SpawnDashingDust(Rotation);

                            if (Dashing_CheckCollide())
                            {
                                DashState = 1;
                                Timer = 0;
                                Owner.velocity = -dir * 9;
                                Owner.AddImmuneTime(ImmunityCooldownID.General, 30);
                                Owner.immune = true;

                                dustSpawner = new MoonDustSpawner()
                                {
                                    pos = Projectile.Center,
                                };

                                return;
                            }
                        }
                        else
                        {
                            Rotation += MathHelper.TwoPi / DashTime;
                            Owner.velocity = Projectile.velocity * 9;
                            SpawnDashingDust(RecordAngle);
                        }
                    }
                    else
                    {
                        if (Projectile.velocity.X == 0)
                        {
                            DashState = 3;
                            Timer = 0;
                            Rotation = -1.57f;
                        }
                        else
                        {
                            SetOwnerSPAttack();
                            Projectile.Kill();
                            Owner.velocity *= 0.5f;
                        }
                    }

                    break;
                case 1://与目标产生碰撞，向后反射并旋转弓
                    const int maxTime = 20;
                    Rotation += 1f / maxTime * MathHelper.TwoPi;
                    SpawnBackingDust();

                    dustSpawner.Spawn(Timer);

                    if (Timer > maxTime)
                    {
                        DashState = 2;
                        Timer = 0;

                        if (Projectile.IsOwnedByLocalPlayer())//生成弹幕
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                int damage = i == 0 ? (int)(Owner.GetDamageWithAmmo(Item) * 9f) : Owner.GetDamageWithAmmo(Item) * 2;
                                int p = Projectile.NewProjectileFromThis<SolunarStrike>(Projectile.Center, Vector2.Zero
                                     , damage, 10, Projectile.whoAmI);

                                (Main.projectile[p].ModProjectile as SolunarStrike).EXRot = i * 0.5f;
                            }
                        }
                        return;
                    }

                    break;
                case 2://射击阶段
                    if (!DownLeft && SPTimer == 0)
                    {
                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            Rotation = Rotation.AngleLerp(ToMouseA, 0.15f);
                        }

                        Projectile.timeLeft = 30;
                    }
                    else
                    {
                        if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                        {
                            Helper.PlayPitched(SoundID.Shimmer1, Projectile.Center, pitchAdjust: -0.8f);
                            Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, Projectile.Center, pitchAdjust: 0.8f);
                            Helper.PlayPitched(CoraliteSoundID.TerraBlade_Item60, Projectile.Center, pitchAdjust: -0.2f);

                            SetOwnerSPAttack();
                            Rotation = ToMouseA;

                            for (int i = 0; i < Main.maxProjectiles; i++)//将弹幕设置为射出状态
                            {
                                Projectile p = Main.projectile[i];

                                if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<SolunarStrike>())
                                    (p.ModProjectile as SolunarStrike).TurnToAttack();
                            }

                            Vector2 dir = Rotation.ToRotationVector2();

                            if (VisualEffectSystem.HitEffect_ScreenShaking)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));

                            handOffset = -30;
                            Owner.velocity = -Rotation.ToRotationVector2() * 6;
                            if (Owner.velocity.Y > 0 && Math.Abs(Owner.velocity.X) < 2)
                                Owner.velocity.Y *= 0.5f;
                        }

                        if (Projectile.IsOwnedByLocalPlayer())
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

                        handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                        SPTimer++;

                        if (SPTimer > 22)
                            Projectile.Kill();
                    }

                    break;
                case 3://特殊射出
                    {
                        if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                        {
                            Helper.PlayPitched(CoraliteSoundID.Bow2_Item102, Projectile.Center, pitchAdjust: 0.8f);
                            Helper.PlayPitched(CoraliteSoundID.TerraBlade_Item60, Projectile.Center, pitchAdjust: -0.2f);

                            SetOwnerSPAttack();

                            Projectile.NewProjectileFromThis<SolunarBallLaser>(Projectile.Center, new Vector2(0, -16), Owner.GetDamageWithAmmo(Item) * 2, 0);
                            handOffset = -30;
                        }

                        handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                        SPTimer++;

                        if (SPTimer > 22)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void SetOwnerSPAttack()
        {
            if (Item.type == ItemType<Solunar>())
            {
                (Item.ModItem as Solunar).SpecialAttack = true;
            }
        }

        public void SpawnDashingDust(float rot)
        {
            // 生成两个方向的旋转向量，dir 为当前旋转方向，n 为垂直于 dir 的方向
            Vector2 dir = rot.ToRotationVector2();
            Vector2 n = (rot + 1.57f).ToRotationVector2();

            //int direction = MathF.Sign(Projectile.rotation.ToRotationVector2().X);
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir.RotatedBy(i * MathF.Sin(1.2f + Timer * 0.4f) * 0.3f) * Main.rand.NextFloat(2, 4);
                Color newColor = i == Owner.direction ? Color.MediumPurple : Color.Gold;
                PRTLoader.NewParticle<SpeedLine>(Owner.Center + i * n * 20, velocity
                    , newColor, Scale: Main.rand.NextFloat(0.1f, 0.2f) * (1 + Timer / DashTime * 1.5f));

                //Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 46, DustType<PixelPoint>(), velocity
                //     , newColor: newColor, Scale: Main.rand.NextFloat(1f, 2f));

                //d.noGravity = true;
            }

            Dust d2 = Dust.NewDustPerfect(Owner.Center, DustID.GoldCoin
                , -dir.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(1, 2)
                , Scale: Main.rand.NextFloat(1f, 1.5f));
            //for (int i = 0; i < 3; i++)
            //{
            //    int i2 = i;
            //    if (Timer % 2 == 0)
            //    {
            //        i2 = 3 - i;
            //    }

            //    for (int j = -1; j < 2; j += 2)
            //    {
            //        Dust d = Dust.NewDustPerfect(Projectile.Center + dir * i * 8, DustID.GoldCoin
            //            , -dir * 2 + dir.RotatedBy(j * MathHelper.PiOver2) * i2 / 3, Alpha: 100,Color.Goldenrod, Scale: 0.9f);
            //        d.noGravity = true;
            //    }
            //}

            //d2.noGravity = true;
        }

        public void SpawnBackingDust()
        {
            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 n = (Rotation + 1.57f).ToRotationVector2();

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir * Main.rand.NextFloat(0.5f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 28, DustID.GoldFlame, velocity
                    , Scale: Main.rand.NextFloat(1f, 1.5f));

                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + i * n * 20, DustID.Shadowflame, velocity
                    , Alpha: 150, Scale: Main.rand.NextFloat(1f, 2f));

                d.noGravity = true;
            }

            Dust d2 = Dust.NewDustPerfect(Projectile.Center + dir * 24, DustID.GoldCoin
                , -dir * Main.rand.NextFloat(0.5f, 1f)
                , Scale: Main.rand.NextFloat(0.75f, 1.1f));
        }

        public bool Dashing_CheckCollide()
        {
            Rectangle rect = GetDashRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    JustCollideNPC(null);
                    return true;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage || npc.immortal)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    JustCollideNPC(npc);
                    return true;
                }
            }

            return false;
        }

        public Rectangle GetDashRect()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(48, 48));
        }

        public void JustCollideNPC(NPC target)
        {
            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, Projectile.Center, pitchAdjust: -0.3f);

            if (target != null && target.CanBeChasedBy())//踢一脚
                target.SimpleStrikeNPC(Owner.GetDamageWithAmmo(Item), Owner.direction, knockBack: 10, damageType: DamageClass.Ranged);

            if (!VisualEffectSystem.HitEffect_Dusts)
                return;

            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Helper.NextVec2Dir(1, 2), Scale: Main.rand.NextFloat(1f, 2f));
        }

        #endregion

        public override void NormalShootAI()
        {
            base.NormalShootAI();
            if (Special == 2)
            {
                if (SPTimer == 0)
                {
                    SPTimer++;
                    handOffset = -30;
                }

                handOffset = Helper.Lerp(handOffset, 0, 0.1f);
            }
        }

        public override Vector2 GetOffset()
            => new(12 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(Glow.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);
            return false;
        }
    }

    /// <summary>
    /// 留在原地的球，玩家冲刺到球上触发将玩家投掷出去并射出2道
    /// </summary>
    public class SolunarBallLaser : BaseHeldProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float LaserHeight => ref Projectile.ai[1];
        public ref float ExAngle => ref Projectile.ai[2];
        public ref float Distance => ref Projectile.localAI[0];

        public Vector2 endPoint1;
        public Vector2 endPoint2;

        public const int ReadyTime = 18;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return false;

            return null;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State > 0)
            {
                float a = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint1, 40, ref a)
                    || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint2, 40, ref a);
            }

            return false;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://旋转中
                    {
                        Projectile.velocity *= 0.95f;
                        Projectile.rotation = Projectile.rotation.AngleLerp((InMousePos - Projectile.Center).ToRotation(), 0.25f);
                        Timer++;

                        float factor = Timer / 30;
                        Distance = Helper.X2Ease(factor) * 20;

                        if (Timer > 30)
                        {
                            State = 1;
                            Timer = 0;
                            ExAngle = 0.2f;
                        }
                    }
                    break;
                case 1://射激光
                    {
                        Shot();
                    }
                    break;
            }
        }

        public void Shot()
        {
            SetEndPoint(ref endPoint1, 1);
            SetEndPoint(ref endPoint2, -1);

            do
            {
                if (Timer == 0)
                {
                    Helper.PlayPitched("Misc/GoodCast", 0.3f, -0.1f, Main.player[Projectile.owner].Center);
                }

                const int height = 2;

                if (Timer < 5)
                {
                    float factor = Helper.SqrtEase(Timer / 5);
                    LaserHeight = Helper.Lerp(0, height, factor);

                    if (Timer % 4 == 0)
                    {
                        SpawnSpeedLine(endPoint1, Color.SaddleBrown, Main.rand.Next(6) * MathHelper.PiOver2 / 7);
                        SpawnSpeedLine(endPoint2, Color.MediumPurple, Main.rand.Next(6) * MathHelper.PiOver2 / 7);
                    }

                    break;
                }

                if (Timer < 18)
                {
                    float factor = Helper.SqrtEase((Timer - 5) / 18);
                    LaserHeight = Helper.Lerp(height, 1, factor);

                    ExAngle = 0.2f - 0.2f * Helper.BezierEase(factor);

                    break;
                }

                if (Timer < 18 + 8)
                {
                    float factor = Helper.SqrtEase((Timer - 18) / 8);
                    ExAngle = 0;
                    LaserHeight = Helper.Lerp(1, 0, factor);

                    SpawnLaserParticle((int)(Projectile.Center - endPoint1).Length() - 100, (Projectile.rotation + ExAngle).ToRotationVector2()
                        , Color.SaddleBrown, DustID.SolarFlare);
                    SpawnLaserParticle((int)(Projectile.Center - endPoint2).Length() - 100, (Projectile.rotation - ExAngle).ToRotationVector2()
                        , Color.MediumPurple, DustID.Shadowflame);

                    if (Timer % 4 == 0)
                    {
                        SpawnSpeedLine(endPoint1, Color.SaddleBrown, Main.rand.Next(6) * MathHelper.PiOver2 / 7);
                        SpawnSpeedLine(endPoint2, Color.MediumPurple, Main.rand.Next(6) * MathHelper.PiOver2 / 7);
                    }

                    break;
                }

                Projectile.Kill();

            } while (false);

            Timer++;

        }

        public void SetEndPoint(ref Vector2 endPoint, int exAngledir)
        {
            Vector2 dir = Vector2.UnitX.RotatedBy(Projectile.rotation + exAngledir * ExAngle);
            endPoint = Projectile.Center + dir * 120 * 16;
            //for (int k = 0; k < 70; k++)
            //{
            //    Vector2 posCheck = Projectile.Center + (dir * k * 16);

            //    if (Helper.PointInTile(posCheck) || k == 69)
            //    {
            //        endPoint = posCheck;
            //        return;
            //    }
            //}
        }

        public void SpawnLaserParticle(int width, Vector2 dir, Color color, int dustid)
        {
            float height = 64 * LaserHeight;
            float min = width / 120f;
            float max = width / 80f;

            for (int i = 0; i < width; i += 16)
            {
                Lighting.AddLight(Projectile.position + (dir * i), color.ToVector3() * height * 0.030f);
                if (Main.rand.NextBool(30))
                {
                    PRTLoader.NewParticle<SpeedLine>(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(8, 8),
                        dir * Main.rand.NextFloat(min, max), Color.SaddleBrown, 0.3f);

                    Dust d = Dust.NewDustPerfect(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(12, 12)
                        , dustid, dir * Main.rand.NextFloat(min, max), Scale: 0.9f);
                    d.noGravity = true;
                }
            }
        }

        public static void SpawnSpeedLine(Vector2 center, Color c, float baseAngle = 0)
        {
            for (int i = 0; i < 4; i++)
            {
                PRTLoader.NewParticle<SpeedLine>(center, (baseAngle + i * MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(2, 4)
                    , c, 0.35f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = new Vector2(0, mainTex.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float alpha;
            if (State == 0)
                alpha = Timer / 40f;
            else
            {
                if (Timer < 18)
                    alpha = 1;
                else
                    alpha = 1 - (Timer - 18) / 8f;
            }

            Color c = Color.SaddleBrown;
            c.A = 40;
            c *= alpha;

            Color c2 = Color.Goldenrod * alpha;

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 exDir = (Projectile.rotation + i * MathHelper.PiOver2).ToRotationVector2() * Distance;
                float rot = Projectile.rotation + i * MathHelper.PiOver2;
                Main.spriteBatch.Draw(mainTex, pos + exDir
                    , null, c2, rot, origin, Projectile.scale, 0, 0);

                for (int j = 0; j < 3; j++)
                    Main.spriteBatch.Draw(mainTex, pos + exDir + (Main.GlobalTimeWrappedHourly + j * MathHelper.TwoPi / 3).ToRotationVector2() * 3
                        , null, c, rot, origin, Projectile.scale, 0, 0);
            }

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State < 1)
                return;

            Texture2D laserTex = CoraliteAssets.Laser.VanillaCoreA.Value;
            Texture2D flowTex = CoraliteAssets.Laser.AirflowA.Value;

            Color color = Color.LightCoral;

            Effect effect = Filters.Scene["GlowingDust"].GetShader().Shader;
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uOpacity"].SetValue(0.75f);

            float height = LaserHeight * laserTex.Height / 4f;
            int width1 = (int)(Projectile.Center - endPoint1).Length();   //这个就是激光长度
            int width2 = (int)(Projectile.Center - endPoint2).Length();   //这个就是激光长度

            Vector2 startPos = Projectile.Center - Main.screenPosition;
            Vector2 endPos1 = endPoint1 - Main.screenPosition;
            Vector2 endPos2 = endPoint2 - Main.screenPosition;

            var laserTarget1 = new Rectangle((int)startPos.X, (int)startPos.Y, width1, (int)(height * 2f));
            var flowTarget1 = new Rectangle((int)startPos.X, (int)startPos.Y, width1, (int)(height * 0.9f));
            var laserTarget2 = new Rectangle((int)startPos.X, (int)startPos.Y, width2, (int)(height * 2f));
            var flowTarget2 = new Rectangle((int)startPos.X, (int)startPos.Y, width2, (int)(height * 0.9f));

            var laserSource1 = new Rectangle((int)(-Main.timeForVisualEffects + Projectile.timeLeft / 30f * laserTex.Width), 0, width1 / 10, laserTex.Height);
            var laserSource2 = new Rectangle((int)(-Main.timeForVisualEffects + Projectile.timeLeft / 30f * laserTex.Width), 0, width2 / 10, laserTex.Height);
            var flowSource = new Rectangle((int)(-2 * Main.timeForVisualEffects + Projectile.timeLeft / 35f * flowTex.Width), 0, flowTex.Width, flowTex.Height);

            var origin = new Vector2(0, laserTex.Height / 2);
            var origin2 = new Vector2(0, flowTex.Height / 2);

            float rot1 = (endPos1 - startPos).ToRotation();
            float rot2 = (endPos2 - startPos).ToRotation();// Projectile.rotation - ExAngle;

            spriteBatch.End();
            spriteBatch.Begin(default, default, SamplerState.LinearWrap, default, default, effect, Main.GameViewMatrix.TransformationMatrix);

            //绘制流动效果
            spriteBatch.Draw(laserTex, laserTarget1, laserSource1, color, rot1, origin, 0, 0);
            spriteBatch.Draw(flowTex, flowTarget1, flowSource, color * 0.5f, rot1, origin2, 0, 0);


            spriteBatch.Draw(laserTex, laserTarget2, laserSource2, color, rot2, origin, 0, 0);
            spriteBatch.Draw(flowTex, flowTarget2, flowSource, color * 0.5f, rot2, origin2, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            //绘制主体光束
            Texture2D bodyTex = CoraliteAssets.Laser.Body.Value;

            color = Color.Goldenrod;
            spriteBatch.Draw(bodyTex, laserTarget1, laserSource1, color * 0.8f, rot1, new Vector2(0, bodyTex.Height / 2), 0, 0);

            color = Color.MediumOrchid;
            spriteBatch.Draw(bodyTex, laserTarget2, laserSource2, color * 0.8f, rot2, new Vector2(0, bodyTex.Height / 2), 0, 0);

            //绘制用于遮盖首尾的亮光
            Texture2D glowTex = CoraliteAssets.LightBall.Ball.Value;
            Texture2D starTex = CoraliteAssets.Sparkle.Cross.Value;

            float f1 = 0.6f + height * 0.004f;

            DrawEndLight(spriteBatch, Color.MediumOrchid, height, endPos2, glowTex, starTex, f1);
            DrawEndLight(spriteBatch, Color.SaddleBrown, height, endPos1, glowTex, starTex, f1);

            color = Color.SaddleBrown;

            spriteBatch.Draw(glowTex, startPos, null, color * (height * 0.01f), 0, glowTex.Size() / 2, 0.5f * f1, 0, 0);
            spriteBatch.Draw(starTex, startPos, null, color * (height * 0.04f), Main.GlobalTimeWrappedHourly * 3, starTex.Size() / 2, new Vector2(0.75f, 1f) * 0.1f * f1, 0, 0);
            spriteBatch.Draw(starTex, startPos, null, color * (height * 0.04f), Projectile.rotation, starTex.Size() / 2, 0.14f * f1, 0, 0);
        }

        private void DrawEndLight(SpriteBatch spriteBatch, Color color, float height, Vector2 endPos1, Texture2D glowTex, Texture2D starTex, float f1)
        {
            spriteBatch.Draw(glowTex, endPos1, null, color * (height * 0.01f), 0, glowTex.Size() / 2, 0.5f * f1, 0, 0);
            spriteBatch.Draw(starTex, endPos1, null, color * (height * 0.06f), Main.GlobalTimeWrappedHourly * 3, starTex.Size() / 2, new Vector2(0.75f, 1f) * 0.14f * f1, 0, 0);
            spriteBatch.Draw(starTex, endPos1, null, color * (height * 0.06f), Projectile.rotation, starTex.Size() / 2, 0.16f * f1, 0, 0);
        }
    }

    /// <summary>
    /// ai0传入拥有者，ai1控制状态
    /// </summary>
    [VaultLoaden(AssetDirectory.ThyphionSeriesItems)]
    public class SolunarStrike : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public float EXRot { get; set; }
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float Alpha => ref Projectile.localAI[0];
        public Vector2 Scale
        {
            get => new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        public static ATex SolunarFlowGradient { get; private set; }

        private Vector2 oldCenter;
        private PRTGroup group;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 12000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
            => State == 1 && Timer < 10;

        public override bool ShouldUpdatePosition()
            => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center
                , Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[2] * (EXRot == 0 ? 400 : 280), 60, ref a);
        }

        public override void AI()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;

            if (!VaultUtils.isServer)
                group ??= [];

            Projectile.Center = owner.Center;
            Projectile.rotation = owner.rotation + EXRot;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3());
            switch (State)
            {
                default:
                case 0://捏在手里的阶段
                    Alpha = Helper.Lerp(Alpha, 0.2f, 0.2f);

                    float xReady = EXRot == 0 ? 0.6f : 0.3f;
                    Scale = Vector2.SmoothStep(Scale, new Vector2(xReady, 0.8f), 0.2f);

                    break;
                case 1://释放
                    if (Timer < 8)
                    {
                        Alpha = Helper.Lerp(Alpha, 1, 0.2f);

                        float y1 = 1.2f;//光束宽度
                        float y2 = 1.8f;

                        float x1 = 3.5f;//光束长度

                        if (EXRot != 0)
                        {
                            y1 = 0.5f;
                            y2 = 0.9f;

                            x1 = 1.2f;
                        }

                        if (Timer < 4)
                        {
                            Projectile.localAI[2] = Helper.Lerp(y1, y2, Timer / 4);
                        }
                        else
                            Projectile.localAI[2] = Helper.Lerp(Projectile.localAI[2], 0.9f, 0.5f);

                        Projectile.localAI[1] = Helper.Lerp(Projectile.localAI[1], x1, 0.45f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();

                        group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                            + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                            , Main.rand.NextFromList(Color.Gold, Color.SandyBrown), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    }

                    else if (Timer < 22)
                    {
                        Alpha = Helper.Lerp(Alpha, 0, 0.2f);
                        Scale = Vector2.SmoothStep(Scale, new Vector2(EXRot == 0 ? 1.5f : 0.8f, 0), 0.35f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();
                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + normal * Scale.Y * 20 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 220), DustID.GoldFlame
                                , dir * 4, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        if (Main.rand.NextBool())
                            group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                                , Main.rand.NextFromList(Color.Gold, Color.SandyBrown), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                    break;
            }

            if (!VaultUtils.isServer)
            {
                if (oldCenter != Vector2.Zero)
                {
                    foreach (var p in group)
                        p.Position += Projectile.Center - oldCenter;
                }

                group?.Update();
                oldCenter = Projectile.Center;
            }

            Timer++;
        }

        public void TurnToAttack()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;

            State = 1;
            Timer = 0;

            if (VisualEffectSystem.HitEffect_ScreenShaking)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.rotation.ToRotationVector2(), 10, 7, 5, 500));
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 35;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 dir = (Projectile.rotation + 1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2();

                for (int i = -1; i < 2; i += 2)
                {
                    Helper.SpawnDirDustJet(target.Center, () => i * dir, 1, 6, i => 1 + i * Main.rand.NextFloat(1, 3)
                        , DustID.GoldCoin, Scale: 1.35f);
                }
            }
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            group?.Draw(spriteBatch);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.LightShot.Value;
            Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 12;
            Vector2 scale = Scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(tex);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(SolunarFlowGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Vector2 origin = new Vector2(tex.Width * 7 / 8, tex.Height / 2);
            float rotation = Projectile.rotation + MathHelper.Pi;

            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(tex, pos, null
                    , Color.White, rotation, origin, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null
                , new Color(255, 255, 255, 0) * 0.65f * Alpha, rotation, origin, scale * 1.1f, 0, 0);

            return base.PreDraw(ref lightColor);
        }
    }

    [VaultLoaden(AssetDirectory.ThyphionSeriesItems)]
    public class SolunarArrow : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public int ArrowType => (int)Projectile.ai[0];

        public Vector2 TargetDir => new Vector2(Projectile.ai[1], Projectile.ai[2]);

        public static ATex SolunarFlowGradient { get; private set; }
        public static ATex SolunarFlow { get; private set; }

        public ref float Timer => ref Projectile.localAI[0];
        public Trail trail;
        private bool init = true;

        public const int trailCount = 12;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Initialize();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 dir = Projectile.rotation.ToRotationVector2();

            switch (ArrowType)
            {
                default:
                case 0://日光箭
                    {
                        float factor = MathHelper.PiOver2 + Timer * 0.2f;
                        factor = MathF.Cos(2 * factor) - MathF.Sin(factor);
                        Projectile.velocity = TargetDir.RotatedBy(factor * 0.2f) * Projectile.velocity.Length();

                        //for (int i = 0; i < 2; i++)
                        //{
                        Vector2 pos = Projectile.Center;//Vector2.Lerp(Projectile.Center, Projectile.oldPos[^3], i / 2f);
                        Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(6, 12), Alpha: 100, Color.Gold, Scale: Main.rand.NextFloat(0.8f, 1f));
                        d.noGravity = true;
                        //}

                        Projectile.UpdateOldPosCache(true);
                        Projectile.UpdateOldRotCache();
                    }
                    break;
                case 1://同辉箭
                    {
                        //for (int i = 0; i < 3; i++)
                        //{
                        //    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[1] + Projectile.Size / 2, i / 3f);
                        //    Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);

                        //    float factor = (int)Main.GlobalTimeWrappedHourly * 0.2f;
                        //    factor = MathF.Cos(2 * factor) - MathF.Sin(factor);

                        //    int i2 = i;
                        //    if (Timer%2==0)
                        //    {
                        //        i2 = 3 - i;
                        //    }

                        //    for (int j = -1; j < 2; j += 2)
                        //    {
                        //        Dust d = Dust.NewDustPerfect(pos, DustID.GoldCoin,-dir*2+ dir.RotatedBy(j * MathHelper.PiOver2) * i2/3, Alpha: 100, Scale: 0.9f);
                        //        d.noGravity = true;
                        //    }
                        //}

                        UpdateOldPos();
                    }
                    break;
                case 2://月光箭
                    {
                        float factor = MathHelper.PiOver2 + Timer * 0.2f;
                        factor = MathF.Cos(2 * factor) - MathF.Sin(factor);
                        Projectile.velocity = TargetDir.RotatedBy(-factor * 0.2f) * Projectile.velocity.Length();

                        //for (int i = 0; i < 2; i++)
                        //{
                        Vector2 pos = Projectile.Center;//Vector2.Lerp(Projectile.Center, Projectile.oldPos[^3], i / 2f);
                        Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, -dir * Main.rand.NextFloat(6, 12), Alpha: 100, Color.Purple, Scale: Main.rand.NextFloat(0.8f, 1f));
                        d.noGravity = true;
                        //}

                        Projectile.UpdateOldPosCache(true);
                        Projectile.UpdateOldRotCache();
                    }
                    break;
            }

            Timer++;
        }

        private void UpdateOldPos()
        {
            if (Timer % 2 == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Projectile.UpdateOldPosCache();

                    Vector2[] pos2 = new Vector2[trailCount + 4];

                    //延长一下拖尾数组，因为使用的贴图比较特别
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                        pos2[i] = Projectile.oldPos[i] + Projectile.velocity;

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    int exLength = 12;

                    for (int i = 1; i < 5; i++)
                        pos2[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;

                    trail.TrailPositions = pos2;
                }
            }
        }

        public void Initialize()
        {
            if (init)
            {
                init = false;

                if (!VaultUtils.isServer)
                {
                    Projectile.InitOldPosCache(trailCount);
                    Projectile.InitOldRotCache(trailCount);
                    if (ArrowType == 1)
                        trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new EmptyMeshGenerator()
                            , f => 24, f => new Color(255, 255, 255, 170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustID = ArrowType switch
            {
                0 => DustID.SolarFlare,
                1 => DustID.GoldFlame,
                _ => DustID.Shadowflame,
            };

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, dustID, Helper.NextVec2Dir(2, 7), 50, Scale: Main.rand.NextFloat(1, 2));
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            int dustID = ArrowType switch
            {
                0 => DustID.SolarFlare,
                1 => DustID.GoldCoin,
                _ => DustID.Shadowflame,
            };

            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, dustID, Projectile.velocity.SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2, 9), 50, Scale: Main.rand.NextFloat(1, 2));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle rect = Projectile.GetTexture().Frame(3, 1, ArrowType);

            if (ArrowType != 1)
            {
                Texture2D mainTex = Projectile.GetTexture();
                Vector2 origin = rect.Size() / 2;

                for (int i = 0; i < 12; i += 2)
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, rect,
                        lightColor * (i * 0.5f / 12), Projectile.oldRot[i] + 1.57f, origin, Projectile.scale, 0, 0);
                }
            }
            Projectile.QuickDraw(rect, lightColor, 1.57f);

            return false;
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(SolunarFlowGradient.Value);
            effect.Parameters["uDissolve"].SetValue(SolunarFlow.Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            trail?.DrawTrail(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (ArrowType != 1)
                return;

            Rectangle rect = Projectile.GetTexture().Frame(3, 1, ArrowType);

            Projectile.QuickDraw(rect, Color.White, 1.57f);
        }
    }
}
