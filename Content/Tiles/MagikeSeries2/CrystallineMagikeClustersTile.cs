using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineMagikeClustersTile : ModTile, ICrystalCluster
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public ushort Level => CoraliteContent.MagikeLevelType<BrilliantLevel>();
        public int ItemType => ModContent.ItemType<CrystallineMagike>();
        public int MagikeCost => 420;

        public int ItemStack => 1;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;
            Main.tileBlockLight[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            MinPick = 110;

            DustType = ModContent.DustType<CrystallineDustSmall>();
            AddMapEntry(Coralite.CrystallinePurple, CreateMapEntryName());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.2f;
            b = 0.35f;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
            Tile t = Main.tile[i, j];
            if (!fail && t.TileFrameX==0&&t.TileFrameY==0)
            {
                Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, new Vector2(i, j) * 16);
            }

            return false;
        }
    }
}
