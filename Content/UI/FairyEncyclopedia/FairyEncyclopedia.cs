using Coralite.Core;
using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyEncyclopedia : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static bool visible;
        public override bool Visible => visible;

        public static UIPanel BackGround;

        /// <summary>
        /// 排列的按钮
        /// </summary>
        public static SortButton[] sortButtons;
        public static UIGrid FairyGrid;
        public static float Timer;

        public int PageIndex;

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
            ShowAll,
            /// <summary>
            /// 收集换奖励界面
            /// </summary>
            CollectPanel,
            /// <summary>
            /// 单个的描述
            /// </summary>
            Details,
        }

        public override void OnInitialize()
        {
            BackGround = new UIPanel();
            BackGround.BackgroundColor = new Color(63, 82, 151) * 0.85f;

            //设置到屏幕中心
            BackGround.HAlign = 0.5f;
            BackGround.VAlign = 0.5f;
            BackGround.Width.Set(Main.screenWidth*0.66f,0);
            BackGround.Height.Set(Main.screenHeight*0.7f,0);

            FairyGrid ??= new UIGrid();

            FairyGrid.Top.Set(40, 0);
            FairyGrid.Width.Set(BackGround.Width.Pixels - 18, 0);
            FairyGrid.Height.Set(BackGround.Height.Pixels - 70, 0);

            Append(BackGround);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.controlInv)
            {
                visible = false;
                return;
            }

            switch (State)
            {
                case UpdateState.ShowAll:
                    {
                        if (Timer < FairySlot.XCount * FairySlot.YCount)//显示过渡小动画
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
            if (BackGround != null)
            {
                BackGround.Width.Set(Main.screenWidth * 0.66f, 0);
                BackGround.Height.Set(Main.screenHeight * 0.7f, 0);
            }

            Main.playerInventory = false;
            PageIndex = 0;
            base.Recalculate();
        }

        public void SetToAllShow()
        {
            BackGround.RemoveAllChildren();
            BackGround.Append(FairyGrid);

            SetShowGrid(0);

            State = UpdateState.ShowAll;
            Recalculate();
        }

        /// <summary>
        /// 传入的值是增或者减少页数
        /// </summary>
        /// <param name="page"></param>
        public void SetShowGrid(int page)
        {
            int currentPage = page + PageIndex;
            PageIndex = Math.Clamp(currentPage, 0, FairyLoader.FairyCount / (FairySlot.XCount * FairySlot.YCount) + 1);

            FairyGrid.Clear();
            Timer = 0;
            for (int i = 0; i < FairySlot.XCount * FairySlot.YCount; i++)
            {
                int type = PageIndex * FairySlot.XCount * FairySlot.YCount + i;

                if (type < FairyLoader.FairyCount)
                {
                    FairySlot slot = new FairySlot(type, i);
                    slot.SetSize(FairyGrid);
                    FairyGrid.Add(slot);
                }
                else
                    break;
            }
        }

        public void Sort(SortStyle sortStyle)
        {
            Timer = 0;
        }
    }
}
