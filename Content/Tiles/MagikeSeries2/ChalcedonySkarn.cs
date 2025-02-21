using Coralite.Content.CustomHooks;
using Coralite.Core;
using Coralite.Core.Attributes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    [AutoLoadTexture(Path = AssetDirectory.MagikeSeries2Tile)]
    public class ChalcedonySkarn : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + "SkarnTile";

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

        //public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        //{
        //    Drawers.SpecialTiles.Add(new Point(i, j));

        //    return base.PreDraw(i, j, spriteBatch);
        //}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            //if (closer)
            //{
            //    Drawers.SpecialTiles.Add(new Point(i, j));
            //}
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Drawers.AddSpecialTile(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ChalcedonyMoss.Value;
            Vector2 screenPosition = Main.Camera.UnscaledPosition;

            //Vector2 offScreen = new(Main.offScreenRange);
            //if (Main.drawToScreen)
            //    offScreen = Vector2.Zero;

            Vector2 pos = new Vector2(i, j).ToWorldCoordinates() /*+offScreen*/- screenPosition;

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

            Rectangle frameBox = tex.Frame(16, 15, xFrame, yFrame);

            spriteBatch.Draw(tex, pos, frameBox, Lighting.GetColor(i, j)
                  , rotation, frameBox.Size() / 2, 1f, 0, 0f);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<SkarnTile>(), true, true);
            //Main.tile[i, j].ResetToType((ushort)ModContent.TileType<SkarnTile>());
        }
    }
}