using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Buffs.Debuffs
{
    public class SnowDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Debuffs + Name;

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.Next(4) < 3)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Snow, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 1.8f;
                dust.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                    dust.scale *= 0.5f;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.Next(4) < 3)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(player.position.X - 2f, player.position.Y - 2f), player.width + 4, player.height + 4, DustID.Snow, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 1.8f;
                dust.velocity.Y -= 0.5f;
                if (Main.rand.NextBool(4))
                    dust.scale *= 0.5f;
            }
        }

    }
}