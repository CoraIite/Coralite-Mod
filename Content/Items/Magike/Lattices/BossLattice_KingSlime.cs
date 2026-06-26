using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Lattices
{
    public class BossLattice_KingSlime() : BaseBossLattice(ItemID.KingSlimeBossBag)
    {
        public override bool? UseItem(Player player)
        {
            NPC.SetEventFlagCleared(ref NPC.downedBoss1, 13);

            return true;
        }
    }
}
