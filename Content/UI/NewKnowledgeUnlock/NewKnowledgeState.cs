using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeState : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        private bool visible = false;

        public static LinkedList<NewKnowledgeInfo> Infos { get; set; }
        public NewKnowledgeBar bar;

        /// <summary>
        /// 添加一条知识解锁提示
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="color"></param>
        public static void AddNewTip(Knowledge knowledge, Color color)
        {
            Infos?.AddLast(new NewKnowledgeInfo(knowledge, color));
            UILoader.GetUIState<NewKnowledgeState>().ActivateUI();
        }

        public static void AddNewTip(int id, Color color)
        {
            Infos?.AddLast(new NewKnowledgeInfo(id, color));
            UILoader.GetUIState<NewKnowledgeState>().ActivateUI();
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public void ActivateUI()
        {
            visible = true;
            Recalculate();
        }

        public void Hide() => visible = false;

        public override void OnInitialize()
        {
            Infos = [];
            bar = new NewKnowledgeBar();
            bar?.SetCenter(new Vector2(0, -160), new Vector2(0.5f, 1));
            Append(bar);
        }

        public override void Recalculate()
        {
            //RemoveAllChildren();
            //bar = new NewKnowledgeBar();
            bar?.SetCenter(new Vector2(0, -160), new Vector2(0.5f, 1));
            //Append(bar);
            base.Recalculate();
        }

        /// <summary>
        /// 移除一个元素，并返回是否有剩余的
        /// </summary>
        /// <returns></returns>
        public static bool Restart()
        {
            if (Infos.Count == 0)//防止一些意外情况
                return false;

            Infos.RemoveFirst();
            if (Infos.Count != 0)
                return true;

            return false;
        }
    }

    public struct NewKnowledgeInfo
    {
        public Knowledge knowledge;
        public Color color;

        public NewKnowledgeInfo(Knowledge knowledge,Color color)
        {
            this.knowledge = knowledge;
            this.color = color;
        }

        public NewKnowledgeInfo(int type, Color color)
        {
            knowledge = CoraliteContent.GetKnowledge(type);
            this.color = color;
        }
    }
}
