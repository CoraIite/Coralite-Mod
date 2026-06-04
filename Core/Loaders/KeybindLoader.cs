using Terraria;

namespace Coralite.Core.Loaders
{
    public class KeybindLoader : ModSystem
    {
        public static ModKeybind ArmorBonus;
        public static ModKeybind SpecialAttack;
        public static ModKeybind Dash;
        public static ModKeybind ConsultInCoraliteNote;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Mod Mod = Coralite.Instance;
            ArmorBonus = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "ArmorBonus", "C");
            SpecialAttack = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "SpecialAttack", "F");
            Dash = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "Dash", "R");
            ConsultInCoraliteNote = Terraria.ModLoader.KeybindLoader.RegisterKeybind(Mod, "ConsultInCoraliteNote", "LeftControl");
        }

        public override void Unload()
        {
            ArmorBonus = null;
            SpecialAttack = null;
            Dash = null;
        }
    }
}
