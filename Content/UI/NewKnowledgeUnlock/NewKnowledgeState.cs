using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System.Collections;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeState : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        private bool visible = false;

        public static LinkedList<NewKnowledgeInfo> Infos { get; set; }

        public void ActivateUI()
        {
            visible = true;
            Recalculate();
        }

        public void Hide() => visible = false;

        public override void OnInitialize()
        {
            Infos = [];

        }
    }

    public struct NewKnowledgeInfo
    {
        public Knowledge knowledge;

        public NewKnowledgeInfo(Knowledge knowledge)
        {
            this.knowledge = knowledge;
        }

        public NewKnowledgeInfo(int type)
        {
            knowledge = CoraliteContent.GetKnowledge(type);
        }
    }
}
