using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter3
{
    public class Chapter3Group : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[4]
            {
                new C3_ChapterName(),
                new C3_BasicLens(),
                new C3_BiomeLens1(),
                new C3_BiomeLens2(),
            };
        }
    }
}
