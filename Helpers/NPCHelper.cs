using System;
using System.Diagnostics;
using System.Reflection;
using Terraria;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void Kill(this NPC NPC)
        {
            bool ModNPCDontDie = NPC.ModNPC?.CheckDead() == false;
            if (ModNPCDontDie)
                return;
            NPC.life = 0;
            NPC.checkDead();
            NPC.HitEffect();
            NPC.active = false;
        }

        /// <summary>
        /// 简易NPC运动，控制单个方向上的运动，做匀加速和匀减速运动
        /// </summary>
        /// <param name="velocity">输入的单个方向的速度</param>
        /// <param name="direction">方向</param>
        /// <param name="velocityLimit">速度限制</param>
        /// <param name="accel">加速度</param>
        /// <param name="turnAccel">转向加速度</param>
        /// <param name="slowDownPercent">减速系数</param>
        public static void Movement_SimpleOneLine(ref float velocity, int direction, float velocityLimit, float accel, float turnAccel, float slowDownPercent)
        {
            if (Math.Abs(velocity) > velocityLimit)
                velocity *= slowDownPercent;
            else
            {
                velocity += direction * (Math.Sign(velocity) == direction ? accel : turnAccel);
                if (Math.Abs(velocity) > velocityLimit)
                    velocity = direction * velocityLimit;
            }
        }

        /// <summary>
        /// 返回NPC的索引，如果没找到则返回-1
        /// </summary>
        /// <param name="npcType"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int GetNPCByType(int npcType)
        {
            int index = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == npcType)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}
