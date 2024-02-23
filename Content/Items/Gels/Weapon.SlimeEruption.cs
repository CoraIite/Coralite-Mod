using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    public class SlimeEruption : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        private int shootCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 30;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.knockBack = 0.5f;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<SlimeEruptionProj>();
            Item.shootSpeed = 18;

            Item.useAmmo = AmmoID.Gel;

            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                shootCount++;
                if (shootCount > 3)
                {
                    Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 1.25f, ProjectileType<SlimeEruptionBall>(), damage, knockback, player.whoAmI);
                    shootCount = 0;
                }

                Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)), type, damage, knockback, player.whoAmI);

                return false;
            }

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14, 0);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.NextBool(2, 3))
            {
                return false;
            }
            return true;
        }
    }

    public class SlimeEruptionProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 60;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;

            AIType = ProjectileID.SlimeGun;
        }
    }

    public class SlimeEruptionBall : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "GelBall";

        protected Vector2 Scale
        {
            get => new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        protected ref float ScaleState => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
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
                    Projectile.rotation += 0.1f;

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

            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 12)
                Projectile.velocity.Y = 12;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 1;
            Projectile.ai[0]++;
            Projectile.netUpdate = true;

            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.8f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.8f;

            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            return Projectile.ai[0] > 3;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor * Projectile.localAI[0];
            var frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 scale = Scale;

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(194, 75 + (int)(60 * factor), 234);
            color *= Projectile.localAI[0] * 0.75f;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.5f, 0.062f, 1, 8, 1, scale, frameBox);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);

            return false;
        }

    }
}
