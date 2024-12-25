using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class HorizonArc : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public int SpecialAttack;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(55, 4f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 27, 7f);

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
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<HorizonArcHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            if (SpecialAttack == 0)//非特殊攻击只射普通箭
                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FarAwaySky>()
                .AddIngredient(ItemID.SoulofSight, 7)
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

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 30;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(3, 12));

            if (Player.whoAmI == Main.myPlayer)
            {
                Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);

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
    public class HorizonArcHeldProj : BaseDashBow, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "HorizonArc";

        public override int GetItemType() => ItemType<HorizonArc>();

        public int dashState;
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public SecondOrderDynamics_Vec2 factor;
        public SecondOrderDynamics_Vec2[] angleFactors;
        public SecondOrderDynamics_Vec2[] streamerFactors;
        public Trail streamer;
        public Vector2[] streamerPos;

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
                    factor => (1 - MathF.Cbrt(factor)) * 45 + 2, factor => Color.White);
            }
        }

        #region 冲刺攻击部分

        public override void DashAttackAI()
        {
            if (Timer < DashTime + 2)//冲刺过程中
            {
                Owner.itemTime = Owner.itemAnimation = 2;

                Dashing_Angle();//改变弓的角度
            }
            else
            {
                if (DownLeft)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.1f);

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
                    Projectile.NewProjectileFromThis<RainbowArrow>(Owner.Center, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One) * 12f
                        , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                    Helper.PlayPitched("Misc/HallowShoot",0.8f,0f, Owner.Center);
                    Projectile.Kill();
                }
            }

            Projectile.rotation = Rotation;
            Timer++;
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

            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
        }

        public float GetStreamerAngle()
        {
            float length = Owner.velocity.Length();
            return Rotation.AngleLerp(Owner.velocity.ToRotation(), Math.Clamp(length / 8, 0, 1));
        }

        #endregion

        public override void NormalShootAI()
        {
            base.NormalShootAI();
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
            => new(22, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0 || Timer < DashTime / 2)
                return false;

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

        #endregion
    }

    /// <summary>
    /// 使用ai0传入目标NPC，如果没有目标那么就会直接正常射出
    /// </summary>
    public class RainbowArrow : ModProjectile, IDrawPrimitive,IDrawNonPremultiplied,IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "HorizonArcArrow";

        public ref float TargetIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float CanDoDamage => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public Trail trail;

        private bool init = true;

        public const int trailPoint = 30;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 35;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
        }

        public override bool? CanDamage()
            => CanDoDamage == 0;

        public override void AI()
        {
            if (init)
            Initialize();

            switch (State)
            {
                default:
                    break;
                case 0://刚射出，略微减速
                    JustShoot();
                    break;
                case 1://加速射出
                    break;
                case 2://消失动画
                    break;
            }

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
                    pos2[trailPoint + i-1] = Projectile.oldPos[^1] + dir * i * 4;

                trail.Positions = pos2;
            }
        }

        public void Initialize()
        {
            init = false;

            if (VaultUtils.isServer)
                return;

            Projectile.InitOldPosCache(trailPoint,true);
            trail = new Trail(Main.instance.GraphicsDevice, trailPoint+6, new NoTip()
                , f => 28, factor => Color.White);
        }

        public void JustShoot()
        {
            Timer++;

            Projectile.velocity *= 0.99f;

            if (Timer > 20)
            {
                Timer = 0;
                State = 1;
                Projectile.netUpdate = true;
            }
        }

        public void TurnToFade()
        {
            CanDoDamage = 1;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TurnToFade();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TurnToFade();
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
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(HorizonArcHeldProj.HorizonArcGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Projectile.QuickDraw(Color.White, 1.57f);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D tex = CoraliteAssets.Trail.Arrow.Value;
            Vector2 pos = Projectile.Center -Projectile.rotation.ToRotationVector2()*20;
            Vector2 origin = tex.Size() / 2;

            Effect effect = Filters.Scene["ArcRainbow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Arrow.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(HorizonArcHeldProj.HorizonArcGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Color c = Color.White;
            Vector2 scale = new Vector2(1, 0.8f) * 0.7f;
            spriteBatch.Draw(tex, pos, null, Main.DiscoColor, Projectile.rotation, origin, scale, 0, 0);
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            c = Color.White;
            c.A = 70;

            spriteBatch.Draw(tex, pos-Main.screenPosition, null, c, Projectile.rotation, origin, scale, 0, 0);
        }
    }

    public class HorizonArcBonus : ModBuff
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;
    }
}