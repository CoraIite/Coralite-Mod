using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeTile : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 410;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            //TileID.Sets.ChecksForMerge[Type] = true;
            //Main.tileMerge[TileID.Stone][Type] = true;
            //Main.tileMerge[Type][TileID.Stone] = true;

            DustType = DustID.GemRuby;
            HitSound = SoundID.Tink;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Coralite.RedJadeRed, name);
        }
    }
}
