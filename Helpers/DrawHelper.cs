﻿using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void DrawTrail(GraphicsDevice device, Action draw
            , BlendState blendState = null, SamplerState samplerState = null, RasterizerState rasterizerState = null)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            BlendState originalBlendState = Main.graphics.GraphicsDevice.BlendState;
            SamplerState originalSamplerState = Main.graphics.GraphicsDevice.SamplerStates[0];

            device.BlendState = blendState ?? originalBlendState;
            device.SamplerStates[0] = samplerState ?? originalSamplerState;
            device.RasterizerState = rasterizerState ?? originalState;

            draw();

            device.RasterizerState = originalState;
            device.BlendState = originalBlendState;
            device.SamplerStates[0] = originalSamplerState;
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
