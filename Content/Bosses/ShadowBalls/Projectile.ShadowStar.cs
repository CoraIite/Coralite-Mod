using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// 使用ai0记录样式，ai1记录引星者索引，ai2在样式0时为每帧弹幕的角速度<br></br>
/// 弹幕时间固定30+70+30
/// </summary>
public class ShadowStar : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.ShadowBalls+Name;

    private ref float Timer => ref Projectile.localAI[1];

    /// <summary>
    /// 样式
    /// </summary>
    private ref float Style => ref Projectile.ai[0]; 
    private ref float OwnerIndex => ref Projectile.ai[1];
    private ref float Sp => ref Projectile.ai[2]; 

    public int TrailCount = 30;
    public int TexType = Main.rand.Next(2);

    public float alpha = 0;

    public Vector2 recordeCenter;
    public float  recordDistance;


    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 500;
    }

    public override bool? CanDamage()
    {
        switch (Style)
        {
            case 0:
                if (Timer < 30 || Timer > 60 + 30)
                    return false;
                break;
            case 1:
                if (Timer < 30 || Timer > Sp + 30)
                    return false;
                break;
            default:
                break;
        }

        return base.CanDamage();
    }

    public override void Initialize()
    {
        if (!OwnerIndex.GetProjectileOwner<StarsGuider>(out Projectile owner, Projectile.Kill))
        {
            return;
        }

        switch (Style)
        {
            default:
            case 0:
                recordeCenter = owner.Center;
                recordDistance = (Projectile.Center - owner.Center).Length();
                TrailCount = 10 + (int)(10 * (Sp / (StarsGuider.ArcHalfAngle * 2 / (30 + 60 + 30))));

                break;
            case 1:
                Projectile.rotation = Projectile.velocity.ToRotation();
                TrailCount = 10 + (int)(20 * 10 / Sp);
                break;
        }

        Projectile.InitOldRotCache(TrailCount);
        Projectile.InitOldPosCache(TrailCount);
    }

    public override void AI()
    {
        switch (Style)
        {
            default:
            case 0://星轨的环形运动轨迹
                StarlineMove(); 
                if (Timer < 30)
                    alpha = Helper.BezierEase(Timer / 30f);
                else if (Timer < 30 + 60)
                    alpha = 1;
                else if (Timer < 30 + 60 + 30)
                    alpha = 1 - Helper.BezierEase((Timer - 30 - 60) / 30f);
                else
                    Projectile.Kill();

                break;
            case 1:

                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Timer < 30)
                    alpha = Helper.BezierEase(Timer / 30f);
                else if (Timer < 30 + Sp)
                    alpha = 1;
                else if (Timer < 30 + Sp + 30)
                    alpha = 1 - Helper.BezierEase((Timer - 30 - Sp) / 30f);
                else
                    Projectile.Kill();

                break;
        }


        Projectile.UpdateOldPosCache();
        Projectile.UpdateOldRotCache();
        Timer++;
    }

    private void StarlineMove()
    {
        //一个环形移动
        Vector2 GuiderCenter = recordeCenter;
        float distance = recordDistance;

        float currRot = (Projectile.Center - GuiderCenter).ToRotation();

        Vector2 targetPos = (currRot + Sp).ToRotationVector2() * distance + GuiderCenter;
        Projectile.velocity = targetPos - Projectile.Center;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.oldPos.Length!=TrailCount)
        {
            return false;
        }

        Color trailColor = Color.Silver * alpha;
        Color c2 = Color.White * alpha;

        float length = 1 + TrailCount / 30f * 0.5f;
        float scale = 1 + TrailCount / 30f * 2f;

        Color lightGreen = new Color(230, 250, 81) * alpha;
        Color green = new Color(101, 209, 187) * alpha;

        for (int i = 0; i < TrailCount; i++)//绘制拖尾
        {
            float f = 1 - i / (float)TrailCount;
            Vector2 pos = Projectile.oldPos[i] - Main.screenPosition;
            Helper.DrawPrettyLine(1 - f, 0, pos, Color.Lerp(lightGreen, green, f), trailColor, f, 0, 0.01f, 0.4f, 1, Projectile.oldRot[i], scale, new Vector2(length, 1));
        }

        Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, c2 with { A = 0 }, Color.White, 0.5f, 0, 0.5f, 0.5f, 1, 0, Vector2.One, Vector2.One * 0.8f);

        Projectile.QuickFrameDraw(new Rectangle(0,TexType,1,2),c2, 0);

        return false;
    }
}
