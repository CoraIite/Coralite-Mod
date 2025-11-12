using System.Collections.Generic;
using System.Reflection;
using Terraria.Localization;

namespace Coralite.Core.Systems.MTBStructure
{
    public class MultiblockSystem : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public static LocalizedText FailText { get; private set; }

        public static Dictionary<int, int> wallTypeToItemType;

        public override void Load()
        {
            FailText = this.GetLocalization(nameof(FailText));
            FieldInfo info = typeof(WallLoader).GetField("wallTypeToItemType", BindingFlags.NonPublic | BindingFlags.Static);
            if (info != null)
                wallTypeToItemType = (Dictionary<int, int>)info.GetValue(null);
        }

        public override void Unload()
        {
            FailText = null;
            wallTypeToItemType = null;
        }
    }
}
