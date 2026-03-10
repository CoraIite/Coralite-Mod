using Coralite.Content.UI.BookUI;
using Coralite.Core.Loaders;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class GroupReadfragment : UIPageGroup
    {
        public override bool CanShowInBook => true;

        public override void InitPages()
        {
            int count = KnowledgeLoader.SortedKnowledgeSerieses.Count;
            Pages = new UI.UILib.UIPage[count + 3];

            Pages[0] = new NamePage();
            Pages[1] = new DescriptionPage();
            Pages[2] = new ReadPage();

            int num = 3;
            foreach (var series in KnowledgeLoader.SortedKnowledgeSerieses)
            {
                Pages[num] = new FragmentPage(series);
                num++;
            }

            //Pages =
            //    [
            //        new FragmentPage(),
            //        new FragmentPage2(),
            //        new FragmentPage3(),
            //        new FragmentPage4(),
            //    ];
        }
    }
}
