using System;
using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 两端平滑的贝塞尔曲线
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float BezierEase(float time)
        {
            return time * time / ((2f * ((time * time) - time)) + 1f);
        }

        public static float BezierEase(int t, int maxTime)
        {
            float time = (float)t / maxTime;
            return time * time / ((2f * ((time * time) - time)) + 1f);
        }

        /// <summary>
        /// 有一个手柄的贝塞尔曲线，手柄在起始位置
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="startHandle"></param>
        /// <returns></returns>
        public static Vector2 OneHandleBezierEase(float factor, Vector2 startPos, Vector2 endPos, Vector2 startHandle)
        {
            Vector2 P11 = Vector2.Lerp(startPos, startHandle, factor);
            Vector2 P12 = Vector2.Lerp(startHandle, endPos, factor);

            return Vector2.Lerp(P11, P12, factor);
        }

        /// <summary>
        /// 有两个手柄的贝塞尔曲线，需要lerp 6次，谨慎使用
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="startHandle"></param>
        /// <param name="endHandle"></param>
        /// <returns></returns>
        public static Vector2 TwoHandleBezierEase(float factor, Vector2 startPos, Vector2 endPos, Vector2 startHandle, Vector2 endHandle)
        {
            Vector2 P11 = Vector2.Lerp(startPos, startHandle, factor);
            Vector2 P12 = Vector2.Lerp(startHandle, endHandle, factor);
            Vector2 P13 = Vector2.Lerp(endHandle, endPos, factor);

            Vector2 P21 = Vector2.Lerp(P11, P12, factor);
            Vector2 P22 = Vector2.Lerp(P12, P13, factor);

            return Vector2.Lerp(P21, P22, factor);
        }


        public static float SwoopEase(float time)
        {
            return (3.75f * (float)Math.Pow(time, 3)) - (8.5f * (float)Math.Pow(time, 2)) + (5.75f * time);
        }

        /// <summary>
        /// 快速到达1后持续增大一小段最后再减小回1的曲线
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float HeavyEase(float time)
        {
            float x_1 = time - 1;
            return 1 + (2.6f * x_1 * x_1 * x_1) + (1.6f * x_1 * x_1);
        }

        /// <summary>
        /// 由角度和长短半轴长计算得到实际的长度
        /// </summary>
        /// <param name="rotation">角度</param>
        /// <param name="halfShortAxis">短半轴长</param>
        /// <param name="halfLongAxis">长半轴长</param>
        /// <returns></returns>
        public static float EllipticalEase(float rotation, float halfShortAxis, float halfLongAxis)
        {
            float halfFocalLength2 = (halfLongAxis * halfLongAxis) - (halfShortAxis * halfShortAxis);
            float cosX = MathF.Cos(rotation);
            return halfLongAxis * halfShortAxis / MathF.Sqrt((halfLongAxis * halfLongAxis) - (halfFocalLength2 * cosX * cosX));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rotation"></param>
        /// <param name="zRot"></param>
        /// <param name="overrideAngle"></param>
        /// <returns></returns>
        public static float EllipticalEase(float Rotation, float zRot, out float overrideAngle)
        {
            //先搞一个圆形，经过变换-Pi/2后就是在XY平面的一个圆
            //然后使用zRot将它转到指定的位置
            //此时的V3D的XY坐标就是投影的坐标
            Vector3 v3 = Rotation.ToRotationVector2().Vec3();
            Vector3 v3D = Vector3.Transform(v3, Matrix.CreateRotationX(zRot - MathHelper.PiOver2));

            //float k1 = -1000 / (v3D.Z - 1000);
            Vector2 targetDir = /*k1 **/ new(v3D.X, v3D.Y);
            overrideAngle = targetDir.ToRotation();
            return targetDir.Length();
        }

        public static float SqrtEase(float factor)
        {
            return MathF.Sqrt(factor);
        }

        public static float SqrtEase(int time, int maxTime)
        {
            return MathF.Sqrt((float)time / maxTime);
        }

        public static float X2Ease(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return factor * factor;
        }

        public static float X2Ease(float factor)
        {
            return factor * factor;
        }

        public static float X3Ease(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return factor * factor;
        }

        public static float X3Ease(float factor)
        {
            return factor * factor * factor;
        }

        public static float SinEase(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return MathF.Sin(factor * MathHelper.Pi);
        }

        public static float SinEase(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi);
        }

        public static float ReverseX2Ease(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            if (factor < 0.5f)
                return 4 * factor * factor;

            factor--;
            return 4 * factor * factor;
        }

        public static float ReverseX2Ease(float factor)
        {
            if (factor < 0.5f)
                return 4 * factor * factor;

            factor--;
            return 4 * factor * factor;
        }

        #region 常用的缓动函数，图像参见 https://easings.net/zh-cn
        public static float EaseInSine(float timer, float duration) => -(float)Math.Cos((double)(timer / duration * MathHelper.PiOver2)) + 1f;
        public static float EaseInSine(float factor) => EaseInSine(factor, 1f);

        public static float EaseOutSine(float timer, float duration) => (float)Math.Sin((double)(timer / duration * MathHelper.PiOver2));
        public static float EaseOutSine(float factor) => EaseOutSine(factor, 1f);

        public static float EaseInOutSine(float timer, float duration) => -0.5f * ((float)Math.Cos((double)(MathHelper.Pi * timer / duration)) - 1f);
        public static float EaseInOutSine(float factor) => EaseInOutSine(factor, 1f);

        public static float EaseInQuad(float timer, float duration) => (timer /= duration) / timer;
        public static float EaseInQuad(float factor) => EaseInQuad(factor, 1f);

        public static float EaseOutQuad(float timer, float duration) => -(timer /= duration) / (timer - 2f);
        public static float EaseOutQuad(float factor) => EaseOutQuad(factor, 1f);

        public static float EaseInOutQuad(float timer, float duration) => (timer /= duration / 2) < 1f ? timer * timer / 2 : -0.5f * ((timer -= 1f) * (timer - 2f) - 1f);
        public static float EaseInOutQuad(float factor) => EaseInOutQuad(factor, 1f);

        public static float EaseInCubic(float timer, float duration) => (timer /= duration) * timer * timer;
        public static float EaseInCubic(float factor) => EaseInCubic(factor, 1f);

        public static float EaseOutCubic(float timer, float duration) => (timer = timer / duration - 1f) * timer * timer + 1f;
        public static float EaseOutCubic(float factor) => EaseOutCubic(factor, 1f);

        public static float EaseInOutCubic(float timer, float duration) => (timer /= duration * 0.5f) < 1f ? 0.5f * timer * timer * timer : 0.5f * ((timer -= 2f) * timer * timer + 2f);
        public static float EaseInOutCubic(float factor) => EaseInOutCubic(factor, 1f);

        public static float EaseInQuart(float timer, float duration) => (timer /= duration) * timer * timer * timer;
        public static float EaseInQuart(float factor) => EaseInQuart(factor, 1f);

        public static float EaseOutQuart(float timer, float duration) => -((timer = timer / duration - 1f) * timer * timer * timer - 1f);
        public static float EaseOutQuart(float factor) => EaseOutQuart(factor, 1f);

        public static float EaseInOutQuart(float timer, float duration) => (timer /= duration * 0.5f) < 1f ? 0.5f * timer * timer * timer * timer : -0.5f * ((timer -= 2f) * timer * timer * timer - 2f);
        public static float EaseInOutQuart(float factor) => EaseInOutQuart(factor, 1f);

        public static float EaseInQuint(float timer, float duration) => (timer /= duration) * timer * timer * timer * timer;
        public static float EaseInQuint(float factor) => EaseInQuint(factor, 1f);

        public static float EaseOutQuint(float timer, float duration) => (timer = timer / duration - 1f) * timer * timer * timer * timer + 1f;
        public static float EaseOutQuint(float factor) => EaseOutQuint(factor, 1f);

        public static float EaseInOutQuint(float timer, float duration) => (timer /= duration * 0.5f) < 1f ? 0.5f * timer * timer * timer * timer * timer : 0.5f * ((timer -= 2f) * timer * timer * timer * timer + 2f);
        public static float EaseInOutQuint(float factor) => EaseInOutQuint(factor, 1f);

        public static float EaseInExpo(float timer, float duration) => timer == 0f ? 0f : (float)Math.Pow(2.0, (double)(10f * (timer / duration - 1f)));
        public static float EaseInExpo(float factor) => EaseInExpo(factor, 1f);

        public static float EaseOutExpo(float timer, float duration) => timer == duration ? 1f : -(float)Math.Pow(2.0, (double)(-10f * timer / duration)) + 1f;
        public static float EaseOutExpo(float factor) => EaseOutExpo(factor, 1f);

        public static float EaseInOutExpo(float timer, float duration)
        {
            if (timer == 0f)
                return 0f;
            if (timer == duration)
                return 1f;
            if ((timer /= duration * 0.5f) < 1f)
                return 0.5f * (float)Math.Pow(2.0, (double)(10f * (timer - 1f)));
            return 0.5f * (-(float)Math.Pow(2.0, (double)(-10f * (timer -= 1f))) + 2f);
        }
        public static float EaseInOutExpo(float factor) => EaseInOutExpo(factor, 1f);

        public static float EaseInCirc(float timer, float duration) => -((float)Math.Sqrt((double)(1f - (timer /= duration) * timer)) - 1f);
        public static float EaseInCirc(float factor) => EaseInCirc(factor, 1f);

        public static float EaseOutCirc(float timer, float duration) => (float)Math.Sqrt((double)(1f - (timer = timer / duration - 1f) * timer));
        public static float EaseOutCirc(float factor) => EaseOutCirc(factor, 1f);

        public static float EaseInOutCirc(float timer, float duration)
        {
            if ((timer /= duration * 0.5f) < 1f)
                return -0.5f * ((float)Math.Sqrt((double)(1f - timer * timer)) - 1f);
            return 0.5f * ((float)Math.Sqrt((double)(1f - (timer -= 2f) * timer)) + 1f);
        }
        public static float EaseInOutCirc(float factor) => EaseInOutCirc(factor, 1f);

        #endregion
    }
}

