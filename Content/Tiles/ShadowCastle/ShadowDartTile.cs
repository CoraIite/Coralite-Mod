using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowDartTile:ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles+Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            DustType = DustID.ShadowbeamStaff;
            AddMapEntry(Color.MediumPurple);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconID = ModContent.ItemType<ShadowDart>();

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            return true;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[] {new Item(ModContent.ItemType<ShadowDart>()) } ;
        }
    }
}
