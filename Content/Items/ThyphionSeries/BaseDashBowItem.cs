using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.DashBowChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;

namespace Coralite.Content.Items.ThyphionSeries
{
    public abstract class BaseDashBowItem : ModItem, IDashable, IConsultableItem
    {
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<DashBowKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<DashBowPage>();

        public float Priority => IDashable.HeldItemDash;

        public abstract bool Dash(Player Player, int DashDir);

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }
    }
}
