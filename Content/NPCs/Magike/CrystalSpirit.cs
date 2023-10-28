using Coralite.Content.Biomes;
using Coralite.Content.Items.Magike;
using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalSpirit : ModNPC
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 36;
            NPC.damage = 12;
            NPC.defense = 4;

            NPC.lifeMax = 40;
            NPC.aiStyle = NPCAIStyleID.FlyingFish;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.GlassBroken_Shatter;
            NPC.knockBackResist = 0.8f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.5f, 0.3f, 0.45f);
            float targetRot = NPC.velocity.Length() * 0.1f * NPC.direction;
            NPC.rotation = NPC.rotation.AngleTowards(targetRot, 0.01f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 9 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
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
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Teleporter, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, Coralite.Instance.MagicCrystalPink, 1f);
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Basalt>(), 4, 0, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicCrystal>(), 4, 0, 2));
        }
    }
}
