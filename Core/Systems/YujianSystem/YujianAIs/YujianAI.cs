namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public abstract class YujianAI
    {
        /// <summary>
        /// 开始攻击时的时间
        /// </summary>
        public abstract int StartTime { get; }

        public bool IsAimingMouse { get; private set; }

        /// <summary>
        /// 仅供外部调用的AI
        /// </summary>
        /// <param name="yujianProj"></param>
        public void AttackAI(BaseYujianProj yujianProj)
        {
            if (yujianProj.Timer == StartTime)
            {
                OnStartAttack(yujianProj);
                IsAimingMouse = yujianProj.AimMouse;
                yujianProj.AimMouse = false;
            }

            Attack(yujianProj);

            if (UpdateTime(yujianProj))
                yujianProj.Timer -= 1f;

            if (yujianProj.Timer <= 0f)
                yujianProj.ChangeState();
        }

        /// <summary>
        /// 开始攻击时执行的AI
        /// </summary>
        /// <param name="YujianProj"></param>
        protected virtual void OnStartAttack(BaseYujianProj YujianProj) { }

        /// <summary>
        /// 每帧执行的具体攻击AI
        /// </summary>
        /// <param name="YujianProj"></param>
        protected abstract void Attack(BaseYujianProj YujianProj);

        /// <summary>
        /// 决定是否能让时间减少
        /// </summary>
        /// <returns></returns>
        public virtual bool UpdateTime(BaseYujianProj yujianProj) => true;


    }

}
