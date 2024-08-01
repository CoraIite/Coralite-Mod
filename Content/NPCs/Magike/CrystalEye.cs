using Coralite.Content.Biomes;
using Coralite.Content.Items.Banner;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalEye : ModNPC
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrystalEyeBannerItem>();

            NPC.CloneDefaults(NPCID.DemonEye);
            NPC.damage = 12;
            NPC.lifeMax = 50;

            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.GlassBroken_Shatter;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.5f, 0.3f, 0.45f);
            NPC.rotation = NPC.velocity.ToRotation();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.townNPCs > 2f)
                return 0;
            if ((!Main.dayTime && spawnInfo.Player.ZoneForest))
                return 0.08f;
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
                return 0.04f;

            return 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Teleporter, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, Coralite.MagicCrystalPink, 1f);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CrystalSerpent_Pink);
                    dust.noGravity = true;
                    dust.velocity = new Microsoft.Xna.Framework.Vector2(hit.HitDirection, 0).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * hit.Knockback * Main.rand.NextFloat(0.9f, 1.1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Basalt>(), 4, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicCrystal>(), 4, 1, 2));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 12));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 12));
        }
    }
}
