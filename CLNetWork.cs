using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.Magike.Tools;
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
        }
    }
}
