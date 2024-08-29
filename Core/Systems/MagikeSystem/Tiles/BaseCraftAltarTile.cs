using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseCraftAltarTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        /// <summary>
        /// 悬浮位置相对于中心的偏移
        /// </summary>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public virtual Vector2 GetFloatingOffset(MagikeHelper.MagikeAlternateStyle alternate)
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 静止位置相对于中心的偏移
        /// </summary>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public virtual Vector2 GetRestOffset(MagikeHelper.MagikeAlternateStyle alternate)
        {
            return Vector2.Zero;
        }
    }
}
