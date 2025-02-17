using Coralite.Core;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Projectiles.Projectiles_Magic
{
    public class PlatycodonBullet2 : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        BasicEffect effect;
        private Trail trail;
        public bool canDamage = true;
        private bool span;

        public ref float Alpha => ref Projectile.localAI[0];

        public PlatycodonBullet2()
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
        
        public void Initialize()
        {
            Alpha = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.oldPos = new Vector2[14];
            for (int i = 0; i < 14; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override bool? CanDamage() => canDamage;

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            if (!canDamage)
            {
                Alpha -= 0.08f;
                if (Alpha < 0)
                    Projectile.Kill();
            }

            trail ??= new Trail(Main.instance.GraphicsDevice, 14, new EmptyMeshGenerator(), factor => Helper.Lerp(1, 4, factor), factor =>
            {
                if (factor.X > 0.8f)
                    return Color.Lerp(new Color(100, 100, 100, 100) * Alpha, Color.White * Alpha, (factor.X - 0.8f) / 0.2f);

                return Color.Lerp(new Color(0, 0, 0, 0), new Color(100, 100, 100, 100) * Alpha, factor.X / 0.8f);
            });

            for (int i = 0; i < 13; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[13] = Projectile.Center + Projectile.velocity;
            trail.TrailPositions = Projectile.oldPos;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            canDamage = false;
            Projectile.Center += Projectile.velocity;
            Projectile.velocity *= 0;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.65f;
            if (Projectile.velocity.Length() < 2f)
                canDamage = false;
            Projectile.damage = (int)(Projectile.damage * 0.8f);
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

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White * Alpha, Projectile.rotation, new Vector2(15, 9), Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}