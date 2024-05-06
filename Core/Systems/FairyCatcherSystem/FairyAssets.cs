using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        public static Asset<Texture2D>[] FairyAssets;

        public void LoadFairyTexture()
        {
            FairyAssets = new Asset<Texture2D>[FairyLoader.FairyCount];
            for (int i = 0; i < FairyLoader.FairyCount; i++)
            {
                Fairy fairy = FairyLoader.GetFairy(i);
                if (fairy != null)
                    FairyAssets[i] = ModContent.Request<Texture2D>(fairy.Texture);
            }
        }

        public void UnloadFairyTexture()
        {
            FairyAssets = null;
        }

    }
}
