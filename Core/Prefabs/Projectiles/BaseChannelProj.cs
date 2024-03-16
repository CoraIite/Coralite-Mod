using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 使用前注意：请一定要设置物品的Item.channel为true !!!!
    /// <para>大部分可蓄力的武器都能用这个模板，应该</para>
    /// </summary>
    public abstract class BaseChannelProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public bool completeAndRelease = false;
        protected bool canChannel = true;

        protected int timer;
        public ref float _Rotation => ref Projectile.ai[1];

        public sealed override void AI()
        {
            AIBefore();

            if (completeAndRelease)
            {
                CompleteAndRelease();
                AfterCompleteAndRelease();
                return;
            }

            AIMiddle();

            if (Owner.channel && canChannel)
                OnChannel();
            else
                OnRelease();

            AIAfter();
        }

        /// <summary>
        /// 大部分情况下或许不需要重写这个方法，此方法将使玩家无法使用其他物品，以及控制玩家手臂朝向
        /// </summary>
        protected virtual void AIBefore()
        {
            Owner.itemTime = Owner.itemAnimation = 2;//这个东西不为0的时候就无法使用其他物品
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;
        }

        /// <summary>
        /// 用于在非蓄力完成且释放的时候执行在蓄力和取消蓄力之前，嗯...可能说起来比较绕口吧
        /// <para>用于将弹幕中心设置为玩家中心，并控制玩家的朝向</para>
        /// </summary>
        protected virtual void AIMiddle()
        {
            Projectile.Center = Owner.Center;
            if (Main.myPlayer == Owner.whoAmI)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
        }

        protected virtual void AIAfter() { }

        /// <summary>
        /// 完成蓄力且释放后执行，将completeAndRelease设置为true即可执行这里，建议在途中不要在此更改这个变量了，否则会出现难以预料的后果
        /// </summary>
        protected virtual void CompleteAndRelease() { }

        /// <summary>
        /// 在完成蓄力和释放之后执行，可以将蓄力和释放方法中共通的部分拿到这里面来写，比如说更新计时器
        /// </summary>
        protected virtual void AfterCompleteAndRelease()
        {
            timer++;
        }

        /// <summary>
        /// 在蓄力的时候执行
        /// </summary>
        protected virtual void OnChannel() { }

        /// <summary>
        /// 在释放的时候执行，基类中包含设置canChannel，这会导致一旦释放就无法在此回到蓄力状态，重写这个方法的时候记得保留父方法的执行，不然的话可能会导致严重的后果
        /// </summary>
        protected virtual void OnRelease()
        {
            canChannel = false;
        }

        public virtual void OnChannelComplete(int timeLeft, int itemTime)
        {
            completeAndRelease = true;
            Projectile.friendly = true;
            Projectile.timeLeft = timeLeft;
            Owner.itemTime = Owner.itemAnimation = itemTime;
            timer = 0;
        }
    }
}
