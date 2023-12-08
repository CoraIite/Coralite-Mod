using Coralite.Content.UI.UILib;
using Terraria.UI;

namespace Coralite.Content.UI.BookUI
{
    /// <summary>
    /// 管理一堆书页的类，
    /// </summary>
    public abstract class UIPageGroup : UIElement
    {
        /// <summary>
        /// 是否能在书中显示
        /// </summary>
        public bool CanShowInBook { get; }

        /// <summary>
        /// 用于存放所有的书页
        /// </summary>
        private UI_Page[] _pages;
        public UI_Page[] Pages { get=>_pages; }

    }
}
