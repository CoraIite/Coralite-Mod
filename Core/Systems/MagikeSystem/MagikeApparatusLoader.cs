using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        private static Dictionary<int, Dictionary<MagikeApparatusLevel, int>> _magikeApparatusLevels;
        private static Dictionary<int, Dictionary< int, MagikeApparatusLevel>> _magikeFrameToLevels;

        public static Dictionary<int, Dictionary<MagikeApparatusLevel, int>> MagikeApparatusLevels
        {
            get
            {
                _magikeApparatusLevels ??= new Dictionary<int, Dictionary<MagikeApparatusLevel, int>>();
                return _magikeApparatusLevels;
            }
        }
        public static Dictionary<int, Dictionary<int,MagikeApparatusLevel>> MagikeFrameToLevels
        {
            get
            {
                _magikeFrameToLevels ??= new Dictionary<int, Dictionary< int, MagikeApparatusLevel>>();
                return _magikeFrameToLevels;
            }
        }

        public static void RegisterApparatusLevel(int tileType, params MagikeApparatusLevel[] levels)
        {
            Dictionary<MagikeApparatusLevel, int> keyValuePairs = new Dictionary<MagikeApparatusLevel, int>();
            Dictionary< int, MagikeApparatusLevel> keyValuePairs2 = new ();

            for (int i = 0; i < levels.Length; i++)
            {
                keyValuePairs.Add(levels[i], i);
                keyValuePairs2.Add( i, levels[i]);
            }

            MagikeApparatusLevels.Add(tileType, keyValuePairs);
            MagikeFrameToLevels.Add(tileType, keyValuePairs2);
        }

        /// <summary>
        /// 根据物块帧获取物块类型<br></br>
        /// 物块帧需要自行计算
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="feameX"></param>
        /// <returns></returns>
        public static MagikeApparatusLevel? FrameToLevel(int tileType,int feameX)
        {
            if (!MagikeFrameToLevels.TryGetValue(tileType, out var keyValuePairs))
                return null;

            if (!keyValuePairs.TryGetValue(feameX, out var level))
                return null;

            return level;
        }
    }
}
