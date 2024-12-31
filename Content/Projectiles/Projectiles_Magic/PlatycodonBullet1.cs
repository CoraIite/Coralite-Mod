using Coralite.Core;
using InnoVault.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Projectiles.Projectiles_Magic
{
    /// <summary>
    /// ai0用于控制弹幕颜色，0是紫色，1是粉色
    /// ai1用于控制旋转方向
    /// </summary>
    public class PlatycodonBullet1 : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        BasicEffect effect;
        private Trail trail;
        public bool canDamage = true;

        public ref float Alpha => ref Projectile.localAI[0];
        public PlatycodonBullet1()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

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

        public override void OnSpawn(IEntitySource source)
        {
            Alpha = 1;
            Projectile.oldPos = new Vector2[12];
            for (int i = 0; i < 12; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
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

                        trail ??= new Trail(Main.instance.GraphicsDevice, 12, new EmptyMeshGenerator(), factor => Helper.Lerp(1, 4, factor), factor =>
                        {
                            if (factor.X > 0.8f)
                                return Color.Lerp(new Color(51, 45, 137, 30) * Alpha, Color.White * Alpha, (factor.X - 0.8f) / 0.2f);

                            return Color.Lerp(new Color(0, 0, 0, 0), new Color(51, 45, 137, 30) * Alpha, factor.X / 0.8f);
                        });
                    }
                    break;
                case 1:     //粉色
                    {


                        trail ??= new Trail(Main.instance.GraphicsDevice, 12, new EmptyMeshGenerator(), factor => Helper.Lerp(1, 4, factor), factor =>
                        {
                            if (factor.X > 0.8f)
                                return Color.Lerp(new Color(134, 45, 137, 30) * Alpha, Color.White * Alpha, (factor.X - 0.8f) / 0.2f);

                            return Color.Lerp(new Color(0, 0, 0, 0), new Color(134, 45, 137, 30) * Alpha, factor.X / 0.8f);
                        });
                    }
                    break;
            }


            for (int i = 0; i < 11; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;
            trail.TrailPositions = Projectile.oldPos;
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
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.DrawTrail(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(1, 2, 0, (int)Projectile.ai[0]), Color.White * Alpha, Projectile.rotation, new Vector2(15, 9), Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}