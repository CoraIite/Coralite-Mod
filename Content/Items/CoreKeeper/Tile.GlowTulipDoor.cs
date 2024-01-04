using Coralite.Content.CustomHooks;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class GlowTulipDoor : BaseDoorItem
    {
        public GlowTulipDoor() : base(Item.sellPrice(0, 0, 0), ItemRarityID.White, ModContent.TileType<GlowTulipDoorClosed>(), AssetDirectory.CoreKeeperItems) { }
    }

    public class GlowTulipDoorClosed : ModTile, ILockableDoor
    {
        public override void SetStaticDefaults()
        {
            this.DoorClosedPrefab(ModContent.TileType<GlowTulipDoorOpen>(), DustID.Stone, Color.Gray);
            MinPick = int.MaxValue;
            HitSound = new Terraria.Audio.SoundStyle("Coralite/Sounds/CoreKeeper/UnbreakableTile")
            {
                Volume = 0.5f,
                Pitch = 0,
                MaxInstances = 0
            };

        }

        public static int rightClickDelay;

        public override string Texture => AssetDirectory.CoreKeeperItems + Name;
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlowTulipDoor>();
        }

        public bool Unlock(int i, int j)
        {
            if (Main.LocalPlayer.HeldItem.type == ItemID.GlowTulip)
                return true;

            if (rightClickDelay == 0)
            {
                CombatText.NewText(new Rectangle(i * 16, j * 16 - 32, 16, 1), Color.Cyan, this.GetLocalization("HowToOpen", () => "需要手持一朵蓝色发光花朵以打开").Value);
                rightClickDelay = 120;
            }
            return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (rightClickDelay > 0)
                rightClickDelay--;
        }
    }

    public class GlowTulipDoorOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.DoorOpenPrefab(ModContent.TileType<GlowTulipDoorClosed>(), DustID.Stone, Color.Gray);
            MinPick = int.MaxValue;
            HitSound = new Terraria.Audio.SoundStyle("Coralite/Sounds/CoreKeeper/UnbreakableTile")
            {
                Volume = 0.5f,
                Pitch = 0,
                MaxInstances = 0
            };
        }

        public override string Texture => AssetDirectory.CoreKeeperItems + Name;
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlowTulipDoor>();
        }
    }
}
