using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
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
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class RadiantSun : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(44, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 24, 10f);

            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 3);

            Item.noUseGraphic = true;
            Item.autoReuse = true;

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
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<RadiantSunHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            if (player.ownedProjectileCounts[ProjectileType<RadiantSunHeldProj>()] == 0)
                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.HellwingBow)
                .AddIngredient(ItemID.MoltenFury)
                .AddIngredient<Afterglow>()
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
                        newVelocity.X = dashDirection * 11;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 85;
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
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<RadiantSunHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<RadiantSunHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 20);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class RadiantSunHeldProj : BaseDashBow, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "RadiantSun";

        private Vector2 arrowPos;

        [AutoLoadTexture(Name = "RadiantSun_Glow")]
        public static ATex GlowTex { get;private set; }
        [AutoLoadTexture(Name = "RadiantSunArrow")]
        public static ATex ArrowTex { get; private set; }
        [AutoLoadTexture(Name = "RadiantSunLight")]
        public static ATex LightTex { get; private set; }

        public ref float ArrowLength => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float ExAlpha;

        public float handOffset = 14;
        public int State;

        public override int GetItemType()
            => ItemType<RadiantSun>();

        public override Vector2 GetOffset()
            => new(handOffset, 0);

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() / 2);
        }

        public override void DashAttackAI()
        {
            switch (State)
            {
                default:
                case 0:
                    DashState();
                    break;
                case 1:
                    ShootState();
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void DashState()
        {
            if (Timer < DashTime + 2)
            {
                Owner.itemTime = Owner.itemAnimation = 2;

                Rotation = Helper.Lerp(RecordAngle, DirSign > 0 ? -1.3f : (3.141f + 1.3f), Coralite.Instance.HeavySmootherInstance.Smoother(Timer / DashTime));
                ExAlpha = Helper.Lerp(0, 1, Math.Clamp(Timer / DashTime, 0, 1));
                handOffset = Helper.Lerp(4, 28, Math.Clamp(Timer / DashTime, 0, 1));

                return;
            }

            if (Owner.controlUseItem && Timer < DashTime + 180)
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    if (Main.rand.NextBool(10))
                    {
                        Vector2 dir = Rotation.ToRotationVector2();
                        Vector2 center = Projectile.Center + dir * 20;
                    }
                }

                Projectile.timeLeft = 2;
                Owner.itemTime = Owner.itemAnimation = 2;
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);

                if (Projectile.IsOwnedByLocalPlayer())
                {
                    State = 1;
                    Timer = 0;
                    Projectile.timeLeft = 10;
                    Vector2 dir = Rotation.ToRotationVector2();
                    WindCircle.Spawn(Projectile.Center + (dir * 20), -dir * 2, Rotation, Color.SaddleBrown, 0.75f, 1.3f, new Vector2(1.2f, 1f));
                    PRTLoader.NewParticle<RadiantSunFlow>(Projectile.Center, dir * 8, Color.White, 0.9f);

                    Projectile.NewProjectileFromThis<RadiantSunLaser>(Projectile.Center, Projectile.rotation.ToRotationVector2() * 10
                        , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * (Main.dayTime ? 1.5f : 1.3f)), Projectile.knockBack);
                }
            }
        }

        public void ShootState()
        {
            if (Timer > 10)
                Projectile.Kill();

            ExAlpha = Helper.Lerp(1, 0, Math.Clamp(Timer / 10, 0, 1));
            if (Timer < 3)
                handOffset -= 8;
            else
                handOffset += 3;
        }

        public override void SetCenter()
        {
            base.SetCenter();
            if (Special == 1)
            {
                if (State == 0)
                    arrowPos = Projectile.Center;
                else
                {
                    Vector2 dir = Rotation.ToRotationVector2();
                    arrowPos = Projectile.Center + dir * ArrowLength;
                }
            }
        }

        public override void InitializeDashBow()
        {
            RecordAngle = Rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Special == 1)
            {
                Texture2D mainTex2 = LightTex.Value;
                var origin2 = new Vector2(mainTex2.Width / 4, mainTex2.Height / 2);
                var pos = Projectile.Center - Main.screenPosition;
                Color c = new(255, 205, 62);
                c.A = 0;
                c *= 0.4f * ExAlpha;

                for (int i = 0; i < 3; i++)
                    Main.spriteBatch.Draw(mainTex2, pos + (Main.GlobalTimeWrappedHourly * 3 + i * MathHelper.TwoPi / 3).ToRotationVector2() * 2
                        , null, c, Projectile.rotation
                        , origin2, 1, 0, 0f);
            }

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (Special == 0 || State == 1)
                return;

            Texture2D mainTex = ArrowTex.Value;
            Vector2 dir = Rotation.ToRotationVector2();
            var origin = new Vector2(mainTex.Width / 2, mainTex.Height * 2 / 3);
            var pos = Projectile.Center - Main.screenPosition;
            Color c = Color.White;
            c.A = (byte)(255 * ExAlpha);

            mainTex = ArrowTex.Value;
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation + 1.57f
                , origin, 1, 0, 0f);
        }
    }

    public class RadiantSunLaser : BaseHeldProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "RadiantSunArrow";

        private Vector2 recordPos;
        public Vector2 endPoint;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float LaserHeight => ref Projectile.localAI[0];

        public ref float LaserRotation => ref Projectile.rotation;

        public const int ReadyTime = 18;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 6;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State > 0 && Timer > ReadyTime)
            {
                float a = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 40, ref a);
            }

            return false;
        }

        public override bool ShouldUpdatePosition()
            => State == 0;

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://箭刚射出向上飞
                    Flying();
                    break;
                case 1://光束射下
                    Shot();
                    break;
            }
        }

        public void Flying()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Main.rand.NextBool(5))
                PRTLoader.NewParticle<SpeedLine>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4), Color.SaddleBrown, 0.45f);
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(DustID.SolarFlare, Main.rand.NextFloat(0.2f, 0.4f), Scale: 0.9f);

            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Vector2 pos = Main.MouseWorld;
                Main.player[Projectile.owner].LimitPointToPlayerReachableArea(ref pos);
                recordPos = pos;
            }

            if ((Projectile.Center.Y - recordPos.Y) < -800)
            {
                State = 1;
                Projectile.velocity *= 0;
                LaserRotation = (recordPos - Projectile.Center).ToRotation();

                SpawnSpeedLine(Projectile.Center);
                Projectile.extraUpdates = 0;
            }
        }

        public void Shot()
        {
            for (int k = 0; k < 60; k++)
            {
                Vector2 posCheck = recordPos + (Vector2.UnitX.RotatedBy(LaserRotation) * k * 16);

                if (Helper.PointInTile(posCheck) || k == 59)
                {
                    endPoint = posCheck;
                    break;
                }
            }

            int width = (int)(Projectile.Center - endPoint).Length() - 100;
            Vector2 dir = Vector2.UnitX.RotatedBy(LaserRotation);
            Color color = Color.Gold;
            Projectile.velocity = LaserRotation.ToRotationVector2();

            do
            {
                if (Timer < ReadyTime)
                {
                    LaserHeight = Helper.Lerp(0, 0.5f, Timer / ReadyTime);
                    Vector2 pos = InMousePos;
                    Main.player[Projectile.owner].LimitPointToPlayerReachableArea(ref pos);
                    recordPos = recordPos.MoveTowards(pos, 8);
                    LaserRotation = (recordPos - Projectile.Center).ToRotation();
                    break;
                }

                if (Timer == ReadyTime)
                {
                    Helper.PlayPitched("Misc/GoodCast", 0.3f, -0.1f, Main.player[Projectile.owner].Center);
                }

                if (Timer % 5 == 0)
                    SpawnSpeedLine(endPoint, Main.rand.Next(6) * MathHelper.PiOver2 / 7);

                const int height = 3;

                if (Timer < ReadyTime + 5)
                {
                    float factor = Coralite.Instance.SqrtSmoother.Smoother((Timer - ReadyTime) / 5);
                    LaserHeight = Helper.Lerp(0.5f, height, factor);
                    break;
                }

                if (Timer < ReadyTime + 16)
                {
                    float factor = Coralite.Instance.SqrtSmoother.Smoother((Timer - ReadyTime - 5) / 11);
                    LaserHeight = Helper.Lerp(height, 0, factor);
                    break;
                }

                Projectile.Kill();

            } while (false);

            Timer++;

            if (Timer > ReadyTime)
            {
                float height = 64 * LaserHeight;
                float min = width / 120f;
                float max = width / 80f;

                for (int i = 0; i < width; i += 16)
                {
                    Lighting.AddLight(Projectile.position + (dir * i), color.ToVector3() * height * 0.030f);
                    if (Main.rand.NextBool(15))
                    {
                        PRTLoader.NewParticle<SpeedLine>(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(8, 8),
                            dir * Main.rand.NextFloat(min, max), Color.SaddleBrown, 0.45f);

                        Dust d = Dust.NewDustPerfect(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(12, 12)
                            , DustID.SolarFlare, dir * Main.rand.NextFloat(min, max), Scale: 0.9f);
                        d.noGravity = true;
                    }
                }
            }
        }

        public static void SpawnSpeedLine(Vector2 center, float baseAngle = 0)
        {
            for (int i = 0; i < 4; i++)
            {
                PRTLoader.NewParticle<SpeedLine>(center, (baseAngle + i * MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(2, 4)
                    , Color.SaddleBrown, 0.55f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                default:
                case 0:
                    Projectile.QuickDraw(Color.White, 0);
                    break;
                case 1:
                    Texture2D laserTex = CoraliteAssets.Laser.VanillaCoreA.Value;
                    Texture2D flowTex = CoraliteAssets.Laser.VanillaFlowA.Value;

                    Color color = Color.Coral;

                    Effect effect = Filters.Scene["GlowingDust"].GetShader().Shader;
                    effect.Parameters["uColor"].SetValue(color.ToVector3());
                    effect.Parameters["uOpacity"].SetValue(1);

                    float height = LaserHeight * laserTex.Height / 4f;
                    int width = (int)(Projectile.Center - endPoint).Length();   //这个就是激光长度

                    Vector2 startPos = Projectile.Center - Main.screenPosition;
                    Vector2 endPos = endPoint - Main.screenPosition;

                    var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 2f));
                    var flowTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.9f));

                    var laserSource = new Rectangle((int)(-Main.timeForVisualEffects + Projectile.timeLeft / 30f * laserTex.Width), 0, laserTex.Width, laserTex.Height);
                    var flowSource = new Rectangle((int)(-2 * Main.timeForVisualEffects + Projectile.timeLeft / 35f * flowTex.Width), 0, flowTex.Width, flowTex.Height);

                    var origin = new Vector2(0, laserTex.Height / 2);
                    var origin2 = new Vector2(0, flowTex.Height / 2);

                    spriteBatch.End();
                    spriteBatch.Begin(default, default, SamplerState.LinearWrap, default, default, effect, Main.GameViewMatrix.TransformationMatrix);

                    //绘制流动效果
                    spriteBatch.Draw(laserTex, laserTarget, laserSource, color, LaserRotation, origin, 0, 0);
                    spriteBatch.Draw(flowTex, flowTarget, flowSource, color * 0.5f, LaserRotation, origin2, 0, 0);

                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                    //绘制主体光束
                    Texture2D bodyTex = CoraliteAssets.Laser.Body.Value;

                    color = Color.Goldenrod;

                    spriteBatch.Draw(bodyTex, laserTarget, laserSource, color * 0.8f, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);

                    color = Color.SaddleBrown;

                    //绘制用于遮盖首尾的亮光
                    Texture2D glowTex = CoraliteAssets.LightBall.Ball.Value;
                    Texture2D starTex = CoraliteAssets.Sparkle.Cross.Value;

                    float f1 = 0.6f + height * 0.004f;

                    spriteBatch.Draw(glowTex, endPos, null, color * (height * 0.012f), 0, glowTex.Size() / 2, 0.5f * f1, 0, 0);
                    spriteBatch.Draw(starTex, endPos, null, color * (height * 0.07f), Main.GlobalTimeWrappedHourly * 3, starTex.Size() / 2, new Vector2(0.75f, 1f) * 0.14f * f1, 0, 0);
                    spriteBatch.Draw(starTex, endPos, null, color * (height * 0.07f), Projectile.rotation, starTex.Size() / 2, 0.16f * f1, 0, 0);

                    spriteBatch.Draw(glowTex, startPos, null, color * (height * 0.02f), 0, glowTex.Size() / 2, 0.5f * f1, 0, 0);
                    spriteBatch.Draw(starTex, startPos, null, color * (height * 0.05f), Main.GlobalTimeWrappedHourly * 3, starTex.Size() / 2, new Vector2(0.75f, 1f) * 0.1f * f1, 0, 0);
                    spriteBatch.Draw(starTex, startPos, null, color * (height * 0.05f), Projectile.rotation, starTex.Size() / 2, 0.14f * f1, 0, 0);

                    break;
            }
        }
    }

    public class RadiantSunFlow : Particle
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "RadiantSunFlow";

        private Vector2 scale;

        public override void SetProperty()
        {
            scale = new Vector2(0.6f, 0.2f);
            Rotation = Velocity.ToRotation();
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;

            scale = Vector2.Lerp(scale, new Vector2(2.2f, 0.7f), 0.2f);

            if (Opacity > 5)
            {
                Color *= 0.74f;
                if (Color.A < 10)
                    active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 origin = TexValue.Size() / 2;

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, Rotation, origin, scale * Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
