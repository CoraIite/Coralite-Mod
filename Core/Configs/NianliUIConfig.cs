using Coralite.Content.UI;
using Coralite.Core.Loaders;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace Coralite.Core.Configs
{
    [BackgroundColor(51,179,189)]
    [Label("念力值显示位置")]
    public class NianliUIConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        //额，这两个变量用于控制念力值UI的显示位置，默认大概在物品栏的右边

        [Range(0,1920)]
        [DefaultValue(620)]
        [Increment(1)]
        [Slider]
        public int nianliUI_X;

        [Range(0, 1080)]
        [DefaultValue(26)]
        [Increment(1)]
        [Slider]
        public int nianliUI_Y;

        public override void OnChanged()
        {
            NianliChargingBar.basePos = new Microsoft.Xna.Framework.Vector2(nianliUI_X, nianliUI_Y);
            try
            {
                UILoader.GetUIState<NianliChargingBar>().Recalculate();
            }
            catch (System.Exception)
            { }
        }
    }
}
