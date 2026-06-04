using Coralite.Content.UI.UILib;
using Coralite.Core.Systems.KeySystem;
using Terraria.UI;

namespace Coralite.Content.UI.BookUI
{
    /// <summary>
    /// 管理一堆书页的类，
    /// </summary>
    public class UIPageGroup : UIElement
    {
        /// <summary>
        /// 绑定的知识
        /// </summary>
        public Knowledge OwnedKnowledge;

        /// <summary>
        /// 是否能在书中显示
        /// </summary>
        public virtual bool CanShowInBook { get => OwnedKnowledge.Unlock; }

        /// <summary>
        /// 用于存放所有的书页
        /// </summary>
        public UIPage[] Pages;

        public UIPageGroup(Knowledge ownedKnowledge, UIPage[] pages)
        {
            OwnedKnowledge = ownedKnowledge;
            Pages = pages;
        }

        public UIPageGroup()
        {
            InitPages();
        }

        /// <summary>
        /// 初始化书页，仅在未传入时调用
        /// </summary>
        public virtual void InitPages()
        {

        }

        public bool TryGetPage<T>(out T page) where T : UIPage
        {
            if (Pages == null)
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
