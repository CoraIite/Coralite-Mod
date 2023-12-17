using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class Chapter2Group : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[2]
            {
                new C2_ChapterName(),
                new C2_1_CrystalCave1(),
            };
        }
    }
}
