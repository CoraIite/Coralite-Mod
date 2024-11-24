using Coralite.Core;
using Coralite.Core.Loaders;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNote : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 30;
            Item.value = Item.sellPrice(0, 1);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI!=Main.myPlayer)
                return true;

            UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
            UILoader.GetUIState<CoraliteNoteUIState>().OpenBook();

            Main.playerInventory = false;
            return true;
        }
    }
}
