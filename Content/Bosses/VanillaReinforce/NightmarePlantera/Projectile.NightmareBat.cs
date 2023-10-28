using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色<br></br>
    /// 使用ai1传入状态，为0时射出后小幅度摇摆，为1时射出后绕圈<br></br>
    /// 使用ai2传入旋转方向
    /// </summary>
    public class NightmareBat : BaseNightmareProj
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public ref float ColorState => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float RotateDir => ref Projectile.ai[2];

        public Color drawColor;

        private bool Init = true;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            Initialize();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            switch ((int)State)
            {
                default:
                case 0:
                    {
                        float dir = ((Projectile.timeLeft % 30) > 15 ? -1 : 1) * 0.02f;
                        Projectile.velocity = Projectile.velocity.RotatedBy(dir);
                    }
                    break;
                case 1:
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(RotateDir);
                    }
                    break;
            }

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            //Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction > 0 ? 0 : 3.141f);
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, drawColor, Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = Helper.NextVec2Dir();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.VilePowder,
                      vel * Main.rand.NextFloat(1, 8), 240, drawColor, Main.rand.NextFloat(1f, 1.5f));
                //d.noGravity = true;
            }
        }

        public void Initialize()
        {
            if (Init)
            {
                if (ColorState == -1)
                    drawColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    drawColor = NightmarePlantera.nightmareRed;
                else
                    drawColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Init = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 5, 0, Projectile.frame);
            Vector2 origin = frameBox.Size() / 2;
            SpriteEffects effect = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //绘制残影
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 10; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (0.5f - i * 0.05f), Projectile.oldRot[i], frameBox.Size() / 2, 1, effect, 0);

            //向上下左右四个方向绘制一遍
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (i * MathHelper.PiOver2).ToRotationVector2() * 4, frameBox, drawColor, Projectile.rotation, origin, 1,
                   effect, 0);
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, new Color(200, 200, 200, 255), Projectile.rotation, origin, 1, effect, 0);
            return false;
        }
    }

    public class NightmareCrow : NightmareBat
    {
        public override void AI()
        {
            Initialize();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(RotateDir);
            Projectile.direction = Math.Sign(Projectile.velocity.X);

            //Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction > 0 ? 0 : 3.141f);

            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                    -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.NextFloat(0.1f, 0.25f), 0, drawColor, Main.rand.NextFloat(0.25f, 0.45f));
            }
        }
    }
}
