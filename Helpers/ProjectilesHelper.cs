using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;

namespace Coralite.Helpers
{
    public static class ProjectilesHelper
    {
        /// <summary>
        /// 自动追踪最近敌人的弹幕
        /// </summary>
        /// <param name="projectile">弹幕</param>
        /// <param name="offset">追踪力度</param>
        /// <param name="chasingSpeed">追踪速度</param>
        /// <param name="distanceMax">最大追踪距离</param>
        public static void AutomaticTracking(Projectile projectile, float offset, float chasingSpeed = 0, float distanceMax = 1000f)
        {
            NPC target = FindCloestEnemy(projectile.Center, distanceMax, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(projectile.Center, 1, 1, n.Center, 1, 1);
            });

            //原本的弹幕速度加上一个弹幕位置指向NPC位置的向量
            if (target != null)
            {
                Vector2 plrToTheNearestNPC = Vector2.Normalize(target.Center - projectile.Center);
                float origionSpeed = projectile.velocity.Length();
                projectile.velocity += plrToTheNearestNPC * offset;
                projectile.velocity = Vector2.Normalize(projectile.velocity) * (chasingSpeed == 0 ? origionSpeed : chasingSpeed);
            }
        }

        /// <summary>
        /// 发射朝向最近敌人的弹幕
        /// 建议只触发一次，不然会变成超强效果的追踪
        /// </summary>
        /// <param name="projectile">弹幕</param>
        /// <param name="speed">速度</param>
        /// <param name="distanceMax">最大瞄准距离</param>
        public static void AimingTheNearestNPC(Projectile projectile, float speed, float distanceMax = 1000f)
        {
            NPC target = FindCloestEnemy(projectile.Center, distanceMax, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(projectile.Center, 1, 1, n.Center, 1, 1);
            });
            if (target != null)
                projectile.velocity = Vector2.Normalize(target.Center - projectile.Center) * speed;
        }

        /// <summary>
        ///  找到距离弹幕最近的敌人
        /// </summary>
        /// <param name="projectile">弹幕</param>
        /// <param name="distanceMax">最大追踪距离</param>
        /// <returns></returns>
        public static NPC FindCloestEnemy(Vector2 position, float maxDistance, Func<NPC, bool> predicate)
        {
            float maxDis = maxDistance;
            NPC res = null;
            foreach (var npc in Main.npc.Where(n => n.active && !n.friendly && predicate(n)))
            {
                float dis = Vector2.Distance(position, npc.Center);
                if (dis < maxDis)
                {
                    maxDis = dis;
                    res = npc;
                }
            }
            return res;
        }
    }
}
