using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// ai[0] 为1时表示冰雪环境; ai[1] 传入NPC索引
/// </summary>
public class GravityStone : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Vanilla+"Projectile_962";

    public ref float Biome => ref Projectile.ai[0];
    public ref float OwnerIndex => ref Projectile.ai[1];
    public ref float State => ref Projectile.ai[2];
    public ref float Recorder => ref Projectile.localAI[0];
    public ref float Alpha => ref Projectile.localAI[1];
    public ref float Timer => ref Projectile.localAI[2];

    public int frameX;
    public int targetPlayer=-1;

    public override void SetStaticDefaults()
    {
        Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
    }

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.scale = 1.3f;

        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 30;
    }

    public override void Initialize()
    {
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    }

    public override bool ShouldUpdatePosition()
    {
        return State != 3;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Recorder);
        writer.Write(targetPlayer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Recorder = reader.ReadSingle();
        targetPlayer = reader.ReadInt32();
    }

    public override void AI()
    {
        if (!OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner, Projectile.Kill))
        {
            return;
        }

        switch (State)
        {
            default:
            case 0://渐入
                {
                    if (Timer==0)
                    {
                        //设置帧图
                        if (Biome==1)
                        {
                            Projectile.frame = Main.rand.Next(2, 4);
                        }
                        else
                            Projectile.frame = Main.rand.Next(0, 2);
                        frameX = Main.rand.Next(3);

                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            Projectile.netUpdate = true;
                            Recorder = Main.rand.Next(400, 600);
                        }
                    }

                    Timer++;
                    DragToOwner(owner);
                    Alpha = Helper.BezierEase(Timer / 50f);

                    if (Timer > 50)
                    {
                        Alpha = 1;
                        Timer = 0;
                        State = 1;
                    }
                }
                break;
            case 1://持续吸引
                {
                    DragToOwner(owner);
                    if (Vector2.Distance(Projectile.Center,owner.Center)< Recorder)
                    {
                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            Projectile.velocity *= Main.rand.NextFloat(0.4f, 0.6f);
                            Projectile.netUpdate = true;
                        }

                        Timer = 0;
                        State = 2;
                    }
                }
                break;
            case 2://减速聚集
                {
                    Projectile.velocity *= 0.93f;
                }
                break;
            case 3:
                {
                    if (Timer > 30 && targetPlayer != -1)
                    {
                        Projectile.velocity = Projectile.velocity.Length() * Projectile.velocity.ToRotation().AngleTowards((Main.player[targetPlayer].Center - Projectile.Center).ToRotation(), 0.1f).ToRotationVector2();
                    }

                    Timer--;
                    if (Timer < 1)
                    {
                        Timer = 0;
                        State = 4;
                    }
                }
                break;
            case 4:
                {
                    Timer++;
                    if (Timer>80)
                    {
                        Projectile.tileCollide = true;
                        if (Projectile.velocity.Y<15)
                        {
                            Projectile.velocity.Y += 0.2f;
                        }

                        Projectile.velocity.X *= 0.95f;
                    }
                }
                break;

        }

        if (State!=3)
        Projectile.rotation += Projectile.velocity.Length() * MathF.Sign(Projectile.velocity.X) / 70f;
    }

    public void DragToOwner(NPC owner)
    {
        float length = Projectile.velocity.Length();
        if (length < 14)
        {
            Projectile.velocity = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (length * 1.02f);
        }
    }

    public void TurnToShoot(int targetPlayer, int time)
    {
        Timer = time;
        State = 3;
       this. targetPlayer = targetPlayer;
        Projectile.velocity = (Main.player[targetPlayer].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle frameBox = new Rectangle(frameX, Projectile.frame, 3, 4);

        for (int i = 0; i < 6; i++)
        {
            Vector2 off = (Main.GlobalTimeWrappedHourly * 1.5f + i * MathHelper.TwoPi / 6).ToRotationVector2() * 3;

            Projectile.QuickFrameDraw(frameBox, (Coralite.ShadowPurple * 0.8f * Alpha) with { A=0}, 0, off);
        }

        Projectile.DrawShadowTrailsSacleStep((Coralite.ShadowPurple* Alpha) with { A = 0 }, 0.7f, 0.7f / 10, 1, 10, 2,0.5f/10,frameBox);

        Projectile.QuickFrameDraw(frameBox, lightColor* Alpha, 0);

        return false;
    }
}
