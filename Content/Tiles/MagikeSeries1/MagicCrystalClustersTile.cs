using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class MagicCrystalClustersTile : ModTile, ICrystalCluster
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public MALevel Level => MALevel.MagicCrystal;
        public int ItemType => ModContent.ItemType<MagicCrystal>();
        public int MagikeCost => 30;

        public int ItemStack => 1;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;
            Main.tileBlockLight[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            //MinPick = 150;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            AddMapEntry(Coralite.MagicCrystalPink, CreateMapEntryName());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.1f;
            b = 0.15f;
        }
    }
}
