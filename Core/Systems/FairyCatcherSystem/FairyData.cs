namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public record FairyData
    {
        /// <summary> 生命值上限种族值 </summary>
        public short LifeMaxSSV;
        /// <summary> 伤害种族值 </summary>
        public short DamageSSV;
        /// <summary> 防御种族值 </summary>
        public short DefenceSSV;
        /// <summary> 速度种族值 </summary>
        public short SpeedSSV;
        /// <summary> 技能等级种族值 </summary>
        public short SkillLevelSSV;
        /// <summary> 耐力种族值 </summary>
        public short StaminaSSV;
    }

    public class FairyIVLevelID
    {
        public const int Weak = 5;
        public const int WeakCommon = 10;
        public const int Common = 20;
        public const int Uncommon = 35;
        public const int Rare = 50;
        public const int Epic = 70;
        public const int Legendary = 95;
        public const int Eternal = 125;
        /// <summary>
        /// 最大上限
        /// </summary>
        public const int Over = 150;
    }
}
