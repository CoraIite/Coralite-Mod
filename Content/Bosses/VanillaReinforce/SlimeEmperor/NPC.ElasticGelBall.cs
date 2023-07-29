using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
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
            NPC.width = NPC.height = 42;
            NPC.damage = 1;
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0.1f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            Player Target = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
                {
                    NPC.EncourageDespawn(10);
                    return;
                }
            }

            switch ((int)State)
            {
                case 0:
                    NPC.ai[2] += 0.1f;  //膨胀小动画
                    NPC.velocity.X *= 0.99f;
                    if (NPC.velocity.Y < 16)
                        NPC.velocity.Y += 0.5f;

                    if (NPC.ai[2] > 1f)
                    {
                        NPC.rotation = NPC.velocity.ToRotation();
                        State = 1;
                        NPC.ai[2] = 1f;
                    }
                    break;
                case 1:
                    NPC.velocity *= 0.99f;
                    NPC.ai[3]++;
                    NPC.ai[2] = 1 + MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f;
                    if (Vector2.Distance(NPC.Center, Target.Center) < NPC.width)
                    {
                        State = 2;
                        Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        Target.velocity.X = dir.X*12;
                        Target.velocity.Y = dir.Y * 16;
                        Target.AddBuff(BuffID.Slimed, 120);
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, NPC.Center);
                    }
                    if (NPC.ai[3] > 1800)
                        State = 2;
                    break;
                case 2:
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 3)
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

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanBeHitByNPC(NPC attacker) => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => State == 1;
        public override bool? CanBeHitByProjectile(Projectile projectile) => projectile.friendly&&State == 1;

        public override bool CheckDead()
        {
            if (State == 1)
            {
                State = 2;
                NPC.life = 1;
                return false;
            }

            return State == 2;
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
