using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Solunar : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(70, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 22, 13.5f);

            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 10);

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

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<SolunarHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            for (int i = -1; i < 2; i++)
            {
                Projectile.NewProjectile(source, player.Center + dir.RotatedBy(i * MathHelper.PiOver2), velocity
                    , ProjectileType<SolunarArrow>(), damage, knockback, player.whoAmI, i + 1, dir.X, dir.Y);
            }

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
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        break;
                    }
                default:
                    return false;
            }

            int mouseDir = Main.MouseWorld.X > Player.Center.X ? 1 : -1;

            if (mouseDir > 0)
            {
                if (dashDirection > 0)
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
                else
                    newVelocity = new Vector2(-1, 0);
            }
            else
            {
                if (dashDirection > 0)
                    newVelocity = new Vector2(1, 0);
                else
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
            }

            float angle = (Main.MouseWorld - Player.Center).ToRotation();
            const float angleLimit = 0.2f;

            if ((angle > -MathHelper.PiOver2 - angleLimit && angle < -MathHelper.PiOver2 + angleLimit)
                || (angle > MathHelper.PiOver2 - angleLimit && angle < MathHelper.PiOver2 + angleLimit))
            {
                dashDirection = Math.Sign(Main.MouseWorld.X - Player.Center.X);
                newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 90;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 18;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;
            Player.AddImmuneTime(ImmunityCooldownID.General, 14);
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
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(dashDirection, 0), ProjectileType<SolunarHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 18);

                //生成光球弹幕

            }

            return true;
        }
    }

    public class SolunarHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(FullMoon);

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;
        public int SPTimer;
        private MoonDustSpawner dustSpawner;

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

        public enum DashHitType
        {
            NotHit,
            HitOther,
            HitSPProj,
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override int GetItemType()
            => ItemType<Solunar>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺

        public override void DashAttackAI()
        {
            LockOwnerItemTime();

            switch (DashState)
            {
                default:
                case 0://朝向目标方向冲刺，并检测碰撞
                    if (Timer < DashTime + 2)
                    {
                        Rotation = RecordAngle;
                        Vector2 dir = Rotation.ToRotationVector2();

                        Owner.velocity = dir * 14;
                        SpawnDashingDust();

                     DashHitType hit  = Dashing_CheckCollide();

                        if (hit==DashHitType.HitOther)
                        {
                            DashState = 1;
                            Timer = 0;
                            Owner.velocity = -dir * 9;
                            Owner.AddImmuneTime(ImmunityCooldownID.General, 26);
                            Owner.immune = true;

                            dustSpawner = new MoonDustSpawner()
                            {
                                pos = Projectile.Center,
                            };

                            return;
                        }
                        else if (hit == DashHitType.HitSPProj)
                        {
                            DashState = 3;
                            Timer = 0;
                            Owner.AddImmuneTime(ImmunityCooldownID.General, 26);
                            Owner.immune = true;
                            RecordAngle = Rotation;

                            return;
                        }
                    }
                    else
                    {
                        Projectile.Kill();
                        Owner.velocity *= 0.5f;
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
                            Projectile.NewProjectileFromThis<FullMoonStrike>(Projectile.Center, Vector2.Zero
                                , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 3f), 10, Projectile.whoAmI);
                        }
                        return;
                    }

                    break;
                case 2://射击阶段
                    if (DownLeft && SPTimer == 0)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                        }

                        Projectile.timeLeft = 30;
                    }
                    else
                    {
                        if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                        {
                            Helper.PlayPitched(SoundID.Shimmer1, Projectile.Center, pitchAdjust: -0.8f);
                            Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, Projectile.Center, pitchAdjust: 0.8f);
                            Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Projectile.Center, pitchAdjust: -0.2f);

                            Rotation = ToMouseAngle;

                            for (int i = 0; i < Main.maxProjectiles; i++)//将弹幕设置为射出状态
                            {
                                Projectile p = Main.projectile[i];

                                if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<FullMoonStrike>())
                                {
                                    (p.ModProjectile as FullMoonStrike).TurnToAttack();
                                    break;
                                }
                            }

                            Vector2 dir = Rotation.ToRotationVector2();

                            if (VisualEffectSystem.HitEffect_ScreenShaking)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));

                            handOffset = -20;
                            Owner.velocity = -Rotation.ToRotationVector2() * 6;
                            if (Owner.velocity.Y > 0 && Math.Abs(Owner.velocity.X) < 2)
                                Owner.velocity.Y *= 0.5f;
                        }

                        if (Main.myPlayer == Projectile.owner)
                        {
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        }

                        handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                        SPTimer++;

                        if (SPTimer > 22)
                            Projectile.Kill();
                    }

                    break;
                case 3://二次冲出
                    {
                        if (Timer<20)
                        {
                            Vector2 dir = RecordAngle.ToRotationVector2();
                            Rotation += MathHelper.TwoPi / 20;

                            Owner.velocity = dir * 14;
                            SpawnDashingDust();
                        }
                        else
                        {
                            DashState = 4;
                            Timer = 0;
                            SPTimer = 0;
                        }
                    }
                    break;
                case 4://特殊射出
                    {
                        if (DownLeft && SPTimer == 0)
                        {
                            if (Main.myPlayer == Projectile.owner)
                            {
                                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                            }

                            Projectile.timeLeft = 30;
                        }
                        else
                        {
                            if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                            {
                                Helper.PlayPitched(SoundID.Shimmer1, Projectile.Center, pitchAdjust: -0.8f);
                                Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, Projectile.Center, pitchAdjust: 0.8f);
                                Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Projectile.Center, pitchAdjust: -0.2f);

                                Rotation = ToMouseAngle;

                                for (int i = 0; i < Main.maxProjectiles; i++)//将弹幕设置为射出状态
                                {
                                    Projectile p = Main.projectile[i];

                                    if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<FullMoonStrike>())
                                    {
                                        (p.ModProjectile as FullMoonStrike).TurnToAttack();
                                        break;
                                    }
                                }

                                Vector2 dir = Rotation.ToRotationVector2();

                                if (VisualEffectSystem.HitEffect_ScreenShaking)
                                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));

                                handOffset = -20;
                                Owner.velocity = -Rotation.ToRotationVector2() * 6;
                                if (Owner.velocity.Y > 0 && Math.Abs(Owner.velocity.X) < 2)
                                    Owner.velocity.Y *= 0.5f;
                            }

                            if (Main.myPlayer == Projectile.owner)
                            {
                                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            }

                            handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                            SPTimer++;

                            if (SPTimer > 22)
                                Projectile.Kill();
                        }
                    }
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void SpawnDashingDust()
        {
            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 n = (Rotation + 1.57f).ToRotationVector2();

            int direction = MathF.Sign(Projectile.rotation.ToRotationVector2().X);
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir.RotatedBy(i * MathF.Sin((float)Main.timeForVisualEffects * 0.4f) * 0.5f) * Main.rand.NextFloat(2, 4);
                PRTLoader.NewParticle<SpeedLine>(Projectile.Center + i * n * 28, velocity
                    , Color.Gold, Scale: Main.rand.NextFloat(1f, 1.5f));

                Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 20, i == direction ? DustID.SolarFlare : DustID.Shadowflame, velocity
                     , Alpha: 150, Scale: Main.rand.NextFloat(1f, 2f));

                d.noGravity = true;
            }

            Dust d2 = Dust.NewDustPerfect(Projectile.Center + dir * 24, DustID.GoldCoin
                , -dir.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(1, 2)
                , Scale: Main.rand.NextFloat(1f, 1.5f));

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

        public DashHitType Dashing_CheckCollide()
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
                    if (proj.type == ProjectileType<SolunarBall>())
                        return DashHitType.HitSPProj;
                    else
                        return DashHitType.HitOther;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    JustCollideNPC(npc);
                    return DashHitType.HitOther;
                }
            }

            return DashHitType.NotHit ;
        }

        public Rectangle GetDashRect()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(48, 48));
        }

        public void JustCollideNPC(NPC target)
        {
            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, Projectile.Center, pitchAdjust: -0.3f);

            if (target != null && target.CanBeChasedBy())//踢一脚
                target.SimpleStrikeNPC(Owner.GetWeaponDamage(Owner.HeldItem), Owner.direction, knockBack: 10, damageType: DamageClass.Ranged);

            if (!VisualEffectSystem.HitEffect_Dusts)
                return;

            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Helper.NextVec2Dir(1, 2), Scale: Main.rand.NextFloat(1f, 2f));
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }

    /// <summary>
    /// 留在原地的球，玩家冲刺到球上触发将玩家投掷出去并射出2道
    /// </summary>
    public class SolunarBall : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();



            return false;
        }
    }

    public class SolunarLaser
    {

    }

    public class SolunarArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public int ArrowType => (int)Projectile.ai[0];

        public Vector2 TargetDir => new Vector2(Projectile.ai[1], Projectile.ai[2]);

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            switch (ArrowType)
            {
                default:
                case 0://日光箭
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(MathHelper.PiOver2+Timer*0.2f)*0.03f);

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[1], i / 3f);
                            Dust d = Dust.NewDustPerfect(pos, DustID.SolarFlare, Vector2.Zero, Alpha: 100, Scale: 0.9f);
                            d.noGravity = true;
                        }
                    }
                    break;
                    case 1://同辉箭
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[1], i / 3f);
                            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);

                            for (int j = -1; j < 2; j++)
                            {
                                Dust d = Dust.NewDustPerfect(pos, DustID.GoldCoin, dir.RotatedBy(j*0.4f*MathF.Sin((int)Main.timeForVisualEffects*0.05f)), Alpha: 100, Scale: 0.9f);
                                d.noGravity = true;
                            }
                        }
                    }
                    break;
                case 2://月光箭
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(-MathF.Sin(MathHelper.PiOver2 + Timer * 0.2f) * 0.03f);

                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[1], i / 3f);
                            Dust d = Dust.NewDustPerfect(pos, DustID.PurpleTorch, Vector2.Zero, Alpha: 100, Scale: 0.9f);
                            d.noGravity = true;
                        }
                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle rect = Projectile.GetTexture().Frame(3, 1, ArrowType);

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 12, 0, 12, 2, Projectile.scale, rect, 1.57f);
            Projectile.QuickDraw(rect, lightColor, 1.57f);

            return false;
        }
    }
}
