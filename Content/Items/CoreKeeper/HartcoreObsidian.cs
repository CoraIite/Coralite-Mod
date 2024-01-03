using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class HartcoreObsidian : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HartcoreObsidianTile>());
        }
    }

    public class HartcoreObsidianTile : ModTile
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        private const int sheetWidth = 234;
        private const int sheetHeight = 90;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;

            MinPick = int.MaxValue;
            DustType = DustID.Obsidian;
            HitSound = new Terraria.Audio.SoundStyle("Coralite/Sounds/CoreKeeper/UnbreakableTile")
            {
                Volume = 0.5f,
                Pitch = 0,
                MaxInstances = 0
            };
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool Slope(int i, int j) => false;

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 2;
            int yPos = j % 2;
            frameXOffset = xPos * sheetWidth;
            frameYOffset = yPos * sheetHeight;
        }
    }
}
