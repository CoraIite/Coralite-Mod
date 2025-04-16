using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        private static Dictionary<int, Dictionary<MALevel, int>> _magikeApparatusLevels;
        private static Dictionary<int, Dictionary<int, MALevel>> _magikeFrameToLevels;
        private static Dictionary<MALevel, List<int>> _magikeLevelToType;

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<int, Dictionary<MALevel, int>> MagikeApparatusLevels
        {
            get
            {
                _magikeApparatusLevels ??= new Dictionary<int, Dictionary<MALevel, int>>();
                return _magikeApparatusLevels;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<int, Dictionary<int, MALevel>> MagikeFrameToLevels
        {
            get
            {
                _magikeFrameToLevels ??= new Dictionary<int, Dictionary<int, MALevel>>();
                return _magikeFrameToLevels;
            }
        }
        /// <summary>
        /// 存储等级对应的物块
        /// </summary>
        public static Dictionary<MALevel, List<int>> MagikeLevelToType
        {
            get
            {
                _magikeLevelToType ??= new();
                return _magikeLevelToType;
            }
        }

        public static void RegisterApparatusLevel(int tileType, params MALevel[] levels)
        {
            Dictionary<MALevel, int> keyValuePairs = new();
            Dictionary<int, MALevel> keyValuePairs2 = new();

            for (int i = 0; i < levels.Length; i++)
            {
                MALevel level = levels[i];
                keyValuePairs.Add(level, i);
                keyValuePairs2.Add(i, level);
                if (MagikeLevelToType.TryGetValue(level, out var list))
                    list.Add(tileType);
                else
                    MagikeLevelToType.Add(level, [tileType]);
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
