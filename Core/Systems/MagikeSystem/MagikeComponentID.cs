namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeComponentID
    {
        public const int ApparatusInformation = 0;
        public const int MagikeContainer = 1;
        public const int MagikeSender = 2;
        public const int MagikeProducer = 3;
        public const int MagikeFactory = 4;
        public const int MagikeFilter = 5;
        public const int ItemContainer = 6;
        public const int ItemGetOnlyContainer = 7;
        public const int ItemSender = 8;

        public const int Count = 9;

        /// <summary>
        /// 是否为单一实例
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool IsSingleton(int ID)
        {
            return ID != MagikeFilter;
        }
    }
}
