using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        public static Asset<Texture2D>[] FairyAssets;

        public static Asset<Texture2D> FairySlotBorder;
        public static Asset<Texture2D> FairySlotHoverBorder;
        public static Asset<Texture2D> FairySlotBackground;

        public void LoadFairyTexture()
        {
            FairyAssets = new Asset<Texture2D>[FairyLoader.FairyCount];
            for (int i = 0; i < FairyLoader.FairyCount; i++)
            {
                Fairy fairy = FairyLoader.GetFairy(i);
                if (fairy != null)
                    FairyAssets[i] = ModContent.Request<Texture2D>(fairy.Texture);
            }

            FairySlotBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotBorder");
            FairySlotHoverBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotHoverBorder");
            FairySlotBackground = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotBackground");
        }

        public void UnloadFairyTexture()
        {
            FairyAssets = null;
        }

    }
}
