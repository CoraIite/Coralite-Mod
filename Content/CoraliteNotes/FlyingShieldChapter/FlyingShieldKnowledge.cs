using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.FlyingShieldChapter
{
    public class FlyingShieldKnowledge : CollectKnowledge
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + nameof(GlassShield);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<HephaesthRelic>();

        public override UIPage[] GetUIPages()
        {
            return [
                    new FlyingShieldPage(),
                    new FlyingShieldCollect(),
                    new FlyingShieldAccessoryPage1(),
                    new FlyingShieldAccessoryPage2(),
                    new FlyingShieldAccessoryPage3(),
                    new FlyingShieldAccessoryPage4(),
                ];
        }

        public override int GetCollectsCount() => (int)SPFlyingShields.Count;

        public enum SPFlyingShields
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
