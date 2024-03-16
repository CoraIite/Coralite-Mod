using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Tiles.Icicle
{
    public class BabyIceDragonRelic : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.IcicleTiles + "BabyIceDragonRelicPedestal";
        public override string RelicTextureName => AssetDirectory.IcicleTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[]
            {
                new Item(ModContent.ItemType<Items.Icicle.BabyIceDragonRelic>())
            };
        }
    }
}
