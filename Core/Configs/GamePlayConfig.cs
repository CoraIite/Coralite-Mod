using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    public class GamePlayConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool SpecialDashFirst;

        public override void OnChanged()
        {
            SetValues();
        }

        public void SetValues()
        {
            GamePlaySystem.SpecialDashFirst = SpecialDashFirst;
        }
    }

    public class GamePlaySystem
    {
        public static bool SpecialDashFirst = true;
    }
}
