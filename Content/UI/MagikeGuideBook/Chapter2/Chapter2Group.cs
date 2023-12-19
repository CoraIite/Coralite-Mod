using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class Chapter2Group : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[8]
            {
                new C2_ChapterName(),
                new C2_1_CrystalCave1(),
                new C2_1_CrystalCave2(),
                new C2_1_CrystalCave3(),
                new C2_1_CrystalCave4(),
                new C2_1_CrystalCave5(),
                new C2_1_CrystalCave6(),
                new C2_1_CrystalCave7(),
            };
        }
    }
}
