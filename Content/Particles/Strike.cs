﻿using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class Strike : BasePRT
    {
        public override string Texture => AssetDirectory.Particles + Name;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame = new Rectangle(0, 0, 128, 128);
            shader = new Terraria.Graphics.Shaders.ArmorShaderData(Coralite.Instance.Assets.Request<Effect>("Effects/StarsDust", ReLogic.Content.AssetRequestMode.ImmediateLoad), "StarsDustPass");
            shader.UseSecondaryColor(Color.White);
        }

        public override void AI()
        {
            shader.UseColor(Color);

            if (Opacity % 2 == 0)
                Frame.Y = (int)(Opacity / 2) * 128;

            float factor = Opacity / 14;

            shader.UseOpacity(0.5f + (factor * 0.4f));
            shader.UseSaturation(2.3f - (factor * 0.8f));

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
