using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能发送器，能够存储魔能，同时可以发送魔能，使用howManyCanConnect来决定能连接多少魔能容器
    /// </summary>
    public abstract class MagikeSender : MagikeContainer
    {
        /// <summary> 接收者的位置 </summary>
        public Point16[] receiverPoints;
        /// <summary> 距离多少才能连接 </summary>
        public readonly int connectLenghMax;
        /// <summary> 每次发送多少，可以自定义 </summary>
        public abstract int HowManyPerSend { get; }

        public MagikeSender(int magikeMax, int connectLenghMax, int howManyCanConnect = 1) : base(magikeMax)
        {
            this.connectLenghMax = connectLenghMax;
            receiverPoints = new Point16[howManyCanConnect];
            for (int i = 0; i < howManyCanConnect - 1; i++)
                receiverPoints[i] = Point16.NegativeOne;
        }

        public override void PreGlobalUpdate()
        {
            if (!CanSend())
                return;

            int howMany = HowManyPerSend;
            if (magike < howMany)   //当前量不够发送时直接返回
                return;

            for (int i = 0; i < receiverPoints.Length - 1; i++)
            {
                Point16 position = receiverPoints[i];
                if (position != Point16.NegativeOne && ByPosition.ContainsKey(position) && ByPosition[position] is MagikeContainer container)
                {
                    if (container.Charge(howMany))//魔能不满时才能发送给接收者，发送后会防止越界情况出现
                    {
                        Charge(-howMany);
                        if (magike < howMany)   //发送完之后如果剩余量不能够继续发送那么直接返回
                            return;
                    }
                }
                else
                    receiverPoints[i] = Point16.NegativeOne;
            }
        }

        /// <summary>
        /// 是否能发送
        /// </summary>
        /// <returns></returns>
        public abstract bool CanSend();

        /// <summary>
        /// 帮助方法，判断两个魔能容器的距离并根据自身的connectLenghMax决定是否能发送给对方
        /// </summary>
        /// <returns></returns>
        public virtual bool CanConnect(MagikeContainer container)
        {
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            for (int i = 0; i < receiverPoints.Length - 1; i++)
                if (receiverPoints[i] != Point16.NegativeOne)
                {
                    tag.Add("Receiver_x" + i, receiverPoints[i].X);
                    tag.Add("Receiver_y" + i, receiverPoints[i].Y);
                }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            for (int i = 0; i < receiverPoints.Length - 1; i++)
            {
                if (tag.TryGet("Receiver_x" + i, out short x) && tag.TryGet("Receiver_y" + i, out short y))
                    receiverPoints[i] = new Point16(x, y);
            }
        }
    }
}
