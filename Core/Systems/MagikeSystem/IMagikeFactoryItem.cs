using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.MagikeSystem
{
    public interface IMagikeFactoryItem
    {
        public string WorkTimeMax { get; }
        public string WorkCost { get; }
    }
}
