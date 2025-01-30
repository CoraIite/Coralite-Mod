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
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class FullMoon : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(44, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 21, 5.3f);

            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 3);

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

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<FullMoonHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.DemonBow)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.TendonBow)
                .AddTile(TileID.Anvils)
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
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<FullMoonHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(dashDirection, 0), ProjectileType<FullMoonHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 18);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FullMoonHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(FullMoon);

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;
        public int SPTimer;
        private MoonDustSpawner dustSpawner;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override int GetItemType()
            => ItemType<FullMoon>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

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

                        if (Dashing_CheckCollide())
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
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void SpawnDashingDust()
        {
            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 n = (Rotation + 1.57f).ToRotationVector2();

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir.RotatedBy(i * MathF.Sin((float)Main.timeForVisualEffects * 0.4f) * 0.5f) * Main.rand.NextFloat(2, 4);
                Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 28, DustID.GoldFlame, velocity
                    , Scale: Main.rand.NextFloat(1f, 1.5f));

                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + i * n * 20, DustID.Shadowflame, velocity
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

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
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
                target.SimpleStrikeNPC(Owner.GetWeaponDamage(Owner.HeldItem), Owner.direction, knockBack: 10, damageType: DamageClass.Ranged);

            if (!VisualEffectSystem.HitEffect_Dusts)
                return;

            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Helper.NextVec2Dir(1, 2), Scale: Main.rand.NextFloat(1f, 2f));
        }

        #endregion

        public override void OnKill(int timeLeft)
        {
            if (Special == 1 && DashState > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)//将弹幕设置为射出状态
                {
                    Projectile p = Main.projectile[i];

                    if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<FullMoonStrike>())
                    {
                        p.Kill();
                        return;
                    }
                }
            }
        }

        public override Vector2 GetOffset()
            => new Vector2(20 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteBatch spriteBatch = Main.spriteBatch;

            bool spDraw = Special == 1 && Timer < DashTime + 3 && DashState < 2;
            if (spDraw)
            {
                Projectile.DrawShadowTrails(Color.White, 0.75f, 1 / 8f, 0, 8, 2, 1 / 8f, 0, 1.2f);
            }

            Vector2 origin = mainTex.Size() / 2;
            spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (spDraw)
            {
                spriteBatch.Draw(mainTex, center, null, new Color(30, 50, 50, 0), Projectile.rotation
                    , origin, 1.1f, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            return false;
        }
    }

    /// <summary>
    /// ai0传入拥有者，ai1控制状态
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FullMoonStrike : BaseHeldProj, IDrawNonPremultiplied, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

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

        public static ATex FullMoonGradient { get; private set; }

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
                , Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[2] * 320, 60, ref a);
        }

        public override void AI()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;

            if (!VaultUtils.isServer)
                group ??= [];

            Projectile.Center = owner.Center;
            Projectile.rotation = owner.rotation;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3());
            switch (State)
            {
                default:
                case 0://捏在手里的阶段

                    Alpha = Helper.Lerp(Alpha, 0.2f, 0.2f);
                    Scale = Vector2.SmoothStep(Scale, new Vector2(0.3f, 0.8f), 0.2f);

                    break;
                case 1://释放
                    if (Timer < 8)
                    {
                        Alpha = Helper.Lerp(Alpha, 1, 0.2f);

                        if (Timer < 4)
                        {
                            Projectile.localAI[2] = Helper.Lerp(1f, 1.4f, Timer / 4);
                        }
                        else
                            Projectile.localAI[2] = Helper.Lerp(Projectile.localAI[2], 0.9f, 0.5f);

                        Projectile.localAI[1] = Helper.Lerp(Projectile.localAI[1], 2.75f, 0.45f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();

                        group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                            + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                            , Main.rand.NextFromList(new Color(245, 156, 255), new Color(212, 69, 255)), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    }

                    else if (Timer < 22)
                    {
                        Alpha = Helper.Lerp(Alpha, 0, 0.2f);
                        Scale = Vector2.SmoothStep(Scale, new Vector2(1.75f, 0), 0.35f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();
                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + normal * Scale.Y * 20 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 220), DustID.VilePowder
                                , dir * 4, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        if (Main.rand.NextBool())
                            group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                                , Main.rand.NextFromList(new Color(245, 156, 255), new Color(212, 69, 255)), Scale: Main.rand.NextFloat(0.2f, 0.35f));
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
                    Helper.SpawnDirDustJet(target.Center, () => i * dir, 1, 5, i => 1 + i * Main.rand.NextFloat(2, 4)
                        , DustID.ShadowbeamStaff, Scale: 1.35f);
                }
            }
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
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

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(tex);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(FullMoonGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Vector2 origin = new Vector2(tex.Width * 5 / 6, tex.Height / 2);
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

    public class FullMoonCircle : ModDust
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(4, 1);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;

            if (dust.fadeIn % 2 == 0 && dust.frame.X < Texture2D.Width() * 3 / 4)
            {
                dust.frame.X += Texture2D.Width() / 4;
            }

            if (dust.fadeIn > 4)
            {
                dust.color *= 0.7f;
            }

            if (dust.fadeIn > 12 || dust.color.A < 10)
            {
                dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame
                , dust.color, 0, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
