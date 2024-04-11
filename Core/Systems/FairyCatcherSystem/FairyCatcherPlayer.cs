using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyCatcherPlayer:ModPlayer
    {

        /// <summary>
        /// 伤害加成
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public float FairyDamageBonus(float damage)
        {
            return damage;
        }
    }
}
