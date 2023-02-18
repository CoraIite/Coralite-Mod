using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeTable : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + Name;

        public override void SetStaticDefaults()
        {
            this.TablePrefab(DustID.GemRuby, "赤玉桌子", Coralite.Instance.RedJadeRed);
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int x, int y, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 48, 32, ModContent.ItemType<Items.RedJadeItems.RedJadeTable>());
        }
    }
}
