using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyBottle:ModItem
    {
        /// <summary>
        /// 仙灵物品数组
        /// </summary>
        public readonly Item[] Fairies;

        /// <summary>
        /// 仙灵瓶的容量，默认10
        /// </summary>
        public virtual int Capacity => 10;

        public BaseFairyBottle()
        {
            Fairies = new Item[Capacity];
            for (int i = 0; i < Capacity; i++)
                Fairies[i] = new Item();
        }


    }
}
