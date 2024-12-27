using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class HorizontalStar : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public const int phase_1 = 8;
        public const int phase_2 = 16;
        public const int phase_3 = 24;
        public const int phase_4 = 32;

        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 126, 93);
            Opacity = 0;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            shader.UseColor(Color);

            do
            {
                //因为所有状态时间都相等所以这里就直接简单粗暴的填第一状态的时间了
                //如果有需要改的话请把所有的都改了
                if (Opacity < phase_1)
                {
                    Scale *= 1.14f;
                    float factor = Opacity / phase_1;
                    shader.UseOpacity(0.65f - (factor * 0.25f));
                    shader.UseSaturation(1.5f + (factor * 0.6f));
                    break;
                }

                if (Opacity < phase_2)
                {
                    Scale *= 0.86f;
                    float factor = (Opacity - phase_1) / phase_1;
                    shader.UseOpacity(0.4f + (factor * 0.4f));
                    shader.UseSaturation(2.3f - (factor * 0.8f));
                    break;
                }

                Velocity *= 0.96f;
                Color *= 0.96f;

                if (Opacity < phase_3)
                {
                    Scale *= 1.1f;
                    float factor = (Opacity - phase_2) / phase_1;
                    shader.UseOpacity(0.65f - (factor * 0.25f));
                    shader.UseSaturation(1.5f + (factor * 0.6f));
                    break;
                }

                Scale *= 0.84f;
                float factor2 = (Opacity - phase_3) / phase_1;
                shader.UseOpacity(0.4f + (factor2 * 0.4f));
                shader.UseSaturation(2.3f - (factor2 * 0.8f));

            } while (false);

            Lighting.AddLight(Position, Color.ToVector3());

            Opacity++;

            if (Opacity > phase_4)
                active = false;
        }
    }
}
