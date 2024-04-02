using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class MagicCrystalMusicBox : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer

            // The following code links the music box's item and tile with a music track:
            //   When music with the given ID is playing, equipped music boxes have a chance to change their id to the given item type.
            //   When an item with the given item type is equipped, it will play the music that has musicSlot as its ID.
            //   When a tile with the given type and Y-frame is nearby, if its X-frame is >= 36, it will play the music that has musicSlot as its ID.
            // When getting the music slot, you should not add the file extensions!
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/CrystalCave"), ModContent.ItemType<MagicCrystalMusicBox>(), ModContent.TileType<MagicCrystalMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<MagicCrystalMusicBoxTile>(), 0);
            Item.rare = ItemRarityID.Pink;
        }
    }

    public class MagicCrystalMusicBoxTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Coralite.Instance.MagicCrystalPink, name);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<MagicCrystalMusicBox>();
        }
    }
}
