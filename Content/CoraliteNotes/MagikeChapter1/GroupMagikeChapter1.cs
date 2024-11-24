using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class GroupMagikeChapter1 : UIPageGroup
    {
        public override bool CanShowInBook => CoraliteContent.GetKKnowledge(KeyKnowledgeID.MagikeS1).Unlock;

        public override void InitPages()
        {
            Pages =
                [
                    //P1：了解魔能
                    new GetMagikeKnowledge1Page(),
                    new WhatIsMagikePage(),

                    //P2：搭建你的第一条魔能产线
                    new PlaceFirstLens(),
                    new PlacePolarizedFilter(),
                    new UIDescriptionPage(),
                    new PutItemIn(),
                    new StoneMaker(),
                ];
        }
    }
}
