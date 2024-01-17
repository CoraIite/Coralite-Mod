using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.SlashBladeSystem
{
    public class ComboManager
    {
        public int combo;
        public int resetTimer;

        public enum ControlType
        {
            Left=0,
            Right=1,
            Left_Up=2,
            Right_Up=3,
            Left_Down=4, 
            Right_Down=5,
        }

        /// <summary>
        /// 使用Control获得对应控制类型的连段表，再使用combo获取到指定的ComboData
        /// </summary>
        public Dictionary<int, Dictionary<int, ComboData>> ComboDatas;

        /// <summary>
        /// 攻击后的后摇时间，超出这个时间后重置连段
        /// </summary>
        public readonly int resetMaxTime;

        public ComboManager(int resetMaxTime=15)
        {
            this.resetMaxTime = resetMaxTime;
        }

        public ComboManager AddCombo(int controlType,int combo,int type,float damageMult)
        {
             
            return this;
        }

        public void UpdateDelay(Player player)
        {
            if (player.ItemTimeIsZero && combo != 0)
            {
                resetTimer++;
                if (resetTimer > resetMaxTime)
                {
                    resetTimer = 0;
                    combo = 0;
                }
            }
        }

        public void Shoot(Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback)
        {

        }
    }

    public struct ComboData
    {
        public int projType;
        public float damageMult;
        public ComboData(int projType, float damageMult)
        {
            this.projType = projType;
            this.damageMult = damageMult;
        }
    }
}
