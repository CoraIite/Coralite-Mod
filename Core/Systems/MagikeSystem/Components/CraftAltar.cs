using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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
            //显示所有物品格子 
            //显示选择的合成表
            //显示所有的合成表
            //展开与关闭的按钮
        }

        #endregion
    }

    public class CraftArrow:UIElement
    {

    }
}
