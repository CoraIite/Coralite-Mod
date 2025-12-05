using InnoVault.GameSystem;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.WorldGeneration.MagikeShrineDatas
{
    public class OceanLensData1 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "OceanLensData1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/OceanLensData1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(8181, 488).GetRectangleFromPoints(new Point16(8214, 519)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            int center_x = 100;
            int center_y = (int)(Main.worldSurface * 0.4f);

            for (; center_y < Main.worldSurface; center_y++)
            {
                Tile tile = Framing.GetTileSafely(center_x, center_y);
                if (tile.HasTile && tile.TileType != TileID.Cloud
                    && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                    && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt
                    && tile.TileType != TileID.Grass && !TileID.Sets.Ore[tile.TileType])
                    break;
            }

            region.ApplyToWorld((short)(center_x - region.Size.X / 2), (short)(center_y - region.Size.Y + 16));

            TagCache.Invalidate(SavePath);//释放缓存
        }

    }
}
