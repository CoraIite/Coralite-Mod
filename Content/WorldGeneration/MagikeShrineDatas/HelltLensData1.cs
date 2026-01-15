using Coralite.Helpers;
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
    public class HelltLensData1 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "HelltLensData1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/HelltLensData1_v1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(3522, 2199).GetRectangleFromPoints(new Point16(3590, 2261)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            Dictionary<ushort, int> tileDictionary = new();

            for (int k = 0; k < 500000; k++)//尝试500000次
            {
                try
                {
                    int x = Main.rand.Next(200, Main.maxTilesX - 200);
                    int y = Main.rand.Next(Main.maxTilesY - 200, Main.maxTilesY - 100);

                    Tile t = Main.tile[x, y];

                    if (!t.HasTile || (t.TileType != TileID.Ash && t.TileType != TileID.AshGrass))
                        continue;
                    if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x + region.Size.X, y + region.Size.Y))
                        continue;

                    for (int i = 1; i < 5; i++)
                    {
                        if (Main.tile[x, y - i].HasSolidTile())
                            goto retry;
                    }

                    WorldUtils.Gen(
                        new Point(x, y),
                        new Shapes.Rectangle(new Rectangle(-30, -10, 60, 20)),
                        new Actions.TileScanner(TileID.Ash, TileID.ObsidianBrick, TileID.AshGrass, TileID.HellstoneBrick).Output(tileDictionary));

                    if (tileDictionary[TileID.ObsidianBrick] > 20 || tileDictionary[TileID.HellstoneBrick] > 20)
                    {
                        tileDictionary.Clear();
                        continue;
                    }

                    //int tileCount = 0;

                    //foreach (var item in tileDictionary)
                    //    tileCount += item.Value;

                    //tileDictionary.Clear();
                    //if (tileCount < 10 * 10)
                    //    continue; //如果物块太少那就换个地方

                    region.ApplyToWorld((short)(x - region.Size.X / 2), (short)(y - region.Size.Y + 20));
                    break;
                }
                catch
                {
                    "地狱遗迹生成失败！".Dump();
                }

            retry:
                ;
            }

            TagCache.Invalidate(SavePath);//释放缓存
        }

    }
}
