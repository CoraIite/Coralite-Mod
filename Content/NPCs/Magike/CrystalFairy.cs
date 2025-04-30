using Coralite.Content.Biomes;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalFairy : ModNPC
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CountsAsCritter[Type] = true;
            this.RegisterBestiaryDescription();
        }

        public override void SetDefaults()
        {
            NPC.width = 10;
            NPC.height = 10;
            NPC.damage = 0;
            NPC.defense = 0;

            NPC.lifeMax = 5;
            NPC.aiStyle = NPCAIStyleID.Firefly;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.DigIce;
            NPC.npcSlots = 0.2f;
            NPC.noGravity = true;

            SpawnModBiomes = [ModContent.GetInstance<MagicCrystalCave>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(
                this.GetBestiaryDescription()
                );
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
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Teleporter, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, Coralite.MagicCrystalPink, 1f);
        }
    }
}
