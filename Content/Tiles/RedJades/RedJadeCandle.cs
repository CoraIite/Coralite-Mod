using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeCandle:ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles+Name;

        public override void SetStaticDefaults()
        {
            this.CandlePrefab(ModContent.ItemType<Items.RedJadeItems.RedJadeCandle>(),"赤玉桌灯", DustID.GemRuby, Coralite.Instance.RedJadeRed);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1.9f;
            g = 0.8f;
            b = 0.8f;
        }
    }
}
