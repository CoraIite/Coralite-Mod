using System.ComponentModel;
using System;
using System.IO;
using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class HyacinthRedBullet : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.timeLeft = 400;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 10;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[24];
            for (int i = 0; i < 24; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 24, new TriangularTip(4), factor => Helper.Lerp(4, 10, factor),
            factor =>
            {
                if (factor.X > 0.7f)
                    return Color.Lerp(Color.Black, Color.Red, (factor.X - 0.7f) / 0.3f);

                return Color.Lerp(new Color(0, 0, 0, 0), Color.Black, factor.X / 0.7f);
            });

            for (int i = 0; i < 23; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[23] = Projectile.Center + Projectile.velocity;
            trail.Positions = Projectile.oldPos;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Main.rand.NextVector2CircularEdge(8, 8), Vector2.Zero,
                    ModContent.ProjectileType<HyacinthExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (Projectile.timeLeft > 390)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightAndFogsTrail").Value);

            trail?.Render(effect);

        }
    }
}