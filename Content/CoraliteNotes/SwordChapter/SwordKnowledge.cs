using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.Misc_Melee;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    public class SwordKnowledge : CollectKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SwordPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override int MainRewardItemType => ModContent.ItemType<ZenithRelic>();

        public override int GetCollectsCount() => (int)Swords.Count;

        public enum Swords
        {
            CopperShortsword,
            EnchantedSword,
            BeeKeeper,
            Starfury,
            Seedler,
            TheHorsemansBlade,
            InfluxWaver,
            StarWrath,
            Meowmere,

            LightsBane,
            BloodButcherer,
            Volcano,
            Muramasa,
            BladeOfGrass,
            NightsEdge,
            TrueNightsEdge,
            BrokenHeroSword,
            Excalibur,
            TrueExcalibur,
            TerraBlade,

            Zenith,
            Count,
        }

        public override UIPageGroup GetUIPageGroup() => new GroupSwordChapter();

        public override void LoadData(KnowledgePlayer player, TagCompound tag)
        {
            Unlock = true;
            base.LoadData(player, tag);
        }

        public override void OnEnterWorld()
        {
            Unlock = true;
        }
    }
}
