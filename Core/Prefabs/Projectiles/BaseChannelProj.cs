using InnoVault.GameContent.BaseEntity;
using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 使用前注意：请一定要设置物品的Item.channel为true !!!!
    /// <para>大部分可蓄力的武器都能用这个模板，应该</para>
    /// <para>多人同步说明：蓄力判定使用 <see cref="BaseHeldProj"/> 已自动同步的 <see cref="BaseHeldProj.DownLeft"/>
    /// 取代原版的 <c>Owner.channel</c>（远端不可靠）；一次性的转阶段标志
    /// <see cref="completeAndRelease"/>/<see cref="canChannel"/> 通过 <see cref="SendBitsByte"/> 同步，
    /// 保证各端阶段一致。生成弹幕/修改玩家等权威逻辑需要在子类中包裹 owner 守卫。</para>
    /// </summary>
    public abstract class BaseChannelProj : BaseHeldProj
    {
        public bool completeAndRelease = false;
        protected bool canChannel = true;

        /// <summary>
        /// 是否已经进入"完成蓄力并释放"阶段，确保 <see cref="OnEnterCompleteAndRelease"/> 只触发一次，
        /// 无论 <see cref="completeAndRelease"/> 是本地置位还是经网络同步收到
        /// </summary>
        private bool enteredCompleteAndRelease;

        protected int timer;
        public ref float _Rotation => ref Projectile.ai[1];

        /// <summary>
        /// 是否处于蓄力状态。使用 <see cref="BaseHeldProj.DownLeft"/>（按键状态，已自动网络同步）替代
        /// 原版 <c>Owner.channel</c>，从而在远端也能可靠判定
        /// </summary>
        public bool Channelling => DownLeft;

        public sealed override void AI()
        {
            AIBefore();

            if (completeAndRelease)
            {
                if (!enteredCompleteAndRelease)
                {
                    enteredCompleteAndRelease = true;
                    OnEnterCompleteAndRelease();
                }

                CompleteAndRelease();
                AfterCompleteAndRelease();
                return;
            }

            AIMiddle();

            if (Channelling && canChannel)
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
        /// 进入"完成蓄力并释放"阶段时只执行一次的初始化逻辑。<br/>
        /// 由于 <see cref="completeAndRelease"/> 会经网络同步，远端可能在本地逻辑尚未转阶段时就收到该标志，
        /// 因此所有一次性的初始化（例如重设拖尾缓存、修改伤害等各端都需要执行的设置）都应放在这里，
        /// 以保证无论本地置位还是同步收到都会安全地执行一次
        /// </summary>
        protected virtual void OnEnterCompleteAndRelease() { }

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
            //owner 端置位一次性转阶段标志后立即请求同步，确保各端阶段一致
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// 同步两个一次性转阶段标志。0、1号位已被 <see cref="BaseHeldProj"/> 占用，这里使用 2、3号位
        /// </summary>
        public override BitsByte SendBitsByte(BitsByte flags)
        {
            flags = base.SendBitsByte(flags);
            flags[2] = completeAndRelease;
            flags[3] = canChannel;
            return flags;
        }

        /// <inheritdoc/>
        public override void ReceiveBitsByte(BitsByte flags)
        {
            base.ReceiveBitsByte(flags);
            completeAndRelease = flags[2];
            canChannel = flags[3];
        }
    }
}
