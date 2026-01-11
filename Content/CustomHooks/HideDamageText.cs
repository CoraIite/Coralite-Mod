#if DEBUG
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class HideDamageText : HookGroup
    {
        public override void Load()
        {
            On_CombatText.NewText_Rectangle_Color_string_bool_bool += On_CombatText_NewText_Rectangle_Color_string_bool_bool;
        }

        public override void Unload()
        {
            On_CombatText.NewText_Rectangle_Color_string_bool_bool -= On_CombatText_NewText_Rectangle_Color_string_bool_bool;
        }

        private int On_CombatText_NewText_Rectangle_Color_string_bool_bool(On_CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Rectangle location, Color color, string text, bool dramatic, bool dot)
        {
            int i = orig.Invoke(location, color, text, dramatic, dot);

            if (i != Main.maxCombatText)
            {
                Main.combatText[i].text = "?";
                Main.combatText[i].color = Color.Yellow;
            }

            return i;
        }
    }
}
#endif