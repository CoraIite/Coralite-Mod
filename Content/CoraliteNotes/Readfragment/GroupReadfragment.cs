using Coralite.Content.UI.BookUI;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class GroupReadfragment : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages =
                [
                    new NamePage(),
                    new DescriptionPage(),
                    new ReadPage(),
                    new FragmentPage(),
                    new FragmentPage2(),
                ];
        }
    }
}
