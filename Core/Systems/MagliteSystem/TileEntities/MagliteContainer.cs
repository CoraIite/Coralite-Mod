using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagliteSystem.TileEntities
{
    public class MagliteContainer:ModTileEntity
    {
        public int maglite;
        public readonly int magliteMax;
        
        public MagliteContainer(int magliteMax)
        {
            this.magliteMax = magliteMax;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return false;
        }
    }
}