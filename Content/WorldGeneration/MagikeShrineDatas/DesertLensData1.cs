using InnoVault.GameSystem;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration.MagikeShrineDatas
{
    public class DesertLensData1 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "DesertLensData1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/DesertLensData1_v1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(4495, 494).GetRectangleFromPoints(new Point16(4524, 531)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            Dictionary<ushort, int> tileDictionary = new();

            for (int k = 0; k < 100000; k++)//尝试100000次
            {
                try
                {
                    int x = Main.rand.Next(200, Main.maxTilesX - 200);
                    int y = Main.rand.Next((int)Main.worldSurface, Main.maxTilesY - 400);

                    Tile t = Main.tile[x, y];

                    if (t.TileType != TileID.Sandstone)
                        continue;


                    if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x + region.Size.X, y + region.Size.Y))
                        continue;

                    WorldUtils.Gen(
                        new Point(x, y),
                        new Shapes.Rectangle(new Rectangle(-20, -20, 40, 40)),
                        new Actions.TileScanner(TileID.Sand, TileID.HardenedSand, TileID.Sandstone).Output(tileDictionary));

                    int tileCount = 0;

                    foreach (var item in tileDictionary)
                        tileCount += item.Value;

                    tileDictionary.Clear();
                    if (tileCount < 20 * 20)
                        continue; //如果物块太少那就换个地方

                    region.ApplyToWorld((short)(x - region.Size.X / 2), (short)(y - region.Size.Y + 3));
                    break;
                }
                catch
                {

                }
            }

            TagCache.Invalidate(SavePath);//释放缓存
        }

    }
}
