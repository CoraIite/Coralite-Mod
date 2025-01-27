using Terraria;

namespace Coralite.Core.Loaders
{
    class KeybindLoader : ModSystem
    {
        public static ModKeybind ArmorBonus;
        public static ModKeybind SpecialAttack;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;
            ArmorBonus = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "ArmorBonus", "C");
            SpecialAttack = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "SpecialAttack", "F");
        }

        public override void Unload()
        {
            ArmorBonus = null;
            SpecialAttack = null;
        }

    }
}
