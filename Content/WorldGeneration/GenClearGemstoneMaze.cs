using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText ClearGemstoneMaze { get; set; }

        public void GenClearGemstoneMaze(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ClearGemstoneMaze.Value;//"正在生成透明宝石迷宫";

            int itemCount = 1;
            int gened = 0;

            if (Main.maxTilesX > 8000)
            {
                itemCount++;
            }

            if (Main.maxTilesX > 6000)
            {
                itemCount++;
            }

            //if (itemCount > heartCount)
            //    itemCount = heartCount;

            Dictionary<Color, int> clearDic = new()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(102, 57, 49)] = TileID.Dirt,
                [new Color(7, 60, 49)] = ModContent.TileType<HartcoreObsidianTile>(),
                [Color.Black] = -1
            };

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    int offset = GenVars.dungeonSide;
                    int origin = GenVars.jungleOriginX - (offset * 50);
                    int junglePos = Main.rand.Next(origin - 20, origin + 20);//(GenVars.jungleMaxX + GenVars.jungleMinX) / 2;

                    float r = Math.Abs(junglePos - (Main.maxTilesX / 2));

                    Vector2 pos = new(Main.maxTilesX / 2, (float)Main.worldSurface);
                    float angle = Main.rand.NextFloat(0, MathHelper.Pi);

                    pos += angle.ToRotationVector2() * r;

                    Point position = pos.ToPoint();

                    Dictionary<ushort, int> tileDictionary = new();
                    if (!WorldGen.InWorld(position.X - 25, position.Y - 25) || !WorldGen.InWorld(position.X - 25 + 58, position.Y - 25 + 67))
                        continue;
                    WorldUtils.Gen(
                        new Point(position.X - 25, position.Y - 25),
                        new Shapes.Rectangle(58, 67),
                        new Actions.TileScanner(TileID.Dirt, TileID.Mud, TileID.JungleGrass).Output(tileDictionary));

                    if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Mud] + tileDictionary[TileID.JungleGrass] < 850)
                        continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                    int whichOne = WorldGen.genRand.Next(2);
                    Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "CoreKeeper/ClearGemstoneMaze" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                    Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "CoreKeeper/ClearGemstoneMazeClear0", AssetRequestMode.ImmediateLoad).Value;

                    position += new Point(-29, -33);
                    if (!WorldGen.InWorld(position.X, position.Y))
                        continue;
                    if (!WorldGen.InWorld(position.X + shrineTex.Width, position.Y + shrineTex.Height))
                        continue;

                    if (!GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, 29 * 2, 33 * 2)))
                        continue;

                    Task.Run(async () =>
                    {
                        await GenIceNestWithTex(clearTex, shrineTex, clearDic, mainDic, position.X, position.Y);
                    }).Wait();

                    //放置箱子
                    Point chestPos = position + new Point(29, 34);

                    int itemType = ModContent.ItemType<ClearGemstone>();

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
