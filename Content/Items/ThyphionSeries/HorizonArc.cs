using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class HorizonArc : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(55, 4f,4);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 26, 10f);

            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4);

            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
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
            bool hasBonus = player.HasBuff<HorizonArcBonus>();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<HorizonArcHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            if (!hasBonus)//非特殊攻击只射普通箭
                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FarAwaySky>()
                .AddIngredient(ItemID.SoulofSight, 15)
                .AddIngredient(ItemID.CrystalShard, 12)
                .AddTile(TileID.WorkBenches)
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
                        newVelocity.X = dashDirection * 8;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 26;
            Player.velocity = newVelocity;
            Player.AddImmuneTime(ImmunityCooldownID.General, 25);
            Player.immune = true;
            //Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(3, 12));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<HorizonArcHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<HorizonArcHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - (Main.MouseWorld.X > Player.Center.X ? 1 : -1) * 1, 1, 35);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class HorizonArcHeldProj : BaseDashBow, IDrawPrimitive, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "HorizonArc";

        public override int GetItemType() => ItemType<HorizonArc>();

        public int dashState;
        public bool dashHit;
        public bool bonus;
        public float handOffset;
        public ref float Release => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public SecondOrderDynamics_Vec2 factor;
        public SecondOrderDynamics_Vec2[] angleFactors;
        public SecondOrderDynamics_Vec2[] streamerFactors;
        public Trail streamer;
        public Vector2[] streamerPos;

        private float arrowAlpha = 0;

        public static ATex HorizonArcGradient { get; private set; }
        public static ATex HorizonArcArrow { get; private set; }

        private enum DashState
        {
            /// <summary>
            /// 冲刺中
            /// </summary>
            dashing,
            /// <summary>
            /// 冲刺动作结束，正在被玩家捏住
            /// </summary>
            holding,
            /// <summary>
            /// 在冲刺过程中与敌对目标产生了碰撞，释放后的三连箭，
            /// </summary>
            specialRelease,
        }

        public override void Initialize()
        {
            RecordAngle = Rotation;
            bonus = Owner.HasBuff<HorizonArcBonus>();

            if (Special == 0)
                return;

            RecordAngle = (Owner.direction > 0 ? 0.2f : MathHelper.Pi - 0.2f);
            Rotation = RecordAngle;
            factor = new SecondOrderDynamics_Vec2(0.8f, 0.1f, 0, RecordAngle.ToRotationVector2());

            if (!VaultUtils.isServer)
            {
                angleFactors = new SecondOrderDynamics_Vec2[20];
                for (int i = 0; i < angleFactors.Length; i++)
                    angleFactors[i] = new SecondOrderDynamics_Vec2(0.4f + i / 20f * 0.35f, 0.5f, 0, GetStreamerAngle().ToRotationVector2());

                streamerFactors = new SecondOrderDynamics_Vec2[20];
                for (int i = 0; i < streamerFactors.Length; i++)
                    streamerFactors[i] = new SecondOrderDynamics_Vec2(
                        1.25f + Coralite.Instance.X3Smoother.Smoother(i, 20) * 10f, 0.75f, 0, Projectile.Center);

                streamerPos = new Vector2[20];
                Array.Fill(streamerPos, Projectile.Center);
                streamer = new Trail(Main.instance.GraphicsDevice, 20, new NoTip(),
                    factor => (1 - MathF.Cbrt(factor)) * 35 + 2, factor => Color.White);
            }
        }

        #region 冲刺攻击部分

        public override void DashAttackAI()
        {
            Color c = Main.hslToRgb((float)(Main.GlobalTimeWrappedHourly % 1.0f), 0.7f, 0.9f);
            Lighting.AddLight(Projectile.Center, c.ToVector3());

            if (Timer < DashTime + 2)//冲刺过程中
            {
                Owner.itemTime = Owner.itemAnimation = 2;

                Dashing_Angle();//改变弓的角度

                if (!dashHit)
                    CheckCollide();

                Timer++;
            }
            else
            {
                arrowAlpha = Helper.Lerp(arrowAlpha, 1, 0.1f);
                if (DownLeft && Release == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.15f);

                        if (Main.rand.NextBool(10))
                        {
                            Vector2 dir = Rotation.ToRotationVector2();
                            Vector2 center = Projectile.Center + dir * 20;
                        }
                    }

                    Projectile.timeLeft = 2;
                    LockOwnerItemTime();
                }
                else
                {
                    Release = 1;
                    if (bonus)
                        PowerfulRainbow();
                    else
                        NotPowerfulRainbow();
                }
            }

            Projectile.rotation = Rotation;
        }

        public void Dashing_Angle()
        {
            float upTime = DashTime / 2f;

            if (Timer < (int)upTime)//前二分之一段，向上抬起弓
            {
                float targetAngle = Owner.direction > 0 ? (-MathHelper.PiOver2 - 0.2f) : (MathHelper.PiOver2 * 3 + 0.2f);

                Rotation = factor.Update(1 / 60f, targetAngle.ToRotationVector2()).ToRotation();
                return;
            }

            if (Timer == (int)upTime)//改变记录角度，之后转向向下
            {
                RecordAngle = Owner.direction > 0 ? (-MathHelper.PiOver2 - 0.2f) : (MathHelper.PiOver2 * 3 + 0.2f);
            }

            if (Timer < (int)(DashTime * 2 / 3))
            {
                float targetAngle = Owner.direction > 0 ? 0 : MathHelper.Pi;
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

                Rotation = factor.Update(1 / 60f, targetAngle.ToRotationVector2()).ToRotation();

                return;
            }

            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.2f);
        }

        public float GetStreamerAngle()
        {
            float length = Owner.velocity.Length();
            return Rotation.AngleLerp(Owner.velocity.ToRotation(), Math.Clamp(length / 8, 0, 1));
        }

        /// <summary>
        /// 在射击方向查找敌怪，如果有敌怪那么就追踪它
        /// </summary>
        /// <returns></returns>
        public int? FindEnemy()
        {
            int index = -1;
            Vector2 dir = Rotation.ToRotationVector2() * 1000;
            float a = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];

                if (!n.CanBeChasedBy())
                    continue;

                if (Collision.CanHit(Projectile, n) &&
                    Collision.CheckAABBvLineCollision(n.TopLeft, n.Size, Projectile.Center, Projectile.Center + dir, 400, ref a))
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
                return null;

            return index;
        }

        public void NotPowerfulRainbow()
        {
            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.2f);

            if (Projectile.IsOwnedByLocalPlayer())
            {
                int? targetIndex = FindEnemy();

                Vector2 dir = Rotation.ToRotationVector2();
                Vector2 velocity = dir * 12f;

                Projectile.NewProjectileFromThis<RainbowArrow>(Owner.Center, velocity
                    , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, targetIndex ?? -1);

                Helper.PlayPitched(CoraliteSoundID.Bow_Item5, Owner.Center, pitchAdjust: 0.5f);
                Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, Owner.Center, pitchAdjust: 0.2f);

                //生成粒子
                float rotation = dir.ToRotation();
                Vector2 velocity1 = -dir * 3;

                Vector2 pos = Projectile.Center + dir * 12;

                for (int i = 0; i < 6; i++)
                {
                    PRTLoader.NewParticle<SpeedLine>(pos, dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(3f, 10f)
                        , Main.hslToRgb(Main.rand.NextFloat(), 0.8f, 0.8f), Main.rand.NextFloat(0.4f, 0.5f));
                }
                for (int i = 0; i < 3; i++)
                {
                    PRTLoader.NewParticle<SpeedLine>(pos, -dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(3f, 7f)
                        , Main.hslToRgb(Main.rand.NextFloat(), 0.8f, 0.8f), Main.rand.NextFloat(0.2f, 0.4f));
                }

                WindCircle.Spawn(Projectile.Center + dir * 24, velocity1, rotation
                    , Color.White, 0.7f, 0.8f, new Vector2(1.4f, 1.4f));
                //WindCircle.Spawn(Projectile.Center + dir * 24, velocity1, rotation
                //    , Color.White,0.7f,0.8f,new Vector2(1.4f,0.8f));
            }

            Projectile.Kill();
        }

        public void PowerfulRainbow()
        {
            Projectile.timeLeft = 200;
            handOffset = Helper.Lerp(handOffset, 0, 0.15f);
            if (dashHit)
                Owner.AddBuff(BuffType<HorizonArcBonus>(), 60 * 3 + 20);

            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.1f);
            }

            float trueTime = Timer - DashTime - 2;

            int t = Owner.itemTimeMax == 0 ? 28 : Owner.itemTimeMax;

            if (trueTime != 0 && trueTime % (t / 3) == 0)
            {
                int angle = (int)(trueTime / (t / 3));

                float angle2 = angle switch
                {
                    1 => 0,
                    2 => 0.3f,
                    _ => -0.3f,
                };
                PowerfulShoot(angle2);
            }

            if (trueTime > t + 1)
                Projectile.Kill();

            Timer++;
        }

        public void PowerfulShoot(float exAngle)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                int? targetIndex = FindEnemy();

                Vector2 dir = (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One);

                if (targetIndex.HasValue)
                    dir = dir.RotatedBy(exAngle);

                Vector2 velocity = dir * 12f;

                Projectile.NewProjectileFromThis<RainbowArrow>(Owner.Center, velocity
                    , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 1.5f), Projectile.knockBack, targetIndex ?? -1);

                Helper.PlayPitched(CoraliteSoundID.Bow2_Item102, Owner.Center, pitchAdjust: 0.5f);
                Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, Owner.Center, pitchAdjust: 0.2f);

                //生成粒子
                float rotation = dir.ToRotation();
                Vector2 velocity1 = -dir * 3;

                Vector2 pos = Projectile.Center + dir * 12;

                for (int i = 0; i < 6; i++)
                    PRTLoader.NewParticle<SpeedLine>(pos, dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(3f, 10f)
                        , Main.hslToRgb(Main.rand.NextFloat(), 0.8f, 0.8f), Main.rand.NextFloat(0.4f, 0.5f));
                for (int i = 0; i < 3; i++)
                    PRTLoader.NewParticle<SpeedLine>(pos, -dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(3f, 7f)
                        , Main.hslToRgb(Main.rand.NextFloat(), 0.8f, 0.8f), Main.rand.NextFloat(0.2f, 0.4f));

                WindCircle.Spawn(Projectile.Center + dir * 32, velocity1, rotation
                    , Color.White, 0.7f, 0.8f, new Vector2(1.4f, 1.4f));

                handOffset = -10;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 10, 7, 4, 800));
                }
            }
        }

        public void CheckCollide()
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {
                    JustCollide();
                    return;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || !Projectile.localNPCImmunity.IndexInRange(i) || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    JustCollide();
                    return;
                }
            }
        }

        public void JustCollide()
        {
            dashHit = true;
            bonus = true;
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);

            if (!VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            Vector2 pos = Projectile.Center;

            Vector2 spinningpoint = new Vector2(0f, -3f - 0.7f).RotatedByRandom(MathHelper.Pi);
            float dustCount = 16f;
            Vector2 vector19 = new(1.05f, 1f);
            for (float i = 0f; i < dustCount; i += 1f)
            {
                int num23 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowTorch, 0f, 0f, 0, Color.Transparent);
                Main.dust[num23].position = Projectile.Center;
                Main.dust[num23].velocity = spinningpoint.RotatedBy((float)Math.PI * 2f * i / dustCount) * vector19 * (0.8f + MathF.Sin(MathHelper.TwoPi * 4 * ((float)i / dustCount)) * 0.3f);
                Main.dust[num23].color = Main.hslToRgb(i / dustCount, 1f, 0.5f);
                Main.dust[num23].noGravity = true;
                Main.dust[num23].scale = 1.6f;
            }

            var p = PRTLoader.NewParticle<RainbowImpactParticle>(pos, Vector2.Zero
                , Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f), 0.2f);
        }

        #endregion

        public override void NormalShootAI()
        {
            base.NormalShootAI();

            if (bonus && Timer == 0)
            {
                Timer++;
                PowerfulShoot(0);
            }

            handOffset = Helper.Lerp(handOffset, 0, 0.15f);

            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.1f);
            }
        }

        public override void AIAfter()
        {
            switch (Special)
            {
                default:
                    break;
                case 1:
                    if (!VaultUtils.isServer)
                    {
                        float rot = GetStreamerAngle();

                        Vector2 center = Projectile.Center + ((Owner.direction > 0 ? -1.57f : 1.57f) + Rotation).ToRotationVector2() * 20;
                        for (int i = 0; i < streamerPos.Length; i++)
                        {
                            Vector2 dir = -angleFactors[i].Update(1 / 60f, rot.ToRotationVector2());

                            int k = streamerPos.Length - 1 - i;
                            float y = k * MathF.Sin(k * 0.6f + (float)(Main.timeForVisualEffects) * 0.05f) / 350f;
                            Vector2 targetPos = center + dir.RotatedBy(-y) * k * 8;

                            streamerPos[i] = streamerFactors[i].Update(1 / 60f, targetPos);
                            streamerPos[i] = Vector2.Lerp(streamerPos[i], targetPos, Coralite.Instance.X3Smoother.Smoother(i, 20));
                        }

                        streamer.Positions = streamerPos;
                    }
                    break;
            }
        }

        public override void NetCodeHeldSend(BinaryWriter writer)
        {
            writer.Write(dashState);
        }

        public override void NetCodeReceiveHeld(BinaryReader reader)
        {
            dashState = reader.ReadInt32();
        }

        #region 绘制部分

        public override Vector2 GetOffset()
            => new(22 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if ((Special == 0 && !bonus) || Timer < DashTime / 2)
                return false;

            //绘制箭
            Texture2D arrowTex = HorizonArcArrow.Value;
            Vector2 dir = Rotation.ToRotationVector2();
            Main.spriteBatch.Draw(arrowTex, center, null, lightColor, Projectile.rotation + 1.57f
                , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }

        public void DrawPrimitives()
        {
            if (streamer == null)
                return;

            Effect effect = Filters.Scene["ArcRainbow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Booster.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(HorizonArcGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);

            streamer?.Render(effect);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if ((Special == 0 && !bonus) || Timer < DashTime)
                return;

            Texture2D tex = CoraliteAssets.Trail.Arrow.Value;
            Vector2 pos = Projectile.Center + Rotation.ToRotationVector2() * 12;
            Vector2 origin = tex.Size() / 2;

            Effect effect = Filters.Scene["ArcRainbow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Arrow.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(HorizonArcGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Color c = Main.DiscoColor;
            c *= arrowAlpha;
            Vector2 scale = new Vector2(0.6f, 0.75f * arrowAlpha) * 0.6f;
            spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            c = Color.White;
            c.A = (byte)(70 * arrowAlpha);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null, c, Projectile.rotation, origin, scale, 0, 0);
        }

        #endregion
    }

    /// <summary>
    /// 使用ai0传入目标NPC，如果没有目标那么就会直接正常射出
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class RainbowArrow : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "HorizonArcArrow";

        public ref float TargetIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float CanDoDamage => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public Trail trail;

        private bool init = true;
        private float TrailWidth = 0;

        public const int trailPoint = 30;

        [AutoLoadTexture(Name = "HorizonArcGradient2")]
        public static ATex Gradient2 { get; private set; }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
            => CanDoDamage == 0;

        public override void AI()
        {
            if (init)
                Initialize();

            Color c = Main.hslToRgb((float)(Main.GlobalTimeWrappedHourly % 1.0f), 0.7f, 0.9f);
            Lighting.AddLight(Projectile.Center, c.ToVector3());

            switch (State)
            {
                default:
                    break;
                case 0://刚射出，略微减速
                    JustShoot();
                    SpawnParticle();
                    break;
                case 1://加速射出
                    Normal();
                    CheckTarget();

                    Timer++;
                    SpawnParticle();
                    break;
                case 2://消失动画
                    Fade();
                    break;
            }

            SpawnDust();
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (!VaultUtils.isServer)
            {
                Projectile.UpdateOldPosCache(true);

                Vector2[] pos2 = new Vector2[trailPoint + 6];

                //延长一下拖尾数组，因为使用的贴图比较特别
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    pos2[i] = Projectile.oldPos[i];

                Vector2 dir = Projectile.rotation.ToRotationVector2();

                for (int i = 1; i < 7; i++)
                    pos2[trailPoint + i - 1] = Projectile.oldPos[^1] + dir * i * 4;

                trail.Positions = pos2;
            }
        }

        public void Initialize()
        {
            init = false;

            if (VaultUtils.isServer)
                return;

            Projectile.InitOldPosCache(trailPoint, true);
            trail = new Trail(Main.instance.GraphicsDevice, trailPoint + 6, new NoTip()
                , f => 26 * TrailWidth, factor => new Color(255,255,255,220));
        }

        public void JustShoot()
        {
            Timer++;

            Projectile.velocity *= 0.95f;

            TrailWidth = Timer / 15;

            if (Timer > 15)
            {
                TrailWidth = 1;
                Timer = 0;
                State = 1;
                Projectile.netUpdate = true;
            }
        }

        public void Normal()
        {
            if (Main.npc.IndexInRange((int)TargetIndex))
            {
                float num481 = 20f;
                Vector2 center = Projectile.Center;
                Vector2 targetCenter = Main.npc[(int)TargetIndex].Center;
                Vector2 dir = targetCenter - center;
                float length = dir.Length();
                if (length < 100f)
                    num481 = 14f;

                length = num481 / length;
                dir *= length;
                Projectile.velocity.X = ((Projectile.velocity.X * 24f) + dir.X) / 25f;
                Projectile.velocity.Y = ((Projectile.velocity.Y * 24f) + dir.Y) / 25f;

                return;
            }

            float length2 = Projectile.velocity.Length();

            if (length2 < 16)
            {
                length2 += 0.25f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * length2;
            }
        }

        public void Fade()
        {
            Timer++;

            TrailWidth = 1 - Timer / 15;

            Projectile.velocity *= 0.93f;

            if (Timer > 15)
                Projectile.Kill();
        }

        public void CheckTarget()
        {
            if (Main.npc.IndexInRange((int)TargetIndex))
            {
                NPC n = Main.npc[(int)TargetIndex];

                if (!n.CanBeChasedBy())
                    TurnToFade();
            }
        }

        public void SpawnParticle()
        {
            if (VaultUtils.isServer)
                return;

            if (Timer % 5 == 0)
            {
                var p = PRTLoader.NewParticle<HorizonArcArrowParticle>(Projectile.Center
                    , Projectile.velocity / 5, Main.hslToRgb(Timer * 0.05f, 0.8f, 0.7f, 180), 0.75f);
                p.scale = new Vector2(0.9f, 0.75f);
            }
        }

        public void SpawnDust()
        {
            if (Timer % 2 == 0 && Main.rand.NextBool())
            {
                int num23 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowTorch, 0f, 0f, 0, Color.Transparent);
                Main.dust[num23].position = Projectile.Center + Main.rand.NextVector2Circular(12, 12);
                Main.dust[num23].velocity = -Projectile.velocity / 2;
                Main.dust[num23].color = Main.hslToRgb(Timer * 0.03f, 1f, 0.8f);
                Main.dust[num23].noGravity = true;
                Main.dust[num23].scale = Main.rand.NextFloat(1f, 1.6f);
            }
        }

        public void TurnToFade()
        {
            CanDoDamage = 1;
            Projectile.tileCollide = false;

            Timer = 0;
            State = 2;

            if (VaultUtils.isServer)
                Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TurnToFade();

            if (!VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            float rot;
            Vector2 pos = Projectile.Center;

            if (hit.Crit)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    rot = Projectile.rotation + (Main.rand.NextFloat(0.4f, 0.7f) * i);

                    for (int j = 0; j < 2; j++)
                    {
                        LightShotParticle.Spawn(pos, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.6f), rot + Main.rand.NextFloat(-0.2f, 0.2f)
                            , new Vector2(Main.rand.NextFloat(0.2f, 0.4f)
                            , 0.02f));
                        LightShotParticle.Spawn(pos, Color.White, rot + Main.rand.NextFloat(-0.4f, 0.4f)
                            , new Vector2(Main.rand.NextFloat(0.1f, 0.25f)
                            , 0.01f));

                        rot += MathHelper.Pi;
                    }
                }
            }

            Vector2 spinningpoint = new Vector2(0f, -3f - 0.7f).RotatedByRandom(MathHelper.Pi);
            float dustCount = 16f;
            Vector2 vector19 = new(1.05f, 1f);
            for (float i = 0f; i < dustCount; i += 1f)
            {
                int num23 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowTorch, 0f, 0f, 0, Color.Transparent);
                Main.dust[num23].position = Projectile.Center;
                Main.dust[num23].velocity = spinningpoint.RotatedBy((float)Math.PI * 2f * i / dustCount) * vector19 * (0.8f + MathF.Sin(MathHelper.TwoPi * 4 * ((float)i / dustCount)) * 0.2f);
                Main.dust[num23].color = Main.hslToRgb(i / dustCount, 1f, 0.5f);
                Main.dust[num23].noGravity = true;
                Main.dust[num23].scale = 1.6f;
            }

            var p = PRTLoader.NewParticle<RainbowImpactParticle>(pos, Vector2.Zero
                , Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f), 0.2f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TurnToFade();
            Projectile.velocity = oldVelocity;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
            => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["ArcRainbow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(Gradient2.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Color c = Color.White;
            c.A = (byte)(255 * TrailWidth);

            Projectile.QuickDraw(c, 1.57f);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D tex = CoraliteAssets.Trail.Arrow.Value;
            Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 20;
            Vector2 origin = tex.Size() / 2;

            Effect effect = Filters.Scene["ArcRainbow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Arrow.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(HorizonArcHeldProj.HorizonArcGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Color c = Color.White*0.9f;
            Vector2 scale = new Vector2(1, 0.75f * TrailWidth) * 0.65f;
            spriteBatch.Draw(tex, pos, null, Main.DiscoColor, Projectile.rotation, origin, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            c = Color.White;
            c.A = 50;

            spriteBatch.Draw(tex, pos - Main.screenPosition, null, c, Projectile.rotation, origin, scale, 0, 0);
        }
    }

    public class HorizonArcArrowParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LightShot";

        public Vector2 scale = Vector2.One;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;

            Color.A = (byte)(Color.A * 0.75f);
            if (Color.A < 10 || Opacity > 20)
                active = false;

            if (Opacity < 3)
                scale.Y *= 1.05f;
            else
                scale.Y *= 0.93f;

            scale.X *= 1.01f;
            Scale *= 0.98f;

            Rotation = Velocity.ToRotation();

            Position += Velocity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;
            spriteBatch.Draw(tex, position, null, Color, Rotation, origin, Scale * scale, 0, 0);

            return false;
        }
    }

    public class RainbowImpactParticle : Particle
    {
        public override string Texture => AssetDirectory.Halos + "Impact";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity > 5)
                Color.A = (byte)(Color.A * 0.75f);

            if (Color.A < 10 || Opacity > 20)
                active = false;

            Scale *= 1.05f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;

            Color c = Color.White;
            c.A = (byte)(Color.A * 0.75f);

            spriteBatch.Draw(tex, position, null, Color, Rotation, origin, Scale, 0, 0);
            spriteBatch.Draw(tex, position, null, c, Rotation, origin, Scale, 0, 0);

            return false;
        }

    }

    public class HorizonArcBonus : ModBuff
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(player.Bottom + new Vector2(Main.rand.NextFloat(-16, 16), Main.rand.NextFloat(-6, 0))
                    , DustID.RainbowTorch, -player.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.1f, 0.2f),
                     50, Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1.0f, 1f, 0.8f), Scale: Main.rand.NextFloat(0.8f, 1.4f));

                d.noGravity = true;
            }
        }
    }
}