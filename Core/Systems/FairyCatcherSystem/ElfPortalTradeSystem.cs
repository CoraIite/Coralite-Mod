using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        internal static Dictionary<int, ElfPortalTrade> ElfPortalTrades = new Dictionary<int, ElfPortalTrade>();

        private static void RegisterElfTrade()
        {
            Mod Mod = Coralite.Instance;
            ElfPortalTrades = new Dictionary<int, ElfPortalTrade>();

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))  //添加魔能重塑合成表
            {
                if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IElfTradable)))
                {
                    IElfTradable trade = Activator.CreateInstance(t) as IElfTradable;
                    trade.AddElfTradable();
                }
            }

            foreach (var mod in ModLoader.Mods)
                if (mod is ICoralite)
                    foreach (Type t in AssemblyManager.GetLoadableTypes(mod.Code))
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IElfTradable)))
                        {
                            IElfTradable trade = Activator.CreateInstance(t) as IElfTradable;
                            trade.AddElfTradable();
                        }
                    }

            foreach (var list in ElfPortalTrades)
            {
                var trade = list.Value;
                Recipe recipe = Recipe.Create(trade.resultType, trade.resultStack);
                recipe.AddIngredient(trade.selfType, trade.selfStack);
                recipe.AddCondition(FairyTradeCondition, () => false);
                recipe.AddCondition(trade.conditions);
                recipe.AddDecraftCondition(FairyTradeCondition, () => false);
                recipe.Register();
            }
        }

        public static void AddRemodelRecipe(int selfType, int resultType, int selfStack = 1, int resultStack = 1, params Condition[] conditions)
        {
            ElfPortalTrade trade = new ElfPortalTrade(selfType, resultType, selfStack, resultStack, conditions);

            if (ElfPortalTrades == null)
                throw new Exception("合成表为null!");

            if (ElfPortalTrades.TryGetValue(selfType, out _))
                ElfPortalTrades[selfType] = trade;
            else
                ElfPortalTrades.Add(selfType, trade);
        }

        public static void AddRemodelRecipe<TSelf, TResult>(int resultStack = 1, int selfStack = 1, params Condition[] conditions)
            where TSelf : ModItem
            where TResult : ModItem
        {
            AddRemodelRecipe(ModContent.ItemType<TSelf>(), ModContent.ItemType<TResult>(), selfStack, resultStack, conditions);
        }

        /// <summary>
        /// 直接添加，需要自行准备实例
        /// </summary>
        /// <param name="trade"></param>
        /// <exception cref="Exception"></exception>
        public static void AddRemodelRecipe(ElfPortalTrade trade)
        {
            if (ElfPortalTrades == null)
                throw new Exception("合成表为null!");

            if (ElfPortalTrades.TryGetValue(trade.selfType, out _))
                ElfPortalTrades[trade.selfType] = trade;
            else
                ElfPortalTrades.Add(trade.selfType,  trade );
        }

        /// <summary>
        /// 尝试获取合成表组
        /// </summary>
        /// <param name="selfType"></param>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public static bool TryGetElfPortalTrades(int selfType, out ElfPortalTrade trade)
        {
            if (ElfPortalTrades != null && ElfPortalTrades.TryGetValue(selfType, out ElfPortalTrade value))
            {
                trade = value;
                return true;
            }

            trade = null;
            return false;
        }
    }

    public class ElfPortalTrade(int selfType, int resultType, int selfStack, int resultStack, params Condition[] condition)
    {
        /// <summary> 自身的类型 </summary>
        public int selfType = selfType;
        /// <summary> 自身需求的数量 </summary>
        public int selfStack = selfStack;

        public int resultType = resultType;
        public int resultStack = resultStack;
        /// <summary> 重塑条件，为null的话就代表无条件 </summary>
        public Condition[] conditions = condition;

        /// <summary>
        /// 在重塑完成时调用，第一个参数为原本的物品，第二个参数为重塑成的物品
        /// </summary>
        public Action<Item, Item> onTrade;

        public bool CanRemodel(Item selfItem, int itemType, int stack)
        {
            bool checkCondition = true;

            if (conditions != null)
                foreach (var condition in conditions)
                {
                    if (!condition.IsMet())
                    {
                        checkCondition = false;
                        break;
                    }
                }
            return itemType == selfType && stack >= selfStack && checkCondition;
        }

        public static ElfPortalTrade CreateElfTrade(int selfType, int resultType)
        {
            return new ElfPortalTrade(selfType, resultType, 1, 1);
        }

        public ElfPortalTrade SetSelfType(int selfType)
        {
            this.selfType = selfType;
            return this;
        }

        public ElfPortalTrade SetSelfType<T>() where T : ModItem
        {
            selfType = ModContent.ItemType<T>();
            return this;
        }

        public ElfPortalTrade SetSelfRequireNumber(int selfRequiredNumber)
        {
            this.selfStack = selfRequiredNumber;
            return this;
        }

        public ElfPortalTrade SetResultType(int resultType)
        {
            this.resultType = resultType;
            return this;
        }

        public ElfPortalTrade SetresultType<T>() where T : ModItem
        {
            resultType = ModContent.ItemType<T>();
            return this;
        }

        public ElfPortalTrade SetresultRequireNumber(int resultStack)
        {
            this.resultStack = resultStack;
            return this;
        }

        public ElfPortalTrade SetCondition(params Condition[] conditions)
        {
            this.conditions = conditions;
            return this;
        }

        /// <summary>
        /// 在重塑完成时调用，第一个参数为原本的物品，第二个参数为重塑成的物品
        /// </summary>
        /// <param name="onRemodel"></param>
        /// <returns></returns>
        public ElfPortalTrade SetOnRemodel(Action<Item, Item> onTrade)
        {
            this.onTrade = onTrade;
            return this;
        }

        public void Register()
        {
            FairySystem.AddRemodelRecipe(this);
        }
    }

    public interface IElfTradable
    {
        void AddElfTradable();
    }
}
