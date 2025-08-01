using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherItems)]
    public partial class FairySystem
    {
        public static ATex[] FairyAssets;
        public static ATex[] FairyCatcherCoreAssets;
        public static ATex[] FairySkillAssets;

        public static ATex DefaultLine { get; set; }
        public static ATex FairyIVIcon { get; set; }
        public static ATex JarAimTarget { get; set; }
        public static ATex JarAimLine { get; set; }

        public static void LoadFairyTexture()
        {
            FairyAssets = new ATex[FairyLoader.FairyCount];
            for (int i = 0; i < FairyLoader.FairyCount; i++)
            {
                Fairy fairy = FairyLoader.GetFairy(i);
                if (fairy != null)
                    FairyAssets[i] = ModContent.Request<Texture2D>(fairy.Texture);
            }

            FairyCatcherCoreAssets = new ATex[FairyLoader.FairyCircleCoreCount];
            for (int i = 0; i < FairyLoader.FairyCircleCoreCount; i++)
            {
                FairyCircleCore f = FairyLoader.GetFairyCircleCore(i);
                if (f != null)
                    FairyCatcherCoreAssets[i] = ModContent.Request<Texture2D>(f.Texture);
            }

            FairySkillAssets = new ATex[FairyLoader.FairySkillCount];
            for (int i = 0; i < FairyLoader.FairyCircleCoreCount; i++)
            {
                FairySkill f = FairyLoader.GetFairySkill(i);
                if (f != null)
                    FairySkillAssets[i] = ModContent.Request<Texture2D>(f.Texture);
            }
        }

        public static void UnloadFairyTexture()
        {
            FairyAssets = null;
            FairyCatcherCoreAssets = null;
            FairySkillAssets = null;
        }
    }
}
