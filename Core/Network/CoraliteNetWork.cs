using Coralite.Content.Biskety;
using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.Magike.Tools;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.WorldValueSystem;
using System.IO;

namespace Coralite.Core.Network
{
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
                case CoraliteNetWorkEnum.ItemContainer:
                    ItemContainer.ReceiveItem(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.MagikeSystem:
                    MagikeSystem.ReceiveMagikePack(reader);
                    break;
                case CoraliteNetWorkEnum.KillBiskety:
                    BisketyHead.KillBiskety();
                    break;
                case CoraliteNetWorkEnum.SpawnBiskety:
                    BisketyHead.SpawnBiskety(reader, whoAmI);
                    break;
                case CoraliteNetWorkEnum.WorldValueRequest:
                    WorldValueSystem.ServerHandleWorldValueRequest(reader);
                    break;
                case CoraliteNetWorkEnum.WorldValue:
                    WorldValueSystem.ReceiveWorldValue(reader);
                    break;
                default:
                    return;
            }
        }
    }
}
