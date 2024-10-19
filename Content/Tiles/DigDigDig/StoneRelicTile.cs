using Coralite.Content.Items.DigDigDig.Stonelimes;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Tiles.DigDigDig
{
    public class StoneRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.DigDigDigTiles + "StoneRelicPedestal";
        public override string RelicTextureName => AssetDirectory.DigDigDigTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<StoneRelic>())
            ];
        }
    }
}
