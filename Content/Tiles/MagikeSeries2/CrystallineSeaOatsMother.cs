using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineSeaOatsMother : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorValidTiles =
                [
                    ModContent.TileType<SkarnTile>(),
                    ModContent.TileType<SmoothSkarnTile>(),
                    ModContent.TileType<SkarnBrickTile>(),
                    ModContent.TileType<ChalcedonySkarn>(),
                    ModContent.TileType<ChalcedonySmoothSkarn>()]
                ;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.Grass;
            AddMapEntry(new Color(181, 91, 235), CreateMapEntryName());
            MinPick = 110;

            RegisterItemDrop(ModContent.ItemType<CrystallineSeaOatsSeed>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.15f;
            g = 0.3f;
            b = 0.5f;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!Main.rand.NextBool(9))
                return;

            Point p = new Point(i, j) + new Point(Main.rand.Next(-10, 11), Main.rand.Next(-6, 3));

            Tile t = Framing.GetTileSafely(p);
            if (!t.HasTile)
                return;

            if (t.TileType != ModContent.TileType<SkarnTile>() && t.TileType != ModContent.TileType<SmoothSkarnTile>())
                return;

            Tile up = Framing.GetTileSafely(p + new Point(0, -1));
            if (up.HasTile || up.LiquidType != LiquidID.Water || up.LiquidAmount < 255)
            {
                return;
            }

            WorldGen.PlaceTile(p.X, p.Y - 1, ModContent.TileType<CrystallineSeaOats1x1>(), true, style: Main.rand.Next(3));
        }
    }
}
