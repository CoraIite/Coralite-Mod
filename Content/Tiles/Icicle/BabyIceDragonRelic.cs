using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Icicle
{
    public class BabyIceDragonRelic: BaseBossRelicTile
    {
        public BabyIceDragonRelic() : base(ModContent.ItemType<Items.Icicle.BabyIceDragonRelic>()) { }

        public override string Texture => AssetDirectory.IcicleTiles + "BabyIceDragonRelicPedestal";
        public override string RelicTextureName => AssetDirectory.IcicleTiles + Name;
    }
}
