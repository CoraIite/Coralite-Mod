using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Core.Systems.MagikeSystem.Components;
using System.IO;

namespace Coralite
{
    public enum CLNetWorkEnum : byte
    {
        Rediancie,
        BabyIceDragon,
        MagikeApparatusPanel_ItemContainerSlot,
    }

    internal class CLNetWork
    {
        public static void NetWorkHander(BinaryReader reader, int whoAmI)
        {
            CLNetWorkEnum cLNetWorkEnum = (CLNetWorkEnum)reader.ReadByte();
            if (cLNetWorkEnum == CLNetWorkEnum.Rediancie)
            {

            }
            else if (cLNetWorkEnum == CLNetWorkEnum.BabyIceDragon)
            {
                BabyIceDragon.FumlerMovesRemove(reader, whoAmI);
            }
            else if (cLNetWorkEnum == CLNetWorkEnum.MagikeApparatusPanel_ItemContainerSlot)
            {
                ItemContainerSlot.ReceiveData(reader, whoAmI);
            }
        }
    }
}
