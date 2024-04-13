using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class BaseFairyCatcherProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public List<Fairy> fairys;

        /// <summary>
        /// 状态，使用<see cref="AIStates"/>来判断
        /// </summary>
        public int State;
        /// <summary>
        /// 捕捉器的圆圈的半径大小
        /// </summary>
        public float WebRadius;

        /// <summary>
        /// 指针的位置
        /// </summary>
        public Vector2 curserCenter;

        public enum AIStates
        {
            /// <summary>
            /// 射击时，到鼠标位置就会停止
            /// </summary>
            Shooting,
            /// <summary>
            /// 捕捉仙灵，绝大部分的逻辑在这里
            /// </summary>
            Catching,
            /// <summary>
            /// 返回玩家
            /// </summary>
            Backing
        }

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            switch (State)
            {
                default: Projectile.Kill(); break;
                case (int)AIStates.Shooting:
                    {
                        //初始指定鼠标位置为目标点
                        //指针射向初始位置
                        //玩家距离过远进入回收阶段
                    }
                    break;
                case (int)AIStates.Catching:
                    {
                        fairys ??= new List<Fairy>();

                        //更新指针

                        //更新仙灵的活动
                        for (int i = fairys.Count - 1; i >= 0; i--)
                        {
                            Fairy fairy = fairys[i];
                            fairy.UpdateInCatcher(this);
                            if (!fairy.active)
                                fairys.Remove(fairy);
                        }

                        //随机刷新仙灵

                    }
                    break;
            }
        }

        /// <summary>
        /// 获取指针的碰撞盒
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCursor()
        {
            int cursorWidth = 4;
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                cursorWidth = (int)fcp.cursorSizeBonus.ApplyTo(cursorWidth);

            return new Rectangle((int)curserCenter.X - cursorWidth / 2, (int)curserCenter.Y - cursorWidth / 2, cursorWidth, cursorWidth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制圆圈

            //绘制背景

            //绘制标红的物块

            return base.PreDraw(ref lightColor);
        }
    }
}
