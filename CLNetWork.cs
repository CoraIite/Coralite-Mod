using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.Magike.Tools;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using System.IO;

namespace Coralite
{
    public enum CLNetWorkEnum : byte
    {
        BabyIceDragon,
        PlaceFilter,
        FilterRemoveButton_LeftClick,
        ClusterWand,
        BrilliantConnectStaff_Sender,
        BrilliantConnectStaff_Receivers,
    }

    internal class CLNetWork
    {
        public static void NetWorkHander(BinaryReader reader, int whoAmI)
        {
            CLNetWorkEnum cLNetWorkEnum = (CLNetWorkEnum)reader.ReadByte();
            if (cLNetWorkEnum == CLNetWorkEnum.BabyIceDragon)
            {
                BabyIceDragon.FumlerMovesRemove(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.PlaceFilter)
            {
                FilterProj.Hander_PlaceFilter(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.FilterRemoveButton_LeftClick)
            {
                FilterRemoveButton.Hander_LeftClick_Data(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.ClusterWand)
            {
                InfinityClusterWandProj.Hander_ClusterWand(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.BrilliantConnectStaff_Sender)
            {
                BrilliantConnectStaffProj.Hander_Sender(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.BrilliantConnectStaff_Receivers)
            {
                BrilliantConnectStaffProj.Hander_Receivers(reader, whoAmI);
            }
        }
    }
}
