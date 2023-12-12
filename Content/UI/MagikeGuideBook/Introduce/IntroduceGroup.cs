using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.MagikeGuideBook.Introduce
{
    public class IntroduceGroup: UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages = new UILib.UIPage[2]
            {
                new C0_P1_BookName(),
                new C0_P2_Catalog()
            };


        }
    }
}
