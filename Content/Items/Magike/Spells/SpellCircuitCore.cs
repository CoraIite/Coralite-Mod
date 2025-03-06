using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.Magike.Spells
{
    /// <summary>
    /// 法术回路核心，用于构建法术回路
    /// </summary>
    public class SpellCircuitCore() : BasePlaceableItem(Item.sellPrice(0, 1)
        , ModContent.RarityType<CrystallineMagikeRarity>(), ModContent.TileType<SpellCircuitTile>(), AssetDirectory.MagikeSpells)
    {

    }

    public class SpellCircuitCoreTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSpellTiles + Name;
    }
}
