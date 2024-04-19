using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyBottle : ModItem,IFairyBottle
    {
        /// <summary>
        /// 仙灵物品数组
        /// </summary>
        private readonly Item[] fairies;

        /// <summary>
        /// 仙灵瓶的容量，默认10
        /// </summary>
        public virtual int Capacity => 10;

        public Item[] Fairies => fairies;

        public BaseFairyBottle()
        {
            fairies = new Item[Capacity];
            for (int i = 0; i < Capacity; i++)
                fairies[i] = new Item();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }
    }

    public interface IFairyBottle
    {
        int Capacity { get; }

        public Item[] Fairies { get; }
    }
}
