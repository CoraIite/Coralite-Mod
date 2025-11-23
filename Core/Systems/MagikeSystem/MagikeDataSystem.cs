using System.Collections.Generic;
using System.Collections.Specialized;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        /// <summary>
        /// 存储了各种可升级仪器的信息<br></br>
        /// 键名为：仪器名+组件名+变量名，获取到的字典使用等级名获取对应值
        /// </summary>
        internal static Dictionary<string, HybridDictionary> MagikeApparatusData { get; set; }


    }
}
