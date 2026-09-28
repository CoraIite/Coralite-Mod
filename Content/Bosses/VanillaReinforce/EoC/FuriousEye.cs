using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    public class FuriousEye : ModNPC
    {
        public override string Texture => AssetDirectory.EoC+Name;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPC.QuickTrailSets(Helper.NPCTrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.timeLeft = 300;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = NPC.lifeMax.IntMult(0.75f * balance * bossAdjustment);
            NPC.damage = NPC.damage.IntMult(0.8f * bossAdjustment);
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

        private static Color GetColor()
        {
            Color color = Color.DarkRed;
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
            NPC.ai[1] = Main.rand.NextFloat(30f);
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            Player player = Main.player[NPC.target];
            Vector2 Pos = player.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(NPC.ai[2])) * 250f;
            Vector2 ToPos = (Pos - NPC.Center).SafeNormalize(Vector2.UnitX);
            Vector2 Dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            float Mult = 1f - NPC.life / (float)NPC.lifeMax;
            if (!player.active || player.dead || Main.dayTime)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.2f;
                return;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.ai[1] += 1f + Mult * 2f;
                Vector2 Vec = ToPos * (10f + 5f * Mult);
                NPC.rotation = Dir.ToRotation();
                NPC.velocity = (NPC.velocity * 20f + Vec) / 21f;
                if (NPC.ai[1] > 150f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.ai[0] = 2f;
                NPC.ai[1] = 0f;
                float Speed = 20f;
                if (Main.getGoodWorld) 
                    Speed = 25f;
                NPC.velocity = Dir * Speed;
                Helper.CircularDust(NPC.Center, NPC.velocity.ToRotation(), ModContent.DustType<GlowDust_Circle>(), 24, new Vector2(3f, 9f), GetColor(), 1f, Dir * -10f);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                NPC.netUpdate = true;
            }
            else if (NPC.ai[1] < 60f)
            {
                NPC.ai[1] += 1f;
                NPC.velocity *= 0.98f;
                NPC.rotation = NPC.velocity.ToRotation();
                Color color = Color.Red;
                color.A = 0;
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Type, 0, 0, 100, color, 1f);
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].velocity += NPC.velocity * 0.5f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
            }
            else
            {
                NPC.ai[0] = 0f;
                NPC.ai[1] = Main.rand.NextFloat(30f);
                NPC.ai[2] += 60f;
                NPC.netUpdate = true;
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

