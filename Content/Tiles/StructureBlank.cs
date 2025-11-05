using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles
{
    /// <summary>
    /// 结构空位，在生成世界后应该把它清除
    /// </summary>
    public class StructureBlank : ModTile
    {
        public override string Texture => AssetDirectory.Tiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Transparent);
        }

        //运行时给它干掉
#if !DEBUG
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Main.tile[i, j].ClearTile();
        }
#endif
    }
}
