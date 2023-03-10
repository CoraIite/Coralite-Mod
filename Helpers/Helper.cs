using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        /// <summary>
        /// 角度转弧度系数
        /// </summary>
        public const float Deg2Rad = 0.0174532924f;

        public static Vector3 Vec3(this Vector2 vector) => new Vector3(vector.X, vector.Y, 0);

        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            float num = Angle(from, to);
            float num2 = Math.Sign(from.X * to.Y - from.Y * to.X);
            return num * num2;
        }

        public static float SqrMagnitude(this Vector2 vector2)
        {
            return vector2.X * vector2.X + vector2.Y + vector2.Y;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            float num = (float)Math.Sqrt(from.SqrMagnitude() * to.SqrMagnitude());
            if (num < 1E-15f)
            {
                return 0f;
            }

            float num2 = Clamp(Dot(from, to) / num, -1f, 1f);
            return (float)Math.Acos(num2) * 57.29578f;
        }

        /// <summary>
        /// 将value限定在min和max之间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static float Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        /// <summary>
        /// 仅仅是多了转float的Atan2
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }

        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }

        /// <summary>
        /// 检测线段碰撞（两个点之间连线和hitbox是否发生了碰撞）
        /// </summary>
        public static bool CheckLinearCollision(Vector2 point1, Vector2 point2, Rectangle hitbox, out Vector2 intersectPoint)
        {
            intersectPoint = Vector2.Zero;

            return
                LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.TopRight(), out intersectPoint) ||
                LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.BottomLeft(), out intersectPoint) ||
                LinesIntersect(point1, point2, hitbox.BottomLeft(), hitbox.BottomRight(), out intersectPoint) ||
                LinesIntersect(point1, point2, hitbox.TopRight(), hitbox.BottomRight(), out intersectPoint);
        }
        /// <summary>
        /// 计算两个线段的相交点
        /// </summary>
        public static bool LinesIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectPoint) //algorithm taken from http://web.archive.org/web/20060911055655/http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
        {
            intersectPoint = Vector2.Zero;

            var denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

            var a = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            var b = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);

            if (denominator == 0)
            {
                if (a == 0 || b == 0) //两条线是重合的
                {
                    intersectPoint = point3; //possibly not the best fallback?
                    return true;
                }

                else return false; //两条线是平行的
            }

            var ua = a / denominator;
            var ub = b / denominator;

            if (ua > 0 && ua < 1 && ub > 0 && ub < 1)
            {
                intersectPoint = new Vector2(point1.X + ua * (point2.X - point1.X), point1.Y + ua * (point2.Y - point1.Y));
                return true;
            }

            return false;
        }

        public static float BezierEase(float time)
        {
            return time * time / (2f * (time * time - time) + 1f);
        }

        public static float SwoopEase(float time)
        {
            return 3.75f * (float)Math.Pow(time, 3) - 8.5f * (float)Math.Pow(time, 2) + 5.75f * time;
        }

        /// <summary>
        /// f为0时返回a,为1时返回b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float f)
        {
            return (a * (1.0f - f)) + (b * f);
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
            float halfFocalLength2 = halfLongAxis * halfLongAxis - halfShortAxis * halfShortAxis;
            float cosx = (float)Math.Cos(rotation);
            return (float)(halfLongAxis * halfShortAxis / Math.Sqrt(halfLongAxis * halfLongAxis - halfFocalLength2 * cosx * cosx));
        }

        public static T[] FastUnion<T>(this T[] front, T[] back)
        {
            T[] combined = new T[front.Length + back.Length];

            Array.Copy(front, combined, front.Length);
            Array.Copy(back, 0, combined, front.Length, back.Length);

            return combined;
        }

        public static SlotId PlayPitched(string path, float volume, float pitch, Vector2? position = null)
        {
            if (Main.netMode == NetmodeID.Server)
                return SlotId.Invalid;

            var style = new SoundStyle($"{nameof(Coralite)}/Sounds/{path}")
            {
                Volume = volume,
                Pitch = pitch,
                MaxInstances = 0
            };

            return SoundEngine.PlaySound(style, position);
        }

        public static void NotOnServer(Action method)
        {
            if (Main.netMode != NetmodeID.Server)
                method();
        }
    }
}
