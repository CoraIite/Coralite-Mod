using Coralite.Content.CoraliteNotes;
using Coralite.Content.UI.BookUI;
using Coralite.Content.UI.UILib;

namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 可以通过长按快捷键快速在珊瑚笔记中找到对应页数的物品
    /// </summary>
    public interface IConsultableItem
    {
        /// <summary>
        /// 获取知识，使用<see cref="CoraliteContent.GetKnowledge{T}()"/>
        /// </summary>
        public Knowledge GetKnowledge { get; }
        /// <summary>
        /// 获得对应的页数，使用<see cref="CoraliteNoteUIState.BookPanel"/>中的<see cref="UIBookPanel.GetPageIndex{T}()"/>
        /// </summary>
        public int GetPageIndex { get; }
    }
}
