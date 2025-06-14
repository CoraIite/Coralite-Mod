using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public record FairyData
    {
        /// <summary> 生命值上限数据 </summary>
        public List<int> LifeMaxData;
        /// <summary> 伤害数据 </summary>
        public List<int> DamageData;
        /// <summary> 防御数据 </summary>
        public List<int> DefenceData;
        /// <summary> 速度数据 </summary>
        public List<float> SpeedData;
        /// <summary> 技能等级数据 </summary>
        public List<int> SkillLevelData;
        /// <summary> 耐力数据 </summary>
        public List<int> StaminaData;

        /// <summary> 生命值上限最大值 </summary>
        public int OverLifeMax;
        /// <summary> 伤害最大值 </summary>
        public int OverDamage;
        /// <summary> 防御最大值 </summary>
        public int OverDefence;
        /// <summary> 速度最大值 </summary>
        public float OverSpeed;
        /// <summary> 技能等级最大值 </summary>
        public int OverSkillLevel;
        /// <summary> 耐力最大值 </summary>
        public int OverStamina;
    }

    public class FairyIVLevelID
    {
        public const int Weak = 0;
        public const int WeakCommon = 1;
        public const int Common = 2;
        public const int Uncommon = 3;
        public const int Rare = 4;
        public const int Epic = 5;
        public const int Legendary = 6;
        public const int Eternal = 7;
        /// <summary>
        /// 一般不会出现的最大上限
        /// </summary>
        public const int Over = 100;
    }
}
