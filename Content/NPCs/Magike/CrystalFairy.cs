using Coralite.Content.Biomes;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalFairy : ModNPC
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 10;
            NPC.height = 10;
            NPC.damage = 0;
            NPC.defense = 0;

            NPC.lifeMax = 10;
            NPC.aiStyle = NPCAIStyleID.Firefly;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.DigIce;
            NPC.npcSlots = 0.2f;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.5f, 0.3f, 0.45f);

            NPC.rotation += 0.03f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
                return 0.05f;

            return 0;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Teleporter, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, Coralite.Instance.MagicCrystalPink, 1f);
        }
    }
}
