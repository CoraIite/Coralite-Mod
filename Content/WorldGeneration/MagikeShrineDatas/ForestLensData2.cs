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
    public class ForestLensData2 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "ForestLensData2.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/ForestLensData2.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(7284, 383).GetRectangleFromPoints(new Point16(7308, 444)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            Dictionary<ushort, int> tileDictionary = new();

            for (int k = 0; k < 8000; k++)//尝试8000次
            {
                try
                {
                    int x = Main.rand.Next(200, Main.maxTilesX - 200);

                    if (Math.Abs(Main.spawnTileX - x) < 20)
                        continue;

                    int y = WorldGenHelper.GoUpAndFindGround(x, (int)(Main.worldSurface - 60), out int tileType);

                    if (tileType != TileID.Grass)
                        continue;

                    if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x + region.Size.X, y + region.Size.Y))
                        continue;

                    WorldUtils.Gen(
                        new Point(x, y),
                        new Shapes.Rectangle(new Rectangle(-14, -5, 28, 20)),
                        new Actions.TileScanner(TileID.Dirt, TileID.Grass, TileID.ClayBlock).Output(tileDictionary));

                    if (tileDictionary[TileID.Grass] < 6)
                    {
                        tileDictionary.Clear();
                        continue;
                    }

                    int tileCount = 0;

                    foreach (var item in tileDictionary)
                        tileCount += item.Value;

                    tileDictionary.Clear();
                    if (tileCount < 20 * 13)
                        continue; //如果物块太少那就换个地方

                    region.ApplyToWorld((short)(x - region.Size.X / 2), (short)(y - region.Size.Y + 22));
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
