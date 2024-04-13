namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyCatcherPlayer:ModPlayer
    {
        /// <summary>
        /// 仙灵捕捉器指针的大小，用于对于其进行缩放<br></br>
        /// 捕捉器默认大小为4*4，查看<see cref="BaseFairyCatcherProj.GetCursor"/>以了解更多
        /// </summary>
        public StatModifier cursorSizeBonus;


        /// <summary>
        /// 伤害加成
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public float FairyDamageBonus(float damage)
        {
            return damage;
        }

        public override void ResetEffects()
        {
            cursorSizeBonus = new StatModifier();
        }
    }
}
