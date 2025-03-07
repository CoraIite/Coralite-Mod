using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText ChippedBladeTemple { get; set; }

        public void GenChippedBladeTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ChippedBladeTemple.Value;

            int itemCount = ValueByWorldSize(1, 2, 3);
            int gened = 0;

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(155, 173, 183)] = TileID.LeadBrick,
                [new Color(7, 60, 49)] = ModContent.TileType<HartcoreObsidianTile>(),
            };

            Dictionary<Color, int> wallDic = new()
            {
                [new Color(77, 146, 185)] = WallID.LeadBrick,
            };

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    int offset = GenVars.dungeonSide;
                    int origin = GenVars.jungleOriginX - (offset * 350);
                    int junglePos = Main.rand.Next(origin - 60, origin + 60);//(GenVars.jungleMaxX + GenVars.jungleMinX) / 2;
                    float r = Math.Abs(junglePos - (Main.maxTilesX / 2));

                    Vector2 pos = new(Main.maxTilesX / 2, (float)Main.worldSurface);
                    float angle = Main.rand.NextFloat(0, MathHelper.Pi);

                    pos += angle.ToRotationVector2() * r;

                    Point position = pos.ToPoint();

                    Dictionary<ushort, int> tileDictionary = new();
                    if (!WorldGen.InWorld(position.X - 25, position.Y - 25) || !WorldGen.InWorld(position.X - 25 + 50, position.Y - 25 + 50))
                        continue;

                    WorldUtils.Gen(
                        new Point(position.X - 25, position.Y - 25),
                        new Shapes.Rectangle(50, 50),
                        new Actions.TileScanner(TileID.Dirt, TileID.Mud, TileID.JungleGrass).Output(tileDictionary));

                    if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Mud] + tileDictionary[TileID.JungleGrass] < 550)
                        continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                    TextureGenerator generator = new TextureGenerator("ChippedBladeTemple",path: AssetDirectory.WorldGen + "CoreKeeper/");

                    position += new Point(-12, -13);
                    if (!WorldGen.InWorld(position.X, position.Y))
                        continue;
                    if (!WorldGen.InWorld(position.X + generator.Width, position.Y + generator.Height))
                        continue;

                    if (!GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, 12 * 2, 14 * 2)))
                        continue;

                    generator.GenerateByTopLeft(position, mainDic, wallDic);

                    //放门

                    Point doorPos = position + new Point(3, 9);
                    WorldGen.PlaceObject(doorPos.X, doorPos.Y, ModContent.TileType<GlowTulipDoorClosed>(), true);
                    doorPos = position + new Point(20, 9);
                    WorldGen.PlaceObject(doorPos.X, doorPos.Y, ModContent.TileType<GlowTulipDoorClosed>(), true);

                    //放置箱子
                    Point chestPos = position + new Point(12, 9);

                    int itemType = ModContent.ItemType<ChippedBlade>();

                    if (WorldGen.AddBuriedChest(chestPos.X, chestPos.Y, itemType,
                         notNearOtherChests: false, 0, trySlope: false, (ushort)ModContent.TileType<AncientChestTile>()))
                    {
                        int index = Chest.FindChest(chestPos.X - 1, chestPos.Y);
                        if (index < 0)
                            goto placeover;
                        Chest chest = Main.chest[index];
                        foreach (var item in chest.item)
                            if (item.IsAir)
                            {
                                item.SetDefaults(ModContent.ItemType<AncientGemstone>());
                                item.stack = Main.rand.Next(4, 8);
                                break;
                            }
                    }

                placeover:
                    GenVars.structures.AddProtectedStructure(new Rectangle(position.X, position.Y, 29 * 2, 33 * 2), 3);

                    progress.Set(i / (float)itemCount);
                    gened++;
                    if (gened >= itemCount)
                        break;
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
