using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineStalactite2x2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorBottom = Terraria.DataStructures.AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.SolidBottom | Terraria.Enums.AnchorType.SolidTile, 2, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.PurpleTorch;
            AddMapEntry(Coralite.CrystallineMagikePurple);

            MinPick = 150;
            MineResist = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<CrystallineMagike>(),2)
            ];
        }
    }

    public class CrystallineStalactite : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = Terraria.DataStructures.AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.SolidBottom | Terraria.Enums.AnchorType.SolidTile, 2, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.CoordinateHeights = [18];
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.PurpleTorch;
            AddMapEntry(Coralite.CrystallineMagikePurple);

            MinPick = 150;
            MineResist = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<CrystallineMagike>())
            ];
        }
    }
}
