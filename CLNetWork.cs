using System.IO;

namespace Coralite
{
    public enum CLNetWorkEnum : byte
    {
        Rediancie,
    }

    internal class CLNetWork
    {
        public static void NetWorkHander(BinaryReader reader, int whoAmI)
        {
            CLNetWorkEnum cLNetWorkEnum = (CLNetWorkEnum)reader.ReadByte();
            if (cLNetWorkEnum == CLNetWorkEnum.Rediancie)
            {

            }
        }
    }
}
