using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
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
    public class MagikeApparatusPanel : BetterUIState
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

        public UIPanel ComponentControllerPanel;

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
            InitComponentController();
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

            BasePanel.BackgroundColor = new Color(60, 19, 68, 200);
            BasePanel.BorderColor = new Color(184, 80, 13, 255);
        }

        //public void InitShowTypeButton()
        //{
        //    ComponentShowTypeButton = new ComponentShowTypeButton();
        //    //默认在左上角
        //    ComponentShowTypeButton.Top.Set(20, 0);
        //    BasePanel.Append(ComponentShowTypeButton);
        //}

        ///// <summary>
        ///// 初始化控制组件显示的按钮i
        ///// </summary>
        //public void InitApparatusButtons()
        //{
        //    if (ShowComponentButtons == null)
        //        ShowComponentButtons = new UIGrid();
        //    else
        //        ShowComponentButtons.Clear();

        //    float recordWidth = 0;
        //    float height = 0;
        //    for (int i = 0; i < MagikeComponentID.Count + 1; i++)
        //    {
        //        ComponentSelectButton button = new ComponentSelectButton(i);
        //        recordWidth += button.Width.Pixels;
        //        height = button.Height.Pixels;
        //        ShowComponentButtons.Add(button);
        //    }

        //    ShowComponentButtons.Width.Set(recordWidth + 4, 0);
        //    ShowComponentButtons.Height.Set(height, 0);

        //    ShowComponentButtons.Top.Set(20, 0);
        //    ShowComponentButtons.Left.Set(ComponentShowTypeButton.OutsideWidth(), 0);

        //    BasePanel.Append(ShowComponentButtons);
        //}

        public void InitComponentController()
        {
            ComponentControllerPanel ??= new UIPanel();
            ComponentControllerPanel.RemoveAllChildren();

            ComponentShowTypeButton = new ComponentShowTypeButton();
            //默认在左上角
            ComponentShowTypeButton.Left.Set(0, 0);
            ComponentShowTypeButton.VAlign = 0.5f;
            ComponentControllerPanel.Append(ComponentShowTypeButton);

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

            ShowComponentButtons.Width.Set(recordWidth + 12, 0);
            ShowComponentButtons.Height.Set(height + 6, 0);

            ShowComponentButtons.Top.Set(0, 0);
            ShowComponentButtons.Left.Set(ComponentShowTypeButton.OutsideWidth() + ComponentShowTypeButton.Left.Pixels + 10, 0);
            ShowComponentButtons.OverflowHidden = false;

            ComponentControllerPanel.Append(ShowComponentButtons);
            ComponentControllerPanel.Width.Set(ComponentShowTypeButton.OutsideWidth() + ShowComponentButtons.Width.Pixels + 30, 0);
            ComponentControllerPanel.Height.Set(ShowComponentButtons.Height.Pixels + 10, 0);
            ComponentControllerPanel.Top.Set(12, 0);

            ComponentControllerPanel.BackgroundColor = new Color(46, 46, 57, 200);
            ComponentControllerPanel.BorderColor = new Color(105, 97, 90, 200);

            BasePanel.Append(ComponentControllerPanel);
        }

        public void InitVerticalLine()
        {
            VerticalLine ??= new UIVerticalLine();

            VerticalLine.Left.Set(ComponentControllerPanel.Width.Pixels, 0);
            //VerticalLine.Width.Set(VerticalLine.Width.Pixels + 24, 0);

            BasePanel.Append(VerticalLine);
        }

        public void InitComponentRollingBar()
        {
            ComponentRollingBar = new UIRollingBar((int i) => CurrentShowComponentIndex = i, () => CurrentShowComponentIndex);
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
            ComponentGrid ??= new UIGrid();
            ComponentGrid.Top.Set(ComponentControllerPanel.Height.Pixels + ComponentControllerPanel.Top.Pixels + 12, 0);

            //宽度和物品按钮相同，高度填满剩余的
            ComponentGrid.Width.Set(ComponentControllerPanel.Width.Pixels, 0);
            ComponentGrid.Height.Set(-ComponentControllerPanel.Height.Pixels - ComponentControllerPanel.Top.Pixels, 1);
            ComponentGrid.OverflowHidden = false;

            BasePanel.Append(ComponentGrid);
        }

        public void InitComponentPanel()
        {
            ComponentPanel ??= new UIPanel();

            ComponentPanel.Left.Set(ComponentControllerPanel.Width.Pixels + VerticalLine.Width.Pixels, 0);
            ComponentPanel.Width.Set(-VerticalLine.Left.Pixels - VerticalLine.Width.Pixels, 1);
            ComponentPanel.Top.Set(0, 0);
            ComponentPanel.Height.Set(0, 1f);
            ComponentPanel.BorderColor = new Color(255, 190, 236, 255);
            ComponentPanel.BackgroundColor = new Color(50, 20, 28, 170);
            ComponentPanel.OverflowHidden = true;

            BasePanel.Append(ComponentPanel);
        }

        #endregion

        public override void Recalculate()
        {
            if (BasePanel == null || CurrentEntity == null)
                return;

            InitBasePanel();

            BasePanel.MinWidth.Set(ComponentControllerPanel.Width.Pixels + VerticalLine.Width.Pixels + 300, 0);
            BasePanel.MinHeight.Set(350, 0);

            BasePanel.MaxWidth.Set(BasePanel.MinWidth.Pixels+400, 0);
            BasePanel.MaxHeight.Set(350+400, 0);

            BasePanel.RemoveAllChildren();
            InitComponentController();

            switch (CurrentComponentShowType)
            {
                default:
                case ComponentShowType.VerticalBar:
                    InitComponentRollingBar();
                    ResetComponentBar();
                    break;
                case ComponentShowType.Grid:
                    InitComponentGird();
                    ResetComponentGrid();
                    break;
            }

            InitVerticalLine();
            InitComponentPanel();
            ResetComponentPanel();

            base.Recalculate();
            base.Recalculate();
        }

        public void ResetComponentButton()
        {
            ResetComponentBar();
            ResetComponentGrid();
        }

        public void BaseRecalculate()
            =>base.Recalculate();

        public void ResetComponentBar()
        {
            ComponentRollingBar.RemoveAllChildren();
            for (int i = 0; i < CurrentEntity.Components.Count; i++)
            {
                var button = new ComponentButtonAlpha(i);
                ComponentRollingBar.Append(button);
            }
        }

        public void ResetComponentGrid()
        {
            ComponentGrid.Clear();
            for (int i = 0; i < CurrentEntity.Components.Count; i++)
            {
                int id = CurrentEntity.ComponentsCache[i].ID;
                if (!ShowComponents[id + 1])
                    continue;

                var button = new ComponentButton(i);
                ComponentGrid.Add(button);
            }
        }

        public void ResetComponentPanel()
        {
            ComponentPanel.RemoveAllChildren();

            if (!CurrentEntity.ComponentsCache.IndexInRange(CurrentShowComponentIndex))
                return;

            if (CurrentEntity.ComponentsCache[CurrentShowComponentIndex] is MagikeComponent mc)
            {
                mc.ShowInUI(ComponentPanel);
                BaseRecalculate();
                BaseRecalculate();
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 pos = Helper.GetMagikeTileCenter(CurrentEntity.Position);
            //一些情况下的关闭
            if (!Main.playerInventory || CurrentEntity == null
                || !Helper.OnScreen(pos.ToScreenPosition()))
            {
                visible = false;
                CurrentEntity = null;
                return;
            }

            if (_currentCount != CurrentEntity.ComponentsCache.Count)
            {
                _currentCount = CurrentEntity.ComponentsCache.Count;
                Recalculate();
            }

            if (Vector2.Distance(Main.LocalPlayer.Center, pos) > 16 * 6)
            {
                visible = false;
                CurrentEntity = null;
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentEntity == null)
                return;

            //绘制方框
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.NonPremultiplied, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            Point16 topLeft = CurrentEntity.Position;
            MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData data, out _);
            Point16 bottomRight = CurrentEntity.Position + new Point16(data.Width - 1, data.Height - 1);

            Color drawColor = Color.Lerp(Coralite.MagicCrystalPink, Color.Coral, MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2 + 0.5f);
            MagikeHelper.DrawRectangleFrame(spriteBatch, topLeft, bottomRight, drawColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

            base.Draw(spriteBatch);
        }
    }
}
