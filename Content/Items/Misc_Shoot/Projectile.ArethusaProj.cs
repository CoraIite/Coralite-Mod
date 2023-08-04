using Coralite.Core.Systems.Trails;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Configs;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class ArethusaHeldProj : BaseGunHeldProj
    {
        public ArethusaHeldProj() : base(0.4f, 6, -6, AssetDirectory.Misc_Shoot) { }
    }

    public class ArethusaBullet : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        private Trail trail;
        BasicEffect effect;

        public ArethusaBullet()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[12];
            for (int i = 0; i < 12; i++)
                Projectile.oldPos[i] = Projectile.Center;

            for (int j = 0; j < 8; j++)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.05f, 0.15f));
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 0.8f));
            if (Projectile.timeLeft > 192)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f);
            else if ((Projectile.timeLeft + 8) % 40 < 20)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f);
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.02f);

            //if (Projectile.timeLeft % 2 == 0)
            //{
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Ice_Purple);
            dust.noGravity = true;
            //}

            for (int i = 0; i < 11; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;

            trail ??= new Trail(Main.instance.GraphicsDevice, Projectile.oldPos.Length, new NoTip(), factor => 2,
                factor =>
                {
                    if (factor.X > 0.7f)
                        return Color.Lerp(new Color(95, 120, 233, 60), new Color(230, 225, 255, 80), (factor.X - 0.7f) / 0.3f);

                    return Color.Lerp(new Color(0, 0, 0, 0), new Color(95, 120, 233, 60), factor.X / 0.7f);//new Color(99, 83, 142, 0)
                });

            trail.Positions = Projectile.oldPos;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ArethusaExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int j = 0; j < 8; j++)
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), (j * 0.785f).ToRotationVector2() * Main.rand.NextFloat(0.5f, 1.5f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBall_Item45, Projectile.Center);
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

            trail?.Render(effect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            //Color lightColor = Lighting.GetColor((Projectile.Center / 16).ToPoint());

            spriteBatch.Draw(mainTex, center, null, Color.White, Projectile.rotation, new Vector2(9, 9), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, center, null, new Color(255, 255, 255, 100), Projectile.rotation + 0.785f, new Vector2(9, 9), 1.5f, SpriteEffects.None, 0f);
        }
    }

    public class ArethusaExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public ref float Scale => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 14;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (Alpha == 0f)
            {
                Scale = 0.3f;
                Alpha = 1f;
            }

            Alpha -= 0.068f;
            Scale += 0.068f;
            Projectile.rotation += 0.04f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 0.8f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White * Alpha, Projectile.rotation, new Vector2(35, 45), Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
