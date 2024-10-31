using Coralite.Content.Bosses.BabyIceDragon;
using System.IO;

namespace Coralite
{
    public enum CLNetWorkEnum : byte
    {
        Rediancie,
        BabyIceDragon,
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
        }
    }
}
