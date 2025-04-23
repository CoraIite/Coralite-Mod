using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Loaders
{
    public class EffectLoader:ModSystem
    {
        /// <summary>
        /// 只是用传入颜色的着色器
        /// </summary>
        public static BasicEffect ColorOnlyEffect {  get;private set; }
        public static BasicEffect TextureColorEffect {  get;private set; }

        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                ColorOnlyEffect = new BasicEffect(Main.instance.GraphicsDevice);
                ColorOnlyEffect.VertexColorEnabled = true;

                TextureColorEffect = new BasicEffect(Main.instance.GraphicsDevice);
                TextureColorEffect.VertexColorEnabled = true;
                TextureColorEffect.TextureEnabled = true;
            });
        }
    }
}
