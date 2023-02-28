using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public abstract class YujianAI
    {
        /// <summary>
        /// 开始攻击时的时间
        /// </summary>
        public int StartTime { get; init;}

        public bool IsAimingMouse { get => _isAimingMouse; private set => _isAimingMouse = value; }
        private bool _isAimingMouse;

        /// <summary>
        /// 仅供外部调用的AI
        /// </summary>
        /// <param name="yujianProj"></param>
        public void AttackAI(BaseYujianProj yujianProj)
        {
            Attack(yujianProj);

            if (UpdateTime(yujianProj))
                yujianProj.Timer -= 1f;

            if (yujianProj.Timer <= 0f)
                yujianProj.ChangeState();
        }

        /// <summary>
        /// 仅供外部调用的方法，在刚开始时调用
        /// </summary>
        /// <param name="yujianProj"></param>
        public void OnStart(BaseYujianProj yujianProj)
        {
            OnStartAttack(yujianProj);
            BaseYujianProj.StartAttack(yujianProj.Projectile);
            yujianProj.Timer = StartTime;
            IsAimingMouse = yujianProj.AimMouse;
            yujianProj.AimMouse = false;
            yujianProj.Projectile.netUpdate = true;
        }

        /// <summary>
        /// 开始攻击时执行的AI
        /// </summary>
        /// <param name="yujianProj"></param>
        protected virtual void OnStartAttack(BaseYujianProj yujianProj) { }

        /// <summary>
        /// 每帧执行的具体攻击AI
        /// </summary>
        /// <param name="yujianProj"></param>
        protected abstract void Attack(BaseYujianProj yujianProj);

        /// <summary>
        /// 决定是否能让时间减少
        /// </summary>
        /// <returns></returns>
        protected virtual bool UpdateTime(BaseYujianProj yujianProj) => true;

        /// <summary>
        /// 一般用于绘制影子拖尾
        /// </summary>
        /// <param name="yujianProj"></param>
        public virtual void DrawAdditive(SpriteBatch spriteBatch,BaseYujianProj yujianProj) { }
        
        /// <summary>
        /// 一般用于绘制拖尾
        /// </summary>
        /// <param name="yujianProj"></param>
        public virtual void DrawPrimitives(BaseYujianProj yujianProj) { }
    }

}
