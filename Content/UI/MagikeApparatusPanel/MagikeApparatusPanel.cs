using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.UI.Elements;
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
         
        public int RecordComponentCount;

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

            //初始化特殊显示面板

            base.OnInitialize();
        }

        public void InitBasePanel()
        {

        }

        public void InitShowTypeButton()
        {

        }

        /// <summary>
        /// 初始化控制组件显示的按钮i
        /// </summary>
        public void InitApparatusButtons()
        {
            ShowComponentButtons = new UIGrid();

            float recordWidth = 0;
            float height = 0;
            for (int i = 0; i < MagikeComponentID.Count + 1; i++)
            {
                ComponentSelectButton button = new ComponentSelectButton(i);
                recordWidth += button.GetOuterDimensions().Width;
                height = button.GetOuterDimensions().Height;
                ShowComponentButtons.Add(button);
            }

            ShowComponentButtons.Width.Set(recordWidth + 4, 0);
            ShowComponentButtons.Height.Set(height, 0);
        }

        public void InitVerticalLine()
        {
            VerticalLine = new UIVerticalLine();


        }

        #endregion

        public override void Recalculate()
        {
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            //一些情况下的关闭
            if (!Main.playerInventory || CurrentEntity == null 
                || !Helper.OnScreen(Helper.GetMagikeTileCenter(CurrentEntity.Position)))
            {
                visible = false;
                CurrentEntity = null;
            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //绘制方框


            base.Draw(spriteBatch);
        }
    }
}
