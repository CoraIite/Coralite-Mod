using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.MiniMagikePanel
{
    public class MiniMagikePanel : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        #region UI组件
        public UIDragablePanel BasePanel;

        /*
         *              👇 就这个
         *  |                  |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 中间这条线 </summary>
        public UIVerticalLine VerticalLine;

        /*
         *                 👇 在这里
         *  |                  |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 最右边的面板 </summary>
        public UIPanel ComponentPanel;
        #endregion

        public override void OnInitialize()
        {

            base.OnInitialize();
        }

        public override void Recalculate()
        {


            base.Recalculate();
        }
    }


}
