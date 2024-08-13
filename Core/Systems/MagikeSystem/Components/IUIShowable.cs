using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IUIShowable
    {
        /// <summary>
        /// 自定义UI中的显示
        /// </summary>
        /// <param name="parent"></param>
        void ShowInUI(UIElement parent);
    }
}
