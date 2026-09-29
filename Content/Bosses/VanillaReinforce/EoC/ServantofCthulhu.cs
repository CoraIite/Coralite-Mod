using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    public class ServantofCthulhu : ModNPC
    {
        public override string Texture => AssetDirectory.Vanilla+"NPC_5";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.ServantofCthulhu];
            NPC.QuickTrailSets(Helper.NPCTrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 15;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.timeLeft = 300;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = NPC.lifeMax.IntMult(0.8f * balance * bossAdjustment);
            NPC.damage = NPC.damage.IntMult(0.85f * bossAdjustment);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = NPC.frame.Y < frameHeight ? frameHeight : 0;
            }
        }

        private Color GetColor()
        {
            Color color = Color.Purple;
            color.A = 0;
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = new(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
            Vector2 Offset = NPC.position - NPC.Center;
            Vector2 Pos = NPC.position - Offset - screenPos;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 OldPos = NPC.oldPos[i] - Offset - screenPos;
                Color color = GetColor();
                color *= 1f - i / (float)NPC.oldPos.Length;
                float Scale = NPC.scale * (1f - i / (float)NPC.oldPos.Length * 0.5f);
                spriteBatch.Draw(texture, OldPos, rectangle, color, NPC.oldRot[i], rectangle.Size() / 2f, Scale, spriteEffects, 0f);
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 Vec = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 4f;
                Color color = GetColor();
                spriteBatch.Draw(texture, Pos + Vec, rectangle, color, NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
            }
            Color boydColor = Lighting.GetColor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16));
            spriteBatch.Draw(texture, Pos, rectangle, NPC.GetAlpha(boydColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.localAI[1] = NPC.velocity.X;
            NPC.localAI[2] = NPC.velocity.Y;
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            Vector2 OrigVel = new(NPC.localAI[1], NPC.localAI[2]);
            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
            if (NPC.ai[0] > 0f)
            {
                NPC Boss = Main.npc[(int)NPC.ai[1]];
                if (Boss.type == ModContent.NPCType<EyeOfCthulhu>() && Boss.active && Boss != null)
                {
                    NPC.target = Boss.target;
                    Player player = Main.player[NPC.target];
                    Vector2 VecOffset = Vector2.Normalize(player.Center - NPC.Center) * 1.2f;
                    float NPCDIS = Vector2.Distance(Boss.Center, NPC.Center);
                    float ProDIS = Vector2.Distance(Boss.Center, player.Center);
                    float DIS = ProDIS - NPCDIS;
                    float Mult = 0f;
                    if(DIS > -50f)
                    {
                        Mult = 1f - DIS / ProDIS;
                        Mult *= 0.5f;
                    }
                    NPC.velocity = Vector2.Normalize(NPC.velocity + VecOffset * Mult) * NPC.velocity.Length();
                }
            }
            else
            {
                NPC.velocity = Vector2.Normalize(NPC.velocity + OrigVel) * NPC.velocity.Length();
            }
            if (NPC.velocity.Length() < 18f)
            {
                NPC.velocity = Vector2.Normalize(NPC.velocity) * (NPC.velocity.Length() + 0.1f);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
                }
                return;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2 * hit.HitDirection, -2f);
            }
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 6);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 7);
        }
    }
}

