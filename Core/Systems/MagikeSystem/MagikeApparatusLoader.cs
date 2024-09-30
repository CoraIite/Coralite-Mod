using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        private static Dictionary<int, Dictionary<MALevel, int>> _magikeApparatusLevels;
        private static Dictionary<int, Dictionary<int, MALevel>> _magikeFrameToLevels;

        public static Dictionary<int, Dictionary<MALevel, int>> MagikeApparatusLevels
        {
            get
            {
                _magikeApparatusLevels ??= new Dictionary<int, Dictionary<MALevel, int>>();
                return _magikeApparatusLevels;
            }
        }
        public static Dictionary<int, Dictionary<int, MALevel>> MagikeFrameToLevels
        {
            get
            {
                _magikeFrameToLevels ??= new Dictionary<int, Dictionary<int, MALevel>>();
                return _magikeFrameToLevels;
            }
        }

        public static void RegisterApparatusLevel(int tileType, params MALevel[] levels)
        {
            Dictionary<MALevel, int> keyValuePairs = new();
            Dictionary<int, MALevel> keyValuePairs2 = new();

            for (int i = 0; i < levels.Length; i++)
            {
                keyValuePairs.Add(levels[i], i);
                keyValuePairs2.Add(i, levels[i]);
            }

            MagikeApparatusLevels.Add(tileType, keyValuePairs);
            MagikeFrameToLevels.Add(tileType, keyValuePairs2);
        }

        /// <summary>
        /// 根据物块帧获取物块类型<br></br>
        /// 物块帧需要自行计算
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="freamX"></param>
        /// <returns></returns>
        public static MALevel? FrameToLevel(int tileType, int freamX)
        {
            if (!MagikeFrameToLevels.TryGetValue(tileType, out var keyValuePairs))
                return null;

            if (!keyValuePairs.TryGetValue(freamX, out var level))
                return null;

            return level;
        }
    }
}
