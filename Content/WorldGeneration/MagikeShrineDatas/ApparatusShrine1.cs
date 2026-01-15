using InnoVault.GameSystem;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration.MagikeShrineDatas
{
    public class ApparatusShrine1 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "ApparatusShrine1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/ApparatusShrine1_v1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(6127, 1213).GetRectangleFromPoints(new Point16(6156, 1249)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            Dictionary<ushort, int> tileDictionary = new();

            for (int k = 0; k < 3000; k++)//尝试3000次
            {
                try
                {
                    int x = Main.rand.Next(200, Main.maxTilesX - 200);

                    if (Math.Abs(Main.spawnTileX - x) < 20)
                        continue;

                    int y = Main.rand.Next((int)Main.worldSurface, Main.maxTilesY - 200);

                    if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x + region.Size.X, y + region.Size.Y))
                        continue;

                    WorldUtils.Gen(
                        new Point(x, y),
                        new Shapes.Rectangle(new Rectangle(-10, -20, 20, 40)),
                        new Actions.TileScanner(TileID.Stone).Output(tileDictionary));

                    if (tileDictionary[TileID.Stone] < 450)
                    {
                        tileDictionary.Clear();
                        continue;
                    }

                    region.ApplyToWorld((short)(x - region.Size.X / 2), (short)(y - region.Size.Y));
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
