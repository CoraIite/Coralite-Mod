using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldKnowledge : CollectKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<HephaesthRelic>();

        public override IEnumerable<Item> GetRewardItemTypes()
        {
            yield return new Item(ModContent.ItemType<HephaesthRelic>());
        }

        public override UIPageGroup GetUIPageGroup() => new GroupFlyingShield();

        public override int GetCollectsCount() => (int)KeyFlyingShields.Count;

        public enum KeyFlyingShields
        {
            TrashCanLid,
            GlassShield,
            MeteorFireball,
            GlazeBulwark,
            GemrainAegis,
            GoldenSamurai,
            TortoiseshellFortress,
            MechRioter,
            Fishronguard,
            ShanHai,
            Leonids,
            Solleonis,
            ConquerorOfTheSeas,
            SilverAngel,
            RoyalAngel,
            Noctiflair,
            Hephaesth,
            Count,
        }
    }
}
