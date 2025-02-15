using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.DigDigDig.EyeOfGlistent
{
    public class ServantOfGlistent : ModNPC
    {
        public override string Texture => AssetDirectory.GlistentItems + "GlistentBar";

        public Player Target => Main.player[NPC.target];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 36;
            NPC.lifeMax = 5;
            NPC.damage = 15;
            NPC.defense = 6;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = 5;
            AIType = NPCID.ServantofCthulhu;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = CoraliteSoundID.DigIce;
            NPC.npcSlots = 0.5f;
        }

    }
}