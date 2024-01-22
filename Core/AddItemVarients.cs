using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.Items;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Coralite.Core
{
    public class AddItemVarients : ModSystem
    {
        public override void SetStaticDefaults()
        {
            Type t = typeof(ItemVariants);
            FieldInfo info = t.GetField("_variants", BindingFlags.NonPublic | BindingFlags.Static);
            if (info != null)
            {
                var entrys = (List<ItemVariants.VariantEntry>[])info.GetValue(null);
                if (entrys != null)
                {
                    var newEntrys=(List<ItemVariants.VariantEntry>[])entrys.Clone();

                    Array.Resize(ref newEntrys, ItemLoader.ItemCount);

                    info.SetValue(null, newEntrys);

                    Mod Mod = Coralite.Instance;

                    foreach (Type t2 in AssemblyManager.GetLoadableTypes(Mod.Code))
                        if (t2.GetInterfaces().Contains(typeof(IVariantItem)) && !t2.IsAbstract)
                        {
                            var item = (IVariantItem)Activator.CreateInstance(t2, null);
                            item.AddVarient();
                        }
                }
            }
        }
    }
}
