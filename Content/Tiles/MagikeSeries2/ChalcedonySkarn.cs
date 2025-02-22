using Coralite.Content.CustomHooks;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    [AutoLoadTexture(Path = AssetDirectory.MagikeSeries2Tile)]
    public class ChalcedonySkarn : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public static ATex ChalcedonyMoss { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<SkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<SkarnTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<SmoothSkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<SmoothSkarnTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<CrystallineSkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<CrystallineSkarnTile>()][Type] = true;

            //Main.tileMerge[TileID.Dirt][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            DustType = DustID.BorealWood_Small;
            HitSound = CoraliteSoundID.Grass;
            AddMapEntry(new Color(122, 144, 151));
        }

        public override bool Slope(int i, int j)
        {
            return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = Main.tile[i, j];

            int xFrame = t.TileFrameX / 18;
            int yFrame = t.TileFrameY / 18;
            if (yFrame == 1 && xFrame > 0 && xFrame < 4)
            {
                Main.tile[i, j].ResetToType((ushort)ModContent.TileType<SkarnTile>());//被包裹在内部的时候转变为矽卡岩
                WorldGen.TileFrame(i, j, true, true);
            }
            //if (closer)
            //{
            //    Drawers.SpecialTiles.Add(new Point(i, j));
            //}
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (ShouldDrawOnTop(i,j))//只有在顶部和侧面的时候才绘制在图层上方
                Drawers.AddSpecialTile(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            SpecialDrawMoss(i, j, Vector2.Zero, spriteBatch);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (ShouldDrawOnTop(i, j))
                return true;

            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            SpecialDrawMoss(i, j,offScreen , spriteBatch);

            return true;
        }

        private static void SpecialDrawMoss(int i, int j, Vector2 offScreen,SpriteBatch spriteBatch)
        {
            Texture2D tex = ChalcedonyMoss.Value;
            Vector2 screenPosition = Main.Camera.UnscaledPosition;

            Vector2 pos = new Vector2(i, j).ToWorldCoordinates() + offScreen - screenPosition;

            int totalPushTime = 60;
            float pushForcePerFrame = 1.26f;

            float windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, CoraliteTileDrawing.sunflowerWindCounter);
            float highestWindGridPushComplex = 0f;
            if (CoraliteTileDrawing.GetHighestWindGridPushComplex != null)
                highestWindGridPushComplex = CoraliteTileDrawing.GetHighestWindGridPushComplex(Main.instance.TilesRenderer, i, j, 1, 1, totalPushTime, pushForcePerFrame, 3, true);

            windCycle += highestWindGridPushComplex;

            Tile t = Main.tile[i, j];
            int xFrame = t.TileFrameX / 18;
            int yFrame = t.TileFrameY / 18;

            float n2 = 0.1f;

            n2 *= -1;

            float rotation = windCycle * n2;

            Rectangle frameBox = tex.Frame(13, 5, xFrame, yFrame);

            int i2 = i;
            int j2 = j;

            if (yFrame == 1)
            {
                if (xFrame > 5 && xFrame < 9)
                    j2 -= 1;
            }
            else if (yFrame == 2)
            {
                if (xFrame > 5 && xFrame < 9)
                    j2 += 1;
            }

            if (xFrame == 10)
            {
                if (yFrame < 3)
                    i2 -= 1;
            }
            else if (xFrame == 11)
            {
                if (yFrame < 3)
                    i2 += 1;
            }

            Color color = Lighting.GetColor(i2, j2);
            spriteBatch.Draw(tex, pos, frameBox, color
                  , rotation, frameBox.Size() / 2, 1f, 0, 0f);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ModContent.ItemType<Skarn>())];
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (effectOnly)
                return;

            if (fail)
            {
                Helper.PlayPitched(CoraliteSoundID.Grab, new Vector2(i, j) * 16);
                WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<SkarnTile>(), true, true);
            }
            //Main.tile[i, j].ResetToType((ushort)ModContent.TileType<SkarnTile>());
        }

        public bool ShouldDrawOnTop(int i, int j)
        {
            Tile t = Main.tile[i, j];
            int xFrame = t.TileFrameX / 18;
            int yFrame = t.TileFrameY / 18;

            switch (yFrame)
            {
                default:
                    break;
                case 0:
                    if (xFrame < 10 || xFrame == 12)
                        return true;
                    break;
                case 1:
                    if (xFrame == 0 || xFrame > 3)
                        return true;
                    break;
                case 2:
                    if (xFrame == 0 || xFrame == 4 || xFrame == 5 || xFrame > 8)
                        return true;
                    break;
                case 3:
                    if (xFrame < 6 || xFrame > 8)
                        return true;
                    break;
            }

            return false;
        }
    }
}