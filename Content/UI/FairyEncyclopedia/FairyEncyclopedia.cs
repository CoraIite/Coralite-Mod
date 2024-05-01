using Coralite.Core;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyEncyclopedia : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        public static UIDragablePanel BackGround;

        /// <summary>
        /// 排列的按钮
        /// </summary>
        public static SortButton[] sortButtons;

        public static float Timer;

        public static UpdateState State;
        public static SortStyle CurrentSortStyle;

        public enum SortStyle
        {
            ByType,
            ByRarity,
            ShowCaught,
        }

        public enum UpdateState
        {
            /// <summary>
            /// 显示所有的
            /// </summary>
            All,
            /// <summary>
            /// 收集换奖励界面
            /// </summary>
            CollectPanel,
            /// <summary>
            /// 单个的描述
            /// </summary>
            Details,
        }

        public override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case UpdateState.All:
                    {
                        if (Timer < 10)
                        {
                            Timer += 0.2f;
                        }
                    }
                    break;
                case UpdateState.CollectPanel:
                    break;
                case UpdateState.Details:
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        public override void Recalculate()
        {

            base.Recalculate();
        }

        public void Sort(SortStyle sortStyle)
        {
            Timer = 0;
        }
    }
}
