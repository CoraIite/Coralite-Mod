using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            Item i = inventory[slot];
            if (i.IsAir)
                return false;

            if (context == ItemSlot.Context.InventoryItem)//shift点击物品栏放入魔能仪器中
            {
                var ui = UILoader.GetUIState<MagikeApparatusPanel>();
                if (ui.visible && MagikeApparatusPanel.CurrentEntity != null
                    && MagikeApparatusPanel.CurrentEntity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                {
                    if (container.CanAddItem(i.type, i.stack))
                    {
                        container.AddItem(i);
                        Helper.PlayPitched(CoraliteSoundID.Grab);
                        return true;
                    }
                }
            }

            return base.ShiftClickSlot(inventory, context, slot);
        }

        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            Item i = inventory[slot];
            if (i.IsAir)
                return false;

            if (context == ItemSlot.Context.InventoryItem && ItemSlot.ShiftInUse && !i.favorited)//shift物品栏放入魔能仪器中，改变鼠标外观
            {
                var ui = UILoader.GetUIState<MagikeApparatusPanel>();
                if (ui.visible && MagikeApparatusPanel.CurrentEntity != null
                    && MagikeApparatusPanel.CurrentEntity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                {
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;
                }
            }

            return base.HoverSlot(inventory, context, slot);
        }

    }
}
