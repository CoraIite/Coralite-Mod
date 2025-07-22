using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    [AutoLoadTexture(Path = AssetDirectory.FairyUI)]
    public class FairyEncyclopedia : BetterUIState
    {
        public static ATex LeftButton { get; set; }
        public static ATex RightButton { get; set; }
        public static ATex LeftButtonHighlight { get; set; }
        public static ATex RightButtonHighlight { get; set; }
        public static ATex FairyPanelBorder { get; set; }
        public static ATex FairyPanelBackGround { get; set; }

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static bool visible;
        public override bool Visible => visible;

        public static FairyPanel BackGround;

        /// <summary>
        /// 排列的按钮
        /// </summary>
        public SortButton[] sortButtons;
        public PageText PageText;
        public UIGrid FairyGrid;
        public FairyUITextPanel<LocalizedText> uITextPanel;

        public SelectPanelButton SelectButton;
        public UIPanel SelectButtonsPanel;

        public UIImageButton SortButton;
        public UIPanel SortButtonsPanel;

        #region 显示具体仙灵界面相关字段

        public FairyCircleShow CircleShow;
        public FairyIVRangeShow IVRangeShow;
        public UIList ConditionShow;
        public UIList SkillShow;

        /// <summary>
        /// 是否是在显示仙灵
        /// </summary>
        public bool ShowFairy;
        public bool ShowSortPanel;
        public bool ShowSelectPanel;

        public static int ShowFairyID;

        #endregion

        public class SelectPanelButton : UIImageButton
        {
            public SelectPanelButton() : base(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "SelectPanelButtonTex", AssetRequestMode.ImmediateLoad))
            {
                SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "PanelButtonHighlight", AssetRequestMode.ImmediateLoad));
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                base.DrawSelf(spriteBatch);

                Vector2 pos = GetDimensions().Position() + new Vector2(74, 18);

                if (selectType.HasValue)
                {
                    Color c = FairySystem.GetRarityColor(selectType.Value);
                    string text = Enum.IsDefined(selectType.Value) ? Enum.GetName(selectType.Value) : "SP";

                    Utils.DrawBorderString(spriteBatch, text, pos, c,
                        anchorx: 0.5f, anchory: 0.5f);
                }
                else
                {
                    Utils.DrawBorderString(spriteBatch, "All", pos, Color.White,
                        anchorx: 0.5f, anchory: 0.5f);
                }
            }
        }

        /// <summary>
        /// 当前所在的页数
        /// </summary>
        public static int PageIndex;
        public static int PageCount;

        public static SortStyle CurrentSortStyle = SortStyle.ByRarity;

        public static FairyRarity? selectType = null;

        private List<Fairy> fairies;

        /// <summary>
        /// 主体面板宽度
        /// </summary>
        public static float PanelWidth => Main.screenWidth * 0.6f;
        /// <summary>
        /// 主体面板高度
        /// </summary>
        public static float PanelHeight => Main.screenHeight * 0.6f;

        public enum SortStyle
        {
            ByType,
            ByRarity,
            ShowCaught,
        }

        #region 各类初始化

        public override void OnInitialize()
        {
            InitBackground();
            InitPageText();
            InitFairyGrid();
            InitSelectPanel();
            InitSortPanel();

            InitFairyShow();

            MakeExitButton(this);

            Append(BackGround);
        }

        private void SortButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (BackGround.HasChild(SelectButtonsPanel))//关闭选择界面
                BackGround.RemoveChild(SelectButtonsPanel);

            if (BackGround.HasChild(SortButtonsPanel))//有就关闭
            {
                BackGround.RemoveChild(SortButtonsPanel);
                ShowSortPanel = false;
                ShowSelectPanel = false;
            }
            else//没有就打开
            {
                ShowSortPanel = true;
                ShowSelectPanel = false;
                Recalculate();
            }
        }

        private void SelectButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (BackGround.HasChild(SortButtonsPanel))//关闭排序界面
                BackGround.RemoveChild(SortButtonsPanel);

            if (BackGround.HasChild(SelectButtonsPanel))//有就关闭
            {
                BackGround.RemoveChild(SelectButtonsPanel);
                ShowSelectPanel = false;
                ShowSortPanel = false;
            }
            else//没有就打开
            {
                ShowSelectPanel = true;
                ShowSortPanel = false;
                Recalculate();
            }
        }

        private static void InitBackground()
        {
            BackGround = new FairyPanel();

            //设置到屏幕中心
            BackGround.HAlign = 0.5f;
            BackGround.VAlign = 0.5f;
            BackGround.Width.Set(Main.screenWidth * 0.66f, 0);
            BackGround.Height.Set(Main.screenHeight * 0.7f, 0);

            BackGround.BackgroundColor = new Color(63, 107, 151) * 0.85f;
            BackGround.BorderColor = Color.White;
        }

        private void InitPageText()
        {
            PageText = new PageText();
            UIImageButton leftButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "LeftButton", AssetRequestMode.ImmediateLoad));
            leftButton.SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "LeftButtonHighlight", AssetRequestMode.ImmediateLoad));
            UIImageButton rightButton = new(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "RightButton", AssetRequestMode.ImmediateLoad));
            rightButton.SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "RightButtonHighlight", AssetRequestMode.ImmediateLoad));

            PageText.Left.Set(0, 0);
            PageText.Width.Set(100, 0);
            PageText.Height.Set(40, 0);

            leftButton.Left.Set(0, 0);
            rightButton.Left.Set(PageText.Width.Pixels - 32, 0);

            leftButton.OnLeftClick += LeftButton_OnLeftClick;
            rightButton.OnLeftClick += RightButton_OnLeftClick;

            PageText.Append(leftButton);
            PageText.Append(rightButton);
        }

        private void InitFairyGrid()
        {
            FairyGrid ??= new UIGrid();

            FairyGrid.Top.Set(40, 0);
            FairyGrid.Width.Set(BackGround.Width.Pixels - 18, 0);
            FairyGrid.Height.Set(BackGround.Height.Pixels - 70, 0);

            fairies = new List<Fairy>();
            for (int i = 0; i < FairyLoader.FairyCount; i++)
                fairies.Add(FairyLoader.fairys[i]);
        }

        private void InitSelectPanel()
        {
            SelectButtonsPanel = new UIPanel();

            Asset<Texture2D> circleButtonTex = TextureAssets.WireUi[0];
            Asset<Texture2D> circleButtonHoverTex = TextureAssets.WireUi[1];
            SelectButton = new SelectPanelButton();
            SelectButton.Left.Set(PageText.Width.Pixels + 10, 0);

            SelectButton.OnLeftClick += SelectButton_OnLeftClick;

            //SelectButtonsPanel.VAlign = 1;
            SelectButtonsPanel.Top.Set(40, 0);
            SelectButtonsPanel.Left.Set(SelectButton.Left.Pixels, 0);
            SelectButtonsPanel.Width.Set((circleButtonTex.Width() * 6) + 10, 0);
            SelectButtonsPanel.Height.Set(BackGround.Height.Pixels / 3, 0);

            UIGrid buttonsGrid = new();
            buttonsGrid.Width.Set(SelectButtonsPanel.Width.Pixels, 0);
            buttonsGrid.Height.Set(SelectButtonsPanel.Height.Pixels, 0);

            SelectButton AllButton = new(circleButtonTex, null);//生成默认按钮
            AllButton.SetHoverImage(circleButtonHoverTex);
            buttonsGrid.Add(AllButton);
            SelectButton SPButton = new(circleButtonTex, (FairyRarity)(-1));
            SPButton.SetHoverImage(circleButtonHoverTex);
            buttonsGrid.Add(SPButton);

            FairyRarity[] rarities = Enum.GetValues<FairyRarity>();//生成所有的稀有度按钮

            foreach (var rairty in rarities)
            {
                SelectButton rarityButton = new(circleButtonTex, rairty);
                rarityButton.SetHoverImage(circleButtonHoverTex);
                buttonsGrid.Add(rarityButton);
            }

            SelectButtonsPanel.Append(buttonsGrid);
        }

        private void InitSortPanel()
        {
            SortButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "SortPanelButtonTex", AssetRequestMode.ImmediateLoad));
            SortButton.SetHoverImage(ModContent.Request<Texture2D>(AssetDirectory.FairyUI + "PanelButtonHighlight", AssetRequestMode.ImmediateLoad));
            SortButton.Left.Set(SelectButton.Left.Pixels + SelectButton.Width.Pixels + 10, 0);
            SortButton.OnLeftClick += SortButton_OnLeftClick;

            SortButtonsPanel = new UIPanel();
            SortButtonsPanel.Top.Set(40, 0);
            SortButtonsPanel.Left.Set(SortButton.Left.Pixels, 0);
            SortButtonsPanel.Width.Set(BackGround.Width.Pixels / 4, 0);
            SortButtonsPanel.Height.Set(BackGround.Height.Pixels / 3, 0);

            UIGrid buttonsGrid = new();
            buttonsGrid.Width.Set(SelectButtonsPanel.Width.Pixels, 0);
            buttonsGrid.Height.Set(SelectButtonsPanel.Height.Pixels, 0);

            MakeSortButton(SortStyle.ByType, () => FairySystem.SortByTypeText, buttonsGrid);
            MakeSortButton(SortStyle.ByRarity, () => FairySystem.SortByRarityText, buttonsGrid);
            MakeSortButton(SortStyle.ShowCaught, () => FairySystem.SortByCaughtText, buttonsGrid);

            SortButtonsPanel.Append(buttonsGrid);
        }

        private void MakeExitButton(UIElement outerContainer)
        {
            uITextPanel = new(Language.GetText("UI.Back"), 0.7f, large: true)
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 0.3f),
                Height = StyleDimension.FromPixels(50f),
                // VAlign = 0.95f,
                HAlign = 0.5f,
                //Top = StyleDimension.FromPixels(-25f)
            };

            uITextPanel.BackgroundColor = new Color(63, 107, 151) * 0.85f;
            uITextPanel.BorderColor = Color.White;

            uITextPanel.OnUpdate += UITextPanel_OnUpdate;
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.SetSnapPoint("ExitButton", 0);
            outerContainer.Append(uITextPanel);
        }

        private void UITextPanel_OnUpdate(UIElement affectedElement)
        {
            if (affectedElement.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        private void MakeSortButton(SortStyle sortStyle, Func<LocalizedText> description, UIGrid grid)
        {
            SortButton sortButton = new(sortStyle, description);

            sortButton.OnMouseOver += FadedMouseOver;
            sortButton.OnMouseOut += FadedMouseOut;
            sortButton.Width.Set(SortButtonsPanel.Width.Pixels, 0);
            sortButton.Height.Set(32, 0);

            grid.Add(sortButton);
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            Helper.PlayPitched("Fairy/ButtonTick", 0.2f, 0);
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 107, 151);
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 107, 151) * 0.85f;
            ((UIPanel)evt.Target).BorderColor = Color.White;
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(CoraliteSoundID.MenuClose);

            if (ShowFairy)
            {
                ShowFairy = false;

                Recalculate();
                return;
            }

            visible = false;
        }

        /// <summary>
        /// 初始化上半部分的仙灵本体显示
        /// </summary>
        public void InitFairyShow()
        {
            CircleShow = new FairyCircleShow();
            CircleShow.SetCenter(new Vector2(PanelWidth / 2, PanelHeight / 2 - 20));

            IVRangeShow = new FairyIVRangeShow();
            IVRangeShow.SetSize((PanelWidth - CircleShow.Width.Pixels) / 2 - 10, PanelHeight);

            ConditionShow = new UIList();
            ConditionShow.SetTopLeft(40, CircleShow.Left.Pixels + CircleShow.Width.Pixels + 10);
            ConditionShow.SetSize((PanelWidth - CircleShow.Width.Pixels) / 2 - 10, (PanelHeight - 40) / 2);
            ConditionShow.QuickInvisibleScrollbar();

            SkillShow = new UIList();
            SkillShow.SetTopLeft(40 + ConditionShow.Height.Pixels, CircleShow.Left.Pixels + CircleShow.Width.Pixels + 10);
            SkillShow.SetSize((PanelWidth - CircleShow.Width.Pixels) / 2 - 10, (PanelHeight - 40) / 2);
            SkillShow.QuickInvisibleScrollbar();
        }

        #endregion

        private void LeftButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SetShowGrid(-1);
            base.Recalculate();
        }

        private void RightButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SetShowGrid(1);
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.controlInv || Main.playerInventory)
            {
                visible = false;
                return;
            }

            base.Update(gameTime);
        }

        public override void Recalculate()
        {
            //设置面板尺寸
            if (BackGround != null)
            {
                BackGround.Width.Set(PanelWidth, 0);
                BackGround.Height.Set(PanelHeight, 0);
            }

            uITextPanel?.Top.Set(Main.screenHeight / 2 + PanelHeight / 2 + uITextPanel.Height.Pixels / 2, 0);

            BackGround?.RemoveAllChildren();

            if (ShowFairy)//显示具体仙灵
            {
                InitFairyShow();
                BackGround?.Append(CircleShow);
                BackGround?.Append(IVRangeShow);

                ConditionShow.Clear();
                if (FairySystem.fairySpawnConditions_InEncyclopedia.TryGetValue(ShowFairyID
                    , out FairySpawnController controller))
                {
                    foreach (var c in controller.Conditions)
                        ConditionShow.Add(new FairySpawnBar(c, ConditionShow.Width.Pixels));
                }
                BackGround?.Append(ConditionShow);

                SkillShow.Clear();
                if (ContentSamples.ItemsByType[FairyLoader.GetFairy(ShowFairyID).ItemType].ModItem is BaseFairyItem bfi)
                {
                    foreach (var skillType in bfi.GetFairySkills())
                        SkillShow.Add(new FairySkillBar(skillType, ConditionShow.Width.Pixels));
                }

                BackGround?.Append(SkillShow);
            }
            else//显示全部仙灵
            {
                if (FairyGrid != null)
                {
                    FairyGrid.Top.Set(40, 0);
                    FairyGrid.Width.Set(BackGround.Width.Pixels - 18, 0);
                    FairyGrid.Height.Set(BackGround.Height.Pixels - 70, 0);
                }

                BackGround?.Append(FairyGrid);
                BackGround?.Append(PageText);
                BackGround?.Append(SelectButton);
                BackGround?.Append(SortButton);

                if (ShowSelectPanel)
                    BackGround.Append(SelectButtonsPanel);
                if (ShowSortPanel)
                    BackGround.Append(SortButtonsPanel);

            }

            Main.playerInventory = false;
            PageIndex = 0;
            base.Recalculate();
        }

        /// <summary>
        /// 在风石碑牌倍右键时调用，初始化所有
        /// </summary>
        public void SetToAllShow()
        {
            SetShowGrid(0);

            selectType = null;
            Select(null);
            Sort(CurrentSortStyle);


            ShowFairy = false;
            ShowFairyID = 0;

            Recalculate();
        }

        /// <summary>
        /// 传入的值是增或者减少页数
        /// </summary>
        /// <param name="page"></param>
        public void SetShowGrid(int page)
        {
            int currentPage = page + PageIndex;
            PageCount = FairyLoader.FairyCount / (FairySlot.XCount * FairySlot.YCount);
            PageIndex = Math.Clamp(currentPage, 0, PageCount);

            FairyGrid.Clear();
            for (int i = 0; i < FairySlot.XCount * FairySlot.YCount; i++)
            {
                int index = (PageIndex * FairySlot.XCount * FairySlot.YCount) + i;

                if (index < fairies.Count)
                {
                    FairySlot slot = new(fairies[index].Type, i);
                    slot.SetSize(FairyGrid);
                    FairyGrid.Add(slot);
                }
                else
                    break;
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    FairySlot slot2 = new(1, fairies.Count+i);
            //    slot2.SetSize(FairyGrid);
            //    FairyGrid.Add(slot2);
            //}

            base.Recalculate();
        }

        /// <summary>
        /// 传入null时为选择全部
        /// 传入小于0的数时为选择除了已有的稀有度以外的特别稀有度
        /// </summary>
        /// <param name="targetRarity"></param>
        public void Select(FairyRarity? targetRarity)
        {
            selectType = targetRarity;
            fairies.Clear();

            if (!targetRarity.HasValue)
            {
                for (int i = 0; i < FairyLoader.FairyCount; i++)
                    fairies.Add(FairyLoader.fairys[i]);

                goto CheckOver;
            }

            if (targetRarity.Value < 0)//查找所有特殊的稀有度
            {
                for (int i = 0; i < FairyLoader.FairyCount; i++)
                    if (!Enum.IsDefined(FairyLoader.fairys[i].Rarity))
                        fairies.Add(FairyLoader.fairys[i]);
                goto CheckOver;
            }

            for (int i = 0; i < FairyLoader.FairyCount; i++)
                if (FairyLoader.fairys[i].Rarity == targetRarity)
                    fairies.Add(FairyLoader.fairys[i]);

                CheckOver:

            Sort(CurrentSortStyle);
        }

        public void Sort(SortStyle sortStyle)
        {
            CurrentSortStyle = sortStyle;

            switch (sortStyle)
            {
                default:
                case SortStyle.ByType:
                    fairies.Sort((f1, f2) => f1.Type.CompareTo(f2.Type));
                    break;
                case SortStyle.ByRarity:
                    fairies.Sort((f1, f2) => -f1.Rarity.CompareTo(f2.Rarity));
                    break;
                case SortStyle.ShowCaught:
                    fairies = [.. fairies.OrderBy(f => !FairySystem.FairyCaught[f.Type])];
                    break;
            }

            SetShowGrid(0);
            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (BackGround.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            if (BackGround.Width.Pixels != PanelWidth)
                Recalculate();
        }
    }
}
