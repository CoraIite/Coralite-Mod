using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class Chapter1Group: UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[6]
            {
                new C1_P1_ChapterName(),
                //1.1.1 魔能历史
                new C1_1_1_History1(),
                new C1_1_1_History2(),
                new C1_1_1_History3(),
                new C1_1_1_History4(),
                new C1_1_1_History5(),
            };


        }
    }
}
