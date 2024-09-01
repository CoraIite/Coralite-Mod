using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        internal Dictionary<int, List<MagikeCraftRecipe>> magikeCraftRecipes;
        internal static FrozenDictionary<int, List<MagikeCraftRecipe>> MagikeCraftRecipes;

        private void RegisterMagikeCraft()
        {
            Mod Mod = Coralite.Instance;

            magikeCraftRecipes = [];

            foreach (var magCraft in from mod in ModLoader.Mods
                                     where mod is ICoralite or Coralite
                                     from Type t in AssemblyManager.GetLoadableTypes(mod.Code)//添加魔能合成表
                                     where !t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikeCraftable))
                                     let magCraft = Activator.CreateInstance(t) as IMagikeCraftable
                                     select magCraft)
            {
                magCraft.AddMagikeCraftRecipe();
            }

            foreach (var recipes in magikeCraftRecipes)     //只是简单整理一下
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));

            //使用性能更高的冻结字典
            MagikeCraftRecipes = magikeCraftRecipes.ToFrozenDictionary();
            magikeCraftRecipes = null;
        }

        #region 重塑帮助方法

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
            MagikeCraftRecipe recipe = MagikeCraftRecipe.CreateRecipe(mainItemType, resultItemType, magikeCost, mainStack, resultStack);
            if (conditions != null)
                foreach (var condition in conditions)
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
           => AddRemodelRecipe(ItemType<TMainItem>(), ItemType<TMainItem>(), magikeCost, mainStack, resultStack, conditions);

        #endregion

        public static bool TryGetMagikeCraftRecipes(int selfType, out List<MagikeCraftRecipe> recipes)
        {
            if (MagikeCraftRecipes != null && MagikeCraftRecipes.TryGetValue(selfType, out List<MagikeCraftRecipe> value))
            {
                recipes = value;
                return true;
            }

            recipes = null;
            return false;
        }
    }

    public record class MagikeCraftRecipe
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
        public int antiMagikeCost;

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
        public bool IsAnnihilation => magikeCost == 0 && antiMagikeCost != 0;

        /// <summary>
        /// 检测是否能合成
        /// </summary>
        /// <returns></returns>
        public bool CanCraft(out string fillText)
        {
            fillText = "";

            if (_conditions == null)
                return true;

            foreach (var condition in Conditions)
                if (!condition.Predicate())
                {
                    fillText = condition.Description.Value;
                    return false;
                }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeCraftRecipe CreateRecipe<TMainItem, TResultItem>(int magikeCost, int MainItenStack = 1, int resultItemStack = 1)
            where TMainItem : ModItem where TResultItem : ModItem
        {
            return new MagikeCraftRecipe()
            {
                MainItem = new(ItemType<TMainItem>(), MainItenStack),
                ResultItem = new(ItemType<TResultItem>(), resultItemStack),
                magikeCost = magikeCost
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public static MagikeCraftRecipe CreateRecipe(int mainItemType, int resultItemType, int magikeCost, int MainItenStack = 1, int resultItemStack = 1)
        {
            return new MagikeCraftRecipe()
            {
                MainItem = new(mainItemType, MainItenStack),
                ResultItem = new(resultItemType, resultItemStack),
                magikeCost = magikeCost
            };
        }

        public MagikeCraftRecipe SetMainStack(int mainItemStack)
        {
            MainItem.stack = mainItemStack;
            return this;
        }

        public MagikeCraftRecipe SetAntiMagikeCost(int antiMagikeCost)
        {
            magikeCost = 0;
            this.antiMagikeCost = antiMagikeCost;
            return this;
        }

        public MagikeCraftRecipe SetMagikeCost(int MagikeCost)
        {
            antiMagikeCost = 0;
            magikeCost = MagikeCost;
            return this;
        }

        public MagikeCraftRecipe AddIngredient(int itemID, int stack = 1)
        {
            RequiredItems.Add(new Item(itemID, stack));
            return this;
        }

        public MagikeCraftRecipe AddIngredient<T>(int stack = 1) where T : ModItem
        {
            RequiredItems.Add(new Item(ItemType<T>(), stack));
            return this;
        }

        public MagikeCraftRecipe AddCondition(Condition condition)
        {
            Conditions.Add(condition);
            return this;
        }

        public void Register()
        {
            Dictionary<int, List<MagikeCraftRecipe>> MagikeCraftRecipes = GetInstance<MagikeSystem>().magikeCraftRecipes;
            if (MagikeCraftRecipes == null)
                return;

            if (MagikeCraftRecipes.TryGetValue(MainItem.type, out List<MagikeCraftRecipe> value))
                value.Add(this);
            else
                MagikeCraftRecipes.Add(MainItem.type, [this]);

            AddVanillaRecipe();
        }

        /// <summary>
        /// 在注册的同时新建一个拥有相同主物品类型和堆叠数的合成表
        /// </summary>
        /// <param name="resultItemType"></param>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public MagikeCraftRecipe RegisterNew(int resultItemType, int magikeCost, int resultItemStack = 1)
        {
            Register();
            return CreateRecipe(MainItem.type, resultItemType, magikeCost, MainItem.stack, resultItemStack);
        }

        /// <summary>
        /// 在注册的同时新建一个拥有相同主物品类型和堆叠数的合成表
        /// </summary>
        /// <typeparam name="TResultItem"></typeparam>
        /// <param name="magikeCost"></param>
        /// <param name="resultItemStack"></param>
        /// <returns></returns>
        public MagikeCraftRecipe RegisterNew<TResultItem>(int magikeCost, int resultItemStack = 1)
            where TResultItem : ModItem
        {
            Register();
            return CreateRecipe(MainItem.type, ItemType<TResultItem>(), magikeCost, MainItem.stack, resultItemStack);
        }

        private void AddVanillaRecipe()
        {
            Recipe recipe = Recipe.Create(ResultItem.type, ResultItem.stack);
            if (magikeCost > 0)
                recipe.AddIngredient<MagikeSymbol>(magikeCost);

            recipe.AddIngredient(MainItem.type, MainItem.stack);

            if (_items != null)
                foreach (var item in RequiredItems)
                    recipe.AddIngredient(item.type, item.stack);
            if (_conditions != null)
                recipe.AddCondition(Conditions);

            recipe.DisableDecraft();
            recipe.Register();
        }
    }

    public class MagikeSymbol : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.maxStack = int.MaxValue;
            Item.rare = RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "Coralite: MagikeNeed", "魔能需求量：" + Item.stack)
            {
                OverrideColor = Coralite.MagicCrystalPink
            };

            tooltips.Add(line);
        }
    }
}
