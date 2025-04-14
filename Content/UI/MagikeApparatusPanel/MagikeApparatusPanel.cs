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
using Terraria.GameInput;
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

        public static List<Action<SpriteBatch>> DrawExtras = new List<Action<SpriteBatch>>(4);

        //public static bool ComponentButtonsVisible = false;

        #region 各类记录用字段

        public static Color BackgroundColor = new(56, 50, 53, 200);
        public static Color BackgroundColor2 = new(85, 79, 81, 150);
        public static Color EdgeColor = new(250, 217, 241, 185);

        public static bool ShouldResetComponentPanel;

        /// <summary> 当前的魔能物块实体 </summary>
        public static MagikeTP CurrentEntity;

        /// <summary> 当前显示的组件的ID </summary>
        public static int CurrentShowComponentIndex;

        /// <summary>
        /// 当前组件按钮数量
        /// </summary>
        private int _currentCount;

        /// <summary> 当前显示的组件 </summary>
        //private static bool[] _showComponents;
        /// <summary> 当前显示的组件 </summary>
        //public static bool[] ShowComponents
        //{
        //    get
        //    {
        //        if (_showComponents == null)
        //        {
        //            _showComponents = new bool[MagikeComponentID.Count + 1];
        //            Array.Fill(_showComponents, true);
        //        }

        //        return _showComponents;
        //    }
        //}

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
        public UIList ComponentList;

        /*
         *  | ■                |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 切换组件显示方式的按钮 </summary>
        //public ComponentShowTypeButton ComponentShowTypeButton;

        /*
         *  |  ◊◊◊◊◊◊          |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         *  |           |      |
         */
        /// <summary> 组件显示切换方格 </summary>
        //public UIGrid ShowComponentButtons;

        //public UIPanel ComponentControllerPanel;

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

        #region 初始化

        public override void OnInitialize()
        {
            //初始化基本面板
            InitBasePanel();
            //初始化切换按钮
            //InitComponentController();
            //初始化组件方格和菱形条
            InitComponentList();

            //初始化数直条
            InitVerticalLine();

            //初始化特殊显示面板
            InitComponentPanel();

            BasePanel.Top.Set(50, 0);
            BasePanel.Left.Set(800, 0);
            //BasePanel.SetSize(ComponentControllerPanel.Width.Pixels + VerticalLine.Width.Pixels + 460, 350);
            BasePanel.SetSize(ComponentList.Width.Pixels + VerticalLine.Width.Pixels + 460, 350);

            Append(BasePanel);
            base.OnInitialize();
        }

        public void InitBasePanel()
        {
            BasePanel ??= new UIDragablePanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"), dragable: true, resizeableX: true, resizeableY: true);

            BasePanel.BackgroundColor = BackgroundColor;
            BasePanel.BorderColor = EdgeColor;
        }

        //public void InitComponentController()
        //{
        //    ComponentControllerPanel ??= new UIPanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
        //        ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"));
        //    ComponentControllerPanel.RemoveAllChildren();

        //    ComponentShowTypeButton = new ComponentShowTypeButton();
        //    //默认在左上角
        //    ComponentShowTypeButton.Left.Set(0, 0);
        //    ComponentShowTypeButton.VAlign = 0.5f;
        //    ComponentControllerPanel.Append(ComponentShowTypeButton);
        //    ComponentControllerPanel.OverflowHidden = false;

        //    float fullWidth = ComponentShowTypeButton.OutsideWidth();
        //    float fullHeight = ComponentShowTypeButton.Height.Pixels;

        //    ShowComponentButtons ??= new UIGrid();

        //    if (ComponentButtonsVisible)
        //    {
        //        ShowComponentButtons.Clear();

        //        float recordWidth = 0;
        //        float height = 0;
        //        for (int i = 0; i < MagikeComponentID.Count + 1; i++)
        //        {
        //            ComponentSelectButton button = new(i);
        //            recordWidth += button.Width.Pixels;
        //            height = button.Height.Pixels;
        //            ShowComponentButtons.Add(button);
        //        }

        //        ShowComponentButtons.Width.Set(recordWidth + 12, 0);
        //        ShowComponentButtons.Height.Set(height + 6, 0);

        //        ShowComponentButtons.Top.Set(0, 0);
        //        ShowComponentButtons.Left.Set(ComponentShowTypeButton.OutsideWidth() + ComponentShowTypeButton.Left.Pixels + 10, 0);
        //        ShowComponentButtons.OverflowHidden = false;

        //        ComponentControllerPanel.Append(ShowComponentButtons);

        //        fullWidth += ShowComponentButtons.Width.Pixels + 24;
        //        fullHeight = ShowComponentButtons.Height.Pixels + 6;
        //    }

        //    ComponentControllerPanel.Width.Set(fullWidth, 0);
        //    ComponentControllerPanel.Height.Set(fullHeight, 0);
        //    ComponentControllerPanel.Top.Set(12, 0);

        //    ComponentControllerPanel.BackgroundColor = BackgroundColor2;
        //    ComponentControllerPanel.BorderColor = EdgeColor;

        //    BasePanel.Append(ComponentControllerPanel);
        //}

        public void InitVerticalLine()
        {
            VerticalLine ??= new UIVerticalLine();

            VerticalLine.Left.Set(ComponentList.Width.Pixels, 0);
            //VerticalLine.Width.Set(VerticalLine.Width.Pixels + 24, 0);

            BasePanel.Append(VerticalLine);
        }

        public void InitComponentList()
        {
            //ComponentGrid ??= new UIGrid();
            if (ComponentList == null)
            {
                ComponentList = new();
                ComponentList.QuickInvisibleScrollbar();
            }

            //ComponentGrid.Top.Set(ComponentControllerPanel.Height.Pixels + ComponentControllerPanel.Top.Pixels + 12, 0);
            //ComponentList.Top.Set(ComponentControllerPanel.Height.Pixels + ComponentControllerPanel.Top.Pixels + 12, 0);
            ComponentList.Top.Set(12, 0);

            //宽度和物品按钮相同，高度填满剩余的
            //ComponentGrid.Width.Set(ComponentControllerPanel.Width.Pixels, 0);
            //ComponentGrid.Height.Set(-ComponentControllerPanel.Height.Pixels - ComponentControllerPanel.Top.Pixels - 12, 1);
            //ComponentGrid.OverflowHidden = true;

            //ComponentList.Width.Set(ComponentControllerPanel.Width.Pixels, 0);
            //ComponentList.Height.Set(-ComponentControllerPanel.Height.Pixels - ComponentControllerPanel.Top.Pixels - 12, 1);
            ComponentList.Width.Set(56, 0);
            ComponentList.Height.Set(-12, 1);
            ComponentList.OverflowHidden = true;

            //if (ComponentButtonsVisible)
            //    BasePanel.Append(ComponentGrid);
            //else
            BasePanel.Append(ComponentList);
        }

        public void InitComponentPanel()
        {
            ComponentPanel ??= new UIPanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground2"),
                ModContent.Request<Texture2D>(AssetDirectory.Blank));

            ComponentPanel.Left.Set(ComponentList.Width.Pixels + VerticalLine.Width.Pixels, 0);
            ComponentPanel.Width.Set(-VerticalLine.Left.Pixels - VerticalLine.Width.Pixels - 6, 1);
            ComponentPanel.Top.Set(0, 0);
            ComponentPanel.Height.Set(0, 1f);
            ComponentPanel.BackgroundColor = BackgroundColor2;
            ComponentPanel.BorderColor = EdgeColor;

            ComponentPanel.OverflowHidden = true;

            BasePanel.Append(ComponentPanel);
        }

        #endregion

        public override void Recalculate()
        {
            if (BasePanel == null || CurrentEntity == null)
                return;
            DrawExtras?.Clear();
            InitBasePanel();

            BasePanel.RemoveAllChildren();
            //InitComponentController();

            InitComponentList();
            ResetComponentGrid();

            InitVerticalLine();
            InitComponentPanel();

            float min = ComponentList.Width.Pixels + VerticalLine.Width.Pixels + 360;

            BasePanel.MinWidth.Set(min, 0);
            BasePanel.MinHeight.Set(250, 0);

            BasePanel.MaxWidth.Set(BasePanel.MinWidth.Pixels + 400, 0);
            BasePanel.MaxHeight.Set(250 + 400, 0);

            BasePanel.Width.Pixels = Math.Clamp(BasePanel.Width.Pixels, BasePanel.MinWidth.Pixels, BasePanel.MaxWidth.Pixels);
            BasePanel.Height.Pixels = Math.Clamp(BasePanel.Height.Pixels, BasePanel.MinHeight.Pixels, BasePanel.MaxHeight.Pixels);

            BasePanel.OverflowHidden = true;

            int index = CurrentEntity.GetMainComponentIndex();
            if (index != -1)
                CurrentShowComponentIndex = index;

            ResetComponentPanel();

            base.Recalculate();
        }

        public void BaseRecalculate()
            => base.Recalculate();

        public void ResetComponentGrid()
        {
            //if (ComponentButtonsVisible)
            //{
            //    ComponentGrid.Clear();
            //    for (int i = 0; i < CurrentEntity.ComponentsCache.Count; i++)
            //    {
            //        int id = CurrentEntity.ComponentsCache[i].ID;
            //        if (!ShowComponents[id + 1])
            //            continue;

            //        var button = new ComponentButton(i);
            //        ComponentGrid.Add(button);
            //    }
            //}
            //else
            //{
            ComponentList.Clear();

            var filterButton = new ComponentButton(-1);
            filterButton.HAlign = 0.3f;
            ComponentList.Add(filterButton);

            for (int i = 0; i < CurrentEntity.ComponentsCache.Count; i++)
            {
                int id = CurrentEntity.ComponentsCache[i].ID;
                //if (!ShowComponents[id + 1])
                //    continue;

                if (id == MagikeComponentID.MagikeFilter && CurrentEntity.ComponentsCache[i] is not IUIShowable)
                    continue;

                var button = new ComponentButton(i);
                button.HAlign = 0.3f;
                ComponentList.Add(button);
            }

            //}
        }

        public void ResetComponentPanel()
        {
            if (ComponentPanel == null)
                return;

            ComponentPanel.RemoveAllChildren();

            if (CurrentEntity.ComponentsCache.Count != 0)
                CurrentShowComponentIndex = Math.Clamp(CurrentShowComponentIndex, -1, CurrentEntity.ComponentsCache.Count - 1);

            if (!CurrentEntity.ComponentsCache.IndexInRange(CurrentShowComponentIndex))
            {
                if (CurrentShowComponentIndex == -1)
                {
                    BaseRecalculate();
                    BaseRecalculate();
                    AddFilterController(ComponentPanel);
                    return;
                }

                return;
            }

            if (CurrentEntity.ComponentsCache[CurrentShowComponentIndex] is IUIShowable showable)
            {
                showable.ShowInUI(ComponentPanel);
                BaseRecalculate();
                BaseRecalculate();
            }
        }

        /// <summary>
        /// 添加滤镜控制器
        /// </summary>
        /// <param name="parent"></param>
        public void AddFilterController(UIElement parent)
        {
            UIElement title = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.FilterController), parent,new Vector2(1.3f));
            parent.Append(title);
            UIElement Text1 = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToRemove), parent);
            Text1.SetTopLeft(title.Height.Pixels + 8, 0);
            parent.Append(Text1);

            FixedUIGrid grid = new FixedUIGrid();
            for (int i = 0; i < CurrentEntity.ExtendFilterCapacity; i++)
                grid.Add(new FilterButton(i));

            grid.SetSize(0, 0, 1, 1);
            grid.SetTopLeft(Text1.Top.Pixels+title.Height.Pixels + 8, 0);

            grid.QuickInvisibleScrollbar();

            parent.Append(grid);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory || CurrentEntity == null)
            {
                visible = false;
                return;
            }

            Vector2 pos = Helper.GetMagikeTileCenter(CurrentEntity.Position);
            //一些情况下的关闭
            if (!VaultUtils.IsPointOnScreen(pos.ToScreenPosition()))
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

            if (Vector2.Distance(Main.LocalPlayer.Center, pos) > 16 * 30)
            {
                visible = false;
                CurrentEntity = null;
                return;
            }

            if (ShouldResetComponentPanel)
            {
                ShouldResetComponentPanel = false;
                ResetComponentPanel();
                RecalculateChildren();
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

            if (data == null)
                return;

            Point16 bottomRight = CurrentEntity.Position + new Point16(data.Width - 1, data.Height - 1);

            Color drawColor = Color.Lerp(Coralite.MagicCrystalPink, Color.Coral, (MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2) + 0.5f);
            MagikeHelper.DrawRectangleFrame(spriteBatch, topLeft, bottomRight, drawColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

            if (BasePanel.IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("Coralite/MagikePanel");

            foreach (var item in DrawExtras)
                item.Invoke(spriteBatch);
            DrawExtras.Clear();

            base.Draw(spriteBatch);
        }
    }
}
