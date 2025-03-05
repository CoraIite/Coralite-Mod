using Coralite.Core.Systems.MagikeSystem.Spells;

namespace Coralite.Content.Items.Magike.Spells
{
    public class LightSoulStruct : SpellStructure
    {
        public override void Load()
        {
            ushort notActive = (ushort)ModContent.TileType<LightSoulCatalystTile>();
            ushort active = (ushort)ModContent.TileType<LightSoulNode>();

            AddSpellShape(notActive, active
                , new Point(0, -8), new Point(2, -2)
                , new Point(8, 0), new Point(2, 2)
                , new Point(0, 8), new Point(-2, 2)
                , new Point(-8, 0), new Point(-2, -2)
                );
        }
    }
}
