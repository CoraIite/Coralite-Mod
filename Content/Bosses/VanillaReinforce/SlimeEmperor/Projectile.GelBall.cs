using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelBall : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

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

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
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

            Projectile.velocity.Y += 0.2f;
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
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }

            //FTW中才会有分裂弹幕
            if (Main.getGoodWorld && Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Helper.NextVec2Dir() * 10, ModContent.ProjectileType<SmallGelBall>(), Projectile.damage * 2 / 3, 0, Projectile.owner);
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
            color = new Color(50, 152 + (int)(100 * factor), 225);
            color *= Projectile.localAI[0] * 0.75f;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.3f, 0.03f, 1, 8, 2, scale, frameBox);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);

            return false;
        }
    }

    public class SmallGelBall : GelBall
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
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
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(0.5f, 1.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }
        }
    }
}
