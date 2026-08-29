using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.ItemTransform
{
    public class ItemTransformSystem : ModSystem,ILocalizedModType
    {
        public static Dictionary<int, int> TransformItem { get; private set; }

        public string LocalizationCategory => "Systems";

        public static LocalizedText TransformTo { get; private set; }

        public override void Load()
        {
            TransformItem = [];
            if (!Main.dedServ)
            {
                TransformTo = this.GetLocalization(nameof(TransformTo));
            }
        }

        public override void Unload()
        {
            TransformItem = null;
            TransformTo = null;
        }
    }
}
