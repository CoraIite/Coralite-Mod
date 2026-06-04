using Coralite.Content.CoraliteNotes.MagikeToolWeapon1;
using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class MagikeS1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + nameof(MagicCrystal);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<GetMagikeKnowledge1Page>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Reel;

        public override void OnKnowldegeUnlock()
        {
            KnowledgeSystem.CheckForUnlock<MagikeToolWeapon1Knowledge>(Coralite.MagicCrystalPink);
        }

        public override UIPage[] GetUIPages()
        {
            return [
                    new GetMagikeKnowledge1Page(),
                    new PartJumpPage(),

                    //P1：了解魔能
                    new WhatIsMagikePage(),

                    //P2：搭建你的第一条魔能产线
                    new PlaceFirstLens(),
                    new PlacePolarizedFilter(),
                    new PlacePolarizedFilter2(),
                    new UIDescriptionPage(),
                    new PutItemIn(),

                    //P3：魔能应该用来干点什么?
                    new StoneMaker(),
                    new ConnectStaff(),
                    new ActivatedTheMachine(),
                    new ActivatedTheMachine2(),

                    //P4：重塑！聚合！魔能合成！
                    new CraftAltar(),
                    new RemodelP1(),
                    new RemodelP2(),
                    new RemodelP3(),
                    new RemodelP4(),
                    new PolymerizeRecipe(),
                    new Pedestal(),
                    new PolymerizeOtherItem(),
                    new PolymerizeFinal(),

                    //P5：扩大生产线！
                    new ExpandProductionLine(),
                    new Prism(),
                    new Column(),

                    //P6：我需要更多魔能！
                    new INeedMoreMagike(),
                ];
        }
    }
}
