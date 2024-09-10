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
using Terraria.GameContent.UI.Elements;
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
            if (!CheckCanCraft_MagickCheck(out string text))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = text,
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
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container)
                || Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender linerSender))
            {
                text = MagikeSystem.Error.Value;
                return false;
            }

            Item[] items = container.Items;
            FrozenDictionary<int, int> otherItems = FillOtherItemDict(linerSender);

            if (ChosenResipe != null && !CheckCanCraft_FindRecipe(items, otherItems, out text))//寻找合成表
                return false;

            //检测物品是否能够合成
            if (!CheckCanCraft_ItemCheck(items, otherItems, out text))
                return false;

            if (!CheckCanCraft_MagickCheck(out text))
                return false;

            return true;
        }

        /// <summary>
        /// 获取连接的所有物品容器内的物品
        /// </summary>
        /// <param name="linerSender"></param>
        /// <returns></returns>
        private static FrozenDictionary<int, int> FillOtherItemDict(MagikeLinerSender linerSender)
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

            return otherItems.ToFrozenDictionary();
        }

        /// <summary>
        /// 寻找当前大概能合成的合成表，仅检测物品类型，不检测物品数量和魔能等
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool CheckCanCraft_FindRecipe(Item[] mainItems, FrozenDictionary<int, int> otherItems, out string text)
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

        /// <summary>
        /// 对于当前选择的合成表检测是否能够合成，是具体的检测
        /// </summary>
        /// <param name="mainItems"></param>
        /// <param name="otherItems"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool CheckCanCraft_ItemCheck(Item[] mainItems, FrozenDictionary<int, int> otherItems, out string text)
        {
            text = "";

            bool noMainItem = false;//没有主物品
            bool mainItemIncorrect = false;//主物品不对
            bool mainItemNotEnough = false;//主物品数量不够
            bool conditionNotMet = false;//条件不满足
            string conditionFillText = "";
            bool otherItemNotEnough = false;//其他物品不足
            Item lackItem = null;//缺失的物品
            int lackAmount = 0; //缺失的数量

            foreach (var item in mainItems)
            {
                if (item is null || item.IsAir)
                {
                    noMainItem = true;
                    continue;
                }

                if (item.type != ChosenResipe.MainItem.type)
                {
                    mainItemIncorrect = true;
                    continue;
                }

                if (item.stack < ChosenResipe.MainItem.stack)
                {
                    mainItemNotEnough = true;
                    continue;
                }

                if (!ChosenResipe.CanCraft(out conditionFillText))
                {
                    conditionNotMet = true;
                    continue;
                }

                foreach (var requireItem in ChosenResipe.RequiredItems)
                {
                    if (otherItems.TryGetValue(requireItem.type, out int currentStack) && currentStack >= requireItem.stack)
                        continue;
                    else
                    {
                        otherItemNotEnough = true;
                        lackItem = requireItem;
                        lackAmount = requireItem.stack - currentStack;
                        break;
                    }
                }
            }

            #region 未成功的描述部分

            if (noMainItem)
            {
                text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.NoMainItem);
                return false;
            }

            if (mainItemIncorrect)
            {
                text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.MainItemIncorrect);
                return false;
            }

            if (mainItemNotEnough)
            {
                text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.MainItemNotEnough);
                return false;
            }

            if (conditionNotMet)
            {
                text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.ConditionNotMet) + conditionFillText;
                return false;
            }

            if (otherItemNotEnough)
            {
                text = MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.OtherItemNotEnough].Format(lackItem.Name, lackAmount);
                return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 检测当前的魔能是否满足合成表的需要
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool CheckCanCraft_MagickCheck(out string text)
        {
            if (Entity.GetMagikeContainer().Magike < ChosenResipe.magikeCost)
            {
                text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.MagikeNotEnough);
                return false;
            }

            text = "";
            return true;
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
            AddButtons(parent, left, top); //展开与关闭的按钮



        }

        public static void AddButtons(UIElement parent,float left,float top)
        {
            CraftSelectButton selectButton = new();
            CraftShowButton showButton = new CraftShowButton();

            selectButton.SetTopLeft(left, top);
            showButton.SetTopLeft(left + selectButton.Width.Pixels, top);

            parent.Append(selectButton);
            parent.Append(showButton);
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

    public class CraftSelectButton : UIElement
    {
        public static SelectStyle CurrentSelectStyle;

        private float _scale = 1f;

        public CraftSelectButton()
        {
            Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width+6, frameBox.Height+6);
        }

        /// <summary>
        /// 魔能合成UI的显示合成表的筛选
        /// </summary>
        public enum SelectStyle
        {
            All,
            CanCraft
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            CurrentSelectStyle = CurrentSelectStyle switch
            {
                SelectStyle.CanCraft => SelectStyle.All,
                _ => SelectStyle.CanCraft
            };

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, (int)CurrentSelectStyle);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2,_scale, 0, 0);
        }
    }

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

        private CraftAltar _altar;

        public CraftController(CraftAltar altar)
        {
            _altar= altar;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer,out ItemContainer container))
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

                Reset();
                return;
            }

            if (record.Count != CurrentItemTypes.Count)
                Reset();
        }

        public void Reset()
        {
            if (!_altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            Recipes.Clear();

            foreach (var item in container.Items)
                if (MagikeSystem.TryGetMagikeCraftRecipes(item.type, out List<MagikeCraftRecipe> recipes))
                    foreach (var recipe in recipes)
                        Recipes.Add(recipe);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }

    /// <summary>
    /// 魔能合成表的条形界面
    /// </summary>
    public class CraftBar
    {

    }

    public class CraftSlot
    {

    }
}
