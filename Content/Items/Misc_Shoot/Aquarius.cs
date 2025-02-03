using Coralite.Content.Dusts;
using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Aquarius : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 6f);
            Item.DefaultToRangedWeapon(ProjectileType<VirgoHeldProj>(), AmmoID.Arrow, 29, 14f);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.BubbleGun_Item85;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 确定的初速度
            float v0 = velocity.Length(); // 使用给定的初始速度

            // 目标点
            Vector2 target = Main.MouseWorld - position;
            float length = target.Length();
            if (length > 800)
                length = 800;

            float lengthFactor = Coralite.Instance.X2Smoother.Smoother(length / 800) / 2;

            float angle = target.ToRotation();

            if (angle < -MathHelper.PiOver2)
                angle += MathHelper.TwoPi;

            if (angle < MathHelper.PiOver2)//右边的情况
            {
                //与0的距离
                float percent = MathF.Abs(angle) / MathHelper.PiOver2;
                percent = Coralite.Instance.SqrtSmoother.Smoother(percent);
                if (angle < 0)
                    percent = Helper.Lerp(lengthFactor, percent, percent);

                angle = 0f.AngleLerp(-MathHelper.PiOver2, percent);
            }
            else//左边
            {
                float percent = MathF.Abs((MathF.Abs(angle) - MathHelper.Pi) / MathHelper.PiOver2);
                percent = Coralite.Instance.SqrtSmoother.Smoother(percent);
                if (angle > MathHelper.Pi)
                    percent = Helper.Lerp(lengthFactor, percent, percent);

                angle = MathHelper.Pi.AngleLerp(3 * MathHelper.PiOver2, percent);
            }

            int x =  Main.rand.Next(-2, 0);
            int k = Main.rand.Next(2, 4);

            int damage2 = (int)(damage * 0.9f);

            for (int i = x; i < k; i++)
            {
                int type2 = type;
                if (type == ProjectileID.WoodenArrowFriendly||!(i == -1 || i == 1))
                    type2 = 0;

                Projectile.NewProjectile(source, player.Center, (angle + i * 0.05f).ToRotationVector2() * v0 * Main.rand.NextFloat(0.95f, 1.05f), ProjectileType<AquariusBall>()
                    , damage2, knockback, player.whoAmI, type2);
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AquariusHeldProj>(), damage, knockback, player.whoAmI, ai2: angle);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiberBow>()
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GelFiberBow>()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 使用ai2传入角度
    /// </summary>
    public class AquariusHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public AquariusHeldProj() : base(0.25f, 16, -8, AssetDirectory.Misc_Shoot) { }

        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemTimeMax;
            MaxTime = Owner.itemTimeMax;
            if (Projectile.IsOwnedByLocalPlayer())
            {
                Owner.direction = MousePos.X > Owner.Center.X ? 1 : -1;
                TargetRot = Projectile.ai[2] + (DirSign > 0 ? 0f : MathHelper.Pi);
                Projectile.netUpdate = true;
            }

            Projectile.netUpdate = true;

            Vector2 dir = Projectile.ai[2].ToRotationVector2();
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir2 = dir.RotateByRandom(-0.75f, 0.75f);
                Color c = Main.rand.NextFromList(Color.CornflowerBlue, Color.DeepSkyBlue, Color.LightSkyBlue) * 0.5f;
                c.A = 10;
                Dust d = Dust.NewDustPerfect(Projectile.Center + dir * 60, DustType<WaterFlower>(), dir2 * (1 + i*0.75f)
                      , newColor: c, Scale: Main.rand.NextFloat(0.8f, 1.3f));
                d.rotation = dir2.ToRotation() +  MathHelper.PiOver2;
            }
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.5f));
        }
    }

    public class AquariusBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public const float FallSpeed = 0.45f;

        protected Vector2 Scale
        {
            get => new(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        protected ref float ScaleState => ref Projectile.ai[1];
        public int timer;

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.scale = Main.rand.NextFloat(1, 1.35f);
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 1)  //膨胀小动画
            {
                Projectile.localAI[0] += 0.1f;
                Projectile.localAI[1] += 0.1f;
                Projectile.localAI[2] += 0.1f;
                if (Projectile.localAI[0] > 1)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.localAI[1] = 1f;
                    Projectile.localAI[2] = 1f;
                }
            }

            switch ((int)ScaleState)
            {
                default:
                case 0:
                    break;
                case 1:
                    Scale = Vector2.Lerp(Scale, new Vector2(1.6f, 0.65f), 0.3f);
                    if (Scale.X > 1.55f)
                        ScaleState = 2;
                    break;
                case 2:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.75f, 1.3f), 0.3f);
                    if (Scale.Y > 1.25f)
                        ScaleState = 3;
                    break;
                case 3:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.2f);
                    if (Math.Abs(Scale.X - 1) < 0.05f)
                    {
                        Scale = Vector2.One;
                        ScaleState = 0;
                    }
                    break;
            }

            Projectile.velocity.Y += FallSpeed;
            if (Projectile.ai[0] != 0)
            {
                timer++;
                if (timer > 30 && Projectile.velocity.Y > 7)
                {
                    Projectile.Kill();
                    float speed = Projectile.velocity.Length();
                    if (speed < 8)
                        speed = 8;
                    Projectile.NewProjectileFromThis(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * speed, (int)Projectile.ai[0]
                        , Projectile.damage, Projectile.knockBack);
                }
            }
            if (Projectile.velocity.Y > 18)
                Projectile.velocity.Y = 18;

            Projectile.UpdateFrameNormally(3, 4);

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(DustID.Water, Main.rand.NextFloat(0.3f, 0.5f), Scale: Main.rand.NextFloat(1, 2));

            if (Main.rand.NextBool(3))
            {
                Color c = Main.rand.NextFromList(Color.CornflowerBlue, Color.DeepSkyBlue, Color.LightSkyBlue) * 0.1f;
                c.A = (byte)Main.rand.NextFromList(0, c.A);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustType<WaterFlower>(), -Projectile.velocity.RotateByRandom(-0.75f, 0.75f) * Main.rand.NextFloat(-0.2f, 0.3f)
                      , newColor: c, Scale: Main.rand.NextFloat(0.4f, 1f));
                d.rotation = (-Projectile.velocity).ToRotation() + MathHelper.PiOver2;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.netUpdate = true;
            Projectile.ai[2]++;
            ScaleState = 1;

            //简易撞墙反弹
            bool top = oldVelocity.Y < 0 && Math.Sign(oldVelocity.Y + Projectile.velocity.X) < 0;
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.8f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.8f;
            if (top)
                Projectile.velocity.Y *= -1;

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Clentaminator_Cyan,
                      Helper.NextVec2Dir(1f, 2.5f), Scale: Main.rand.NextFloat(0.75f, 1f));
                d.noGravity = true;
            }

            return Projectile.ai[2] > 4;
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.NoUse_ToxicBubble2_Item112, Projectile.Center, volumeAdjust: -0.4f);
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + (j * 0.8f)), Scale: 1.3f - (j * 0.12f));
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor * Projectile.localAI[0];
            var frameBox = mainTex.Frame(2, 5, 0, Projectile.frame);

            Vector2 scale = Projectile.scale * Scale;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(Color.White, 0.2f, 0.2f / 8, 0, 8, 1, scale, frameBox, 1.57f);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation + 1.57f, frameBox.Size() / 2, scale, 0, 0);
            frameBox = mainTex.Frame(2, 5, 1, Projectile.frame);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.CornflowerBlue * Projectile.localAI[0], Projectile.rotation + 1.57f, frameBox.Size() / 2, scale, 0, 0);
            return false;
        }
    }
}
