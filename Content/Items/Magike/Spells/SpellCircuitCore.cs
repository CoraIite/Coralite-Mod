using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.Magike.Spells
{
    public class SpellCircuitCore():BasePlaceableItem(Item.sellPrice(0,1)
        ,ModContent.RarityType<CrystallineMagikeRarity>(),ModContent.TileType<SpellCircuitCoreTile>(),AssetDirectory.MagikeSeries2Item)
    {

    }

    public class SpellCircuitCoreTile:ModTile
    {
        public override string Texture => AssetDirectory.MagikeSpellTiles+Name;
    }
}
