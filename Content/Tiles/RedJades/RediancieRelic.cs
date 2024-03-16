using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Tiles.RedJades
{
    public class RediancieRelic : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + "RediancieRelicPedestal";
        public override string RelicTextureName => AssetDirectory.RedJadeTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[]
            {
                new Item(ModContent.ItemType<Items.RedJades.RediancieRelic>())
            };
        }
    }
}
