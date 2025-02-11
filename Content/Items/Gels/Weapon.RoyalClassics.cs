using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    public class RoyalClassics : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public int shootCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 31;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.mana = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 4;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<SpikeGelBall>();
            Item.shootSpeed = 18;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = false;

            Item.UseSound = CoraliteSoundID.BubbleGun_Item85;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                switch (shootCount)
                {
                    default:
                    case 0://射出1发弹弹球
                        Projectile proj = Projectile.NewProjectileDirect(source, player.Center, velocity, ProjectileType<SlimeEruptionBall>(), damage, knockback, player.whoAmI);
                        proj.DamageType = DamageClass.Magic;
                        break;
                    case 1://射出环绕的凝胶球
                        for (int i = -1; i < 2; i += 2)
                            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<SmallGelBall_Friendly>(), (int)(damage * 0.65f), knockback, player.whoAmI, ai1: i);
                        break;
                    case 2://射出3发小弹弹球
                        for (int i = -1; i < 2; i++)
                            Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(i * 0.2f), ProjectileType<SmallGelBall_Friendly>(), (int)(damage * 0.7f), knockback, player.whoAmI, 1);
                        break;
                    case 3://射出刺球
                        Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<SpikeGelBall_Friendly>(), damage, knockback, player.whoAmI);
                        break;
                }

                shootCount++;
                if (shootCount > 3)
                    shootCount = Main.rand.Next(4);

                return false;
            }

            return false;
        }
    }

    public class SpikeGelBall_Friendly : BaseHeldProj
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SpikeGelBall";

        protected Vector2 Scale
        {
            get => new(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 70;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void Initialize()
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

            if (Projectile.timeLeft < 50)
            {
                Projectile.velocity *= 0.97f;
            }

            Projectile.rotation += Projectile.velocity.Length() / 80;
        }

        public override void OnKill(int timeLeft)
        {
            //生成粒子
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }

            if (Projectile.IsOwnedByLocalPlayer())
            {
                //生成一些尖刺弹幕
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height),
                        -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(10, 14), ProjectileType<GelSpike_Friendly>(), (int)(Projectile.damage * 0.8f), 0, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;

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

    public class GelSpike_Friendly : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "GelSpike";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 14)
                Projectile.velocity.Y = 14;

            //粒子
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, 0.8f, 0, 0);
            return false;
        }

    }

    /// <summary>
    /// 使用ai0传入状态，0：在自身环绕 1：普通射出并弹弹
    /// 使用ai1传入自身环绕时的方向
    /// </summary>
    public class SmallGelBall_Friendly : BaseHeldProj
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SmallGelBall";

        protected Vector2 Scale
        {
            get => new(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        public ref float State => ref Projectile.ai[0];
        protected ref float ScaleState => ref Projectile.ai[2];

        public int collisionCount;
        public float baseRot;
        public float distance;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1800;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void Initialize()
        {
            baseRot = (Main.MouseWorld - Owner.Center).ToRotation();
            distance = Main.rand.Next(16 * 5, 16 * 8);
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

            switch (State)
            {
                case 0:
                    Projectile.rotation += 0.1f;
                    float currentRot = baseRot + (Projectile.ai[1] * (1800 - Projectile.timeLeft) / 350f * MathHelper.TwoPi);

                    Vector2 center = Owner.Center + (currentRot.ToRotationVector2() * distance);
                    Vector2 dir = center - Projectile.Center;

                    float velRot = Projectile.velocity.ToRotation();
                    float targetRot = dir.ToRotation();

                    float speed = Projectile.velocity.Length();
                    float aimSpeed = Math.Clamp(dir.Length() / 250f, 0, 1) * 16;

                    Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);

                    break;
                default:
                case 1:
                    {
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

                        Projectile.velocity.Y += 0.2f;
                        if (Projectile.velocity.Y > 12)
                            Projectile.velocity.Y = 12;
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((int)State == 0)
                return false;

            ScaleState = 1;
            collisionCount++;
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

            return collisionCount > 3;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }
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
