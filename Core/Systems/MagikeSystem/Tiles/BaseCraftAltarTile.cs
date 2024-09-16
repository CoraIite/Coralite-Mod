using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;

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
        public virtual Vector2 GetFloatingOffset(float rotation, MagikeApparatusLevel level)
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 静止位置相对于中心的偏移
        /// </summary>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public virtual Vector2 GetRestOffset(float rotation, MagikeApparatusLevel level)
        {
            return Vector2.Zero;
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity, MagikeApparatusLevel level)
        {
            if (!entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;


        }
    }
}
