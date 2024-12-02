using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.Magike.Tools;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using System.IO;

namespace Coralite
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
        /// 同步魔能的改动
        /// 
        /// </summary>
        MagikeSystem,
    }

    public class CoraliteNetWork
    {
        public static void NetWorkHander(BinaryReader reader, int whoAmI)
        {
            CoraliteNetWorkEnum coraliteNetWorkEnum = (CoraliteNetWorkEnum)reader.ReadByte();

            switch (coraliteNetWorkEnum)
            {
                case CoraliteNetWorkEnum.BabyIceDragon:
                    BabyIceDragon.FumlerMovesRemove(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.PlaceFilter:
                    FilterProj.Hander_PlaceFilter(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.FilterRemoveButton_LeftClick:
                    FilterRemoveButton.Hander_LeftClick_Data(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.ClusterWand:
                    InfinityClusterWandProj.Hander_ClusterWand(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.BrilliantConnectStaff_Sender:
                    BrilliantConnectStaffProj.Hander_Sender(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.BrilliantConnectStaff_Receivers:
                    BrilliantConnectStaffProj.Hander_Receivers(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.ItemContainer_SpecificIndex:
                    ItemContainer.ReceiveSpecificItem(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.MagikeSystem:
                    MagikeSystem.ReceiveMagikePack(reader);
                    break;
                default:
                    return;
            }
        }
    }
}
