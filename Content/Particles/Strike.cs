using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Strike : Particle
    {
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, 128, 128);
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void Update()
        {
            shader.UseColor(color);

            if (fadeIn % 2 == 0)
                Frame.Y = (int)(fadeIn / 2) * 128;

            float factor = fadeIn / 14;

            shader.UseOpacity(0.5f + factor * 0.4f);
            shader.UseSaturation(2.3f - factor * 0.8f);

            fadeIn++;

            if (fadeIn > 16)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(96, 96);

            spriteBatch.Draw(GetTexture().Value, Center - Main.screenPosition, Frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);

        }
    }
}
