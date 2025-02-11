using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.NPCs.Magike
{
    /// <summary>
    /// 使用ai0控制激光角度
    /// </summary>
    public class CrystalLaser : BaseHeldProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Lasers + "VanillaCoreA";

        public static Asset<Texture2D> GlowTex;
        public static Asset<Texture2D> StarTex;
        public static Asset<Texture2D> BlackTex;

        public Vector2 endPoint;
        private SlotId soundSlot;

        public ref float LaserRotation => ref Projectile.ai[0];

        public ref float LaserHeight => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.timeLeft = 85;
            Projectile.width = Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GlowTex = ModContent.Request<Texture2D>(AssetDirectory.Dusts + "GlowBall");
            StarTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "Hexagram2");
            BlackTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "GradientBlack");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GlowTex = null;
            StarTex = null;
            BlackTex = null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft > 10)
            {
                float a = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 16, ref a);
            }

            return false;
        }

        public override void Initialize()
        {
            SoundStyle style = CoraliteSoundID.PhantasmalDeathray_Zombie104;
            style.Volume = 0.2f;
            style.Pitch = 1f;
            soundSlot = SoundEngine.PlaySound(style);
        }

        public override void AI()
        {
            for (int k = 0; k < 80; k++)
            {
                Vector2 posCheck = Projectile.Center + (Vector2.UnitX.RotatedBy(LaserRotation) * k * 8);

                if (Helper.PointInTile(posCheck) || k == 79)
                {
                    endPoint = posCheck;
                    break;
                }
            }

            int width = (int)(Projectile.Center - endPoint).Length();
            Vector2 dir = Vector2.UnitX.RotatedBy(LaserRotation);
            Color color = new(162, 42, 131);

            do
            {
                if (Projectile.timeLeft > 81)
                {
                    LaserHeight += 0.8f;
                    break;
                }

                if (Projectile.timeLeft > 78)
                {
                    LaserHeight -= 0.6f;
                    break;
                }

                if (Projectile.timeLeft > 10)
                {
                    LaserHeight = 1f;
                    break;
                }

                if (Projectile.timeLeft == 8)
                {
                    for (int i = 0; i < width - 128; i += 24)
                    {
                        Dust.NewDustPerfect(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                            dir * Main.rand.NextFloat(width / 160f), 0, color, 0.35f);
                    }
                }

                LaserHeight -= 0.1f;

            } while (false);

            if (Projectile.timeLeft > 10)
            {
                float height = 64 * LaserHeight;
                float min = width / 120f;
                float max = width / 80f;

                for (int i = 0; i < width; i += 16)
                {
                    Lighting.AddLight(Projectile.position + (Vector2.UnitX.RotatedBy(LaserRotation) * i), color.ToVector3() * height * 0.030f);
                    if (Main.rand.NextBool(20))
                    {
                        Dust.NewDustPerfect(Projectile.Center + (dir * i) + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowBall>(),
                            dir * Main.rand.NextFloat(min, max), 0, color, 0.25f);
                    }
                }
            }

            if (Projectile.timeLeft < 15)
            {
                if (SoundEngine.TryGetActiveSound(soundSlot, out var sound))
                {
                    sound.Volume -= 0.0667f;
                }
            }

        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out var sound))
            {
                sound.Stop();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D laserTex = Projectile.GetTexture();
            Texture2D flowTex = CoraliteAssets.Laser.VanillaFlowA.Value;

            Color color = new(162, 42, 131);

            Effect effect = Coralite.Instance.Assets.Request<Effect>("Effects/GlowingDust").Value;

            effect.Parameters["uColor"].SetValue(color.ToVector3());

            float height = LaserHeight * laserTex.Height / 4f;
            int width = (int)(Projectile.Center - endPoint).Length();   //这个就是激光长度

            Vector2 startPos = Projectile.Center - Main.screenPosition;
            Vector2 endPos = endPoint - Main.screenPosition;

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 2f));
            var flowTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.9f));

            var laserSource = new Rectangle((int)(Projectile.timeLeft / 30f * laserTex.Width), 0, laserTex.Width, laserTex.Height);
            var flowSource = new Rectangle((int)(Projectile.timeLeft / 45f * flowTex.Width), 0, flowTex.Width, flowTex.Height);

            var origin = new Vector2(0, laserTex.Height / 2);
            var origin2 = new Vector2(0, flowTex.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, effect, Main.GameViewMatrix.TransformationMatrix);

            //绘制流动效果
            spriteBatch.Draw(laserTex, laserTarget, laserSource, color, LaserRotation, origin, 0, 0);
            spriteBatch.Draw(flowTex, flowTarget, flowSource, color * 0.5f, LaserRotation, origin2, 0, 0);

            //由于在地下效果不明显所以删了，这个大概就是绘制一个黑影子的效果
            //spriteBatch.End();
            //spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            //Texture2D texDark = BlackTex.Value;
            //float opacity = height / (laserTex.Height / 2f) * 0.75f;

            //spriteBatch.Draw(texDark, startPos, null, Color.White * opacity, LaserRotation, new Vector2(texDark.Width / 2, 0), 50, 0, 0);
            //spriteBatch.Draw(texDark, startPos, null, Color.White * opacity, LaserRotation - 3.14f, new Vector2(texDark.Width / 2, 0), 50, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            //绘制主体光束
            Texture2D bodyTex = CoraliteAssets.Laser.Body.Value;

            color = new Color(211, 103, 156);

            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color * 0.8f, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);

            //绘制用于遮盖首尾的亮光
            Texture2D glowTex = GlowTex.Value;
            Texture2D starTex = StarTex.Value;

            spriteBatch.Draw(glowTex, endPos, null, color * (height * 0.012f), 0, glowTex.Size() / 2, 1.8f, 0, 0);
            spriteBatch.Draw(starTex, endPos, null, color * (height * 0.03f), Main.GlobalTimeWrappedHourly, starTex.Size() / 2, 0.24f, 0, 0);

            spriteBatch.Draw(glowTex, startPos, null, color * (height * 0.02f), 0, glowTex.Size() / 2, 1.1f, 0, 0);
            spriteBatch.Draw(starTex, startPos, null, color * (height * 0.05f), Main.GlobalTimeWrappedHourly * -2, starTex.Size() / 2, 0.14f, 0, 0);
        }
    }
}
