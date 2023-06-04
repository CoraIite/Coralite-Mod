using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RediancieRelic : BaseBossRelicTile
    {
        public RediancieRelic() : base(ModContent.ItemType<Items.RedJades.RediancieRelic>()) { }

        public override string Texture => AssetDirectory.RedJadeTiles + "RediancieRelicPedestal";
        public override string RelicTextureName => AssetDirectory.RedJadeTiles + Name;
    }
}
