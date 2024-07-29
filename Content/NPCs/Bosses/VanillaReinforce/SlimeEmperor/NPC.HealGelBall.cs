using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 使用ai0传入BOSS
    /// </summary>
    public class HealGelBall : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "ElasticGelBall";

        public ref float Boss => ref NPC.ai[0];

        public override void SetDefaults()
        {
            NPC.damage = 1;
            NPC.lifeMax = 25;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0.1f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 25 + numPlayers * 25;
        }

        public override void AI()
        {
            NPC boss = Main.npc[(int)Boss];
            if (!boss.active)
                NPC.Kill();

            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -NPC.velocity * Main.rand.NextFloat(0.1f, 0.3f);

            Vector2 targetVec = boss.Center - NPC.Center;
            NPC.velocity = targetVec.SafeNormalize(Vector2.One) * 3;
            if (targetVec.Length() < 80)
            {
                NPC.ai[2] = 1;
                NPC.Kill();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
                for (int i = 0; i < 10; i++)
                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), DustID.t_Slime,
                         Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
        }

        public override void OnKill()
        {
            if ((int)NPC.ai[2] == 1)
            {
                NPC boss = Main.npc[(int)Boss];
                if (!boss.active)
                    return;

                boss.HealEffect(100);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            var frameBox = mainTex.Frame(1, 7, 0, 0);
            var origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.ai[2], 0, 0);

            return false;
        }
    }
}
