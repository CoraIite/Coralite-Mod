using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowCastle
{
    public class MercuryChest : BaseChestItem
    {
        public MercuryChest() : base(Item.sellPrice(0, 0, 0, 10), 
            ItemRarityID.Blue, ModContent.TileType<MercuryChestTile>(), AssetDirectory.ShadowCastleItems)
        { }
    }
}
