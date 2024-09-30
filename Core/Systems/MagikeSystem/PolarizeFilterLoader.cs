using Coralite.Content.Items.Magike;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static FrozenDictionary<MALevel, Color> PolarizeFilterColor;
        public static FrozenDictionary<MALevel, int> PolarizeFilterItemType;

        public static void LoadPolarizeFilter()
        {
            Dictionary<MALevel, Color> tempColor = [];
            Dictionary<MALevel, int> tempItemType = [];

            foreach (var mod in ModLoader.Mods)
            {
                if (mod is ICoralite or Coralite)
                    foreach (var item in from Type t in AssemblyManager.GetLoadableTypes(mod.Code)
                                         where !t.IsAbstract && t.IsSubclassOf(typeof(PolarizedFilterItem))
                                         let item = Activator.CreateInstance(t) as PolarizedFilterItem
                                         select item)
                    {
                        PolarizedFilter Filter = item.GetFilterComponent() as PolarizedFilter;
                        MALevel level = Filter.Level;
                        tempColor.Add(level, item.FilterColor);
                        tempItemType.Add(level, ModContent.Find<ModItem>(mod.Name, item.Name).Type);
                    }
            }

            PolarizeFilterColor = tempColor.ToFrozenDictionary();
            PolarizeFilterItemType = tempItemType.ToFrozenDictionary();
        }

        public static Color GetColor(MALevel level)
        {
            if (PolarizeFilterColor.TryGetValue(level, out Color c))
                return c;

            return Color.Gray;
        }

        public static int GetPolarizedFilterItemType(MALevel level)
        {
            if (PolarizeFilterItemType.TryGetValue(level, out int type))
                return type;

            return ModContent.ItemType<BasicFilter>();
        }


        public static void UnLoadPolarizeFilter()
        {
            PolarizeFilterColor = null;
            PolarizeFilterItemType = null;
        }
    }
}
