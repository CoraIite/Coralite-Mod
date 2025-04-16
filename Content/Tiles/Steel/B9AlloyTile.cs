using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Steel
{
    public class B9AlloyTile : ModTile
    {
        public override string Texture => AssetDirectory.SteelTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            MineResist = 2f;
            DustType = DustID.Titanium;
            HitSound = CoraliteSoundID.Metal_NPCHit4;
            MinPick = 110;

            AddMapEntry(Color.Gray);
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
