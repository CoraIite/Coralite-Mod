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
    public class GlowMushroomLensData1 : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "GlowMushroomLensData1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/GlowMushroomLensData1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(4148, 494).GetRectangleFromPoints(new Point16(4211, 544)));
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

                    if (t.TileType != TileID.MushroomGrass)
                        continue;
                    if (Main.tile[x, y - 1].HasReallySolidTile())
                        continue;


                    if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x + region.Size.X, y + region.Size.Y))
                        continue;

                    WorldUtils.Gen(
                        new Point(x, y),
                        new Shapes.Rectangle(new Rectangle(-20, -5, 40, 20)),
                        new Actions.TileScanner(TileID.Mud, TileID.MushroomGrass).Output(tileDictionary));

                    if (tileDictionary[TileID.MushroomGrass] < 10)
                    {
                        tileDictionary.Clear();
                        continue;
                    }

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
