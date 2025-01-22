using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
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

        public static Color BackgroundColor = new(56, 50, 53, 200);
        public static Color BackgroundColor2 = new(85, 79, 81, 150);
        public static Color EdgeColor = new(250, 217, 241, 185);

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

        public void InitBasePanel()
        {
            BasePanel ??= new UIDragablePanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"), dragable: true, resizeableX: true, resizeableY: true);

            BasePanel.BackgroundColor = BackgroundColor;
            BasePanel.BorderColor = EdgeColor;
        }

        public override void Recalculate()
        {


            base.Recalculate();
        }
    }


}
