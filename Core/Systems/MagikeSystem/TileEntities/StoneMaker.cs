using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class StoneMaker : ItemMaker
    {
        public override int ItemType => ItemID.StoneBlock;

        public StoneMaker(int magikeMax, int workTimeMax, int magikeCost, int stack) : base(magikeMax, workTimeMax, magikeCost, stack)
        {
        }
    }
}
