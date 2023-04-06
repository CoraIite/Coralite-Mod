using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

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
        public static bool AutomaticTracking(Projectile projectile, float offset, float chasingSpeed = 0, float distanceMax = 1000f)
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
                return true;
            }
            return false;
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

        /// <summary>
        /// 找到同类弹幕并知道自己是第几个弹幕
        /// </summary>
        /// <param name="projType"></param>
        /// <param name="whoAmI"></param>
        /// <param name="owner"></param>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        public static void GetMyProjIndexWithSameType(int projType, int whoAmI, int owner, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == owner && projectile.type == projType)
                {
                    if (whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        public static void GetMyProjIndexWithModProj<T>(Projectile Projectile, out int index, out int totalIndexesInGroup) where T : ModProjectile
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.ModProjectile is T)
                {
                    if (Projectile.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }
    }
}
