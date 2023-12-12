using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter1
{
    public class Chapter1Group: UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[9]
            {
                new C1_ChapterName(),
                //1.1.1 魔能历史
                new C1_1_1_History1(),
                new C1_1_1_History2(),
                new C1_1_1_History3(),
                new C1_1_1_History4(),
                new C1_1_1_History5(),
                //1.1.2 魔能的性质
                new C1_1_2_Property(),
                //1.1.3 魔能的分类
                new C1_1_3_Classify(),
                //1.2 魔能的来源
                new C1_2_Origin()
            };


        }
    }
}
