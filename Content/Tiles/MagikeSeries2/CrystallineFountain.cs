using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineFountain : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int FrameHeight = 18 * 4;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 3);
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<SkarnDust>();
            LocalizedText pylonName = CreateMapEntryName();
            AddMapEntry(Color.White, pylonName);

            RegisterItemDrop(ModContent.ItemType<CrystallineFountainItem>());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            Point p = Helper.FindTopLeftPoint(i, j);

            int xOff = Main.tile[p].TileFrameX > 18 ? 0 : 18 * 2;

            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 4; y++)
                {
                    Main.tile[p + new Point(x, y)].TileFrameX = (short)(xOff + x * 18);
                    Main.tile[p + new Point(x, y)].TileFrameY = (short)(y * 18);
                }

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Point p = Helper.FindTopLeftPoint(i, j);
            if (p.X != i || p.Y != j)
                return;

            if (closer && Main.tile[p].TileFrameX > 18)
            {
                if (Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
                    cp.CrystallineSkyIslandEffect = 30;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 1)
                frameCounter = 0;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            Point p = Helper.FindTopLeftPoint(i, j);
            if (p.X != i || p.Y != j)
                return;

            int fc = Main.tileFrameCounter[type];
            if (fc != 0)
                return;

            Tile t = Main.tile[i, j];
            if (t.TileFrameX < 18 * 2)
                return;

            int frameY = Main.tile[p].TileFrameY / FrameHeight;
            frameY++;
            if (frameY > 23)
                frameY = 0;

            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 4; y++)
                    Main.tile[p + new Point(x, y)].TileFrameY = (short)(y * 18 + frameY * FrameHeight);
        }
    }
}
