using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Loaders
{
    class KeybindLoader : ModSystem
    {
        public static ModKeybind ArmorBonus;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;
            ArmorBonus = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "套装效果", "C");
        }

        public override void Unload()
        {
            ArmorBonus = null;
        }

    }
}
