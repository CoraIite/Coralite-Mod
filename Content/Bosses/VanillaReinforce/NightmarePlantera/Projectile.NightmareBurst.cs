using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareBurst : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float BaseRot => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[2];

        private Trail trail;
        private float tentacleWidth = 40;

        public Color burstColor = NightmarePlantera.nightmareSparkleColor;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 400;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[20];
            for (int i = 0; i < 20; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            if (Timer < 20)
            {
                Projectile.velocity = (BaseRot + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Projectile.velocity.Length();

                for (int i = 0; i < 19; i++)
                    Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                Projectile.oldPos[19] = Projectile.Center + Projectile.velocity;
            }

            if (Timer > 100)
            {
                tentacleWidth *= 0.96f;
                burstColor *= 0.95f;
                if (burstColor.A < 10)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Timer++;

            trail ??= new Trail(Main.graphics.GraphicsDevice, 20, new EmptyMeshGenerator(), factor =>
            {
                return Helper.Lerp(tentacleWidth, 0, factor);
            }, factor =>
            {
                if (factor.X < 0.7f)
                    return Color.Lerp(Color.Transparent, burstColor, factor.X / 0.7f);

                return burstColor;
            });

            trail.TrailPositions = Projectile.oldPos;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["NightmareTentacle"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
            effect.Parameters["sampleTexture"].SetValue(NightmarePlantera.tentacleTex.Value);
            effect.Parameters["extraTexture"].SetValue(NightmarePlantera.waterFlowTex.Value);
            effect.Parameters["flowAlpha"].SetValue(0.85f);
            effect.Parameters["warpAmount"].SetValue(3);

            trail?.DrawTrail(effect);
        }
    }
}
