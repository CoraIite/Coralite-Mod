using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelFiberBow : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GelFiberBall>();
            Item.shootSpeed = 6.6f;

            Item.useAmmo = AmmoID.Arrow;

            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GelFiberBall>(), damage, knockback, player.whoAmI, type);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GelFiber>(30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class GelFiberBall : ModProjectile
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public ref float ArrowType => ref Projectile.ai[0];

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
            for (int i = 0; i < 8; i++)
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
            else
            {
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 12)
                    Projectile.velocity.Y = 12;
            }

            Projectile.rotation += 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.netUpdate = true;

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
            {
                Projectile.velocity.Y *= -1;
            }

            Projectile.NewProjectileFromThis(Projectile.Center + (top ? new Vector2(0, Projectile.height / 2) : Vector2.Zero)
                , Projectile.velocity, (int)ArrowType, Projectile.damage, Projectile.knockBack);

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.NoUse_ToxicBubble2_Item112, Projectile.Center, volumeAdjust: -0.4f);

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.5f, 0.062f, 1, 8, 1, 0.1f, frameBox);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, 1, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(194, 75 + (int)(60 * factor), 234);
            //color *= Projectile.localAI[0] * 0.75f;

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, 1, 0, 0);

            return false;
        }

    }
}
