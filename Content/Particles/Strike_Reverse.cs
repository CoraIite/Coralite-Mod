using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    internal class Strike_Reverse : Particle
    {
        public override void SetProperty()
        {
            Color = Color.White;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/JustTexture", ReLogic.Content.AssetRequestMode.ImmediateLoad), "JustTexturePass");
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void AI()
        {
            if (fadeIn % 2 == 0)
                Frame.Y = (int)(fadeIn / 2) * 128;

            fadeIn++;

            if (fadeIn > 16)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new(96, 96);

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, Frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
