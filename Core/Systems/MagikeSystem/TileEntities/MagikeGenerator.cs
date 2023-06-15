using Coralite.Helpers;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能生产器，能够存储，发送和生产魔能，如何生产请自定义
    /// </summary>
    public abstract class MagikeGenerator : MagikeSender
    {
        public MagikeGenerator(int magikeMax,int connectLenghMax, int howManyCanConnect = 1) : base(magikeMax, connectLenghMax, howManyCanConnect)
        {

        }

        public override void Update()
        {
            Generate();
        }

        /// <summary>
        /// 如何生产魔能
        /// </summary>
        public virtual void Generate() { }


    }

    /// <summary>
    /// 通过放入含有魔能的物品并消耗它来获取魔能
    /// </summary>
    public abstract class MagikeGenerator_FromMagItem : MagikeGenerator
    {
        /// <summary> 内部存储的物品，仅当物品魔能含量不小于0是才能消耗物品并获得魔能  </summary>
        public Item itemToCosume = new Item();
        /// <summary> 计时器 </summary>
        public int generateTimer;
        /// <summary> 每隔多久生成一次魔能  </summary>
        public readonly int generateDelay;

        public MagikeGenerator_FromMagItem(int magikeMax, int connectLenghMax,int generateDelay, int howManyCanConnect = 1) : base(magikeMax, connectLenghMax, howManyCanConnect)
        {
            this.generateDelay = generateDelay;
        }

        public override void Generate()
        {
            if (itemToCosume.IsAir)
                return;

            generateTimer++;
            if (generateTimer < generateDelay) //每隔固定时间检测物品，并消耗
                return;

            int add = itemToCosume.GetMagikeItem().magiteAmount;    //消耗前检测一下，防止出现意外事故
            if (add <= 0)
                return;

            itemToCosume.stack--;
            if (itemToCosume.stack < 1)
                itemToCosume.TurnToAir();   //消耗物品并获得魔能

            magike += add;
            //TODO: 生成粒子
            Limit();
            generateTimer = 0;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!itemToCosume.IsAir)
            {
                tag.Add("itemToCosume", itemToCosume);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("itemToCosume",out Item item))
            {
                itemToCosume = item;
            }
        }
    }
}
