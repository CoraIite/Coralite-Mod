using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI;

namespace Coralite.Core.Loaders
{
    public class CurrencyLoader : IReflactionLoader
    {
        public static Dictionary<CustomCurrencySystem, int> CurrencySystemIDs;
        public static FrozenDictionary<CustomCurrencySystem, int> CurrencySystemIDsF;

        public int Priority => 2;
        public IReflactionLoader.LoadSide Side => IReflactionLoader.LoadSide.All;

        public void PreSetUp(Mod Mod)
        {
            CurrencySystemIDs = new Dictionary<CustomCurrencySystem, int>();
        }

        public void SetUp(Mod Mod, Type type)
        {
            if (type.IsSubclassOf(typeof(CustomCurrencySystem)) && !type.IsAbstract)
            {
                var system = (CustomCurrencySystem)Activator.CreateInstance(type, null);

                CurrencySystemIDs[system] = CustomCurrencyManager.RegisterCurrency(system);
            }
        }

        public void PostSetUp(Mod Mod)
        {
            CurrencySystemIDsF = CurrencySystemIDs.ToFrozenDictionary();
            CurrencySystemIDs = null;
        }

        public static int GetCurrencyID<T>() where T : CustomCurrencySystem => CurrencySystemIDsF.FirstOrDefault(n => n.Key is T).Value;

        public void Unload(Mod Mod)
        {
            CurrencySystemIDs = null;
            CurrencySystemIDsF = null;
        }
    }
}
