using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    [AutoLoadTexture(Path=AssetDirectory.FairyCatcherItems)]
    public partial class FairySystem
    {
        public static ATex[] FairyAssets;
        public static ATex[] FairyCatcherCoreAssets;

        public static ATex FairySlotBorder;
        public static ATex FairySlotHoverBorder;
        public static ATex FairySlotBackground;

        public static ATex DefaultLine { get; set; }

        public static void LoadFairyTexture()
        {
            FairyAssets = new ATex[FairyLoader.FairyCount];
            for (int i = 0; i < FairyLoader.FairyCount; i++)
            {
                Fairy fairy = FairyLoader.GetFairy(i);
                if (fairy != null)
                    FairyAssets[i] = ModContent.Request<Texture2D>(fairy.Texture);
            }

            FairyCatcherCoreAssets = new ATex[FairyCircleCoreLoader.FairyCircleCoreCount];
            for (int i = 0; i < FairyCircleCoreLoader.FairyCircleCoreCount; i++)
            {
                FairyCircleCore f = FairyCircleCoreLoader.GetFairyCircleCore(i);
                if (f != null)
                    FairyCatcherCoreAssets[i] = ModContent.Request<Texture2D>(f.Texture);
            }

            FairySlotBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotBorder");
            FairySlotHoverBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotHoverBorder");
            FairySlotBackground = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySlotBackground");
        }

        public static void UnloadFairyTexture()
        {
            FairyAssets = null;
        }
    }
}
