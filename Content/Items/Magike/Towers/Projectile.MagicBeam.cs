using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Towers
{
    public class MagicBeam : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        //private Trail trail;
        private static VertexStrip _vertexStrip = new();
        private bool span;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 18);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = true;
        }
        
        public void Initialize()
        {
            //Projectile.oldPos = new Vector2[12];
            //for (int i = 0; i < 12; i++)
            //    Projectile.oldPos[i] = Projectile.Center;
            SoundEngine.PlaySound(CoraliteSoundID.Crystal_Item101, Projectile.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.4f;

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                Projectile.velocity.Y = oldVelocity.Y * -0.4f;

            Projectile.ai[0] += 1;
            return Projectile.ai[0] > 3;
        }


        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            //trail ??= new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(16), factor => 16,
            //factor =>
            //{
            //    return Color.Lerp(new Color(0, 0, 0, 0), Coralite.MagicCrystalPink, factor.X);
            //});

            //for (int i = 0; i < 11; i++)
            //    Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            //Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;
            //trail.Positions = Projectile.oldPos;
            Projectile.rotation = Projectile.velocity.ToRotation();

            int num18 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, new Color(162, 42, 131), 1f);
            Main.dust[num18].velocity *= 0.1f;
            Main.dust[num18].velocity += Projectile.velocity * 0.2f;
            Main.dust[num18].position.X = Projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            Main.dust[num18].position.Y = Projectile.Center.Y + Main.rand.Next(-2, 3);
            Main.dust[num18].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Teleporter, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 100, Coralite.MagicCrystalPink, 1f);
                //dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-1.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + (Projectile.Size / 2f));
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, Projectile.oldPos[0] - Main.screenPosition,
                Color.White * 0.8f, Coralite.MagicCrystalPink, 0.5f, 0f, 0.5f, 0.5f, 0f, 0f,
                new Vector2(2f, 1f), Vector2.One);
            return false;
        }

        public void DrawPrimitives()
        {
            //Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            //Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            //Matrix view = Main.GameViewMatrix.ZoomMatrix;
            //Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            //effect.Parameters["sampleTexture"].SetValue(TextureAssets.Extra[189].Value);

            //trail?.Render(effect);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, new Color(162, 42, 131), Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A = 64;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(16f, 26f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
    }
}
