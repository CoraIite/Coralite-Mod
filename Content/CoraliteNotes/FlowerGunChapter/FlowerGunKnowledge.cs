using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.HyacinthSeries;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    public class FlowerGunKnowledge : CollectKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<HyacinthRelic>();

        public override int GetCollectsCount() => (int)KeyFlowerGuns.Count;

        public enum KeyFlowerGuns
        {
            Wisteria,
            SunflowerGun,
            Floette,
            Arethusa,
            Datura,
            GhostPipe,
            Aloe,
            Rosemary,
            Snowdrop,
            ThunderDukeVine,
            EternalBloom,
            StarsBreath,
            QueenOfNight,
            Lycoris,
            Hyacinth,

            Count,
        }

        public override UIPageGroup GetUIPageGroup() => new GroupFlowerGun();
    }
}
