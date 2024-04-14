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
        public int state;

        /// <summary>
        /// 捕捉器的圆圈的中心点坐标
        /// </summary>
        public Point webCenter;

        /// <summary>
        /// 捕捉器的圆圈的半径大小
        /// </summary>
        public float webRadius;

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

            switch (state)
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
                        SpawnFairy();

                        //更新仙灵的活动
                        for (int i = fairys.Count - 1; i >= 0; i--)
                        {
                            Fairy fairy = fairys[i];
                            fairy.UpdateInCatcher(this);
                            if (!fairy.active)
                                fairys.Remove(fairy);
                        }

                        //随机刷新仙灵
                        SpawnFairy();

                        //右键一下就结束捕捉
                        if (Main.mouseRight&&Main.mouseRightRelease)
                            TrunToBacking();
                    }
                    break;
                case (int)AIStates.Backing:
                    {
                        //直接向玩家lerp
                        Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.1f);
                        curserCenter = Vector2.Lerp(curserCenter, Projectile.Center, 0.1f);

                        if (Vector2.Distance(Projectile.Center, Owner.Center) < 48)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (state)
            {
                default: return false;
                case (int)AIStates.Shooting://射击时撞墙直接返回
                    TurnToCatching();
                    return false;
                case (int)AIStates.Catching: return false;
                case (int)AIStates.Backing://返回时撞墙直接kill
                    return true;
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

        /// <summary>
        /// 转换到捕捉状态
        /// </summary>
        public void TurnToCatching()
        {
            state = (int)AIStates.Catching;

            Projectile.tileCollide = false;
        }

        public void TrunToBacking()
        {
            state = (int)AIStates.Catching;

            Projectile.timeLeft = 60 * 10;
        }

        public void SpawnFairy()
        {

        }

        public virtual void UpdateCurser()
        {

        }


        /// <summary>
        /// 绘制指针与手持物品间的连线
        /// </summary>
        public virtual void DrawLine(Vector2 handlePos)
        {

        }

        public virtual void DrawCursor()
        {

        }

        public virtual void DrawHandle()
        {

        }

        /// <summary>
        /// 获取线的点的列表
        /// </summary>
        /// <param name="handlePos"></param>
        /// <returns></returns>
        public virtual List<Vector2> GetLinePointList(Vector2 handlePos)
        {
            return null;
        }

        public virtual Vector2 GetHandlePos()
        {
            return Vector2.Zero;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制圆圈

            //绘制背景

            //绘制标红的物块

            Vector2 handlePos = GetHandlePos();

            //绘制连线
            DrawLine(handlePos);
            //绘制指针
            DrawCursor();
            //绘制手持
            DrawHandle();

            return base.PreDraw(ref lightColor);
        }


    }
}
