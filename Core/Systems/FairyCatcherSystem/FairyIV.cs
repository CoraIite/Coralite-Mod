using Coralite.Helpers;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的个体值，内部仅存储各种具体数值
    /// </summary>
    public struct FairyIV
    {
        /// <summary> 生命值上限 </summary>
        public int LifeMax { get; private set; }
        /// <summary> 攻击 </summary>
        public int Damage { get; private set; }
        /// <summary> 防御 </summary>
        public int Defence { get; private set; }
        /// <summary> 速度 </summary>
        public int Speed { get; private set; }
        /// <summary> 技能等级 </summary>
        public int SkillLevel { get; private set; }
        /// <summary> 耐力，决定单次射出能够使用多少次技能 </summary>
        public int Stamina { get; private set; }
        /// <summary> 没什么大用的尺寸属性 </summary>
        public int Scale { get; private set; }

        /// <summary> 
        /// 生命值上限等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short LifeMaxLevel { get; private set; }

        /// <summary> 
        /// 伤害等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short DamageLevel { get; private set; }

        /// <summary> 
        /// 防御等级<br></br>
        /// 使用<see cref=""/>获取对应本地化名称
        /// </summary>
        public short DefenceLevel { get; private set; }


        public static FairyIV GetFairyIV(int fairyType, FairyCatcherPlayer player)
        {
            if (!FairySystem.fairyDatas.TryGetValue(fairyType, out FairyData data))
                return default(FairyIV);

            FairyIV iv = new FairyIV();

            //生命值上限
            GetLifeMaxIV(player, data, ref iv);
            //伤害
            GetDamageIV(player, data, ref iv);
            //防御
            GetDefenceIV(player, data, ref iv);


            return iv;
        }

        private static void GetLifeMaxIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float lifeMaxLevel = player.LifeMaxRand.RandValue;
            iv.LifeMaxLevel = (short)lifeMaxLevel;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)lifeMaxLevel > data.LifeMaxData.Count - 1)
                iv.LifeMax = (int)Helper.Lerp(
                    data.LifeMaxData[^1],
                    data.OverLifeMax,
                    (lifeMaxLevel - data.LifeMaxData.Count - 1) / 100);
        }

        private static void GetDamageIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float lifeMaxLevel = player.DamageRand.RandValue;
            iv.DamageLevel = (short)lifeMaxLevel;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)lifeMaxLevel > data.DamageData.Count - 1)
                iv.LifeMax = (int)Helper.Lerp(
                    data.DamageData[^1],
                    data.OverDamage,
                    (lifeMaxLevel - data.DamageData.Count - 1) / 100);
        }

        private static void GetDefenceIV(FairyCatcherPlayer player, FairyData data, ref FairyIV iv)
        {
            float lifeMaxLevel = player.DefenceRand.RandValue;
            iv.DefenceLevel = (short)lifeMaxLevel;
            //数值高于永恒，从永恒到最大值之间缩放
            if ((int)lifeMaxLevel > data.DefenceData.Count - 1)
                iv.Defence = (int)Helper.Lerp(
                    data.DefenceData[^1],
                    data.OverDefence,
                    (lifeMaxLevel - data.DefenceData.Count - 1) / 100);
        }
    }
}
