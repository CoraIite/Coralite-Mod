using Coralite.Content.Items.Magike;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static FrozenDictionary<MagikeApparatusLevel, Color> PolarizeFilterColor;
        public static FrozenDictionary<MagikeApparatusLevel, int> PolarizeFilterItemType;

        public static void LoadPolarizeFilter()
        {
            Dictionary<MagikeApparatusLevel, Color> tempColor = [];
            Dictionary<MagikeApparatusLevel, int> tempItemType = [];

            foreach (var mod in ModLoader.Mods)
            {
                if (mod is ICoralite or Coralite)
                    foreach (var item in from Type t in AssemblyManager.GetLoadableTypes(mod.Code)
                                         where !t.IsAbstract && t.IsSubclassOf(typeof(PolarizedFilterItem))
                                         let item = Activator.CreateInstance(t) as PolarizedFilterItem
                                         select item)
                    {
                        PolarizedFilter Filter = item.GetFilterComponent() as PolarizedFilter;
                        MagikeApparatusLevel level = Filter.Level;
                        tempColor.Add(level, item.FilterColor);
                        tempItemType.Add(level, ModContent.Find<ModItem>(mod.Name, item.Name).Type);
                    }
            }

            PolarizeFilterColor = tempColor.ToFrozenDictionary();
            PolarizeFilterItemType = tempItemType.ToFrozenDictionary();
        }

        public static Color GetColor(MagikeApparatusLevel level)
        {
            if (PolarizeFilterColor.TryGetValue(level, out Color c))
                return c;

            return Color.Gray;
        }

        public static int GetPolarizedFilterItemType(MagikeApparatusLevel level)
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
