using Coralite.Content.ModPlayers;
using Coralite.Content.NPCs.GlobalNPC;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderElectrified : ModBuff
    {
        public override string Texture => AssetDirectory.Debuffs + Name;

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.thunderElectrified = true;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC gnpc))
            {
                gnpc.ThunderElectrified = true;
            }

            Rectangle spawnArea = npc.getRect();
            int dustType = DustID.PortalBoltTrail;

            Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 50, Coralite.Instance.ThunderveinYellow);
            dust.velocity *= 1.5f;
            dust.noGravity = true;
        }
    }
}
