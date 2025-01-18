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

        public void ShowUI()
        {

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
