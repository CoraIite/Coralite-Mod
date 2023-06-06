using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagliteSystem.TileEntities
{
    public abstract class MagliteContainer : ModTileEntity
    {
        public int maglite;
        public readonly int magliteMax;

        public abstract ushort TileType { get; }

        public MagliteContainer(int magliteMax)
        {
            this.magliteMax = magliteMax;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }
    }
}