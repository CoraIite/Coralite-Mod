using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    public class DashBowKnowledge : CollectKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<DashBowPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<ThyphionRelic>();

        public override int GetCollectsCount() => (int)DashBows.Count;

        public enum DashBows
        {
            Afterglow,
            TremblingBow,
            FarAwaySky,
            IcicleBow,
            Turbulence,
            RadiantSun,
            FullMoon,
            PlasmaBow,
            HorizonArc,
            SeismicWave,

            ReversedFlash,
            Glaciate,
            Solunar,
            Aurora,

            Thyphion,
            Count,
        }

        public override UIPageGroup GetUIPageGroup() => new GroupDashBowChapter();
    }
}
