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

        /// <summary>
        /// 组件方格，显示在最左边
        /// </summary>
        /*
         * 0代表物品组件的按钮
         *  |                  |
         *  |  0 0 0    |      |
         *  |  0 0      |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        public UIGrid ComponentGrid;
        public UIDragablePanel BasePanel;

        /// <summary>
        /// 当前的魔能物块实体
        /// </summary>
        public static MagikeTileEntity CurrentEntity;

        public UIGrid ShowComponentButtons;

        /// <summary> 组件按钮显示的方式 </summary>
        public enum ComponentShowType
        {
            /// <summary> 竖直滚动条 </summary>
            VerticalBar,
            /// <summary> 方格 </summary>
            Grid
        }

        public override void OnInitialize()
        {
            //初始化基本面板
            //初始化切换按钮
            //初始化控制组件显示的按钮
            //初始化数直条
            //初始化组件方格和菱形条
            //初始化特殊显示面板
            base.OnInitialize();
        }

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
