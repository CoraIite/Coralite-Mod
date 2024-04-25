using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyBottle : ModItem, IFairyBottle
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
            for (int i = 0; i < Capacity; i++)
                tag.Add("Fairies" + i, fairies[i]);
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (tag.TryGet("Fairies" + i, out Item fairy))
                    fairies[i] = fairy;
                else
                    fairies[i] = new Item();
            }
        }
    }

    public interface IFairyBottle
    {
        int Capacity { get; }

        public Item[] Fairies { get; }
    }
}
