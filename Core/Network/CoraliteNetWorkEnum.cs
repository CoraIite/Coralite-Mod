namespace Coralite.Core.Network
{
    /// <summary>
    /// Coralite 模组自定义包类型。首字节写入 <see cref="ModPacket"/>，由 <see cref="CoraliteNetWork.NetWorkHander"/> 分发。
    /// </summary>
    public enum CoraliteNetWorkEnum : byte
    {
        /// <summary>已废弃：冰龙宝宝 Moves 自定义包（FSM 迁移后由 ai[0] 同步，保留占位以稳定后续成员序数）。</summary>
        Unused_BabyIceDragon,
        /// <summary>放置魔能滤镜</summary>
        PlaceFilter,
        /// <summary>同步移除滤镜的按钮</summary>
        FilterRemoveButton_LeftClick,
        /// <summary>同步无限晶簇魔杖的使用</summary>
        ClusterWand,
        /// <summary>同步璀璨连接杖的选取发送者</summary>
        BrilliantConnectStaff_Sender,
        /// <summary>同步璀璨连接杖的选取接收者</summary>
        BrilliantConnectStaff_Receivers,
        /// <summary>魔能容器：指定索引物品</summary>
        ItemContainer_SpecificIndex,
        /// <summary>魔能容器：整包物品</summary>
        ItemContainer,
        /// <summary>同步魔能系统的改动</summary>
        MagikeSystem,
        /// <summary>
        /// 服务端杀死姜饼人 NPC（客户端禁止直接 Kill）。
        /// </summary>
        KillBiskety,
        /// <summary>
        /// 服务端生成姜饼人 NPC（客户端禁止直接 NewNPC）。
        /// </summary>
        SpawnBiskety,
        /// <summary>
        /// 客户端 → 服务端：请求下发世界变量整表。
        /// 格式：[enum][myPlayer:int32]
        /// </summary>
        WorldValueRequest,
        /// <summary>
        /// 服务端 → 客户端：权威世界变量整表广播。
        /// 格式：[enum][count:int32][packed bools…]
        /// </summary>
        WorldValue,
        /// <summary>
        /// 客户端 → 服务端：请求修改单个世界 flag（服务端校验后广播）。
        /// 格式：[enum][flagType:int32][value:bool]
        /// </summary>
        WorldFlagChangeRequest,
        /// <summary>
        /// 城镇 NPC 雕像传送客户端特效（服务端权威位移由原版负责）。
        /// 格式：[enum][npcWhoAmI:int32]
        /// </summary>
        TownNPCStatueTeleport,
        /// <summary>
        /// 服务端 → 指定客户端：解锁知识并弹窗。
        /// 格式：[enum][knowledgeId:int32][colorRGBA:4×byte]
        /// </summary>
        UnlockKnowledge,
        /// <summary>
        /// 客户端 → 服务端：请求生成 NPC（由 <see cref="NetAuthority"/> 使用）。
        /// 格式：[enum][x:int32][y:int32][type:int32][start:byte][ai0:float][ai1:float][target:sbyte]
        /// </summary>
        RequestSpawnNPC,
        /// <summary>
        /// 客户端 → 服务端：请求杀死 NPC（由 <see cref="NetAuthority"/> 使用）。
        /// 格式：[enum][whoAmI:int16]
        /// </summary>
        RequestKillNPC,
        /// <summary>
        /// CoralitePlayer 可同步字段广播（tML SendClientChanges / SyncPlayer 模板使用）。
        /// 客户端 → 服务端 → 其它客户端中继。
        /// 格式：[enum][playerWhoAmI:byte][...同步字段，见 CoralitePlayer.WritePlayerSyncFields]
        /// </summary>
        SyncCoralitePlayer,
        /// <summary>
        /// 仅服务端向客户端发送
        /// 生成珊瑚笔记蕴魔空岛章节的石板解锁文字
        /// </summary>
        UnlockSlabs,
    }
}
