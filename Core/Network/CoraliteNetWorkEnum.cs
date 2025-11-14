namespace Coralite.Core.Network
{
    public enum CoraliteNetWorkEnum : byte
    {
        BabyIceDragon,
        PlaceFilter,
        /// <summary>
        /// 同步移除滤镜的按钮
        /// </summary>
        FilterRemoveButton_LeftClick,
        /// <summary>
        /// 同步无限晶簇魔杖的使用
        /// </summary>
        ClusterWand,
        /// <summary>
        /// 同步璀璨连接杖的选取发送者
        /// </summary>
        BrilliantConnectStaff_Sender,
        /// <summary>
        /// 同步璀璨连接杖的选取接收者
        /// </summary>
        BrilliantConnectStaff_Receivers,
        /// <summary>
        /// 接收特定的
        /// </summary>
        ItemContainer_SpecificIndex,
        /// <summary>
        /// 接收特定的
        /// </summary>
        ItemContainer,
        /// <summary>
        /// 同步魔能的改动
        /// </summary>
        MagikeSystem,
        /// <summary>
        /// 为什么会有这个网络组？因为服务器他妈的疯疯疯疯疯疯疯疯疯疯疯疯疯了！！！
        /// 如果你只选择在客户端上发送杀死NPC的包，在那之前服务器会有自己的想法，它大概率会在接收这个包之前自己再同步一次
        /// 这样的结果是会让客户端杀死的NPC重新出现在世界上，这都算好的结果了，更多的时候是会让世界出现更多的隐形NPC
        /// 只在服务端存在的NPC，这样的结果是灾难性的。
        /// 所以，记住了像杀死姜饼人这样的例子，不 要 试 图 在 客 户 端 去 杀 死 NPC，这样的事情让服务器去干！
        /// </summary>
        KillBiskety,
        /// <summary>
        /// 该   死   的   姜   饼   人
        /// 生成NPC的事情也不要让客户端去干，而是让服务器去做，然后同步给所有客户端
        /// </summary>
        SpawnBiskety,
        /// <summary>
        /// 由玩家端向服务端发送的世界变量同步请求
        /// </summary>
        WorldValueRequest,
        /// <summary>
        /// 世界变量同步
        /// </summary>
        WorldValue,
    }
}
