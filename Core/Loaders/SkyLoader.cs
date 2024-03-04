using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Loaders
{
    public class SkyLoader : IOrderedLoadable
    {
        public float Priority => 2f;

        public void Load()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))
            {
                if (t.IsSubclassOf(typeof(CustomSky)))
                    SkyManager.Instance[t.Name] = (CustomSky)Activator.CreateInstance(t);
            }
        }

        public void Unload()
        {
        }
    }
}
