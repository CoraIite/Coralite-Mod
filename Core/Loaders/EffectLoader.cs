using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Loaders
{
    public class EffectLoader : ModSystem
    {
        /// <summary>
        /// 只是用传入颜色的着色器
        /// </summary>
        public static BasicEffect ColorOnlyEffect { get; private set; }
        public static BasicEffect TextureColorEffect { get; private set; }

        public static RasterizerState OverflowHiddenRasterizerState { get; private set; }

        public static BlendState ReverseBlendState { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                ColorOnlyEffect = new BasicEffect(Main.instance.GraphicsDevice);
                ColorOnlyEffect.VertexColorEnabled = true;

                TextureColorEffect = new BasicEffect(Main.instance.GraphicsDevice);
                TextureColorEffect.VertexColorEnabled = true;
                TextureColorEffect.TextureEnabled = true;
            });

            OverflowHiddenRasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };

            ReverseBlendState = new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha
            };
        }
    }
}
