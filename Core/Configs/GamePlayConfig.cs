using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    public class GamePlayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Magike")]
        [DefaultValue(20)]
        [Range(1, 150)]
        public int SelectSize;

        [DefaultValue(true)]
        public bool ShowUpgradeableItemIcon;

        [Header("Dash")]
        [DefaultValue(true)]
        public bool SpecialDashFirst;

        [DefaultValue(false)]
        public bool OnlyDashKey;

        [Header("CoraliteNote")]
        [DefaultValue(true)]
        public bool CoraliteNoteConsultAbility;

        public override void OnChanged()
        {
            SetValues();
        }

        public void SetValues()
        {
            GamePlaySystem.SpecialDashFirst = SpecialDashFirst;
            GamePlaySystem.ShowUpgradeableItemIcon = ShowUpgradeableItemIcon;
            GamePlaySystem.SelectSize = SelectSize;
            GamePlaySystem.OnlyDashKey = OnlyDashKey;
            GamePlaySystem.CoraliteNoteConsultAbility = CoraliteNoteConsultAbility;
        }
    }

    public class GamePlaySystem
    {
        public static bool SpecialDashFirst = true;
        public static bool ShowUpgradeableItemIcon = true;
        public static int SelectSize = 20;
        public static bool OnlyDashKey = false;
        public static bool CoraliteNoteConsultAbility = true;
    }
}
