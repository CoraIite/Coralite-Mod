using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        /// <summary>
        /// 使用<see cref="Recorder"/> 记录第一次短冲的冲刺次数
        /// </summary>
        /// <returns></returns>
        public bool SmallDash()
        {
            const int smallDashTime = 10;
            const int smallDashSpeed = 45;

            if (Timer == 0)
            {
                //生成弹幕并随机速度方向
                int damage = Helper.GetProjDamage(20, 30, 70);
                NPC.NewProjectileDirectInAI<PurpleDash>(NPC.Center, Vector2.Zero, damage, 0
                    , NPC.target, smallDashTime - 1, NPC.whoAmI, 10);

                ElectricSound();
                float targetrot = (Target.Center - NPC.Center).ToRotation();

                //距离小于700那就不会朝向玩家冲刺
                if (Vector2.Distance(NPC.Center, Target.Center) < 700)
                    targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(0.7f, 1.2f);
                NPC.velocity = targetrot.ToRotationVector2() * smallDashSpeed;
                NPC.rotation = NPC.velocity.ToRotation();
                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                IsDashing = true;
            }
            else if (Timer > smallDashTime / 4 && Timer < smallDashTime)//根据目标位置偏移一下
            {
                float distance = NPC.Center.Distance(Target.Center);
                if (distance < 500)
                {
                    float factor = 1 - Math.Clamp(distance / 600, 0.01f, 1);

                    //目标方向
                    float targetDir = (Target.Center - NPC.Center).ToRotation() + MathHelper.Pi;
                    float velocityDir = NPC.velocity.ToRotation().AngleTowards(targetDir, 0.2f * factor);
                    NPC.velocity = velocityDir.ToRotationVector2() * smallDashSpeed;
                    NPC.rotation = velocityDir;
                }
            }

            UpdateAllOldCaches();

            Timer++;
            if (Timer > smallDashTime)
            {
                Timer = 0;
                NPC.velocity *= 0;
                IsDashing = false;

                Recorder--;
                if (Recorder < 1)//结束短冲
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 小冲刺初始值设置
        /// </summary>
        public void SmallDashSetStartValue()
        {
            //根据与玩家的距离增加短突的次数
            Recorder = Main.rand.Next(1, 4);

            float distance = Vector2.Distance(NPC.Center, Target.Center);
            if (distance > 700)
                Recorder++;
            if (distance > 1000)
                Recorder++;

            ResetAllOldCaches();
            canDrawShadows = true;
        }

    }
}
