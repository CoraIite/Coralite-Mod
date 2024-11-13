using Coralite.Content.Bosses.BabyIceDragon;
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
        }
    }
}
