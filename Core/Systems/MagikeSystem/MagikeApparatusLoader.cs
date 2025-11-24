using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        private static Dictionary<int, List<ushort>> _magikeApparatusLevels;
        private static Dictionary<ushort, List<int>> _magikeLevelToType;

        /// <summary>
        /// 魔能仪器等级
        /// </summary>
        public static Dictionary<int, List< ushort>> MagikeApparatusLevels
        {
            get
            {
                _magikeApparatusLevels ??= new Dictionary<int, List<ushort>>();
                return _magikeApparatusLevels;
            }
        }

        /// <summary>
        /// 存储等级对应的物块
        /// </summary>
        public static Dictionary<ushort, List<int>> MagikeLevelToType
        {
            get
            {
                _magikeLevelToType ??= new();
                return _magikeLevelToType;
            }
        }

        /// <summary>
        /// 注册仪器的魔能等级
        /// </summary>
        /// <param name="tileType"></param>
        /// <param name="levels"></param>
        public static void RegisterApparatusLevel(int tileType, params List<ushort> levels)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                ushort level = levels[i];
                if (MagikeLevelToType.TryGetValue(level, out var list))
                    list.Add(tileType);
                else
                    MagikeLevelToType.Add(level, [tileType]);
            }

            MagikeApparatusLevels.Add(tileType, levels);
        }
    }
}
