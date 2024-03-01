using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.OtherNPC
{
    public abstract class Maragon : EasyWorm_LikeVanila
    {
        public override void Init()
        {
            directional = true;
            flies = true;
            minLength = 5;
            maxLength = 8;
            tailType = ModContent.NPCType<MaragonTrail>();
            bodyType = ModContent.NPCType<MaragonBody>();
            headType = ModContent.NPCType<MaragonHead>();
            speed = 5.5f;
            turnSpeed = 0.06f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), DustID.Shadowflame, null, 0, default, 1f);

            }
        }
    }

    public class MaragonHead : Maragon
    {
        public override string Texture => AssetDirectory.OtherNPC + Name;

        public override void SetStaticDefaults()
        {
            //var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            //{ // Influences how the NPC looks in the Bestiary
            //    CustomTexturePath = "ExampleMod/Content/NPCs/ExampleWorm_Bestiary", 
            //    Position = new Vector2(40f, 24f),
            //    PortraitPositionXOverride = 0f,
            //    PortraitPositionYOverride = 12f
            //};
            //NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 22;
            NPC.damage = 20;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0f;
            NPC.value = 300f;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.behindTiles = true;
        }

        public override void Init()
        {
            base.Init();
            head = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicalPowder>(), 2, 3, 6));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.townNPCs > 2f)
                return 0;

            if (spawnInfo.Player.statLifeMax2 > 150 && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight)
                return 0.04f;
            return 0f;
        }
    }

    public class MaragonBody : Maragon
    {
        public override string Texture => AssetDirectory.OtherNPC + Name;

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 22;
            NPC.damage = 20;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0f;
            NPC.value = 300f;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.behindTiles = true;
        }

    }

    public class MaragonTrail : Maragon
    {
        public override string Texture => AssetDirectory.OtherNPC + Name;

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 22;
            NPC.damage = 20;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0f;
            NPC.value = 300f;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.behindTiles = true;
        }

        public override void Init()
        {
            base.Init();
            tail = true;
        }
    }

}
