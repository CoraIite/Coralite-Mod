using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderveinDragonRelic : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + "ThunderveinDragonRelicPedestal";
        public override string RelicTextureName => AssetDirectory.ThunderTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[]
            {
                new Item(ModContent.ItemType<Items.Thunder.ThunderveinDragonRelic>())
            };
        }
    }
}
