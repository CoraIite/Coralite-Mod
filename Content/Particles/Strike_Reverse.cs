using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    internal class Strike_Reverse : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Color = Color.White;
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/JustTexture", ReLogic.Content.AssetRequestMode.ImmediateLoad), "JustTexturePass");
            Frame = new Rectangle(0, 0, 128, 128);
        }

        public override void AI()
        {
            if (Opacity % 2 == 0)
                Frame.Y = (int)(Opacity / 2) * 128;

            Opacity++;

            if (Opacity > 16)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new(96, 96);

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, Frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
