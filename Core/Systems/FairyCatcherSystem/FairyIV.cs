namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的个体值，内部仅存储各种具体数值
    /// </summary>
    public readonly struct FairyIV
    {
        /// <summary>
        /// 生命值上限
        /// </summary>
        public readonly int LifeMax;
        /// <summary>
        /// 攻击
        /// </summary>
        public readonly int Damage;
        /// <summary>
        /// 防御
        /// </summary>
        public readonly int Defence;
        /// <summary>
        /// 速度
        /// </summary>
        public readonly int Speed;
        /// <summary>
        /// 技能等级
        /// </summary>
        public readonly int SkillLevel;
        /// <summary>
        /// 耐力，决定单次射出能够使用多少次技能
        /// </summary>
        public readonly int Stamina;



    }
}
