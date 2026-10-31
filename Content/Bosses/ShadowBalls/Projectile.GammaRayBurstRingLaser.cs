using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>伽玛射线暴的闭合贝塞尔激光环，先绘制白色预判线，再激活伤害。</summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public class GammaRayBurstRingLaser : CoraliteBossHostileProj
    {
        private const int TelegraphTime = 30;

        public override string Texture => AssetDirectory.Lasers + "MultLines";

        /// <summary>影子球本体的 NPC 索引</summary>
        public ref float OwnerIndex => ref Projectile.ai[0];
        /// <summary>额外的等待时间</summary>
        public ref float ExtraOffsetTime => ref Projectile.ai[1];
        public ref float Side => ref Projectile.ai[2];

        /// <summary>生成时的远端世界坐标，用于解析环的朝向和长度，不是移动速度</summary>
        public ref Vector2 EndPoint => ref Projectile.velocity;

        public static ATex MercuryBurstGradient { get; private set; }
        public List<Vector2> laserTrailPoints = new();

        private float laserLength;
        private float laserWidth;
        private int timer;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 24;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;
        public override bool? CanDamage()
        {
            if (Phase1())
            {
                return false;
            }

            int time = timer - TelegraphTime - (int)ExtraOffsetTime;
            if (time < 8 || time > 80 - 30)
            {
                return false;
            }

            return null;
        }

        public override void Initialize()
        {
            Vector2 offset = EndPoint - Projectile.Center;
            laserLength = offset.Length();
        }

        public override void AI()
        {
            const float MaxLaserWidth = 32f;
            const int ShootTime = 80;
            const int FadeTime = 30;

            laserTrailPoints?.Clear();
            if (!OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner))
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation=owner.rotation+(Side>0?0:MathHelper.Pi);

            Projectile.Center = owner.Center;
            if (timer < TelegraphTime + ExtraOffsetTime)
            {
                float f = timer / (TelegraphTime + ExtraOffsetTime);

                laserWidth = Helper.Lerp(laserWidth, 4, 0.05f);
            }
            else
            {
                int shootTimer = timer - TelegraphTime- (int)ExtraOffsetTime;

                if (shootTimer < 8)
                    laserWidth = 50f * (shootTimer + 1) / 8f;
                else if (shootTimer < ShootTime - FadeTime)
                {
                    laserWidth = Helper.Lerp(laserWidth, MaxLaserWidth, 0.5f);

                    laserLength += 2f;
                }
                else if (shootTimer < ShootTime)
                    laserWidth = Math.Max(0, laserWidth - MaxLaserWidth / FadeTime);
                else
                    Projectile.Kill();
            }

            UpdateCaches();
            timer++;
        }

        public bool Phase1()
        {
            return timer < TelegraphTime + ExtraOffsetTime;
        }

        private void UpdateCaches()
        {
            Vector2 origin = Projectile.Center;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 perpendicular = (Projectile.rotation-MathHelper.PiOver2).ToRotationVector2();
            Vector2 middle = origin + dir * laserLength;
            // 两端手柄等长，半段曲线的横向最大位移为手柄长度的 3/4。
            float handleLength = laserLength * 3f / 4f;
            float laserLengthF = laserLength / 30;
            Vector2 handle = perpendicular * handleLength;

            // 三次曲线的导数上界为 3 * 最大控制边长，确保每隔两点检测不会漏出大段空隙。
            int halfSegments = Math.Max(24, (int)MathF.Ceiling(3f * Math.Max(laserLength, handleLength) / 8f));
            int segments = halfSegments * 2;
            float noisePhase = timer * 0.08f;
            for (int i = 0; i <= segments; i++)
            {
                bool firstHalf = i <= halfSegments;
                float t = firstHalf ? i / (float)halfSegments : (i - halfSegments) / (float)halfSegments;
                Vector2 point = firstHalf
                    ? Helper.TwoHandleBezierEase(t, origin, middle, origin + handle, middle + handle)
                    : Helper.TwoHandleBezierEase(t, middle, origin, middle - handle, origin - handle);

                // 减去随 timer 增长的相位，使连续随机扰动沿取点顺序向前流动。
                // 沿环朝向最多偏移 4 像素，锚点处平滑归零以保持闭合接缝。
                float factor = i / (float)segments;
                float envelope = 4f * t * (t-1f);
                float noise =  SmoothNoise(factor * 22f - noisePhase + Projectile.identity * 1.37f);
                float value = MathF.Sin(factor * MathHelper.PiOver2);
                point += dir.RotatedBy(value*1.2f) * (noise * laserLengthF * envelope * envelope);
                laserTrailPoints.Add(point);
            }
        }

        private static float SmoothNoise(float value)
        {
            int index = (int)MathF.Floor(value);
            float fraction = value - index;
            fraction = fraction * fraction * (3f - 2f * fraction);
            return MathHelper.Lerp(RandomValue(index), RandomValue(index + 1), fraction);
        }

        private static float RandomValue(int index)
        {
            //相邻整数间插值后得到连续随机函数
            uint value = unchecked((uint)index + 0x9E3379B9u);
            value = unchecked((value ^ (value >> 16)) * 0x85EBCA6Bu);
            value = unchecked((value ^ (value >> 13)) * 0xC2B2AE35u);
            value ^= value >> 16;
            return (value & 0x00FFFFFFu) / 8388607.5f - 1f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Phase1() || laserWidth <= 0)
                return false;

            for (int i = 0; i < laserTrailPoints.Count; i += 3)
            {
                Vector2 point = laserTrailPoints[i];
                Rectangle hitbox = new((int)(point.X - 12), (int)(point.Y - 12), 24, 24);
                if (hitbox.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (laserTrailPoints.Count < 3 || laserWidth <= 0)
                return false;

            DrawPrimitive(Main.spriteBatch);
            return false;
        }

        public void DrawPrimitive(SpriteBatch spriteBatch)
        {
            if (laserTrailPoints.Count < 3)
                return;

            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            int last = laserTrailPoints.Count - 1;
            Texture2D mainTex = Projectile.GetTextureValue();
            float per = 2f * laserTrailPoints.Count / mainTex.Width;
            Vector2 fallback = Projectile.rotation.ToRotationVector2();

            for (int i = 0; i <= last; i++)
            {
                // 环的首尾是同一个点，使用跨接缝的邻点计算法线，避免闭合处翻折。
                Vector2 previous = laserTrailPoints[i == 0 ? last - 1 : i - 1];
                Vector2 next = laserTrailPoints[i == last ? 1 : i + 1];
                Vector2 tangent = (next - previous).SafeNormalize(fallback);
                Vector2 width = new Vector2(-tangent.Y, tangent.X) * laserWidth * MathF.Sin((float)i / last * MathHelper.Pi)*(1f+MathF.Sin((i)*0.1f + Main.GlobalTimeWrappedHourly*3) *0.3f);
                Vector2 center = laserTrailPoints[i];
                float u = (1f - i / (float)last) * per;
                bars.Add(new(center + width, Color.White, new Vector2(u, 0)));
                bars.Add(new(center - width, Color.White, new Vector2(u, 1)));
            }

            Effect effect = ShaderLoader.GetShader("ShadowLaser");
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1.5f + Projectile.whoAmI * 0.2143f);
            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(mainTex);
            effect.Parameters["gradientTexture"].SetValue(Phase1()
                ? CoraliteAssets.Misc.White32x32.Value : MercuryBurstGradient.Value);
            effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
            effect.Parameters["warptTexture"].SetValue(CoraliteAssets.Distortion.Twist.Value);
            effect.Parameters["cOff"].SetValue(0.05f);

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            }
        }
    }
}
