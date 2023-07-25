using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 弹弹凝胶球NPC
    /// </summary>
    public class ElasticGelBall : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        public ref float State => ref NPC.ai[0];

        public override void SetDefaults()
        {
            NPC.damage = 1;
            NPC.lifeMax = 50;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0.1f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            NPC.ai[1]++;
            if (NPC.ai[1] > 60)
                NPC.life -= 1;

            if (State != 2 && NPC.life < 10)
                State = 2;

            switch ((int)State)
            {
                case 0:
                    NPC.ai[2] += 0.1f;  //膨胀小动画
                    if (NPC.ai[2] > 1f)
                    {
                        NPC.velocity.X *= 0.99f;
                        if (NPC.velocity.Y > -5)
                            NPC.velocity.Y -= 0.1f;
                        State = 1;
                        NPC.ai[2] = 1f;
                    }
                    break;
                case 1:
                    NPC.frameCounter++;
                    if (NPC.frameCounter>4)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        if (NPC.frame.Y > 6)
                            NPC.Kill();
                    }
                    break;
                default:
                    break;
            }
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanBeHitByNPC(NPC attacker) => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (int)State == 1;

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            State = 2;
            target.velocity += (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 14;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            var frameBox = mainTex.Frame(1, 7, 0, NPC.frame.Y);
            var origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor * NPC.ai[2], NPC.rotation, origin, NPC.ai[2],0,0);
            return false;
        }
    }
}
