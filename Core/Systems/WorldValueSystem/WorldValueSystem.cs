using Coralite.Core.Loaders;
using Coralite.Core.Network;
using Coralite.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.WorldValueSystem
{
    public class WorldValueSystem : ModSystem
    {
        /*
         * 一共3种情况
         * 
         * 情况1：玩家进入世界
         * 由CoralitePlayer中的OnEnterWorld向服务端发送请求，之后服务端单独发包给该玩家
         * RequestForSync客=》ServerHandleWorldValueSync（SendWorldValue）服务=》ReceiveWorldValue客
         * 
         * 情况2：玩家端改变世界变量，之后经由服务端广播给全体玩家
         * SendWorldValue客=》ReceiveWorldValue服务=》SendWorldValue服务=》ReceiveWorldValue客
         * 
         * 情况3：服务端改变世界变量，之后广播给全体玩家(最简单)
         * SendWorldValue服务=》ReceiveWorldValue客
         */



        /// <summary>
        /// 在此记录所有的世界bool值
        /// </summary>
        public static bool[] WorldFlags { get; set; }

        /// <summary>
        /// 本地端向服务器发送同步请求后等待服务器同步，如果超时或者丢包了就再请求一次
        /// </summary>
        internal static bool waitForSync;
        /// <summary>
        /// 本地端等待服务端同步的时间
        /// </summary>
        internal static int waitForSyncTime;

        
        //在此插入一段用于在夜晚开始时同步自己的内容
        public override void Load()
        {
            On_Main.UpdateTime_StartNight += StartNightWorldValueSync;
        }

        public override void Unload()
        {
            On_Main.UpdateTime_StartNight -= StartNightWorldValueSync;
        }

        private void StartNightWorldValueSync(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig.Invoke(ref stopEvents);
            if (VaultUtils.isServer)
                SendWorldValue(-1, false);
        }

        public override void SetStaticDefaults()
        {
            //在这里初始化世界bool的数组
            WorldFlags = new bool[WorldValueLoader.FlagCount];
        }

        public override void PostUpdateTime()
        {
            if (waitForSync)
            {
                if (waitForSyncTime>0)
                {
                    waitForSyncTime--;
                    if (waitForSyncTime < 1)//我包呢？再请求一次
                        RequestForSync();
                }
            }
        }

        /// <summary>
        /// 玩家端发送同步请求
        /// </summary>
        public static void RequestForSync()
        {
            if (!VaultUtils.isClient)
                return;

            waitForSync = true;
            waitForSyncTime = 60 * 10;//10秒没收到就再发送一次请求
            ModPacket p = Coralite.Instance.GetPacket();

            p.Write((byte)CoraliteNetWorkEnum.WorldValueRequest);
            p.Write(Main.myPlayer);
            p.Send();
        }

        public static void ServerHandleWorldValueRequest(BinaryReader reader)
        {
            int toWho = reader.ReadInt32();
            SendWorldValue(toWho,false);
        }

        /// <summary>
        /// 由服务端发送世界变量，用<paramref name="enterWorld"/>判断是否执行刚进入世界的一些东西
        /// </summary>
        /// <param name="toWho"></param>
        public static void SendWorldValue(int toWho, bool clientToServer)
        {
            ModPacket p = Coralite.Instance.GetPacket();

            p.Write((byte)CoraliteNetWorkEnum.WorldValue);
            p.Write(clientToServer);
            p.WriteBools(WorldFlags);
            int count = WorldValueLoader.FlagCount;
            int writeCount = 0;
            for (int i = 0; i < count / 8 + 1; i++)
            {
                BitsByte bt = new BitsByte();
                for (int j = 0; j < 8; j++)
                {
                    bt[j] = WorldFlags[writeCount];//写入对应的值

                    writeCount++;
                    if (writeCount > count)
                        break;
                }

                p.Write(bt);
            }

            p.Send(toWho);
        }

        /// <summary>
        /// 客户端接受世界变量
        /// </summary>
        public static void ReceiveWorldValue(BinaryReader reader)
        {
            if (reader.ReadBoolean() && VaultUtils.isServer)//判断这个是玩家端改变了值然后向其他端同步的情况
            {
                reader.ReadBools(WorldFlags);
                SendWorldValue(-1, false);

                return;
            }

            waitForSync = false;
            waitForSyncTime = 0;

            reader.ReadBools(WorldFlags);
        }

        public static void OnEnterWorld()
        {
            foreach (var flag in WorldValueLoader.flags)
                flag.OnEnterWorld();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            for (int i = 0; i < WorldValueLoader.flags.Count; i++)
            {
                WorldFlag flag = WorldValueLoader.flags[i];
                bool value = WorldFlags[i];

                if (value)
                    tag.Add(flag.Name, true);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            //如果因为某些加载顺序问题导致旧版本无法适配... ...我只能说也没办法了
            //但是好消息是WorldValue是W开头的，应该在比较后面
            for (int i = 0; i < WorldValueLoader.flags.Count; i++)
            {
                WorldFlag flag = WorldValueLoader.flags[i];
                WorldFlags[i] = tag.ContainsKey(flag.Name);
            }
        }

        //暂时不用
        public override void NetSend(BinaryWriter writer)
        {
            
        }

        public override void NetReceive(BinaryReader reader)
        {
            
        }
    }
}
