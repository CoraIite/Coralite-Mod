using Coralite.Core.Systems.BotanicalSystem;
using Terraria;

namespace Coralite.Helpers
{
    public static partial class BotanicalHelper
    {
        /// <summary>
        /// 杂交并突变
        /// 输入父植物，母植物，催化剂，根据父母植物中的字典来判断是否会产生突变
        /// </summary>
        /// <param name="father">父物品</param>
        /// <param name="mother">母物品</param>
        /// <param name="catalyst">催化剂物品</param>
        /// <param name="sonPlantType">输出的子植物的类型</param>
        /// <returns></returns>
        public static CrossBreedState CrossBreed_Mutant(Item father, Item mother, Item catalyst, out int sonPlantType)
        {
            //一些前置条件的判断，不满足的话直接结束
            if (father.IsAir || mother.IsAir)
            {
                sonPlantType = 0;
                return CrossBreedState.error;
            }

            BotanicalItem bFather = father.GetBotanicalItem();
            BotanicalItem bMother = mother.GetBotanicalItem();
            if (!bFather.botanicalItem || !bMother.botanicalItem)
            {
                sonPlantType = 0;
                return CrossBreedState.error;
            }

            //如果两个植物没有能突变的情况，直接随机父母
            if (bFather.CrossBreedDatas == null && bMother.CrossBreedDatas == null)
            {
                sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                return CrossBreedState.notfound;
            }

            int fatherType = father.type;
            int motherType = mother.type;
            //检测父物品的杂交种类字典
            if (bFather.CrossBreedDatas != null)
            {
                foreach (var item in bFather.CrossBreedDatas)
                {
                    if (item.Key == motherType)
                    {
                        int catalystPower = 0;//催化剂效果
                        if (!catalyst.IsAir)
                            if (catalyst.GetCatalystItem().isCatalyst)
                                catalystPower = catalyst.GetCatalystItem().catalystPower;

                        int percentage = item.Value.Percentage - catalystPower;//这个是突变成功的概率，最小为1
                        if (percentage < 1)
                            percentage = 1;
                        if (Main.rand.NextBool(percentage))
                        {
                            sonPlantType = item.Value.MutantPlantType;
                            return CrossBreedState.success;
                        }
                        else
                        {
                            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                            return CrossBreedState.failure;
                        }
                    }
                }
            }
            //检测母物品的杂交种类字典
            if (bMother.CrossBreedDatas != null)
            {
                foreach (var item in bMother.CrossBreedDatas)
                {
                    if (item.Key == fatherType)
                    {
                        int catalystPower = 0;//催化剂效果
                        if (!catalyst.IsAir)
                            if (catalyst.GetCatalystItem().isCatalyst)
                                catalystPower = catalyst.GetCatalystItem().catalystPower;

                        int percentage = item.Value.Percentage - catalystPower;//这个是突变成功的概率，最小为1
                        if (percentage < 1)
                            percentage = 1;

                        if (Main.rand.NextBool(percentage))
                        {
                            sonPlantType = item.Value.MutantPlantType;
                            return CrossBreedState.success;
                        }
                        else
                        {
                            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                            return CrossBreedState.failure;
                        }
                    }
                }
            }
            
            //遍历2个字典都没能找到能突变的情况的话直接在这里随机返回父母植物类型
            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
            return CrossBreedState.notfound;
        }

        public static Item CrossBreed(Item father,Item mother,Item catalyst,out string text)
        {
            CrossBreedState state = CrossBreed_Mutant(father, mother, catalyst, out int sonPlantType);
            //通过上面一行来判断物品是不是空还有是不是植物
            if (state == CrossBreedState.error)
            {
                text = "发生错误！";
                return new Item();
            }

            Item son = new Item();
            son.SetDefaults(sonPlantType);
            BotanicalItem bFather = father.GetBotanicalItem();
            BotanicalItem bMother = mother.GetBotanicalItem();

            switch (state)
            {
                //未找到突变组合，普通杂交
                case CrossBreedState.notfound:
                    BotanicalItem bSon=son.GetBotanicalItem();
                    bSon.DominantGrowTime = Main.rand.NextBool() ? bFather.DominantGrowTime : bMother.DominantGrowTime;
                    bSon.RecessiveGrowTime = Main.rand.NextBool() ? bFather.RecessiveGrowTime : bMother.RecessiveGrowTime;
                    bSon.DominantLevel = Main.rand.NextBool() ? bFather.DominantLevel : bMother.DominantLevel;
                    bSon.RecessiveLevel = Main.rand.NextBool() ? bFather.RecessiveLevel : bMother.RecessiveGrowTime;
                    text = "杂交成功";
                    break;
                //突变成功，不需要改变它的各种基因
                case CrossBreedState.success:
                    text = "突变成功！";
                    break;
                //突变失败，变成普通杂交
                case CrossBreedState.failure:
                    BotanicalItem bSon2 = son.GetBotanicalItem();
                    bSon2.DominantGrowTime = Main.rand.NextBool() ? bFather.DominantGrowTime : bMother.DominantGrowTime;
                    bSon2.RecessiveGrowTime = Main.rand.NextBool() ? bFather.RecessiveGrowTime : bMother.RecessiveGrowTime;
                    bSon2.DominantLevel = Main.rand.NextBool() ? bFather.DominantLevel : bMother.DominantLevel;
                    bSon2.RecessiveLevel = Main.rand.NextBool() ? bFather.RecessiveLevel : bMother.RecessiveGrowTime;
                    text = "突变失败！";
                    break;
                default: text = ""; break;
            }

            return son;
        }
    }
}
