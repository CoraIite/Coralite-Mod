namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IConnectLengthModify
    {
        /// <summary> 基础连接距离 </summary>
        public int LengthBase { get; set; }
        /// <summary> 额外连接距离 </summary>
        public int LengthExtra { get; set; }
    }
}
