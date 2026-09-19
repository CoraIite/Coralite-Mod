using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// 使用ai0记录目标距离，ai1记录NPC索引，速度的X表示状态1速度，速度Y表示状态2速度
/// </summary>
public class ShadowBallStar : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    // 弹幕状态
    private ref float State => ref Projectile.localAI[0];
    private ref float Timer => ref Projectile.localAI[1];
    private ref float RecordedAngle => ref Projectile.localAI[2];  // 记录的角度

    // ai参数作为ref属性
    private ref float TargetDistance => ref Projectile.ai[0];  // 目标距离
    private ref float NPCIndex => ref Projectile.ai[1];        // NPC索引
    private ref float CurrentDistance => ref Projectile.ai[2];        // NPC索引
    public float[] oldLength;

    public const int TrailCount = 15;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 500;
    }

    public override bool ShouldUpdatePosition() => false;

    public override void Initialize()
    {
        if (!NPCIndex.GetNPCOwner<ShadowBall>(out NPC owner, Projectile.Kill))
        {
            return;
        }

        RecordedAngle = (Projectile.Center - owner.Center).ToRotation();
        Projectile.rotation = RecordedAngle;
        Projectile.InitOldRotCache(TrailCount);

        oldLength = new float[TrailCount];
        for (int i = 0; i < TrailCount; i++)
            oldLength[i] = 0;
    }

    public override void AI()
    {
        if (!NPCIndex.GetNPCOwner<ShadowBall>(out NPC owner, Projectile.Kill))
        {
            return;
        }

        OrbitAI(owner);

        Timer++;
    }

    /// <summary>
    /// 新的轨道AI：相对运动射出 -> 特效闪烁 -> 绕NPC旋转
    /// </summary>
    private void OrbitAI(NPC owner)
    {
        const int State_LinearShoot = 0;      // 状态1：相对运动射出
        const int State_FlashEffect = 1;      // 状态2：特效闪烁
        const int State_OrbitRotation = 2;    // 状态3：绕NPC旋转

        switch ((int)State)
        {
            default:
            case State_LinearShoot:
                {
                    if (Timer < 30)
                    {
                        Projectile.velocity.X *= 0.92f;
                    }
                    else
                    {
                        if (Projectile.velocity.X < 20)
                            Projectile.velocity.X += 0.25f;
                    }

                    CurrentDistance += Projectile.velocity.X;
                    Projectile.Center = owner.Center + RecordedAngle.ToRotationVector2() * CurrentDistance;

                    if (CurrentDistance >= TargetDistance)
                    {
                        TargetDistance = CurrentDistance;
                        // 距离超过ai0，进入状态2且速度设为0
                        Projectile.velocity = Vector2.Zero;
                        State = State_FlashEffect;
                        Timer = 0;
                    }
                }
                break;

            case State_FlashEffect:
                {
                    // 弹幕状态2：特效闪烁（留空）
                    // 将弹幕中心直接设置为当前与NPC的相对角度乘以ai0
                    Vector2 direction = (Projectile.Center - owner.Center).SafeNormalize(Vector2.Zero);
                    float currentAngle = direction.ToRotation();
                    Projectile.Center = owner.Center + currentAngle.ToRotationVector2() * TargetDistance;

                    // 计时器大于30后进入弹幕状态3
                    if (Timer > 30)
                    {
                        // 进入时记录与NPC的角度方向
                        RecordedAngle = (Projectile.Center - owner.Center).ToRotation();
                        State = State_OrbitRotation;
                        Timer = 0;
                    }
                }
                break;

            case State_OrbitRotation:
                {
                    // 弹幕状态3：绕着NPC进行旋转
                    // 与NPC之间的角度每帧增加0.02f
                    RecordedAngle += 0.02f;
                    Projectile.rotation = RecordedAngle;

                    // 根据角度设置弹幕中心位置
                    Projectile.Center = owner.Center + RecordedAngle.ToRotationVector2() * TargetDistance;

                    // 60*4帧后消失
                    if (Timer > 60 * 4)
                    {
                        Projectile.Kill();
                    }
                }
                break;
        }

        if (oldLength != null)
        {
            //更新拖尾
            Projectile.UpdateOldRotCache();

            for (int i = 0; i < TrailCount - 1; i++)
                oldLength[i] = oldLength[i + 1];
            oldLength[^1] = CurrentDistance;//根据长度和角度计算位置
        }
    }

    public override bool? CanDamage()
    {
        // 新AI在状态2（特效闪烁）时不造成伤害
        if ((int)State == 1)
            return false;

        return base.CanDamage();
    }

    public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
    {
        float alpha = 1;
        DrawTrail();

        Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, Color.White, Color.White, 0.5f, 0, 0.5f, 0.5f, 1, 0, Vector2.One, Vector2.One);

        return false;
    }

    private void DrawTrail()
    {
        if (oldLength == null)
            return;
        if (!NPCIndex.GetNPCOwner<ShadowBall>(out NPC owner, Projectile.Kill))
            return;

        float off = 0;
        if (State == 0)
        {
            off = MathHelper.PiOver2;
        }

        for (int i = 0; i < TrailCount; i++)
        {
            float f = 1 - i / (float)TrailCount;
            Vector2 pos = owner.Center - Main.screenPosition
                + Projectile.oldRot[i].ToRotationVector2() * oldLength[i];
            Helper.DrawPrettyLine(1 - f, 0, pos, Color.Silver, Color.Silver, f, 0, 0.01f, 0.4f, 1, Projectile.oldRot[i] + MathHelper.PiOver2 + off, 1, Vector2.One);
        }
    }
}
