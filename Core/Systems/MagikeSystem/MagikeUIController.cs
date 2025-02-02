using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static UIType MagikeUIShowStyle;

        public Point PanelSize;

        public enum UIType
        {
            /// <summary>
            /// 宝宝模式，仅显示主要内容
            /// </summary>
            Baby,
            /// <summary>
            /// 专业模式，显示全部信息
            /// </summary>
            Pro
        }

        #region 帮助方法

        /// <summary>
        /// UI是否启用
        /// </summary>
        /// <returns></returns>
        public static bool UIActive()
        {
            return UILoader.GetUIState<MagikeApparatusPanel>().Visible;
        }

        public void ShowUI()
        {

        }

        /// <summary>
        /// 刷新魔能UI的组件显示面板
        /// </summary>
        public static void RecalculateComponentPanel()
        {
            MagikeApparatusPanel.ShouldResetComponentPanel = true;

        }

        #endregion


        public void SaveUI(TagCompound tag)
        {
            tag.Add(nameof(MagikeUIShowStyle), (int)MagikeUIShowStyle);
        }

        public void LoadUI(TagCompound tag)
        {
            MagikeUIShowStyle = (UIType)tag.GetInt(nameof(MagikeUIShowStyle));
        }
    }
}
