using Coralite.Content.UI.BookUI;
using Coralite.Core;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class GroupMagikeChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge<MagikeS1Knowledge>().Unlock;

        public override void InitPages()
        {
            Pages =
                [
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
                    new CraftAltar2(),
                    new PolymerizeCraft(),
                    new PolymerizeCraft2(),

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
