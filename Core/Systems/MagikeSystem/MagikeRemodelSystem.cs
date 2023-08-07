using Coralite.Content.Raritys;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem,ILocalizedModType
    {
        /// <summary> 魔能重塑的合成表 </summary>
        internal static Dictionary<int, List<RemodelRecipe>> remodelRecipes = new Dictionary<int, List<RemodelRecipe>>();

        public static MagikeSystem Instance { get; private set; }

        public MagikeSystem()
        {
            Instance = this;
        }

        public override void PostAddRecipes()
        {
            if (Main.dedServ)
                return;

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

            foreach (var recipes in remodelRecipes)     //只是简单整理一下
            {
                recipes.Value.Sort((r1, r2) => r1.magikeCost.CompareTo(r2.magikeCost));
            }

            foreach (var list in remodelRecipes)
            {
                foreach (var remodel in list.Value)
                {
                    Recipe recipe = Recipe.Create(remodel.itemToRemodel.type, remodel.itemToRemodel.stack);
                    recipe.AddIngredient(remodel.selfType, remodel.selfRequiredNumber);
                    recipe.AddIngredient<MagikeSymbol>(remodel.magikeCost);
                    recipe.AddDecraftCondition(this.GetLocalization("DecraftCondition"), () => false);
                    recipe.Register();
                }
            }
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            if (remodelRecipes != null)
                remodelRecipes.Clear();
            remodelRecipes = null;
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <param name="selfType">自身type</param>
        /// <param name="selfRequiredNumber">自身需求数量</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="itemToRemodel">重塑成的物品</param>
        /// <param name="condition"></param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(int selfType, int magikeCost, Item itemToRemodel, int selfRequiredNumber = 1, IMagikeRemodelCondition condition = null)
        {
            if (itemToRemodel.TryGetGlobalItem(out MagikeItem magikeItem))
            {
                magikeItem.magikeRemodelRequired = magikeCost;
                magikeItem.stackRemodelRequired = selfRequiredNumber;
                magikeItem.condition = condition;
            }
            RemodelRecipe recipe = new RemodelRecipe(selfType, selfRequiredNumber, magikeCost, itemToRemodel, condition);

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
        /// <param name="remodelStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(int selfType, int magikeCost, int remodelType, int remodelStack = 1, int selfRequiredNumber = 1, IMagikeRemodelCondition condition = null)
        {
            AddRemodelRecipe(selfType, magikeCost, new Item(remodelType, remodelStack), selfRequiredNumber, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身为模组物品，对方是原版物品
        /// </remarks>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="remodelType">重塑成的物品type</param>
        /// <param name="remodelStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TSelf>(int magikeCost, int remodelType, int remodelStack = 1, int selfRequiredNumber = 1, IMagikeRemodelCondition condition = null) where TSelf : ModItem
        {
            AddRemodelRecipe(ModContent.ItemType<TSelf>(), magikeCost, new Item(remodelType, remodelStack), selfRequiredNumber, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身为原版物品，对方是模组物品
        /// </remarks>
        /// <param name="selfType">自身type</param>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="remodelStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TRemodel>(float justToNotRepeat, int selfType, int magikeCost, int remodelStack = 1, int selfRequiredNumber = 1, IMagikeRemodelCondition condition = null) where TRemodel : ModItem
        {
            AddRemodelRecipe(selfType, magikeCost, new Item(ModContent.ItemType<TRemodel>(), remodelStack), selfRequiredNumber, condition);
        }

        /// <summary>
        /// 向重塑合成表字典中添加重塑合成，请实现<see cref="IMagikeRemodelable"/>接口并在其中调用该方法
        /// </summary>
        /// <remarks>
        /// 该重载的理想使用情况：自身和对方都是模组物品
        /// </remarks>
        /// <param name="magikeCost">魔能消耗量</param>
        /// <param name="remodelStack">重塑成的物品数量，默认1</param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe<TSelf, TRemodel>(int magikeCost, int remodelStack = 1, int selfRequiredNumber = 1, IMagikeRemodelCondition condition = null)
            where TSelf : ModItem
            where TRemodel : ModItem
        {
            AddRemodelRecipe(ModContent.ItemType<TSelf>(), magikeCost, new Item(ModContent.ItemType<TRemodel>(), remodelStack), selfRequiredNumber, condition);
        }


        /// <summary>
        /// 尝试获取合成表组
        /// </summary>
        /// <param name="selfType"></param>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public static bool TryGetRemodelRecipes(int selfType,out List<RemodelRecipe> recipes)
        {
            if (remodelRecipes.ContainsKey(selfType))
            {
                recipes = remodelRecipes[selfType];
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
        /// <summary> 重塑成的物品 </summary>
        public Item itemToRemodel;
        /// <summary> 重塑条件，为null的话就代表无条件 </summary>
        public IMagikeRemodelCondition condition;

        public RemodelRecipe(int selfType, int selfRequiredNumber, int magikeCost, Item itemToRemodel, IMagikeRemodelCondition condition = null)
        {
            this.selfType = selfType;
            this.selfRequiredNumber = selfRequiredNumber;
            this.magikeCost = magikeCost;
            this.itemToRemodel = itemToRemodel;
            this.condition = condition;
        }

        public bool CanRemodel(Item selfItem,int magike, int itemType, int stack)
        {
            bool checkCondition = condition is null ? true : condition.CanRemodel(selfItem);
            return magike >= magikeCost && itemType == selfType && stack >= selfRequiredNumber && checkCondition;
        }
    }

    public interface IMagikeRemodelCondition
    {
        bool CanRemodel(Item item);
        string Description { get; }
    }

    public class MagikeSymbol:ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems+Name;

        public override void SetDefaults()
        {
            Item.maxStack = 999999;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override bool OnPickup(Player player) => false;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "Coralite: MagikeNeed", "魔能需求量："+Item.stack);

            if (Item.stack < 300)
                line.OverrideColor = Coralite.Instance.MagicCrystalPink;
            else if (Item.stack < 1000)
                line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
            else if (Item.stack < 2_0000)
                line.OverrideColor = Coralite.Instance.SplendorMagicoreLightBlue;
            else
                line.OverrideColor = Color.Orange;

            tooltips.Add(line);
        }
    }
}
