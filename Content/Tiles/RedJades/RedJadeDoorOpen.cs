using Coralite.Content.Items.RedJadeItems;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeDoorOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.DoorOpenPrefab(ModContent.TileType<RedJadeDoorClosed>(), DustID.GemRuby, "赤玉门", Coralite.Instance.RedJadeRed);
        }

        public override string Texture => AssetDirectory.RedJadeTiles + Name;
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<RedJadeDoor>());
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
