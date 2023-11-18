using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Loaders
{
    public class CurrencyLoader : ModSystem
    {
        public static Dictionary<CustomCurrencySystem, int> CurrencySystemIDs;

        public override void PostSetupContent()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;

            CurrencySystemIDs = new Dictionary<CustomCurrencySystem, int>();

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))
            {
                if (t.IsSubclassOf(typeof(CustomCurrencySystem)) && !t.IsAbstract)
                {
                    var system = (CustomCurrencySystem)Activator.CreateInstance(t, null);

                    CurrencySystemIDs[system] = CustomCurrencyManager.RegisterCurrency(system);
                }
            }
        }

        public static int GetCurrencyID<T>() where T : CustomCurrencySystem => CurrencySystemIDs.FirstOrDefault(n => n.Key is T).Value;

        public override void Unload()
        {
            CurrencySystemIDs = null;
        }
    }
}
