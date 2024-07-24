using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        private static Dictionary<int, Dictionary<MagikeApparatusLevel, int>> _magikeApparatusLevels;

        public static Dictionary<int, Dictionary<MagikeApparatusLevel, int>> MagikeApparatusLevels
        {
            get
            {
                _magikeApparatusLevels??=new Dictionary<int, Dictionary<MagikeApparatusLevel, int>>();
                return _magikeApparatusLevels;
            }
        }

        public static void RegisterApparatusLevel(int tileType, params MagikeApparatusLevel[] levels)
        {
            Dictionary<MagikeApparatusLevel, int> keyValuePairs = new Dictionary<MagikeApparatusLevel, int>();

            for (int i = 0; i < levels.Length; i++)
                keyValuePairs.Add(levels[i], i);

            MagikeApparatusLevels.Add(tileType, keyValuePairs);
        }
    }
}
