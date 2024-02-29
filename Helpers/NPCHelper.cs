using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

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

        public static Texture2D GetTexture(this NPC npc)
        {
            return TextureAssets.Npc[npc.type].Value;
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
        /// 简易NPC运动，控制单个方向上的运动，做匀加速和匀减速运动
        /// </summary>
        /// <param name="velocity">输入的单个方向的速度</param>
        /// <param name="currentDistance">当前的距离</param>
        /// <param name="direction">方向</param>
        /// <param name="velocityLimit">速度限制</param>
        /// <param name="distanceLimit">距离限制</param>
        /// <param name="accel">加速度</param>
        /// <param name="turnAccel">转向加速度</param>
        /// <param name="slowDownPercent">减速系数</param>
        public static void Movement_SimpleOneLine_Limit(ref float velocity, float currentDistance, int direction, float velocityLimit
            , float distanceLimit, float accel, float turnAccel, float slowDownPercent)
        {
            if (currentDistance > distanceLimit)
                Movement_SimpleOneLine(ref velocity, direction, velocityLimit, accel, turnAccel, slowDownPercent);
            else
                velocity *= slowDownPercent;
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

        public static bool GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper)
        {
            if (!Main.GameModeInfo.IsJourneyMode) //从源码里抄过来的，只能说旅途模式写的什么B玩意
            {
                journeyScale = 1f;
                nPCStrengthHelper = default;
                return false;
            }

            journeyScale = 1f;
            CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
            if (power != null && power.GetIsUnlocked())
                journeyScale = power.StrengthMultiplierToGiveNPCs;

            nPCStrengthHelper = new NPCStrengthHelper(Main.GameModeInfo, journeyScale, Main.getGoodWorld);
            return true;
        }

        /// <summary>
        /// 找到同类型并且相同target的NPC，输出一共多少个和自身位置<br></br>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        [DebuggerHidden]
        public static void GetMyNpcIndexWithModNPC<T>(NPC n, out int index, out int totalIndexesInGroup) where T : ModNPC
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.target == n.target && npc.ModNPC is T)
                {
                    if (n.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        /// <summary>
        /// 根据NPC与目标的位置关系快速设置<br></br>
        /// <see cref="NPC.spriteDirection"/><br></br>
        /// <see cref="NPC.directionY"/><br></br>
        /// <see cref="Entity.direction"/>
        /// </summary>
        /// <param name="npc"></param>
        public static void QuickSetDirection(this NPC npc)
        {
            Player p = Main.player[npc.target];

            npc.direction = npc.spriteDirection = p.Center.X > npc.Center.X ? 1 : -1;
            npc.directionY = p.Center.Y > npc.Center.Y ? 1 : -1;
        }

        [DebuggerHidden]
        public static int NewProjectileInAI(this NPC npc, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static int NewProjectileInAI<T>(this NPC npc, Vector2 position, Vector2 velocity, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
            where T : ModProjectile
        {
            return Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static Projectile NewProjectileDirectInAI(this NPC npc, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return Projectile.NewProjectileDirect(npc.GetSource_FromAI(), position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static Projectile NewProjectileDirectInAI<T>(this NPC npc, Vector2 position, Vector2 velocity, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
            where T : ModProjectile
        {
            return Projectile.NewProjectileDirect(npc.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockBack, owner, ai0, ai1, ai2);
        }

        public static void InitOldPosCache(this NPC npc, int trailCount, bool useCenter = true)
        {
            npc.oldPos = new Vector2[trailCount];

            for (int i = 0; i < trailCount; i++)
            {
                if (useCenter)
                    npc.oldPos[i] = npc.Center;
                else
                    npc.oldPos[i] = npc.position;
            }
        }

        public static void InitOldRotCache(this NPC npc, int trailCount)
        {
            npc.oldRot = new float[trailCount];

            for (int i = 0; i < trailCount; i++)
            {
                npc.oldRot[i] = npc.rotation;
            }
        }

        public static void UpdateOldPosCache(this NPC npc, bool useCenter = true, bool addVelocity = true)
        {
            for (int i = 0; i < npc.oldPos.Length - 1; i++)
                npc.oldPos[i] = npc.oldPos[i + 1];
            npc.oldPos[^1] = (useCenter ? npc.Center : npc.position) + (addVelocity ? npc.velocity : Vector2.Zero);
        }

        public static void UpdateOldRotCache(this NPC npc)
        {
            for (int i = 0; i < npc.oldRot.Length - 1; i++)
                npc.oldRot[i] = npc.oldRot[i + 1];
            npc.oldRot[^1] = npc.rotation;
        }


    }
}
