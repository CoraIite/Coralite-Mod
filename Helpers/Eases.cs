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

    }
}
