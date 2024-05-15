using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class HorizontalStar : Particle
    {
        public const int phase_1 = 8;
        public const int phase_2 = 16;
        public const int phase_3 = 24;
        public const int phase_4 = 32;

        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 126, 93);
            fadeIn = 0;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void Update()
        {
            shader.UseColor(color);

            do
            {
                //因为所有状态时间都相等所以这里就直接简单粗暴的填第一状态的时间了
                //如果有需要改的话请把所有的都改了
                if (fadeIn < phase_1)
                {
                    Scale *= 1.14f;
                    float factor = fadeIn / phase_1;
                    shader.UseOpacity(0.65f - factor * 0.25f);
                    shader.UseSaturation(1.5f + factor * 0.6f);
                    break;
                }

                if (fadeIn < phase_2)
                {
                    Scale *= 0.86f;
                    float factor = (fadeIn - phase_1) / phase_1;
                    shader.UseOpacity(0.4f + factor * 0.4f);
                    shader.UseSaturation(2.3f - factor * 0.8f);
                    break;
                }

                Velocity *= 0.96f;
                color *= 0.96f;

                if (fadeIn < phase_3)
                {
                    Scale *= 1.1f;
                    float factor = (fadeIn - phase_2) / phase_1;
                    shader.UseOpacity(0.65f - factor * 0.25f);
                    shader.UseSaturation(1.5f + factor * 0.6f);
                    break;
                }

                Scale *= 0.84f;
                float factor2 = (fadeIn - phase_3) / phase_1;
                shader.UseOpacity(0.4f + factor2 * 0.4f);
                shader.UseSaturation(2.3f - factor2 * 0.8f);

            } while (false);

            Lighting.AddLight(Center, color.ToVector3());

            fadeIn++;

            if (fadeIn > phase_4)
                active = false;
        }
    }
}
