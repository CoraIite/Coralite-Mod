using Coralite.Content.Raritys;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem, ILocalizedModType
    {
        /// <summary> 魔能重塑的合成表 </summary>
        internal static Dictionary<int, List<RemodelRecipe>> remodelRecipes = new();

        private void RegisterRemodel()
        {
            Mod Mod = Coralite.Instance;
            remodelRecipes = new Dictionary<int, List<RemodelRecipe>>();

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))  //添加魔能重塑合成表
            {
                if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikeRemodelable)))
                {
                    IMagikeRemodelable magRemodel = Activator.CreateInstance(t) as IMagikeRemodelable;
                    magRemodel.AddMagikeRemodelRecipe();
                }
            }

            //跨模组添加重塑合成表
            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite)
                    foreach (Type t in AssemblyManager.GetLoadableTypes(mod.Code))  //添加魔能重塑合成表
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IMagikeRemodelable)))
                        {
                            IMagikeRemodelable magRemodel = Activator.CreateInstance(t) as IMagikeRemodelable;
                            magRemodel.AddMagikeRemodelRecipe();
                        }
                    }

            foreach (var recipes in remodelRecipes)     //只是简单整理一下
            {
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));
            }

            foreach (var list in remodelRecipes)
            {
                foreach (var remodel in list.Value)
                {
                    Recipe recipe = Recipe.Create(remodel.resultItem.type, remodel.resultItem.stack);
                    recipe.AddIngredient(remodel.selfType, remodel.selfRequiredNumber);
                    recipe.AddIngredient<MagikeSymbol>(remodel.magikeCost);
                    recipe.AddDecraftCondition(this.GetLocalization("DecraftCondition"), () => false);
                    recipe.Register();
                }
            }

        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <param name="selfType">自身type</param>
        /// <param name="selfStack">自身需求数量</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="resultItem">重塑成的物品</param>
        /// <param name="condition"></param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(int selfType, int magikeCost, Item resultItem, int selfStack = 1, IMagikeCraftCondition condition = null)
        {
            if (resultItem.TryGetGlobalItem(out MagikeItem magikeItem))
            {
                magikeItem.magike_CraftRequired = magikeCost;
                magikeItem.stack_CraftRequired = selfStack;
                magikeItem.condition = condition;
            }

            RemodelRecipe recipe = new(selfType, selfStack, magikeCost, resultItem, condition);

            if (remodelRecipes == null)
                throw new Exception("合成表为null!");

            if (remodelRecipes.ContainsKey(selfType))
                remodelRecipes[selfType].Add(recipe);
            else
                remodelRecipes.Add(selfType, new List<RemodelRecipe> { recipe });
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是原版物品
        /// </remarks>
        /// <param name="selfType">自身type</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="remodelType">重塑成的物品type</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(int selfType, int magikeCost, int remodelType, int resultStack = 1, int selfStack = 1, IMagikeCraftCondition condition = null)
        {
            AddRemodelRecipe(selfType, magikeCost, new Item(remodelType, resultStack), selfStack, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身为模组物品，对方是原版物品
        /// </remarks>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="remodelType">重塑成的物品type</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TSelf>(int magikeCost, int remodelType, int resultStack = 1, int selfStack = 1, IMagikeCraftCondition condition = null) where TSelf : ModItem
        {
            AddRemodelRecipe(ModContent.ItemType<TSelf>(), magikeCost, new Item(remodelType, resultStack), selfStack, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身为原版物品，对方是模组物品
        /// </remarks>
        /// <param name="selfType">自身type</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TRemodel>(float justToNotRepeat, int selfType, int magikeCost, int resultStack = 1, int selfStack = 1, IMagikeCraftCondition condition = null) where TRemodel : ModItem
        {
            AddRemodelRecipe(selfType, magikeCost, new Item(ModContent.ItemType<TRemodel>(), resultStack), selfStack, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是模组物品
        /// </remarks>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="resultStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TSelf, TRemodel>(int magikeCost, int resultStack = 1, int selfStack = 1, IMagikeCraftCondition condition = null)
            where TSelf : ModItem
            where TRemodel : ModItem
        {
            AddRemodelRecipe(ModContent.ItemType<TSelf>(), magikeCost, new Item(ModContent.ItemType<TRemodel>(), resultStack), selfStack, condition);
        }

        /// <summary>
        /// 直接添加，需要自行准备实例
        /// </summary>
        /// <param name="recipe"></param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(RemodelRecipe recipe)
        {
            if (recipe.resultItem != null && recipe.resultItem.TryGetGlobalItem(out MagikeItem magikeItem))
            {
                magikeItem.magike_CraftRequired = recipe.magikeCost;
                magikeItem.stack_CraftRequired = recipe.selfRequiredNumber;
                magikeItem.condition = recipe.condition;
            }

            if (remodelRecipes == null)
                throw new Exception("合成表为null!");

            if (remodelRecipes.ContainsKey(recipe.selfType))
                remodelRecipes[recipe.selfType].Add(recipe);
            else
                remodelRecipes.Add(recipe.selfType, new List<RemodelRecipe> { recipe });
        }

        /// <summary>
        /// 尝试获取合成表组
        /// </summary>
        /// <param name="selfType"></param>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public static bool TryGetRemodelRecipes(int selfType, out List<RemodelRecipe> recipes)
        {
            if (remodelRecipes != null && remodelRecipes.TryGetValue(selfType, out List<RemodelRecipe> value))
            {
                recipes = value;
                return true;
            }

            recipes = null;
            return false;
        }
    }

    public class RemodelRecipe
    {
        /// <summary> 自身的类型 </summary>
        public int selfType;
        /// <summary> 自身需求的数量 </summary>
        public int selfRequiredNumber;
        /// <summary> 消耗魔能多少 </summary>
        public int magikeCost;
        public Item selfItem;
        /// <summary> 重塑成的物品 </summary>
        public Item resultItem;
        /// <summary> 重塑条件，为null的话就代表无条件 </summary>
        public IMagikeCraftCondition condition;

        /// <summary>
        /// 在重塑完成时调用，第一个参数为原本的物品，第二个参数为重塑成的物品
        /// </summary>
        public Action<Item, Item> onRemodel;

        public RemodelRecipe(int selfType, int selfRequiredNumber, int magikeCost, Item resultItem, IMagikeCraftCondition condition = null)
        {
            this.selfType = selfType;
            this.selfRequiredNumber = selfRequiredNumber;
            selfItem = new Item(selfType, selfRequiredNumber);
            this.magikeCost = magikeCost;
            this.resultItem = resultItem;
            this.condition = condition;
        }

        public bool CanRemodel(Item selfItem, int magike, int itemType, int stack)
        {
            bool checkCondition = condition is null || condition.CanCraft(selfItem);
            return magike >= magikeCost && itemType == selfType && stack >= selfRequiredNumber && checkCondition;
        }

        public static RemodelRecipe CreateRemodelRecipe(int selfType = 0, int selfRequiredNumber = 1, int magikeCost = 1, Item resultItem = null, IMagikeCraftCondition condition = null)
        {
            return new RemodelRecipe(selfType, selfRequiredNumber, magikeCost, resultItem, condition);
        }

        public RemodelRecipe SetSelfType(int selfType)
        {
            this.selfType = selfType;
            return this;
        }

        public RemodelRecipe SetSelfType<T>() where T : ModItem
        {
            selfType = ModContent.ItemType<T>();
            return this;
        }

        public RemodelRecipe SetSelfRequireNumber(int selfRequiredNumber)
        {
            this.selfRequiredNumber = selfRequiredNumber;
            return this;
        }

        public RemodelRecipe SetMagikeCost(int magikeCost)
        {
            this.magikeCost = magikeCost;
            return this;
        }

        public RemodelRecipe SetItemToRemodel(Item itemToRemodel)
        {
            this.resultItem = itemToRemodel;
            return this;
        }

        public RemodelRecipe SetCondition(IMagikeCraftCondition condition)
        {
            this.condition = condition;
            return this;
        }

        /// <summary>
        /// 在重塑完成时调用，第一个参数为原本的物品，第二个参数为重塑成的物品
        /// </summary>
        /// <param name="onRemodel"></param>
        /// <returns></returns>
        public RemodelRecipe SetOnRemodel(Action<Item, Item> onRemodel)
        {
            this.onRemodel = onRemodel;
            return this;
        }

        public void Register()
        {
            MagikeSystem.AddRemodelRecipe(this);
        }
    }

    public interface IMagikeCraftCondition
    {
        bool CanCraft(Item item);
        string Description { get; }
    }

    public class MagikeSymbol : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.maxStack = 999999;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "Coralite: MagikeNeed", "魔能需求量：" + Item.stack);

            if (Item.stack < 300)
                line.OverrideColor = Coralite.MagicCrystalPink;
            else if (Item.stack < 1000)
                line.OverrideColor = Coralite.CrystallineMagikePurple;
            else if (Item.stack < 2_0000)
                line.OverrideColor = Coralite.SplendorMagicoreLightBlue;
            else
                line.OverrideColor = Color.Orange;

            tooltips.Add(line);
        }
    }
}
