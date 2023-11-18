namespace Coralite.Core.Systems.MagikeSystem
{
    public interface IMagikeSenderItem
    {
        public string SendDelay { get; }
        public int HowManyPerSend { get; }
        /// <summary> 连接距离 </summary>
        public int ConnectLengthMax { get; }
        /// <summary> 连接数量 </summary>
        public int HowManyCanConnect { get => 1; }
    }
}
