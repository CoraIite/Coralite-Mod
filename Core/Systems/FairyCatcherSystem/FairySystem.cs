using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public  class FairySystem:ModSystem
    {
        /// <summary>
        /// 键值是墙壁的type，-1表示没有墙壁
        /// </summary>
        public Dictionary<int, List<FairySpawnCondition>> FairySpawnConditions;

    }
}
