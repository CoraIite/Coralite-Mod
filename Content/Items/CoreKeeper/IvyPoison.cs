using Coralite.Content.NPCs.VanillaNPC;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class IvyPoison : ModBuff
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegen -= 1;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC globalNPC))
                globalNPC.IvyPosion = true;
        }
    }
}
