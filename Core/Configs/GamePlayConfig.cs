using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    public class GamePlayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Magike")]
        [DefaultValue(20)]
        [Range(1, 50)]
        public int SelectSize;

        [Header("Misc")]
        [DefaultValue(true)]
        public bool SpecialDashFirst;

        public override void OnChanged()
        {
            SetValues();
        }

        public void SetValues()
        {
            GamePlaySystem.SpecialDashFirst = SpecialDashFirst;
            GamePlaySystem.SelectSize = SelectSize;
        }
    }

    public class GamePlaySystem
    {
        public static bool SpecialDashFirst = true;
        public static int SelectSize = 20;
    }
}
