using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class CraftAltar : MagikeFactory, IUpgradeable, IUIShowable
    {
        public MagikeCraftRecipe ChosenResipe { get; set; }

        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Work()
        {
            if (ChosenResipe == null)
                return;

            //检测魔能量是否足够
            MagikeCraftAttempt attempt = new MagikeCraftAttempt();
            ChosenResipe.CanCraft_CheckMagike(Entity.GetMagikeContainer().Magike, ref attempt);
            if (!attempt.Success)
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = attempt.OutputText(),
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Helper.GetMagikeTileCenter((Entity as MagikeTileEntity).Position) - (Vector2.UnitY * 32));
                return;
            }

            //先减少魔能
            Entity.GetMagikeContainer().ReduceMagike(ChosenResipe.magikeCost);

            if (!WorkCheck_CostMainItem())//消耗主要物品与次要物品
                return;

            if (ChosenResipe.RequiredItems != null)
                if (!WorkCheck_CostOtherItem())
                    return;

            //生成物品并放入
            if (Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                container.AddItem(ChosenResipe.ResultItem.type, ChosenResipe.ResultItem.stack);
        }

        /// <summary>
        /// 消耗主要物品
        /// </summary>
        /// <returns></returns>
        private bool WorkCheck_CostMainItem()
        {
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return false;

            int count = ChosenResipe.MainItem.stack;

            foreach (var item in container.Items)
            {
                if (item.IsAir)
                    continue;

                if (item.type != ChosenResipe.MainItem.type)
                    continue;

                int cost = Math.Min(item.stack, count);
                item.stack -= cost;

                if (item.stack < 1)
                    item.TurnToAir();

                count -= cost;
                if (count == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 消耗其他物品
        /// </summary>
        /// <returns></returns>
        private bool WorkCheck_CostOtherItem()
        {
            //获取物品容器
            if (!Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender linerSender))
                return false;

            List<Item> otherItems = FillOtherItemList(linerSender);

            //挨个消耗物品
            foreach (var item in ChosenResipe.RequiredItems)
            {
                int howManyNeed = item.stack;

                foreach (var pos in linerSender.Receivers)
                {
                    if (!MagikeHelper.TryGetEntity(pos, out MagikeTileEntity receiverEntity))
                        continue;

                    Item[] tempItems = null;

                    if (receiverEntity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                        tempItems = container2.Items;

                    if (receiverEntity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                        tempItems = container3.Items;

                    foreach (var tempitem in tempItems)
                        if (!tempitem.IsAir && tempitem.type == item.type)
                        {
                            int cost = Math.Min(item.stack, howManyNeed);
                            tempitem.stack -= cost;

                            if (tempitem.stack < 1)
                                tempitem.TurnToAir();

                            howManyNeed -= cost;
                            if (howManyNeed < 1)
                                goto costEnd;
                        }

                    return false;//这块给我写迷糊了，总之就是遍历所有的物品然后挨个消耗
                }

            costEnd:
                ;
            }

            return true;
        }

        /// <summary>
        /// 获取连接的所有物品容器内的物品
        /// </summary>
        /// <param name="linerSender"></param>
        /// <returns></returns>
        private static List<Item> FillOtherItemList(MagikeLinerSender linerSender)
        {
            List<Item> otherItems = [];

            foreach (var pos in linerSender.Receivers)
            {
                if (!MagikeHelper.TryGetEntity(pos, out MagikeTileEntity entity))
                    continue;

                Item[] tempItems = null;

                if (entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                    tempItems = container2.Items;

                if (entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                    tempItems = container3.Items;

                if (tempItems == null)
                    continue;

                //添加物品
                foreach (var tempitem in tempItems)
                    if (!tempitem.IsAir)
                        otherItems.Add(tempitem);
            }

            return otherItems;
        }

        #region 检测能否开始工作

        public override bool CanActivated_SpecialCheck(out string text)
        {
            //获取物品容器
            if (!GetItems(out Item[] items, out Dictionary<int, int> otherItems))
            {
                text = MagikeSystem.Error.Value;
                return false;
            }

            FrozenDictionary<int, int> otherItems2 = otherItems.ToFrozenDictionary();

            if (ChosenResipe != null && !CheckCanCraft_FindRecipe(items, otherItems2, out text))//寻找合成表
                return false;

            //检测物品是否能够合成
            if (!ChosenResipe.CanCraft(items, otherItems2, Entity.GetMagikeContainer().Magike, out text))
                return false;

            return true;
        }

        /// <summary>
        /// 获取所有的物品
        /// </summary>
        /// <param name="mainItems"></param>
        /// <param name="otherItems"></param>
        /// <returns></returns>
        public bool GetItems(out Item[] mainItems, out Dictionary<int, int> otherItems)
        {
            mainItems = null;
            otherItems = null;

            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container)
                || Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender linerSender))
            {
                return false;
            }

            mainItems = container.Items;
            otherItems = FillOtherItemDict(linerSender);

            return true;
        }

        /// <summary>
        /// 获取连接的所有物品容器内的物品
        /// </summary>
        /// <param name="linerSender"></param>
        /// <returns></returns>
        public static Dictionary<int, int> FillOtherItemDict(MagikeLinerSender linerSender)
        {
            Dictionary<int, int> otherItems = [];

            foreach (var pos in linerSender.Receivers)
            {
                if (!MagikeHelper.TryGetEntity(pos, out MagikeTileEntity entity))
                    continue;

                Item[] tempItems = null;

                if (entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                    tempItems = container2.Items;

                if (entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                    tempItems = container3.Items;

                if (tempItems == null)
                    continue;

                //添加物品
                foreach (var tempitem in tempItems)
                    if (!tempitem.IsAir)
                    {
                        if (otherItems.TryGetValue(tempitem.type, out int current))
                            otherItems[tempitem.type] = tempitem.stack + current;
                        else
                            otherItems.Add(tempitem.type, tempitem.stack);
                    }
            }

            return otherItems;
        }

        /// <summary>
        /// 寻找当前大概能合成的合成表，仅检测物品类型，不检测物品数量和魔能等
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool CheckCanCraft_FindRecipe(Item[] mainItems, IDictionary<int, int> otherItems, out string text)
        {
            text = "";
            List<MagikeCraftRecipe> remodelRecipes = [];
            MagikeCraftRecipe polymerizeRecipe = null;
            float matchPercent = 0;

            foreach (Item mainItem in mainItems)
            {
                if (mainItem.IsAir || !MagikeSystem.TryGetMagikeCraftRecipes(mainItem.type, out List<MagikeCraftRecipe> recipes))
                    continue;

                foreach (var recipe in recipes)
                {
                    if (recipe.IsAnnihilation)
                        continue;

                    if (recipe.RequiredItems == null)
                    {
                        remodelRecipes.Add(recipe);
                        continue;
                    }

                    int matchCount = 0;
                    int all = recipe.RequiredItems.Count;

                    foreach (var requiredItem in recipe.RequiredItems)//检测合成表中的物品与当前物品的匹配程度
                        if (otherItems.ContainsKey(requiredItem.type))
                            matchCount++;

                    if (matchCount / (float)all > matchPercent)//如果匹配程度高于当前的就替换当前的
                        polymerizeRecipe = recipe;
                }
            }

            if (remodelRecipes.Count > 0)
            {
                ChosenResipe = remodelRecipes.MinBy(r => r.magikeCost);
                if (ChosenResipe != null)
                    return true;
            }

            if (polymerizeRecipe != null)
            {
                ChosenResipe = polymerizeRecipe;
                return true;
            }

            text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.NoCraftRecipe);
            return false;
        }

        #endregion

        #region 开始工作部分

        public override void StarkWork()
        {
            base.StarkWork();

            Point16 pos = (Entity as MagikeTileEntity).Position;

            if (Helper.OnScreen(pos.ToWorldCoordinates(), new Vector2(16 * 20)))
            {
                //在视野内生成特殊合成粒子

            }
        }

        #endregion

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.CraftAltarName, parent);

            float top = title.Height.Pixels + 8;

            //显示所有物品格子与选择的合成表
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            if (!Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer getOnlyContainer))
                return;

            UIList list = [];

            for (int i = 0; i < container.Items.Length; i++)
            {
                ItemContainerSlot slot = new(container, i);
                list.Add(slot);
            }

            list.Add(new CraftArrow(this));

            for (int i = 0; i < getOnlyContainer.Items.Length; i++)
            {
                GetOnlyItemSlot slot = new(getOnlyContainer, i);
                list.Add(slot);
            }

            list.QuickInvisibleScrollbar();
            list.SetTopLeft(top, 0);

            parent.Append(list);

            float left = 60f;
            AddButtons(parent, left, ref top); //展开与关闭的按钮
            AddController(parent, this, left, ref top);
            AddRecipeShow(parent, left, top);
        }

        public static void AddButtons(UIElement parent, float left, ref float top)
        {
            //CraftSelectButton selectButton = new();
            CraftShowButton showButton = new CraftShowButton();

            //selectButton.SetTopLeft(left, top);
            showButton.SetTopLeft(left /*+ selectButton.Width.Pixels*/, top);

            //parent.Append(selectButton);
            parent.Append(showButton);

            top += showButton.Height.Pixels;
        }

        public static void AddController(UIElement parent,CraftAltar altar,float left,ref float top)
        {
            CraftController controller = new CraftController(altar);
            controller.SetSize(-left, 12, 1, 0);
            controller.SetTopLeft(top, left);

            CraftController.Reset();
            parent.Append(controller);

            top += controller.Height.Pixels;
        }

        public static void AddRecipeShow(UIElement parent ,float left ,float top)
        {
            switch (CraftShowButton.CurrentShowStyle)
            {
                case CraftShowButton.ShowStyle.VerticleLine:
                    {
                        UIList list = new UIList();
                        list.SetTopLeft(top, left);
                        list.SetSize(-left, -top, 1, 1);
                        list.QuickInvisibleScrollbar();

                        foreach (var recipe in CraftController.Recipes)
                            list.Add(new CraftBar(recipe));

                        parent.Append(list);
                    }
                    break;
                default:
                case CraftShowButton.ShowStyle.Grid:
                    {
                        UIGrid grid= new UIGrid();
                        grid.SetTopLeft(top,left );
                        grid.SetSize(-left, -top, 1, 1);

                        foreach (var recipe in CraftController.Recipes)
                            grid.Add(new CraftSlot(recipe, CraftSlot.SlotType.ResultItem));

                        parent.Append(grid);
                    }
                    break;
            }
        }

        #endregion
    }

    public class CraftArrow : UIElement
    {
        private CraftAltar _altar;
        private static Item _voidItem = new();

        public CraftArrow(CraftAltar altar)
        {
            Texture2D tex = MagikeSystem.MagikeContainerBar.Value;
            Vector2 size = tex.Frame(1, 2).Size();

            Width.Set(size.X + 10, 0);
            Height.Set(size.Y, 0);

            _altar = altar;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.MagikeContainerBar.Value;

            CalculatedStyle Dimensions = GetDimensions();

            Rectangle frame = tex.Frame(1, 2);

            var pos = Dimensions.Position();
            var center = Dimensions.Center();

            spriteBatch.Draw(tex, center, frame, Color.White, 0, frame.Size() / 2, 1, 0, 0);

            if (_altar.IsWorking)//工作中就只显示百分比，不然就显示合成表
            {
                float percent = 1 - (float)_altar.Timer / _altar.WorkTime;
                string percentText = MathF.Round(100 * percent, 1) + "%";

                frame = new Rectangle(0, tex.Height / 2, tex.Width, (int)(tex.Height / 2 * percent));

                spriteBatch.Draw(tex, pos, frame, Color.White, 0, Vector2.Zero, 1, 0, 0);

                Utils.DrawBorderString(spriteBatch, percentText, center, Color.White, 0.75f, anchorx: 0.5f, anchory: 0.5f);
            }
            else
            {
                Item i = _voidItem;
                if (_altar.ChosenResipe != null)
                {
                    i = _altar.ChosenResipe.ResultItem;
                }

                Vector2 position = pos + new Vector2(0, 12) * Main.inventoryScale;
                ItemSlot.Draw(spriteBatch, ref i, ItemSlot.Context.ShopItem, position, Coralite.MagicCrystalPink);
            }
        }
    }

    //public class CraftSelectButton : UIElement
    //{
    //    public static SelectStyle CurrentSelectStyle;

    //    private float _scale = 1f;

    //    public CraftSelectButton()
    //    {
    //        Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

    //        var frameBox = mainTex.Frame(2, 1);
    //        this.SetSize(frameBox.Width+6, frameBox.Height+6);
    //    }

    //    /// <summary>
    //    /// 魔能合成UI的显示合成表的筛选
    //    /// </summary>
    //    public enum SelectStyle
    //    {
    //        All,
    //        CanCraft
    //    }

    //    public override void MouseOver(UIMouseEvent evt)
    //    {
    //        base.MouseOver(evt);
    //        Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
    //    }

    //    public override void LeftClick(UIMouseEvent evt)
    //    {
    //        base.LeftClick(evt);

    //        CurrentSelectStyle = CurrentSelectStyle switch
    //        {
    //            SelectStyle.CanCraft => SelectStyle.All,
    //            _ => SelectStyle.CanCraft
    //        };

    //        Helper.PlayPitched("UI/Tick", 0.4f, 0);
    //        UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
    //    }

    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

    //        var dimensions = GetDimensions();

    //        if (IsMouseHovering)
    //        {
    //            _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
    //        }
    //        else
    //            _scale = Helper.Lerp(_scale, 1f, 0.2f);

    //        var framebox = mainTex.Frame(2, 1, (int)CurrentSelectStyle);
    //        spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2,_scale, 0, 0);
    //    }
    //}

    public class CraftShowButton : UIElement
    {
        public static ShowStyle CurrentShowStyle;

        private float _scale = 1f;

        public CraftShowButton()
        {
            Texture2D mainTex = MagikeSystem.CraftShowButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);
        }

        /// <summary>
        /// 魔能合成表UI的显示方式
        /// </summary>
        public enum ShowStyle
        {
            VerticleLine,
            Grid
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            CurrentShowStyle = CurrentShowStyle switch
            {
                ShowStyle.VerticleLine => ShowStyle.Grid,
                _ => ShowStyle.VerticleLine
            };

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.CraftShowButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, (int)CurrentShowStyle);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

    /// <summary>
    /// 魔能合成UI的中控类，兼任绘制水平横条的任务
    /// </summary>
    public class CraftController : UIElement
    {
        /// <summary>
        /// 当前显示的所有合成表
        /// </summary>
        public static List<MagikeCraftRecipe> Recipes = [];
        /// <summary>
        /// 当前的主要物品类型
        /// </summary>
        public static List<int> CurrentItemTypes = [];

        public static Dictionary<int, int> OtherItemTypes;

        public static CraftAltar altar;

        public CraftController(CraftAltar altar)
        {
            CraftController.altar = altar;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container)
                || !altar.Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender sender))
            {
                return;
            }

            Item[] items = container.Items;

            List<int> record = [];

            foreach (var item in items)
            {
                if (!record.Contains(item.type))
                    record.Add(item.type);

                if (CurrentItemTypes.Contains(item.type))
                    continue;

              UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
                return;
            }

            if (record.Count != CurrentItemTypes.Count)
                UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();

            OtherItemTypes = CraftAltar.FillOtherItemDict(sender);
        }

        /// <summary>
        /// 刷新现有的合成表
        /// </summary>
        public static void Reset()
        {
            if (!altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            Recipes.Clear();

            foreach (var item in container.Items)
                if (MagikeSystem.TryGetMagikeCraftRecipes(item.type, out List<MagikeCraftRecipe> recipes))
                    foreach (var recipe in recipes)
                        Recipes.Add(recipe);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D line = TextureAssets.FishingLine.Value;

            var style = GetDimensions();
            Rectangle frame = new(0, 0, line.Width, (int)style.Width);

            spriteBatch.Draw(line, style.Center(), frame, Color.White, 1.57f, frame.Size() / 2, 1, 0, 0);
        }
    }

    //public class CanCraftShow : UIElement
    //{
    //    private MagikeCraftRecipe recipe;
    //    private bool canCraft;
    //    private string FailText;

    //    public CanCraftShow(MagikeCraftRecipe recipe)
    //    {
    //        this.recipe = recipe;

    //        this.SetSize(44, 52);
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);

    //        if (CraftController.altar != null
    //            && CraftController.altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
    //        {
    //            canCraft = recipe.CanCraft(items, otherItems, CraftController.altar.Entity.GetMagikeContainer().Magike, out FailText);
    //        }
    //    }

    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        Texture2D mainTex = MagikeSystem.CanCraftShow.Value;

    //        float scale = 1;

    //        if (IsMouseHovering)
    //        {
    //            scale = 1.2f;
    //            if (canCraft)//显示鼠标信息
    //            {

    //            }
    //            else
    //            {

    //            }
    //        }

    //        Vector2 center = GetDimensions().Center();
    //        var frameBox = mainTex.Frame(2, 1, canCraft ? 0 : 1);
    //        spriteBatch.Draw(mainTex, center, frameBox, Color.White, 0, frameBox.Size() / 2, scale, 0, 0);
    //    }
    //}

    /// <summary>
    /// 魔能合成表的条形界面
    /// </summary>
    public class CraftBar : UIPanel
    {
        private UIGrid grid = new UIGrid();

        public CraftBar(MagikeCraftRecipe recipe)
        {
            this.SetSize(0, 52, 1);

            grid.SetSize(1000, 0, 0, 1);
            //grid.Add(new CanCraftShow(recipe));

            grid.Add(new CraftSlot(recipe, CraftSlot.SlotType.ResultItem));
            grid.Add(new UIVerticalLine());
            if (recipe.RequiredItems.Count > 0)
                for (int i = 0; i < recipe.RequiredItems.Count; i++)
                {
                    grid.Add(new CraftSlot(recipe, CraftSlot.SlotType.RequiredItem, i));
                }
        }
    }

    public class CraftSlot:UIElement
    {
        private Item showItem;
        private MagikeCraftRecipe recipe;
        private bool canCraft;
        private string FailText;
        private SlotType slotType;

        private float _scale;

        public enum SlotType
        {
            ResultItem,
            RequiredItem
        }

        public CraftSlot(MagikeCraftRecipe recipe,SlotType slotType,int requiredIndex=0)
        {
            this.recipe= recipe;
            this.slotType= slotType;
            this.SetSize(60, 60);

            showItem = slotType switch
            {
                SlotType.RequiredItem => recipe.RequiredItems[requiredIndex].Clone(),
                _ => recipe.ResultItem.Clone(),
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (slotType)
            {
                case SlotType.ResultItem://对于最后的物品直接计算是否能合成
                    if (CraftController.altar != null
                        && CraftController.altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
                    {
                        canCraft = recipe.CanCraft(items, otherItems, CraftController.altar.Entity.GetMagikeContainer().Magike, out FailText);
                    }
                    break;
                case SlotType.RequiredItem://其他物品只需要判断一下有没有足够的就行
                    if (CraftController.OtherItemTypes.TryGetValue(showItem.type,out int stack))
                        canCraft = stack >= showItem.stack;
                    else
                    {
                        canCraft = false;
                        FailText = MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.OtherItemNotEnough].Format(showItem.Name, showItem.stack-stack);
                    }

                    break;
                default:
                    break;
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item inv2 = showItem;

            int context = canCraft ? ItemSlot.Context.InventoryItem : ItemSlot.Context.TrashItem;

            if (IsMouseHovering)
            {
                if (canCraft)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    ItemSlot.OverrideHover(ref inv2, context);
                    ItemSlot.MouseHover(ref inv2, context);
                }
                else
                {
                    UICommon.TooltipMouseText(FailText);
                }

                _scale = Helper.Lerp(_scale, 1.1f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv2, context, position, Coralite.MagicCrystalPink);

            Main.inventoryScale = scale;
        }
    }
}
