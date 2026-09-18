using Coralite.Core;
using Coralite.Helpers;
using InnoVault.Vectors;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Projectiles.Projectiles_Magic
{
    /// <summary>
    /// ai0用于控制弹幕颜色，0是紫色，1是粉色
    /// ai1用于控制旋转方向
    /// </summary>
    public class PlatycodonBullet1 : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        private StrokeStyle trailStyle;
        public bool canDamage = true;
        private bool span;

        public ref float Alpha => ref Projectile.localAI[0];

        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanDamage() => canDamage;

        public void Initialize()
        {
            Alpha = 1;
            Projectile.InitOldPosCache(12);
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            if (canDamage)
            {
                int timer = 200 - Projectile.timeLeft;
                float factor = Projectile.ai[1] * 0.04f;

                if ((timer % 40) < 20)
                    Projectile.velocity = Projectile.velocity.RotatedBy(-factor);
                else
                    Projectile.velocity = Projectile.velocity.RotatedBy(factor);

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Alpha -= 0.04f;
                if (Alpha < 0)
                    Projectile.Kill();
            }

            switch (Projectile.ai[0])
            {
                default:
                case -1:     //紫色
                    {
                        trailStyle ??= new StrokeStyle {
                            Parameterization = StrokeParameterization.PointIndex,
                            WidthFunction = TrailWidth,
                            ColorFunction = TrailColorPurple,
                        };
                    }
                    break;
                case 1:     //粉色
                    {
                        trailStyle ??= new StrokeStyle {
                            Parameterization = StrokeParameterization.PointIndex,
                            WidthFunction = TrailWidth,
                            ColorFunction = TrailColorPink,
                        };
                    }
                    break;
            }


            for (int i = 0; i < 11; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FadeOut();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FadeOut();
        }

        public void FadeOut()
        {
            canDamage = false;
            Projectile.Center += Projectile.velocity;
            Projectile.velocity *= 0;
        }

        public void DrawPrimitives()
        {
            if (trailStyle == null)
                return;

            VectorRenderer.DrawStroke(Projectile.oldPos, trailStyle, new VectorDrawOptions(VectorSpace.World) {
                Blend = BlendState.AlphaBlend,
            });
        }

        private float TrailWidth(float t) => Helper.Lerp(1, 4, t) * 2f; //全宽

        private Color TrailColorPurple(float t, float side)
        {
            if (t > 0.8f)
                return Color.Lerp(new Color(51, 45, 137, 30) * Alpha, Color.White * Alpha, (t - 0.8f) / 0.2f);

            return Color.Lerp(new Color(0, 0, 0, 0), new Color(51, 45, 137, 30) * Alpha, t / 0.8f);
        }

        private Color TrailColorPink(float t, float side)
        {
            if (t > 0.8f)
                return Color.Lerp(new Color(134, 45, 137, 30) * Alpha, Color.White * Alpha, (t - 0.8f) / 0.2f);

            return Color.Lerp(new Color(0, 0, 0, 0), new Color(134, 45, 137, 30) * Alpha, t / 0.8f);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(1, 2, 0, (int)Projectile.ai[0]), Color.White * Alpha, Projectile.rotation, new Vector2(15, 9), Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}