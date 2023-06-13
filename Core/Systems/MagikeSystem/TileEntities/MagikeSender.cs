using System;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能发送器，能够存储魔能，同时可以发送魔能，使用howManyCanConnect来决定能连接多少魔能容器
    /// </summary>
    public abstract class MagikeSender : MagikeContainer
    {
        public int[] receiverIDs;

        public MagikeSender(int magikeMax, int howManyCanConnect = 1) : base(magikeMax)
        {
            receiverIDs = new int[howManyCanConnect];
            for (int i = 0; i < howManyCanConnect - 1; i++)
                receiverIDs[i] = -1;
        }

        public override void PreGlobalUpdate()
        {
            for (int i = 0; i < receiverIDs.Length - 1; i++)
            {
                int id = receiverIDs[i];
                if (id != -1 && ByID.ContainsKey(id) && ByID[id] is MagikeContainer container)
                {
                    if (container.magike < magikeMax)//魔能不满时才能发送给接收者，发送后会防止越界情况出现
                    {
                        Send(container);
                        container.magike = Math.Clamp(container.magike, 0, container.magikeMax);
                    }
                }
                else
                    receiverIDs[i] = -1;
            }
        }

        /// <summary>
        /// 如何将魔能发送给接受者，进在接收者魔能不满时才会调用
        /// </summary>
        /// <param name="container">接受者</param>
        public virtual void Send(MagikeContainer container) { }
    }
}
