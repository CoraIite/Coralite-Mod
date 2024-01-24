using Coralite.Content.Biomes;
using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Shadow
{
    public class ShadowGuard : ModNPC
    {
        public override string Texture => AssetDirectory.ShadowNPCs + Name;

        public Player Target => Main.player[NPC.target];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("影子守卫");
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 64;
            NPC.lifeMax = 75;
            NPC.damage = 15;
            NPC.defense = 8;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.knockBackResist = 0.7f;
            NPC.aiStyle = NPCAIStyleID.HoveringFighter;
            NPC.aiAction = NPCID.Wraith;
        }

        public override void Load()
        {
            for (int i = 0; i < 3; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.ShadowGores + "ShadowGuard_Gore" + i);
        }

        public override void PostAI()
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y++;

                if (NPC.frame.Y > 2)
                    NPC.frame.Y = 0;
            }

            NPC.spriteDirection = Math.Sign(Target.Center.X - NPC.Center.X);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            Rectangle frameBox = new Rectangle(0, NPC.frame.Y * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.None;

            Vector2 pos = NPC.Center - screenPos;
            Color c = Color.Purple;
            c.A = 0;
            c *= 0.4f;
            for (int i = 0; i < 3; i++)
                spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2() * 2
                    , frameBox, c, NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * 0.7f, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Helper.PlayPitched("Shadows/Shadow_Hurt0", 0.4f, 0f, NPC.Center);
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), DustID.Granite, null, 0, default, 1f);

                if (NPC.life <= 0)
                    for (int j = 0; j < 3; j++)
                        Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("ShadowGuard_Gore" + j).Type);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowEnergy>(), 2, 1, 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<ShadowCastleBiome>() && CoraliteSets.TileShadowCastle[spawnInfo.SpawnTileType])
                return 0.1f;
            return 0f;
        }
    }
}
