using Terraria.UI;

namespace Coralite.Content.UI.UILib
{
    /// <summary>
    /// 书页
    /// </summary>
    public abstract class UI_Page : UIElement
    {
        /// <summary>
        /// 是否能在书中显示
        /// </summary>
        public bool CanShowInBook { get; }

        public UI_Page()
        {
            OverflowHidden = true;
        }

    }
}
