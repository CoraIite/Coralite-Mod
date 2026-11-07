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
    public override string Texture => AssetDirectory.ShadowBalls+Name;

    private ref float Timer => ref Projectile.localAI[1];

    /// <summary>
    /// 目标距离
    /// </summary>
    private ref float Style => ref Projectile.ai[0]; 
    private ref float OwnerIndex => ref Projectile.ai[1];
    private ref float Sp => ref Projectile.ai[2]; 

    public const int TrailCount = 30;

    public float alpha = 0;

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
        if (!OwnerIndex.GetProjectileOwner<StarsGuider>(out Projectile owner, Projectile.Kill))
        {
            return;
        }

        Projectile.InitOldRotCache(TrailCount);
        Projectile.InitOldPosCache(TrailCount);
    }

    public override void AI()
    {
        if (!OwnerIndex.GetProjectileOwner<StarsGuider>(out Projectile owner, Projectile.Kill))
        {
            return;
        }

        switch (Style)
        {
            default:
            case 0://星轨的环形运动轨迹
                StarlineMove(owner);
                break;
            case 1:
                break;
        }

        if (Timer < 30)
        {
            alpha = Helper.BezierEase(Timer / 30f);
        }
        else if (Timer < 30 + 80)
        {
            alpha = 1;
        }
        else if (Timer < 30 + 80 + 30)
        {
            alpha = 1 - Helper.BezierEase((Timer - 30 - 80) / 30f);
        }
        else
            Projectile.Kill();

        Projectile.UpdateOldPosCache();
        Timer++;
    }

    private void StarlineMove(Projectile owner)
    {
        //一个环形移动
        Vector2 GuiderCenter = owner.Center;
        float distance = owner.ai[1];

        float currRot = (Projectile.Center - GuiderCenter).ToRotation();

        Vector2 targetPos = (currRot + Sp).ToRotationVector2() * distance + GuiderCenter;
        Projectile.velocity = targetPos - Projectile.Center;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color trailColor = Color.Silver * alpha;
        Color c2 = Color.Wheat * alpha;

        for (int i = 0; i < TrailCount; i++)//绘制拖尾
        {
            float f = 1 - i / (float)TrailCount;
            Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.oldPos[i];
            Helper.DrawPrettyLine(1 - f, 0, pos, trailColor, trailColor, f, 0, 0.01f, 0.4f, 1, Projectile.oldRot[i] + MathHelper.PiOver2 , 1, Vector2.One);
        }

        Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, c2, Color.White, 0.5f, 0, 0.5f, 0.5f, 1, 0, Vector2.One, Vector2.One);

        Projectile.QuickDraw(c2, 0.5f, 0f);


        return false;
    }
}
