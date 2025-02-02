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

            float lengthFactor = Coralite.Instance.X2Smoother.Smoother(length / 800)/2;

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
                float percent =  MathF.Abs( (MathF.Abs(angle) - MathHelper.Pi) / MathHelper.PiOver2);
                percent = Coralite.Instance.SqrtSmoother.Smoother(percent);
                if (angle>MathHelper.Pi)
                percent = Helper.Lerp(lengthFactor, percent, percent);

                angle = MathHelper.Pi.AngleLerp(3 * MathHelper.PiOver2, percent);
            }

            int x = Main.rand.Next(-2, 0);
            int k = Main.rand.Next(2, 4);

            for (int i = x; i < k; i++)
            {
                Projectile.NewProjectile(source, player.Center, (angle+i*0.05f).ToRotationVector2() * v0*Main.rand.NextFloat(0.95f,1.05f), ProjectileType<AquariusBall>()
                    , damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AquariusHeldProj>(), damage, knockback, player.whoAmI, ai2: angle);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
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
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = MousePos.X > Owner.Center.X ? 1 : -1;
                TargetRot = Projectile.ai[2] + (DirSign > 0 ? 0f : MathHelper.Pi);
            }

            Projectile.netUpdate = true;
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.5f));
        }
    }

    public class AquariusBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public const float FallSpeed = 0.35f;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                      Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.2f, 0.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 15)  //膨胀小动画
                Projectile.localAI[0]++;

            Projectile.velocity.Y += 0.45f;
            if (Projectile.velocity.Y > 18)
                Projectile.velocity.Y = 18;

            Projectile.UpdateFrameNormally(3, 4);

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(DustID.Water, Main.rand.NextFloat(0.3f, 0.5f),Scale:Main.rand.NextFloat(1,2));

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.netUpdate = true;
            Projectile.ai[0]++;

            //简易撞墙反弹
            bool top = oldVelocity.Y < 0 && Math.Sign(oldVelocity.Y + Projectile.velocity.X) < 0;
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.7f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.7f;
            if (top)
                Projectile.velocity.Y *= -1;

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            return Projectile.ai[0]>4;
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.NoUse_ToxicBubble2_Item112, Projectile.Center, volumeAdjust: -0.4f);

            for (int i = 0; i < 6; i++)
            {
               Dust d= Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Clentaminator_Cyan,
                     Helper.NextVec2Dir(1f, 2.5f), Scale: Main.rand.NextFloat(0.75f, 1f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor;
            var frameBox = mainTex.Frame(1, 5, 0, Projectile.frame);

            //绘制影子拖尾
            Projectile.DrawShadowTrails(Color.White, 0.3f, 0.3f/8, 0, 8, 1, 1f, frameBox,1.57f);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation+1.57f, frameBox.Size() / 2, 1, 0, 0);
            return false;
        }
    }
}
