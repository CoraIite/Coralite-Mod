using Coralite.Content.Items.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeDoorClosed : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.DoorClosedPrefab(ModContent.TileType<RedJadeDoorOpen>(), DustID.GemRuby,  Coralite.Instance.RedJadeRed);
        }

        public override string Texture => AssetDirectory.RedJadeTiles + Name;
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<RedJadeDoor>();
        }
    }
}
