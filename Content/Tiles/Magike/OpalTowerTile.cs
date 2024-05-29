using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Core;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Magike
{
    public class OpalTowerTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.Instance.MagicCrystalPink);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            AnimationFrameHeight = 18 * 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer || Main.tile[i, j].TileFrameY < 18 * 3)
                return;
            Main.LocalPlayer.AddBuff(ModContent.BuffType<OpalBuff>(), 60);
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            if (Main.tile[i, j].TileFrameY < 18 * 3)
            {
                frameYOffset = 0;
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter > 8)
            {
                frameCounter = 0;
                if (++frame > 7)
                    frame = 1;
            }
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ModContent.ItemType<OpalTower>())];
        }

        public override bool RightClick(int i, int j)
        {
            int x;
            for (x = Main.tile[i, j].TileFrameX / 18; x >= 2; x -= 2)
            {
            }

            int y;
            for (y = Main.tile[i, j].TileFrameY / 18; y >= 3; y -= 3)
            {
            }

            Tile topLeft = Framing.GetTileSafely(i - x, j - y);

            if (topLeft.TileFrameY >= 18 * 3)
                SetFrame(i - x, j - y, 2, 3);
            else
                SetFrame(i - x, j - y, 2, 3);
            return false;
        }

        public void SetFrame(int i, int j, int frameWidth, int frameHeight)
        {
            for (int k = i; k < i + frameWidth; k++)
            {
                for (int l = j; l < j + frameHeight; l++)
                {
                    Tile tile = Main.tile[k, l];
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileFrameY < 18 * 3)
                        tile.TileFrameY += 18 * 3;
                    else
                        tile.TileFrameY -= 18 * 3;
                }
            }
        }
    }
}