using Coralite.Core;
using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNote : ModItem
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 30;
            Item.value = Item.sellPrice(0, 1);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;

            var ui = UILoader.GetUIState<CoraliteNoteUIState>();
            if (ui.visible)
                UILoader.GetUIState<CoraliteNoteUIState>().closeingBook = true;
            else
                UILoader.GetUIState<CoraliteNoteUIState>().OpenBook();

            UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();

            Main.playerInventory = false;
            return true;
        }
    }

    public class CoraliteNoteTile : ModTile
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.Direction = TileObjectDirection.None;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            DustType = DustID.Coralstone;

            AddMapEntry(Color.Pink, CreateMapEntryName());
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ModContent.ItemType<CoraliteNote>())];
        }

        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            return true;
        }
    }
}
