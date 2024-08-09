using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class MagikeApparatusPanel:BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        #region 各类记录用字段

        /// <summary> 当前的魔能物块实体 </summary>
        public static MagikeTileEntity CurrentEntity;

        /// <summary> 当前组件的显示方式 </summary>
        public static ComponentShowType CurrentComponentShowType;
        /// <summary> 当前显示的组件的ID </summary>
        public static int CurrentShowComponentIndex;

        /// <summary>
        /// 当前组件按钮数量
        /// </summary>
        private int _currentCount;

        /// <summary> 当前显示的组件 </summary>
        public static bool[] _showComponents;
        public static bool[] ShowComponents
        {
            get
            {
                if (_showComponents == null)
                {
                    _showComponents = new bool[MagikeComponentID.Count + 1];
                    Array.Fill(_showComponents, true);
                }

                return _showComponents;
            }
        }

        #endregion

        #region UI组件
        
        public UIDragablePanel BasePanel;

        /*
         * 0代表物品组件的按钮
         *  |                  |
         *  |  0 0 0    |      |
         *  |  0 0      |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 组件方格，显示在最左边 </summary>
        public UIGrid ComponentGrid;

        /*
         * 0代表物品组件的按钮
         *  |                  |
         *  |    0      |      |
         *  |    0      |      |
         *  |  < 0 >    |      |
         *  |    0      |      |
         *  |    0      |      |
         */
        /// <summary> 组件方格，显示在最左边 </summary>
        public UIRollingBar ComponentRollingBar;

        /*
         *  | ■                |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 切换组件显示方式的按钮 </summary>
        public ComponentShowTypeButton ComponentShowTypeButton;

        /*
         *  |  ◊◊◊◊◊◊          |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 组件显示切换方格 </summary>
        public UIGrid ShowComponentButtons;

        /*
         *              👇 就这个
         *  |                  |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 中间这条线 </summary>
        public UIVerticalLine VerticalLine;

        /*
         *                 👇 在这里
         *  |                  |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 最右边的面板 </summary>
        public UIPanel ComponentPanel;

        #endregion

        /// <summary> 组件按钮显示的方式 </summary>
        public enum ComponentShowType
        {
            /// <summary> 方格 </summary>
            Grid,
            /// <summary> 竖直滚动条 </summary>
            VerticalBar,
        }

        #region 初始化

        public override void OnInitialize()
        {
            //初始化基本面板
            InitBasePanel();
            //初始化切换按钮
            InitShowTypeButton();
            //初始化控制组件显示的按钮
            InitApparatusButtons();
            //初始化数直条
            InitVerticalLine();
            //初始化组件方格和菱形条
            InitComponentRollingBar();
            InitComponentGird();

            //初始化特殊显示面板
            InitComponentPanel();

            BasePanel.MinWidth.Set(ComponentShowTypeButton.OutsideWidth()
                + ShowComponentButtons.OutsideWidth() + VerticalLine.OutsideWidth() + 300, 0);

            BasePanel.MinHeight.Set(VerticalLine.Width.Pixels + 300, 0);
            
            BasePanel.Width.Set(ComponentShowTypeButton.OutsideWidth()
                + ShowComponentButtons.OutsideWidth() + VerticalLine.OutsideWidth() + 300, 0);

            BasePanel.Height.Set(ShowComponentButtons.OutsideHeight() + VerticalLine.Width.Pixels + 300, 0);
            BasePanel.Top.Set(500, 0);
            BasePanel.Left.Set(500, 0);

            Append(BasePanel);
            base.OnInitialize();
        }

        public void InitBasePanel()
        {
            BasePanel ??= new UIDragablePanel(true, true, true);
        }

        public void InitShowTypeButton()
        {
            ComponentShowTypeButton = new ComponentShowTypeButton();
            //默认在左上角
            ComponentShowTypeButton.Top.Set(20, 0);
            BasePanel.Append(ComponentShowTypeButton);
        }

        /// <summary>
        /// 初始化控制组件显示的按钮i
        /// </summary>
        public void InitApparatusButtons()
        {
            if (ShowComponentButtons == null)
                ShowComponentButtons = new UIGrid();
            else
                ShowComponentButtons.Clear();

            float recordWidth = 0;
            float height = 0;
            for (int i = 0; i < MagikeComponentID.Count + 1; i++)
            {
                ComponentSelectButton button = new ComponentSelectButton(i);
                recordWidth += button.Width.Pixels;
                height = button.Height.Pixels;
                ShowComponentButtons.Add(button);
            }

            ShowComponentButtons.Width.Set(recordWidth + 4, 0);
            ShowComponentButtons.Height.Set(height, 0);

            ShowComponentButtons.Top.Set(20, 0);
            ShowComponentButtons.Left.Set(ComponentShowTypeButton.OutsideWidth(), 0);

            BasePanel.Append(ShowComponentButtons);
        }

        public void InitVerticalLine()
        {
            VerticalLine ??= new UIVerticalLine();

            VerticalLine.Left.Set(ComponentShowTypeButton.Width.Pixels
                + ShowComponentButtons.Width.Pixels, 0);
            //VerticalLine.Width.Set(VerticalLine.Width.Pixels + 24, 0);

            BasePanel.Append(VerticalLine);
        }

        public void InitComponentRollingBar()
        {
            ComponentRollingBar = new UIRollingBar((int i) => CurrentShowComponentIndex = i,()=>CurrentShowComponentIndex);
            ComponentRollingBar.Top.Set(ShowComponentButtons.OutsideHeight(), 0);

            //宽度和物品按钮相同，高度填满剩余的
            ComponentRollingBar.Width.Set(ComponentShowTypeButton.OutsideWidth()
                + ShowComponentButtons.OutsideWidth(), 0);
            ComponentRollingBar.Height.Set(BasePanel.Height.Pixels - ShowComponentButtons.OutsideHeight(), 0);
            ComponentRollingBar.OverflowHidden = true;

            BasePanel.Append(ComponentRollingBar);
        }

        public void InitComponentGird()
        {
            ComponentGrid ??=new UIGrid();
            ComponentGrid.Top.Set(ShowComponentButtons.Top.Pixels + ShowComponentButtons.OutsideHeight(), 0);

            //宽度和物品按钮相同，高度填满剩余的
            ComponentGrid.Width.Set(ComponentShowTypeButton.Width.Pixels
                + ShowComponentButtons.Width.Pixels, 0);
            ComponentGrid.Height.Set(BasePanel.Height.Pixels - ShowComponentButtons.Height.Pixels-ShowComponentButtons.Top.Pixels, 0);
            ComponentGrid.OverflowHidden = true;

            BasePanel.Append(ComponentGrid);
        }

        public void InitComponentPanel()
        {
            ComponentPanel ??= new UIPanel();

            ComponentPanel.Left.Set(ComponentShowTypeButton.Width.Pixels
                + ShowComponentButtons.Width.Pixels + VerticalLine.Width.Pixels, 0);
            ComponentPanel.Width.Set(-VerticalLine.Left.Pixels - VerticalLine.Width.Pixels, 1);
            ComponentPanel.Top.Set(20, 0);
            ComponentPanel.Height.Set(-20, 1);
            ComponentPanel.BackgroundColor = new Color(108, 19, 68,150);
            ComponentPanel.BorderColor = new Color(184, 80, 13,150);
            ComponentPanel.OverflowHidden = true;

            BasePanel.Append(ComponentPanel);
        }

        #endregion

        public override void Recalculate()
        {
            if (BasePanel == null || CurrentEntity == null)
                return;

            InitBasePanel();

            BasePanel.MinWidth.Set(ComponentShowTypeButton.Width.Pixels
                + ShowComponentButtons.Width.Pixels + VerticalLine.Width.Pixels + 300, 0);

            BasePanel.MinHeight.Set( 350, 0);
            //BasePanel.Width.Set(ComponentShowTypeButton.OutsideWidth()
            //    + ShowComponentButtons.OutsideWidth() + VerticalLine.OutsideWidth() + 300, 0);

            //BasePanel.Height.Set(ShowComponentButtons.OutsideHeight() + VerticalLine.Width.Pixels + 300, 0);

            BasePanel.RemoveAllChildren();
           // BasePanel.Append(ComponentShowTypeButton);
            //BasePanel.Append(ShowComponentButtons);
            //foreach (var element in ShowComponentButtons._items)
            //    (element as ComponentSelectButton).ResetScale();
            InitShowTypeButton();
            InitApparatusButtons();

            switch (CurrentComponentShowType)
            {
                default:
                case ComponentShowType.VerticalBar:
                    BasePanel.Append(ComponentRollingBar);

                    ComponentRollingBar.RemoveAllChildren();
                    for (int i = 0; i < CurrentEntity.Components.Count; i++)
                    {
                        var button = new ComponentButtonAlpha(i);
                        ComponentRollingBar.Append(button);
                    }
                    break;
                case ComponentShowType.Grid:
                    //BasePanel.Append(ComponentGrid);

                    InitComponentGird();

                    ComponentGrid.Clear();
                    for (int i = 0; i < CurrentEntity.Components.Count; i++)
                    {
                        var button = new ComponentButton(i);
                        ComponentGrid.Add(button);
                    }
                    break;
            }

            //BasePanel.Append(VerticalLine);
            InitVerticalLine();
            InitComponentPanel();
            //BasePanel.Append(ComponentPanel);

            base.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            //一些情况下的关闭
            if (!Main.playerInventory || CurrentEntity == null 
                || !Helper.OnScreen(Helper.GetMagikeTileCenter(CurrentEntity.Position).ToScreenPosition()))
            {
                visible = false;
                CurrentEntity = null;
                return;
            }

            if (_currentCount != CurrentEntity.ComponentsCache.Count)
            {
                _currentCount= CurrentEntity.ComponentsCache.Count;
                Recalculate();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentEntity == null)
                return;

            //绘制方框
            //Point16 topLeft = CurrentEntity.Position;
            //MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData data, out _);
            //Point16 bottomRight = CurrentEntity.Position+new Point16(data.Width-1,data.Height-1);

            //MagikeHelper.DrawRectangleFrame(spriteBatch, topLeft, bottomRight, Coralite.MagicCrystalPink);
            base.Draw(spriteBatch);
        }
    }
}
