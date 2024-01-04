using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Terraria.ID;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void GenChippedBladeTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在生成破碎剑刃神庙";

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

            Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> mainDic = new Dictionary<Color, int>()
            {
                [new Color(155, 173, 183)] = TileID.LeadBrick,
                [new Color(7, 60, 49)] = ModContent.TileType<HartcoreObsidianTile>(),
                [Color.Black] = -1
            };

            Dictionary<Color, int> wallDic = new Dictionary<Color, int>()
            {
                [new Color(77, 146, 185)] = WallID.LeadBrick,
                [Color.Black] = -1
            };

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    int offset = GenVars.dungeonSide;
                    int origin = GenVars.jungleOriginX - offset * 250;
                    int junglePos = Main.rand.Next(origin - 60, origin + 60);//(GenVars.jungleMaxX + GenVars.jungleMinX) / 2;
                    float r = Math.Abs(junglePos - Main.maxTilesX / 2);

                    Vector2 pos = new Vector2(Main.maxTilesX / 2, (float)Main.worldSurface);
                    float angle = Main.rand.NextFloat(0, MathHelper.Pi);

                    pos += angle.ToRotationVector2() * r;

                    Point position = pos.ToPoint();

                    Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
                    WorldUtils.Gen(
                        new Point(position.X - 25, position.Y - 25),
                        new Shapes.Rectangle(50, 50),
                        new Actions.TileScanner(TileID.Dirt, TileID.Mud, TileID.JungleGrass).Output(tileDictionary));

                    if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Mud] + tileDictionary[TileID.JungleGrass] < 750)
                        continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                    Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "CoreKeeper/ChippedBladeTemple", AssetRequestMode.ImmediateLoad).Value;
                    Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "CoreKeeper/ChippedBladeTempleClear", AssetRequestMode.ImmediateLoad).Value;
                    Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "CoreKeeper/ChippedBladeTempleWall", AssetRequestMode.ImmediateLoad).Value;

                    position += new Point(-12, -13);
                    if (!WorldGen.InWorld(position.X, position.Y))
                        continue;
                    if (!GenVars.structures.CanPlace(new Rectangle(position.X, position.Y, 12 * 2, 14 * 2)))
                        continue;

                    Task.Run(async () =>
                    {
                        await GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, position.X, position.Y);
                    }).Wait();

                    //放门

                    Point doorPos = position + new Point(3,9);
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
