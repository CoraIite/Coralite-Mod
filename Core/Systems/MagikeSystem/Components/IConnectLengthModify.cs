using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IConnectLengthModify
    {
        public int LengthBase { get; set; }
        /// <summary> 额外连接距离 </summary>
        public int LengthExtra { get; set; }
    }
}
