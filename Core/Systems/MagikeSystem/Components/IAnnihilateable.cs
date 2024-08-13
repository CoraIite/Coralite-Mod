using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IAnnihilateable
    {
        /// <summary>
        /// 在湮灭反应时执行特定工作
        /// </summary>
        void OnAnnihilate();
    }
}
