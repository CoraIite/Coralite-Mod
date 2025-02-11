using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;
using static System.Net.Mime.MediaTypeNames;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        internal static Dictionary<int, List<MagikeRecipe>> MagikeCraftRecipesDic { get; private set; } = [];
        internal static FrozenDictionary<int, List<MagikeRecipe>> MagikeCraftRecipesFrozen { get; private set; }
        internal static List<RemodelRecipeStruct> RemodelRecipeByActions { get; private set; } = [];

        private void RegisterMagikeCraft()
        {
            List<IMagikeCraftable> magikeCraftables = VaultUtils.GetSubclassInstances<IMagikeCraftable>();
            foreach (var mag in magikeCraftables)
            {
                mag.AddMagikeCraftRecipe();
            }

            foreach (var recipeAction in RemodelRecipeByActions)
            {
                AddRemodelRecipe(recipeAction);
            }

            foreach (var recipes in MagikeCraftRecipesDic)//只是简单整理一下
            {
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));
            }
            //使用性能更高的冻结字典
            MagikeCraftRecipesFrozen = MagikeCraftRecipesDic.ToFrozenDictionary();
            MagikeCraftRecipesDic?.Clear();
        }

        public override void OnModUnload()
        {
            MagikeCraftRecipesDic?.Clear();
            RemodelRecipeByActions?.Clear();
        }

        #region 重塑帮助方法

        public struct RemodelRecipeStruct
        {
            public int mainItemType;
            public int resultItemType;
            public int magikeCost;
            public int mainStack = 1;
            public int resultStack = 1;
            public Condition[] conditions;

            public RemodelRecipeStruct()
            {
            }
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是原版物品
        /// </remarks>
        /// <param name="mainItemType">自身type</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="resultItemType">重塑成的物品type</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        public static void AddRemodelRecipe(int mainItemType, int resultItemType, int magikeCost, int mainStack = 1, int resultStack = 1, params Condition[] conditions)
        {
            MagikeRecipe recipe = MagikeRecipe.CreateCraftRecipe(mainItemType, resultItemType, magikeCost, mainStack, resultStack);
            if (conditions != null)
                foreach (var condition in conditions)
                    recipe.AddCondition(condition);

            recipe.Register();
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是原版物品
        /// </remarks>
        public static void AddRemodelRecipe(RemodelRecipeStruct remodelRecipe)
        {
            MagikeRecipe recipe = MagikeRecipe.CreateCraftRecipe(remodelRecipe.mainItemType
                , remodelRecipe.resultItemType, remodelRecipe.magikeCost, remodelRecipe.mainStack, remodelRecipe.resultStack);
            if (remodelRecipe.conditions != null)
                foreach (var condition in remodelRecipe.conditions)
                    recipe.AddCondition(condition);

            recipe.Register();
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是模组物品
        /// </remarks>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        public static void AddRemodelRecipe<TMainItem, TResultItem>(int magikeCost, int mainStack = 1, int resultStack = 1, params Condition[] conditions)
            where TMainItem : ModItem
            where TResultItem : ModItem
           => AddRemodelRecipe(ItemType<TMainItem>(), ItemType<TResultItem>(), magikeCost, mainStack, resultStack, conditions);

        #endregion

        public static bool TryGetMagikeCraftRecipes(int selfType, out List<MagikeRecipe> recipes)
        {
            if (MagikeCraftRecipesFrozen != null && MagikeCraftRecipesFrozen.TryGetValue(selfType, out List<MagikeRecipe> value))
            {
                recipes = value;
                return true;
            }

            recipes = null;
            return false;
        }
    }

    public record class MagikeRecipe
    {
        /// <summary>
        /// 主物品，以此物品为基础进行合成
        /// </summary>
        public required Item MainItem { get; set; }
        /// <summary>
        /// 结果物品，最后产出的物品
        /// </summary>
        public required Item ResultItem { get; set; }

        public int magikeCost;
        //public int antiMagikeCost;

        private List<Condition> _conditions;
        public List<Condition> Conditions
        {
            get
            {
                _conditions ??= [];
                return _conditions;
            }
        }

        private List<Item> _items;
        public List<Item> RequiredItems
        {
            get
            {
                _items ??= [];
                return _items;
            }
        }

        public Action<Item, Item> onCraft;

        /// <summary>
        /// 是湮灭反应（需求的是反魔能而不是魔能）
        /// </summary>
        //public bool IsAnnihilation => magikeCost == 0 && antiMagikeCost != 0;

        /// <summary>
        /// 合成表类型
        /// </summary>
        public RecipeType recipeType = RecipeType.MagikeCraft;

        public enum RecipeType
        {
            /// <summary>
            /// 魔能合成
            /// </summary>
            MagikeCraft,
            /// <summary>
            /// 魔能火山烧矿
            /// </summary>
            MagikeSmelting
        }

        #region 能否合成检测

        /// <summary>
        /// 检测是否能够合成
        /// </summary>
        /// <param name="mainItems"></param>
        /// <param name="otherItems"></param>
        /// <param name="magikeAmount"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CanCraft(IList<Item> mainItems, IDictionary<int, int> otherItems, int magikeAmount, out string text)
        {
            text = "";
            MagikeCraftAttempt attempt = new MagikeCraftAttempt();

            CanCraft_CheckCondition(ref attempt);
            CanCraft_ItemsCheck(mainItems, otherItems, ref attempt);
            //CanCraft_CheckMagike(magikeAmount, ref attempt);

            if (attempt.Success)
                return true;

            text = attempt.OutputText();
            return false;
        }

        /// <summary>
        /// 检测是否能合成，仅检测，不输出失败信息
        /// </summary>
        /// <param name="mainItems"></param>
        /// <param name="otherItems"></param>
        /// <param name="magikeAmount"></param>
        /// <returns></returns>
        public bool CanCraftJustCheck(IList<Item> mainItems, IDictionary<int, int> otherItems, int magikeAmount)
        {
            MagikeCraftAttempt attempt = new MagikeCraftAttempt();

            CanCraft_CheckCondition(ref attempt);
            CanCraft_ItemsCheck(mainItems, otherItems, ref attempt);

            if (attempt.Success)
                return true;

            return false;
        }

        /// <summary>
        /// 检测合成条件
        /// </summary>
        /// <returns></returns>
        public void CanCraft_CheckCondition(ref MagikeCraftAttempt attempt)
        {
            if (_conditions == null)
                return;

            foreach (var condition in Conditions)
                if (!condition.Predicate())
                {
                    attempt.conditionNotMet = true;
                    attempt.conditionFailText = condition.Description.Value;
                }
        }

        public void CanCraft_ItemsCheck(IList<Item> mainItems, IDictionary<int, int> otherItems, ref MagikeCraftAttempt attempt)
        {
            foreach (var item in mainItems)
            {
                if (item is null || item.IsAir)
                {
                    attempt.noMainItem = true;
                    continue;
                }

                if (item.type != MainItem.type)
                {
                    attempt.mainItemIncorrect = true;
                    continue;
                }

                if (item.stack < MainItem.stack)
                {
                    attempt.mainItemNotEnough = true;
                    attempt.lackMainItem = MainItem.Clone();
                    attempt.lackMainAmount = MainItem.stack - item.stack;
                    continue;
                }

                foreach (var requireItem in RequiredItems)
                {
                    if (otherItems.TryGetValue(requireItem.type, out int currentStack) && currentStack >= requireItem.stack)
                        continue;
                    else
                    {
                        attempt.otherItemNotEnough = true;
                        attempt.lackItem = requireItem;
                        attempt.lackAmount = requireItem.stack - currentStack;
                        break;
                    }
                }
            }
        }

        //public void CanCraft_CheckMagike(int magikeAmount, ref MagikeCraftAttempt attempt)
        //{
        //    //if (magikeAmount < 0)//反魔能
        //    //{
        //    //    if (antiMagikeCost == 0 && magikeCost != 0)
        //    //        attempt.magikeNotEnough = true;

        //    //    if (magikeAmount > -1)//反魔能需要反一下
        //    //    {
        //    //        attempt.antimagikeNotEnough = true;
        //    //    }

        //    //    return;
        //    //}

        //    //if (magikeAmount == 0 && antiMagikeCost != 0)
        //    //    attempt.antimagikeNotEnough = true;

        //    if (magikeAmount < 1)
        //    {
        //        attempt.magikeNotEnough = true;
        //        attempt.targetMagike = magikeCost;
        //        attempt.selfMagike = magikeAmount;
        //    }
        //}

        #endregion

        #region 新建魔能合成的合成表部分

        /// <summary>
        /// 创建魔能合成表
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeRecipe CreateCraftRecipe<TMainItem, TResultItem>(int magikeCost, int MainItemStack = 1, int resultItemStack = 1)
            where TMainItem : ModItem where TResultItem : ModItem
        {
            return new MagikeRecipe()
            {
                MainItem = new(ItemType<TMainItem>(), MainItemStack),
                ResultItem = new(ItemType<TResultItem>(), resultItemStack),
                magikeCost = magikeCost
            };
        }

        /// <summary>
        /// 创建魔能合成表
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeRecipe CreateCraftRecipe(int mainItemType, int resultItemType, int magikeCost, int MainItenStack = 1, int resultItemStack = 1)
        {
            return new MagikeRecipe()
            {
                MainItem = new(mainItemType, MainItenStack),
                ResultItem = new(resultItemType, resultItemStack),
                magikeCost = magikeCost
            };
        }

        //public MagikeRecipe SetAntiMagikeCost(int antiMagikeCost)
        //{
        //    magikeCost = 0;
        //    this.antiMagikeCost = antiMagikeCost;
        //    return this;
        //}

        /// <summary>
        /// 在注册的同时新建一个拥有相同主物品类型和堆叠数的合成表
        /// </summary>
        /// <param name="resultItemType"></param>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public MagikeRecipe RegisterNewCraft(int resultItemType, int magikeCost, int resultItemStack = 1)
        {
            Register();
            return CreateCraftRecipe(MainItem.type, resultItemType, magikeCost, MainItem.stack, resultItemStack);
        }

        /// <summary>
        /// 在注册的同时新建一个拥有相同主物品类型和堆叠数的合成表
        /// </summary>
        /// <typeparam name="TResultItem"></typeparam>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public MagikeRecipe RegisterNewCraft<TResultItem>(int magikeCost, int resultItemStack = 1)
            where TResultItem : ModItem
        {
            Register();
            return CreateCraftRecipe(MainItem.type, ItemType<TResultItem>(), magikeCost, MainItem.stack, resultItemStack);
        }

        #endregion

        #region 通用方法

        /// <summary>
        /// 设置主要物品的需求量
        /// </summary>
        /// <param name="mainItemStack"></param>
        /// <returns></returns>
        public MagikeRecipe SetMainStack(int mainItemStack)
        {
            MainItem.stack = mainItemStack;
            return this;
        }

        /// <summary>
        /// 重新设置魔能需求量
        /// </summary>
        /// <param name="MagikeCost"></param>
        /// <returns></returns>
        public MagikeRecipe SetMagikeCost(int MagikeCost)
        {
            //antiMagikeCost = 0;
            magikeCost = MagikeCost;
            return this;
        }

        /// <summary>
        /// 添加次要物品
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public MagikeRecipe AddIngredient(int itemID, int stack = 1)
        {
            RequiredItems.Add(new Item(itemID, stack));
            return this;
        }

        /// <summary>
        /// 添加次要物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <returns></returns>
        public MagikeRecipe AddIngredient<T>(int stack = 1) where T : ModItem
        {
            RequiredItems.Add(new Item(ItemType<T>(), stack));
            return this;
        }

        /// <summary>
        /// 向合成表内添加合成条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MagikeRecipe AddCondition(Condition condition)
        {
            Conditions.Add(condition);
            return this;
        }

        /// <summary>
        /// 向合成表内添加一堆合成条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MagikeRecipe AddConditions(params Condition[] condition)
        {
            Conditions.AddRange(condition);
            return this;
        }

        /// <summary>
        /// 注册合成表
        /// </summary>
        public void Register()
        {
            Dictionary<int, List<MagikeRecipe>> MagikeCraftRecipes = MagikeSystem.MagikeCraftRecipesDic;
            if (MagikeCraftRecipes == null)
                return;

            if (MagikeCraftRecipes.TryGetValue(MainItem.type, out List<MagikeRecipe> value))
                value.Add(this);
            else
                MagikeCraftRecipes.Add(MainItem.type, [this]);

            AddVanillaRecipe();
        }

        /// <summary>
        /// 添加原版合成表用于方便查找
        /// </summary>
        private void AddVanillaRecipe()
        {
            Recipe recipe = Recipe.Create(ResultItem.type, ResultItem.stack);
            if (magikeCost > 0)
                recipe.AddIngredient<SymbolOfMagike>(magikeCost);

            recipe.AddIngredient(MainItem.type, MainItem.stack);

            if (_items != null)
                foreach (var item in RequiredItems)
                    recipe.AddIngredient(item.type, item.stack);

            switch (recipeType)
            {
                case RecipeType.MagikeCraft:
                    recipe.AddCondition(CoraliteConditions.MagikeCraft);
                    break;
                case RecipeType.MagikeSmelting:

                    break;
                default:
                    break;
            }

            if (_conditions != null)
                recipe.AddCondition(Conditions);

            recipe.DisableDecraft();
            recipe.Register();
        }

        #endregion

        public void Save(TagCompound tag)
        {
            tag.Add("ResultItem", ResultItem);
            tag.Add("MainItem", MainItem);
            tag.Add("RecipeType", (int)recipeType);

            tag.Add(nameof(magikeCost), magikeCost);
        }

        public static MagikeRecipe Load(TagCompound tag)
        {
            if (tag.TryGet("ResultItem", out Item resultItem) && tag.TryGet("MainItem", out Item mainItem) && tag.TryGet("RecipeType", out int recipeType)
                && MagikeSystem.TryGetMagikeCraftRecipes(mainItem.type, out List<MagikeRecipe> recipes))
            {
                int magikeCost = tag.GetInt(nameof(magikeCost));
                return recipes.FirstOrDefault(r => r.ResultItem.type == resultItem.type && r.magikeCost == magikeCost && r.recipeType == (RecipeType)recipeType, null);
            }

            return null;
        }
    }

    public struct MagikeCraftAttempt()
    {
        /// <summary>
        /// 没有主物品
        /// </summary>
        public bool noMainItem = false;
        /// <summary>
        /// 主物品不对
        /// </summary>
        public bool mainItemIncorrect = false;
        /// <summary>
        /// 主物品数量不够
        /// </summary>
        public bool mainItemNotEnough = false;
        public Item lackMainItem = null;
        public int lackMainAmount = 0;

        /// <summary>
        /// 条件不满足
        /// </summary>
        public bool conditionNotMet = false;
        public string conditionFailText = "";

        /// <summary>
        /// 魔能不够
        /// </summary>
        //public bool magikeNotEnough = false;
        //public int targetMagike;
        //public int selfMagike;

        /// <summary>
        /// 魔能不够
        /// </summary>
        //public bool antimagikeNotEnough = false;
        /// <summary>
        /// 其他物品不足
        /// </summary>
        public bool otherItemNotEnough = false;
        /// <summary>
        /// 缺失的物品
        /// </summary>
        public Item lackItem = null;
        /// <summary>
        /// 缺失的数量
        /// </summary>
        public int lackAmount = 0;

        public string OutputText()
        {
            string text = "";
            if (noMainItem)
                text = ConcatText(text, MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.NoMainItem));

            if (mainItemIncorrect)
                text = ConcatText(text, MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.MainItemIncorrect));

            if (mainItemNotEnough)
                text = ConcatText(text, MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.MainItemNotEnough].Format(lackMainItem.Name, lackMainAmount));

            if (conditionNotMet)
                text = ConcatText(text, MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.ConditionNotMet) + conditionFailText);

            if (otherItemNotEnough)
                text = ConcatText(text, MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.OtherItemNotEnough].Format(lackItem.Name, lackAmount));

            //if (magikeNotEnough)
            //    text = ConcatText(text, MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.MagikeNotEnough].Format(selfMagike, targetMagike));

            //if (antimagikeNotEnough)
            //    text = ConcatText(text, MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.AntimagikeNotEnough));

            return text;
        }

        private static string ConcatText(string text, string newText)
        {
            if (string.IsNullOrEmpty(text))
                return newText;

            return text + "\n" + newText;
        }

        public readonly bool Success
        {
            get
            {
                return !(noMainItem || mainItemIncorrect || mainItemNotEnough
                    || conditionNotMet /*|| magikeNotEnough*/ || otherItemNotEnough);
            }
        }
    }

    public class SymbolOfMagike : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public static LocalizedText MagikeNeed { get; private set; }

        public override void Load()
        {
            MagikeNeed = this.GetLocalization(nameof(MagikeNeed));
        }

        public override void Unload()
        {
            MagikeNeed = null;
        }

        public override void SetDefaults()
        {
            Item.maxStack = int.MaxValue;
            Item.rare = RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "Coralite: MagikeNeed", MagikeNeed.Value + Item.stack)
            {
                OverrideColor = Coralite.MagicCrystalPink
            };

            tooltips.Add(line);
        }
    }
}
