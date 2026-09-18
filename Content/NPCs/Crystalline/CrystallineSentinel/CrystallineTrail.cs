using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 刀光的拖尾：48 点缓存的渐变拖尾图元，24 帧内收窄淡出。与 <see cref="CrystallineSlash"/> 配对使用。
    /// </summary>
    public class CrystallineTrail : TrailParticle
    {
        public ref float Alpha => ref ai[0];
        public ref float ScaleY => ref ai[1];

        public override void SetProperty()
        {
            Color = Color.White;
            Alpha = 1;
            Rotation = Velocity.ToRotation() + 1.57f;
            InitializeCaches(48);
            trail ??= new Trail(Main.graphics.GraphicsDevice, 48, new EmptyMeshGenerator(), WidthFunction, ColorFunction);
        }

        public float WidthFunction(float factor)
        {
            return 28 * 14 * Scale * (1 - factor);
        }

        public Color ColorFunction(Vector2 coords) => Color.White;

        public override void AI()
        {
            Opacity++;

            if (Opacity > 4)
            {
                Velocity *= 0.99f;
                ScaleY -= 0.07f;
                if (Scale > 0)
                    Scale -= 0.05f;
                Alpha = Utils.MultiLerp((Opacity - 4) / 20, 1f, 0.9f, 0.8f, 0.6f, 0.3f, 0f);
            }

            if (Opacity > 24 || Alpha < 0.01f)
                active = false;

            UpdatePositionCache(48);
            if (!Main.dedServ)
                trail.TrailPositions = oldPositions;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void DrawPrimitive()
        {
            if (trail == null)
                return;

            Effect effect = ShaderLoader.GetShader("AlphaGradientTrail");

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBlurVMirror.Value);
            effect.Parameters["gradientTexture"].SetValue(CrystallineSentinelSwing.GradientTextureBlack.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail?.DrawTrail(effect);
        }
    }
}
