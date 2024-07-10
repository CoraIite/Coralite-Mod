using Coralite.Content.Items.MagikeSeries1;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class Refinery : ItemMaker
    {
        public override int ItemType => ModContent.ItemType<MagicCrystal>();

        public Refinery(int magikeMax, int workTimeMax, int magikeCost, int stack) : base(magikeMax, workTimeMax, magikeCost, stack)
        {
        }
    }
}
