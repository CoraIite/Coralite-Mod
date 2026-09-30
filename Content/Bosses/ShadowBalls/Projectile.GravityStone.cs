using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// Stone projectile placeholder. ai[0] selects the snow biome; ai[1] stores the owning NPC index.
/// </summary>
public class GravityStone : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public ref float Biome => ref Projectile.ai[0];
    public ref float OwnerIndex => ref Projectile.ai[1];
    public ref float State => ref Projectile.ai[2];
    public ref float Recorder => ref Projectile.localAI[0];
    public ref float Alpha => ref Projectile.localAI[1];
    public ref float Timer => ref Projectile.localAI[2];

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 10;
    }

    public override bool? CanDamage() => false;

    public override void Initialize()
    {
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Recorder);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Recorder = reader.ReadSingle();
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
                    }

                    Timer++;
                    DragToOwner(owner);
                    Alpha = Helper.BezierEase(Timer / 50f);

                    if (Timer>50)
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
                    if (Vector2.Distance(Projectile.Center,owner.Center)<200)
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
            case 2:
                {
                    Projectile.velocity *= 0.93f;
                }
                break;
        }
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
        // TODO: Implement the aimed shot.
    }
}
