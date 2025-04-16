using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineSeaOats1x1 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.AllowLightInWater[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.AnchorValidTiles =
                [
                    ModContent.TileType<SkarnTile>(),
                    ModContent.TileType<SmoothSkarnTile>(),
                    ModContent.TileType<SkarnBrickTile>(),
                    ModContent.TileType<ChalcedonySkarn>(),
                    ModContent.TileType<ChalcedonySmoothSkarn>()]
                ;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;
            DustType = ModContent.DustType<CrystallineSeaOatDust>();
            MinPick = 110;
            AddMapEntry(new Color(181, 91, 235));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.35f;
            b = 0.2f;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!Main.rand.NextBool(10))
                return;

            Tile t = Framing.GetTileSafely(i, j - 1);
            if (t.HasTile)
                return;

            WorldGen.KillTile(i, j, noItem: true);
            WorldGen.PlaceTile(i, j, ModContent.TileType<CrystallineSeaOats1x2>(), true, style: Main.rand.Next(2));
        }
    }

    public class CrystallineSeaOats1x2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.AllowLightInWater[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 2;
            TileObjectData.newTile.AnchorValidTiles =
                [
                    ModContent.TileType<SkarnTile>(),
                    ModContent.TileType<SmoothSkarnTile>(),
                    ModContent.TileType<SkarnBrickTile>(),
                    ModContent.TileType<ChalcedonySkarn>(),
                    ModContent.TileType<ChalcedonySmoothSkarn>()]
                ;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;
            DustType = ModContent.DustType<CrystallineSeaOatDust>();
            MinPick = 110;
            AddMapEntry(new Color(181, 91, 235));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.35f;
            b = 0.2f;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!Main.rand.NextBool(10))
                return;

            Point bottom = Helper.FindTopLeftPoint(i, j) + new Point(0, 1);

            Tile t = Framing.GetTileSafely(bottom.X, bottom.Y - 2);
            if (t.HasTile)
                return;

            //WorldGen.KillTile(bottom.X, bottom.Y, noItem: true);
            for (int k = 0; k < 2; k++)
            {
                if (Main.tile[bottom + new Point(0, -k)].TileType == ModContent.TileType<CrystallineSeaOats1x2>())
                    Main.tile[bottom + new Point(0, -k)].ClearTile();
            }

            WorldGen.PlaceTile(bottom.X, bottom.Y, ModContent.TileType<CrystallineSeaOats1x3>(), true, style: Main.rand.Next(4));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (effectOnly)
                return;

            if (fail)
            {
                Helper.PlayPitched(CoraliteSoundID.Grass, new Vector2(i, j) * 16);
                Point p = Helper.FindTopLeftPoint(i, j);
                WorldGen.KillTile(i, j);


                WorldGen.PlaceTile(p.X, p.Y + 1, (ushort)ModContent.TileType<CrystallineSeaOats1x1>(), true, true);
            }

            if (!noItem)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Point(i, j).ToWorldCoordinates(), ModContent.ItemType<CrystallineSeaOats>());
        }
    }

    public class CrystallineSeaOats1x3 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.AllowLightInWater[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.CoordinateWidth = 20;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 7;
            TileObjectData.newTile.AnchorValidTiles =
                [
                    ModContent.TileType<SkarnTile>(),
                    ModContent.TileType<SmoothSkarnTile>(),
                    ModContent.TileType<SkarnBrickTile>(),
                    ModContent.TileType<ChalcedonySkarn>(),
                    ModContent.TileType<ChalcedonySmoothSkarn>()]
                ;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;
            DustType = ModContent.DustType<CrystallineSeaOatDust>();
            MinPick = 110;
            AddMapEntry(new Color(181, 91, 235));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.35f;
            b = 0.2f;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (effectOnly)
                return;

            if (fail)
            {
                Helper.PlayPitched(CoraliteSoundID.Grass, new Vector2(i, j) * 16);
                Point p = Helper.FindTopLeftPoint(i, j);
                WorldGen.KillTile(i, j);
                WorldGen.PlaceTile(p.X, p.Y + 2, (ushort)ModContent.TileType<CrystallineSeaOats1x1>(), true, true);
            }

            if (!noItem)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Point(i, j).ToWorldCoordinates(), ModContent.ItemType<CrystallineSeaOats>());
        }
    }
}
