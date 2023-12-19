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
        public abstract bool CanShowInBook { get; }

        /// <summary>
        /// 用于存放所有的书页
        /// </summary>
        public UIPage[] Pages;

        public abstract void InitPages();

        public bool TryGetPage<T>(out  T page) where T : UIPage
        {
            if (Pages ==null)
            {
                page = null;
                return false;
            }

            foreach (var p in Pages)
            {
                if (p is T t)
                {
                    page = t;
                    return true;
                }
            }

            page = null;
            return false;
        }
    }
}
