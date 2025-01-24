using System;
using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 将目标物品加入抽奖袋中，找到包含必有物品的特殊组后加入进去
        /// </summary>
        /// <param name="itemLoot"></param>
        /// <param name="mustContain">抽奖袋中必须包含的物品</param>
        /// <param name="addItemType">目标物品</param>
        public static void AddToLootBag(this ItemLoot itemLoot, int mustContain, params int[] addItemType)
        {
            foreach (var rule in itemLoot.Get())
            {
                if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(mustContain))
                {
                    var original = oneFromOptionsDrop.dropIds.ToList();
                    foreach (var type in addItemType)
                        original.Add(type);

                    oneFromOptionsDrop.dropIds = [.. original];
                }
            }
        }

        /// <summary>
        /// 将目标物品加入抽奖袋中，找到包含必有物品的特殊组后加入进去
        /// </summary>
        /// <param name="itemLoot"></param>
        /// <param name="mustContain">抽奖袋中必须包含的物品</param>
        public static void AddToLootBag<T>(this ItemLoot itemLoot, int mustContain) where T : ModItem
        {
            foreach (var rule in itemLoot.Get())
            {
                if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(mustContain))
                {
                    var original = oneFromOptionsDrop.dropIds.ToList();
                    original.Add(ModContent.ItemType<T>());
                    oneFromOptionsDrop.dropIds = [.. original];
                }
            }
        }

        /// <summary>
        /// 将目标物品加入抽奖袋中，找到包含必有物品的特殊组后加入进去
        /// </summary>
        /// <param name="itemLoot"></param>
        /// <param name="mustContain">抽奖袋中必须包含的物品</param>
        public static void AddToLootBag<T1,T2>(this ItemLoot itemLoot, int mustContain) where T1 : ModItem where T2:ModItem
        {
            foreach (var rule in itemLoot.Get())
            {
                if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(mustContain))
                {
                    var original = oneFromOptionsDrop.dropIds.ToList();
                    original.Add(ModContent.ItemType<T1>());
                    original.Add(ModContent.ItemType<T2>());
                    oneFromOptionsDrop.dropIds = [.. original];
                }
            }
        }


        /// <summary>
        /// 将目标物品加入抽奖袋中
        /// </summary>
        /// <param name="itemLoot"></param>
        /// <param name="addItemType">目标物品</param>
        public static void AddCommonItem(this ItemLoot itemLoot, int addItemType, int chanceDenominator = 1, int minimumDropped = 1, int maximumDropped = 1)
        {
            itemLoot.Add(ItemDropRule.Common(addItemType, chanceDenominator, minimumDropped, maximumDropped));
        }

        /// <summary>
        /// 将目标物品加入抽奖袋中
        /// </summary>
        /// <param name="itemLoot"></param>
        public static void AddCommonItem<T>(this ItemLoot itemLoot, int chanceDenominator = 1, int minimumDropped = 1, int maximumDropped = 1) where T : ModItem
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<T>(), chanceDenominator, minimumDropped, maximumDropped));
        }



    }
}
