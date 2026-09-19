using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Vectors;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class FantasyBall : CoraliteBossHostileProj, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private StrokeStyle trailStyle;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1800;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(16);
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft < 1765)
            {
                #region 同叶绿弹的追踪
                float velLength = Projectile.velocity.Length();
                float localAI0 = Projectile.localAI[0];
                if (localAI0 == 0f)
                {
                    Projectile.localAI[0] = velLength;
                    localAI0 = velLength;
                }

                Projectile.netUpdate = true;

                float num197 = localAI0;
                Vector2 center = Projectile.Center;
                float num198 = np.Center.X - center.X;
                float num199 = np.Center.Y - center.Y;
                float dis2Target = MathF.Sqrt((num198 * num198) + (num199 * num199));
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 8;

                Projectile.velocity.X = ((Projectile.velocity.X * (chase - 1)) + num198) / chase;
                Projectile.velocity.Y = ((Projectile.velocity.Y * (chase - 1)) + num199) / chase;

                #endregion
            }

            Projectile.rotation += 0.25f;

            for (int i = 0; i < 15; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

            if (Main.rand.NextBool(3))
                for (int i = -1; i < 2; i += 2)
                {
                    int type = Main.rand.NextFromList(DustID.PlatinumCoin, DustID.GoldCoin);
                    Vector2 dir = new(i, 0);
                    Dust.NewDustPerfect(Projectile.Center, type, dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.5f));
                }

            trailStyle ??= new StrokeStyle
            {
                Parameterization = StrokeParameterization.PointIndex,
                WidthFunction = FantasyTrailWidth,
                ColorFunction = FantasyTrailColor,
            };
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.type == ModContent.NPCType<NightmarePlantera>();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle particle = PRTLoader.NewParticle(Vector2.Lerp(Projectile.Center, target.Center, 0.5f), Vector2.Zero, CoraliteContent.ParticleType<Strike>(), FantasyGod.shineColor, 1f);
            particle.Rotation = Projectile.velocity.ToRotation() + MathHelper.Pi + 2.2f + Main.rand.NextFloat(-0.5f, 0.5f);
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 scale = new(0.75f);
            Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White, FantasyGod.shineColor * 0.6f,
                0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, scale, Vector2.One * 2);

            for (int i = 0; i < 4; i++)
            {
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos + ((Projectile.rotation + (i * MathHelper.PiOver2)).ToRotationVector2() * 16), Color.White, FantasyGod.shineColor * 0.6f,
                    0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, scale, Vector2.One * 2);
            }
            return false;
        }

        public void DrawPrimitives()
        {
            if (trailStyle == null)
                return;

            Effect effect = ShaderLoader.GetShader("FantasyTentacle");

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
            effect.Parameters["sampleTexture"].SetValue(NightmarePlantera.tentacleTex.Value);
            effect.Parameters["extraTexture"].SetValue(NightmarePlantera.waterFlowTex.Value);
            effect.Parameters["flowAlpha"].SetValue(0.5f);
            effect.Parameters["warpAmount"].SetValue(3);

            VectorRenderer.DrawStroke(Projectile.oldPos, trailStyle, new VectorDrawOptions(VectorSpace.World, effect)
            {
                Blend = BlendState.AlphaBlend,
                MatrixParameter = "transformMatrix",
            });
        }

        private float FantasyTrailWidth(float t)
        {
            if (t < 0.7f)
                return Helper.Lerp(0, 16, t / 0.7f) * 2f; //全宽

            return Helper.Lerp(16, 0, (t - 0.7f) / 0.3f) * 2f; //全宽
        }

        private Color FantasyTrailColor(float t, float side)
        {
            if (t < 0.7f)
                return Color.Lerp(new Color(0, 0, 0, 0), Color.White, t / 0.7f);

            return Color.Lerp(Color.White, FantasyGod.shineColor, (t - 0.7f) / 0.3f);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, FantasyGod.shineColor, 0, mainTex.Size() / 2, 0.3f, 0, 0);
        }
    }
}
