using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Evevts.ShadowCastle
{
    public class BlackHoleTrials:ModSystem
    {
        public static bool DownedBlackHoleTrails;

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("DownedBlackHoleTrails", DownedBlackHoleTrails);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            DownedBlackHoleTrails = tag.Get<bool>("DownedBlackHoleTrails");
        }
    }
}
