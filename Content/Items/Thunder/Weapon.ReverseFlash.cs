using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ReverseFlash : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(64, 7f, 10);
            Item.DefaultToRangedWeapon(ProjectileType<ReverseFlashProj>(), AmmoID.Arrow
                , 22, 15f, true);

            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 2, 50);

            Item.noUseGraphic = true;
            Item.useTurn = false;

            Item.UseSound = CoraliteSoundID.Bow2_Item102;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 16, ProjectileType<ReverseFlashProj>(),
                damage, knockback, player.whoAmI);

            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<ReverseFlashHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(3)
                .AddIngredient<ElectrificationWing>(2)
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
                        newVelocity.X = dashDirection * 48;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 4;
            Player.AddImmuneTime(ImmunityCooldownID.General, 5);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            if (Player.whoAmI == Main.myPlayer)
            {
                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<ReverseFlashHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, Player.Center, pitchAdjust: -0.3f);

                //生成手持弹幕
                int damage = Player.GetWeaponDamage(Player.HeldItem);
                Main.instance.CameraModifiers.Add(new MoveModifyer(3, 15));

                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                   damage / 2, Player.HeldItem.knockBack, Player.whoAmI, 5, DashDir > 0 ? 0 : 3.141f, ai2: 5);

                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ReverseFlashHeldProj>(),
                        damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 3);
            }

            return true;
        }
    }

    public class ReverseFlashHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThunderItems + "ReverseFlash";

        public ref float Timer => ref Projectile.localAI[1];

        public override int GetItemType()
            => ItemType<ReverseFlash>();

        public override Vector2 GetOffset()
            => new(12, 0);

        public override void DashAttackAI()
        {
            LockOwnerItemTime();

            if (Timer < DashTime)
                Owner.velocity.X = Math.Sign(Owner.velocity.X) * 48;
            else if (Timer == DashTime)
                Owner.velocity.X = Math.Sign(Owner.velocity.X) * 2;
            else
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.2f);// Helper.Lerp(RecordAngle, , Coralite.Instance.HeavySmootherInstance.Smoother(Timer / DashTime));
            }

            if (Timer == DashTime + 14)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Projectile.Center);
                Projectile.NewProjectileFromThis<ReverseFlashProj>(Projectile.Center, Rotation.ToRotationVector2() * 16,
                    Owner.GetWeaponDamage(Owner.HeldItem), Owner.HeldItem.knockBack, -1);
            }
            if (Timer > DashTime + 20)
                Projectile.Kill();

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1
                , DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }
    }

    public class ReverseFlashProj : ModProjectile
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderProj";

        public ref float State => ref Projectile.ai[0];
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(Projectile.ai[1], Projectile.ai[2]);
            }
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case -1:
                case 0:
                    if (Projectile.localAI[0] == 0)//直接飞速运动命中
                    {
                        Projectile.timeLeft = 100;
                        Projectile.extraUpdates = 100;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1://返回
                    {
                        if (Projectile.localAI[0] == 0)//直接飞速运动命中
                        {
                            Projectile.friendly = true;
                            TargetPos = Main.player[Projectile.owner].Center;
                            Projectile.localAI[0] = 1;
                        }

                        if (Vector2.Distance(Projectile.Center, TargetPos) < 32)
                        {
                            Projectile.Kill();
                        }

                        Projectile.localAI[1]++;
                        if (Projectile.localAI[1] > 10)
                        {
                            Projectile.velocity = (TargetPos - Projectile.Center).SafeNormalize(Vector2.Zero)
                                .RotateByRandom(-0.2f, 0.2f) * Projectile.velocity.Length();
                            Projectile.localAI[1] = 0;

                            for (int i = 0; i < 3; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Helper.NextVec2Dir(0.5f, 4),
                                     newColor: Coralite.ThunderveinYellow);
                                d.noGravity = true;
                            }
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (State == -1)
            {
                Projectile.NewProjectileFromThis<ReverseFlashThunder>(Projectile.Center, (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16,
                    Projectile.damage, Projectile.knockBack);

                return;
            }

            if (Projectile.owner == Main.myPlayer && State == 0)
            {
                Projectile.NewProjectileFromThis<ReverseFlashProj>(Projectile.Center,
                    (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16, Projectile.damage, Projectile.knockBack, 1);
            }

            if (State > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.oldPos[i] + (new Vector2(Projectile.width, Projectile.height) / 2), DustID.PortalBoltTrail, Projectile.velocity.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(0.2f, 0.6f)
                        , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State < 1)
                return false;

            Projectile.DrawShadowTrails(Color.White, 0.8f, 0.8f / 10, 0, 10, 1);
            Projectile.QuickDraw(Color.White, 0);

            return false;
        }
    }

    public class ReverseFlashThunder : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Halos + "Circle";

        public ref float State => ref Projectile.ai[0];
        public ref float Hited => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];
        public Vector2 TargetPos;

        public float Alpha;
        public float fade = 0;

        private Vector2 TargetCenter;
        private bool init = true;
        public ThunderTrail trail;

        LinkedList<Vector2> trailList;
        public static Asset<Texture2D> HorizontalStar;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            HorizontalStar = Request<Texture2D>(AssetDirectory.Particles + "HorizontalStar");
        }

        public override void Unload()
        {
            HorizontalStar = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.extraUpdates = 6;
            Projectile.timeLeft = 10 * 100;
        }

        public override bool? CanDamage()
        {
            return State == 0 && Hited == 0;
        }

        public override float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return ThunderAlpha * (factor - fade) / (1 - fade);
        }

        public virtual Color ThunderColorFunc(float factor)
            => Coralite.ThunderveinYellow;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Coralite.ThunderveinYellow.ToVector3() / 2);
            //生成后以极快的速度前进

            Init();
            switch (State)
            {
                default:
                case 0://找到敌人，以极快的速度追踪
                    Chase();

                    if (Main.rand.NextBool())
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Projectile.velocity.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.2f, 0.6f)
                            , newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    break;
                case 1://后摇，闪电逐渐消失
                    {
                        Timer++;
                        fade = Coralite.Instance.X2Smoother.Smoother((int)Timer, 30);
                        ThunderWidth = Coralite.Instance.X2Smoother.Smoother(60 - (int)Timer, 60) * 20;

                        float factor = Timer / 30;
                        float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                        if (Timer > 30)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public void Init()
        {
            if (!init)
                return;

            init = false;
            ThunderAlpha = 1;
            ThunderWidth = 20;
            trailList = new LinkedList<Vector2>();

            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;

            Asset<Texture2D> trailTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrail2");

            trail = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc, GetAlpha)
            {
                CanDraw = true,
                UseNonOrAdd = true,
                PartitionPointCount = 2,
                BasePositions =
                [
                    Projectile.Center,Projectile.Center
                ]
            };
            trail.SetRange((0, 7));
            trail.SetExpandWidth(7);

            TargetPos = Main.player[Projectile.owner].Center;

            TargetCenter = Projectile.Center +
                ((TargetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 125);
        }

        public void Chase()
        {
            Timer++;

            Vector2 targetCenter = TargetCenter;

            float speed = Projectile.velocity.Length();

            if (Projectile.Center.Distance(targetCenter) < speed * 4)//距离目标点近了就换一个
            {
                if (Projectile.Center.Distance(TargetPos) < speed * 10)
                {
                    targetCenter = TargetPos;
                    TargetCenter = TargetPos;
                }
                else
                {
                    Vector2 dir2 = TargetPos - Projectile.Center;
                    float length2 = dir2.Length();
                    if (length2 > 150)
                        length2 = 150;
                    dir2 = dir2.SafeNormalize(Vector2.Zero);
                    Vector2 center2 = Projectile.Center + (dir2 * length2);
                    Vector2 pos = center2 + (dir2.RotatedBy(Main.rand.NextFromList(1.57f, -1.57f)) * length2);// Main.rand.NextVector2Circular(length2,length2);

                    targetCenter = pos;
                    TargetCenter = pos;
                    Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                }
            }

            float selfAngle = Projectile.velocity.ToRotation();
            float targetAngle = (targetCenter - Projectile.Center).ToRotation();

            float factor = 1 - Math.Clamp(Vector2.Distance(targetCenter, Projectile.Center) / 500, 0, 1);

            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.5f + (0.5f * factor)).ToRotationVector2() * 24f;

            if (Main.rand.NextBool(8))
                Projectile.SpawnTrailDust(DustID.RainbowMk2, Main.rand.NextFloat(0.1f, 0.3f)
                    , Scale: Main.rand.NextFloat(0.4f, 0.8f), newColor: Coralite.ThunderveinYellow);

            trailList.AddLast(Projectile.Center);

            if (Timer % Projectile.MaxUpdates == 0)
            {
                trail.BasePositions = [.. trailList];//消失的时候不随机闪电
                trail.RandomThunder();
            }

            if (Vector2.Distance(Projectile.Center, TargetPos) < speed * 2)
                Fade();
        }

        public void Fade()
        {
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Hited = 1;
            Timer = 0;
            State = 1;

            if (trail != null)
            {
                trail.BasePositions = [.. trailList];
                if (trail.BasePositions.Length > 3)
                    trail.RandomThunder();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Helper.NextVec2Dir(2f, 5f), newColor: Coralite.ThunderveinYellow);
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 0 && Timer < 5)
                return false;

            trail?.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }
}
