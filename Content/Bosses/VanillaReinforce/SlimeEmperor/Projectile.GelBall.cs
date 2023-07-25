using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelBall : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

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
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 1)  //膨胀小动画
            {
                Projectile.localAI[0] += 0.05f;
                Projectile.localAI[1] += 0.05f;
                Projectile.localAI[2] += 0.05f;
                if (Projectile.localAI[0]>1)
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
                    Scale = Vector2.Lerp(Scale, new Vector2(1.3f, 0.8f), 0.15f);
                    if (Scale.X > 1.25f)
                        ScaleState = 2;
                    break;
                case 2:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.9f, 1.2f), 0.15f);
                    if (Scale.Y > 1.15f)
                        ScaleState = 3;
                    break;
                case 3:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.1f);
                    if (Math.Abs(Scale.X-1)<0.05f)
                    {
                        Scale = Vector2.One;
                        ScaleState=0;
                    }
                    break;
            }

            Projectile.velocity.Y += 0.02f;
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;

            Projectile.rotation += 0.25f;
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
                Projectile.velocity.X = -oldVelX;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY;
            return Projectile.ai[0] > 3;
        }

        public override void Kill(int timeLeft)
        {
            //FTW中才会有分裂弹幕
            if (Main.getGoodWorld && Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Helper.NextVec2Dir() * 16, ModContent.ProjectileType<SmallGelBall>(), Projectile.damage * 2 / 3, 0, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            var pos = Projectile.Center - Main.screenPosition;
            Color color = lightColor * Projectile.localAI[0];
            var frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 scale = Scale;

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);
            //绘制影子拖尾
            Projectile.DrawShadowTrails(new Color(30, 55, 117), 0.3f, 0.03f, 1, 8, 2, scale);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(50, 152 + (int)(50 * factor), 225);
            color *= Projectile.localAI[0] * 0.75f;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);

            return false ;
        }
    }

    public class SmallGelBall : GelBall
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 1)
                Projectile.localAI[0] += 0.05f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 0.01f;
            if (Projectile.velocity.Y > 16)
                Projectile.velocity.Y = 16;
        }

        public override void Kill(int timeLeft)
        {

        }
    }
}
