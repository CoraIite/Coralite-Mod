using Coralite.Content.Items.AlchorthentSeries;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public class EclipseMoon : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    private ref float EclipseType => ref Projectile.ai[0];
    private ref float State => ref Projectile.ai[1];
    private ref float CircleRadius => ref Projectile.ai[2];
    private ref float Timer => ref Projectile.localAI[0];

    private LineDrawer circle;
    private LineDrawer moonLine;

    private bool HasMoonLine => EclipseType is 1 or 2 or 3 or 5 or 6 or 7;

    private static int MoonMorphTime = 15;
    private static int StarFlashTime = 25;
    private static int ShrinkTime = 25;
    private static  int CircleExpandTime = 15;


    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 45;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void Initialize()
    {
        if (Main.dedServ)
            return;

        // 单位圆通过SetScale控制实际半径。
        circle = new LineDrawer([
            new LineDrawer.WarpLine(Vector2.UnitX, 64,
                factor => (factor * MathHelper.TwoPi).ToRotationVector2(),CoraliteAssets.Laser.MultLinesSPA)
        ]);
        circle.SetLineWidth(32);
        circle.SetScale(0);

        if (HasMoonLine)
        {
            moonLine = new LineDrawer([
                new LineDrawer.WarpLine(GetMoonLinePoint(0), 64, GetMoonLinePoint)
            ]);
            moonLine.SetLineWidth(32);
            moonLine.SetScale(CircleRadius);
        }
    }

    // localAI不会自动同步，随状态同步包传递当前计时。
    public override void SendExtraAI(BinaryWriter writer) => writer.Write(Timer);

    public override void ReceiveExtraAI(BinaryReader reader) => Timer = reader.ReadSingle();

    public override bool? CanDamage() => State == 2 ? base.CanDamage() : false;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 closest = new(
            MathHelper.Clamp(Projectile.Center.X, targetHitbox.Left, targetHitbox.Right),
            MathHelper.Clamp(Projectile.Center.Y, targetHitbox.Top, targetHitbox.Bottom));
        return Vector2.DistanceSquared(Projectile.Center, closest) <= CircleRadius * CircleRadius;
    }
    public override bool ShouldUpdatePosition() => false;

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        const int AttackTime = 20;

        switch (State)
        {
            case 0:
                Timer++;
                if (Timer >= StarFlashTime)
                {
                    State = 1;
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
                break;
            case 1:
                Timer++;
                circle?.SetScale(CircleRadius * Helper.SqrtEase(MathHelper.Clamp(Timer / CircleExpandTime, 0, 1)));
                if (Timer >= CircleExpandTime)
                {
                    State = 2;
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
                break;
            case 2:
                Timer++;
                if (Timer >= AttackTime)
                {
                    State = 3;
                    Timer = 0;
                    Projectile.netUpdate = true;
                }
                break;
            case 3:
                Timer++;
                if (Timer >= ShrinkTime)
                    Projectile.Kill();
                break;
        }
    }

    // 半椭圆的上下端点固定在单位圆上，横向系数控制月相弯曲程度。
    private Vector2 GetMoonLinePoint(float factor)
    {
        float morph = State == 2 ? MathHelper.Clamp(Timer / MoonMorphTime, 0, 1) : 1;
        float startBend = EclipseType < 4 ? 1 : -1;
        float targetBend = EclipseType switch
        {
            3 or 7 => MathF.Sqrt(0.5f),
            1 or 5 => -MathF.Sqrt(0.5f),
            _ => 0
        };
        float bend = MathHelper.Lerp(startBend, targetBend, morph);
        float angle = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, factor);
        return new Vector2(MathF.Cos(angle) * bend, MathF.Sin(angle)).RotatedBy(Projectile.rotation);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (State == 0)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Clamp(Timer / StarFlashTime, 0, 1);
            Vector2 scale = Vector2.One * Projectile.scale*2.5f;
            Color starColor = Color.MediumPurple;

            // 底层：不旋转，前半段渐亮，后半段渐暗。
            Helper.DrawPrettyStarSparkle(1, 0, pos, starColor, starColor,
                factor, 0, 0.5f, 0.5f, 1, 0, scale, scale);

            // 高光层：整体尺寸为底层的0.7倍。
            Helper.DrawPrettyStarSparkle(1, 0, pos, Color.White, Color.White,
                factor, 0, 0.5f, 0.5f, 1, 0, scale * 0.7f, scale * 0.7f);

            // 第三层：随状态进度旋转一圈。
            float rotation = MathHelper.Lerp(0, MathHelper.TwoPi, factor);
            Helper.DrawPrettyStarSparkle(1, 0, pos, starColor, starColor,
                factor, 0, 0.5f, 0.5f, 1, rotation, new Vector2(1.75f, 1f) * Projectile.scale, scale);
        }

        else if (State is 1 or 2 or 3)
            DrawCircle();

        return false;
    }

    private void DrawCircle()
    {
        if (circle == null || (State == 1 && Timer <= 0))
            return;

        float fade = State == 3 ? 1 - MathHelper.Clamp(Timer / ShrinkTime, 0, 1) : 1;
        float radius = State == 1
            ? CircleRadius * Helper.SqrtEase(MathHelper.Clamp(Timer / CircleExpandTime, 0, 1))
            : CircleRadius * fade;
        circle.SetScale(radius);

        Effect shader = ShaderLoader.GetShader("LineAdditive");
        shader.Parameters["uFlowTex"]?.SetValue(CoraliteAssets.Laser.TwistLaser.Value);
        shader.Parameters["uTime"]?.SetValue((int)Main.timeForVisualEffects * 0.02f);
        shader.Parameters["flowAdd"]?.SetValue(4);
        shader.Parameters["lineO"]?.SetValue(1f);
        shader.Parameters["lineC"]?.SetValue(Color.MediumPurple.ToVector4() * fade);
        shader.Parameters["powC"]?.SetValue(0.2f);
        shader.Parameters["lineEx"]?.SetValue(0.5f);
        shader.Parameters["transformMatrix"]?.SetValue(VaultUtils.GetTransfromMatrix());

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
            SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone,
            shader, Main.GameViewMatrix.TransformationMatrix);

        shader.CurrentTechnique.Passes[0].Apply();
        circle.Draw(Projectile.Center);

        if (HasMoonLine && moonLine != null && (State == 2 || State == 3))
        {
            float alpha = State == 2 ? MathHelper.Clamp(Timer / MoonMorphTime, 0, 1) : fade;
            moonLine.SetScale(radius);
            // 起点也随朝向旋转，与曲线委托保持一致。
            moonLine.lines[0].StartPos = GetMoonLinePoint(0);
            shader.Parameters["lineC"]?.SetValue(Color.MediumPurple.ToVector4() * alpha);
            shader.CurrentTechnique.Passes[0].Apply();
            moonLine.Draw(Projectile.Center);
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
            SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            null, Main.GameViewMatrix.TransformationMatrix);
    }
}
