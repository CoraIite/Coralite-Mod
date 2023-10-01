using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai2传入按照惯性运动的时间，使用ai3传入自由瞄准玩家运动的时间，请注意惯性时间一定要小于自由运动时间！
    /// </summary>
    public class NightmareCatcher : ModNPC
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Player Target => Main.player[NPC.target];
        private static NPC NightmareOwner => Main.npc[NightmarePlantera.NPBossIndex];
        private RotateTentacle tentacle;

        public ref float State => ref NPC.ai[0];
        public ref float Hited => ref NPC.ai[1];
        public ref float Timer => ref NPC.localAI[0];
        private ref float MouseAngle=>ref NPC.localAI[1];

        public ref float NotFreeTime => ref NPC.ai[2];
        public ref float MaxFreeTime => ref NPC.ai[3];

        public static Asset<Texture2D> VineTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            VineTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareCatcherVine");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            VineTex = null;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.ProjectileNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 24;
            NPC.lifeMax = 2500;
            NPC.defense = 40;
            NPC.damage = 20;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;
            NPC.DeathSound = CoraliteSoundID.Fleshy_NPCDeath1;
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => Hited == 0;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            Hited = 1;
            State = 2;
            NPC.target = target.whoAmI;
            NPC.frame.Y = 0;
        }

        public override bool PreKill()
        {
            return false;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                NPC.Kill();
                return;
            }
            else
            {
                NPC.timeLeft = NPC.activeTime;
            }

            tentacle ??= new RotateTentacle(20, TentacleColor, TentacleWidth, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);

            Vector2 dir = NightmareOwner.Center - NPC.Center;
            float distance = dir.Length();
            float tentacleLength = distance * 0.8f / 20f;

            tentacle.SetValue(NPC.Center, np.Center, NPC.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(tentacleLength);

            switch ((int)State)
            {
                default:
                case 0: //发射阶段
                    MouseAngle = Math.Abs(MathF.Sin(Timer*0.15f)) * 0.6f;

                    do
                    {
                        if (Timer < NotFreeTime)
                            break;

                        if (Timer < MaxFreeTime)
                        {
                            NPC.velocity *= 0.98f;
                            if (((int)Timer) % 15 == 0)
                            {
                                NPC.TargetClosest();
                                NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));
                                NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.4f,0.4f)) * 3f;
                            }
                            break;
                        }

                        NPC.velocity *= 0;
                        Timer = 0;
                        State = 1;
                        NPC.netUpdate = true;
                        return;

                    } while (false);

                    NPC.rotation = NPC.velocity.ToRotation();
                    Timer++;
                    break;
                case 1: //收回阶段
                    if (MouseAngle>0)
                    {
                        MouseAngle -= 0.1f;
                        if (MouseAngle < 0)
                            MouseAngle = 0;
                    }

                    float speed = NPC.velocity.Length();
                    if (speed < 16)
                    {
                        speed += 0.25f;
                    }

                    NPC.velocity = dir.SafeNormalize(Vector2.Zero) * speed;
                    NPC.rotation = (NPC.Center - NightmareOwner.Center).ToRotation();

                    if (distance < 32)
                        NPC.Kill();
                    break;
                case 2: //拖拽玩家阶段
                    if (MouseAngle > 0)
                    {
                        MouseAngle -= 0.1f;
                        if (MouseAngle < 0)
                            MouseAngle = 0;
                    }

                    NPC.velocity = dir.SafeNormalize(Vector2.Zero) * 2f;
                    NPC.rotation = (NPC.Center - NightmareOwner.Center).ToRotation();

                    Target.velocity *= 0;
                    Target.Center = NPC.Center;

                    if (distance < 138)
                        NPC.Kill();
                    break;
            }
        }

        public static Color TentacleColor(float factor)
        {
            return Color.Lerp(NightmarePlantera.nightmareSparkleColor, Color.Transparent, factor);
        }

        public static float TentacleWidth(float factor)
        {
            if (factor > 0.6f)
                return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

            return Helper.Lerp(0, 25, factor / 0.6f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            tentacle.DrawTentacle( (i) => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;
            Vector2 pos = NPC.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 origin = frameBox.BottomLeft();
            origin.X += 2;
            origin.Y -= 4;
            Color c = Color.White * 0.8f;

            float scale = NPC.scale * 1.1f;
            float rot = NPC.rotation - MouseAngle;
            Vector2 dir = rot.ToRotationVector2();
            Main.spriteBatch.Draw(mainTex, pos + dir * 4, frameBox, c, rot, origin, scale, 0, 0);

            rot = NPC.rotation + MouseAngle;
            dir = rot.ToRotationVector2();
            frameBox = mainTex.Frame(1, 2, 0, 1);
            Main.spriteBatch.Draw(mainTex, pos + dir * 4, frameBox, c, rot, new Vector2(2, 2), scale, 0, 0);

            Texture2D vineTex = VineTex.Value;

            Main.spriteBatch.Draw(vineTex, pos, null, c, NPC.rotation, new Vector2(vineTex.Width*0.75f,vineTex.Height/2), scale, 0, 0);
            return false;
        }
    }
}
