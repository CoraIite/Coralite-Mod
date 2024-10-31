using Coralite.Content.UI.BookUI;

namespace Coralite.Content.UI.CoraliteNote.Readfragment
{
    public class GroupReadfragment : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            Pages =
                [
                    new NamePage(),
                    new ReadPage()
                ];
        }
    }
}
