using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem.Spells;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Spells
{
    public class LightSoulCatalyst() : BasePlaceableItem(Item.sellPrice(0, 0, 10), ItemRarityID.LightRed
        , ModContent.TileType<LightSoulCatalystTile>(), AssetDirectory.MagikeSpells)
    {
    }

    public class LightSoulCatalystTile() : Base1x1FloatTile(Color.Pink, DustID.CrystalSerpent_Pink, AssetDirectory.MagikeSpellTiles)
    {
    }

    public class LightSoulNode() : SpellTile(1, 1, Color.Pink, DustID.CrystalSerpent_Pink)
    {
        public override int DropItemType => ModContent.ItemType<LightSoulCatalyst>();
    }

    public class LightSoulNodeEntity : SpellNodeEntity<LightSoulNode>
    {
        public override SpellConnector GetStartConnector()
            => new SpellConnector(16 * 20, 1, Color.Pink);
    }
}
