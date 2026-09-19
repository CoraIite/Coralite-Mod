using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.Vectors;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareBurst : CoraliteBossHostileProj, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float BaseRot => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[2];

        private StrokeStyle trailStyle;
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

        public override void Initialize()
        {
            Projectile.InitOldPosCache(20);
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

            trailStyle ??= new StrokeStyle
            {
                Parameterization = StrokeParameterization.PointIndex,
                WidthFunction = BurstTrailWidth,
                ColorFunction = BurstTrailColor,
            };
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */ => false;

        public void DrawPrimitives()
        {
            if (trailStyle == null)
                return;

            Effect effect = ShaderLoader.GetShader("NightmareTentacle");

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
            effect.Parameters["sampleTexture"].SetValue(NightmarePlantera.tentacleTex.Value);
            effect.Parameters["extraTexture"].SetValue(NightmarePlantera.waterFlowTex.Value);
            effect.Parameters["flowAlpha"].SetValue(0.85f);
            effect.Parameters["warpAmount"].SetValue(3);

            VectorRenderer.DrawStroke(Projectile.oldPos, trailStyle, new VectorDrawOptions(VectorSpace.World, effect)
            {
                Blend = BlendState.AlphaBlend,
                MatrixParameter = "transformMatrix",
            });
        }

        private float BurstTrailWidth(float t) => Helper.Lerp(tentacleWidth, 0, t) * 2f; //全宽

        private Color BurstTrailColor(float t, float side)
        {
            if (t < 0.7f)
                return Color.Lerp(Color.Transparent, burstColor, t / 0.7f);

            return burstColor;
        }
    }
}
