using Coralite.Core.Systems.FairyCatcherSystem;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        /// <summary> 角度转弧度系数 </summary>
        public const float Deg2Rad = 0.0174532924f;

        public static Vector3 RGBtoHSV(Color c)
        {
            const float Epsilon = 1e-10F;

            Vector3 rgb = c.ToVector3();
            Vector3 HCV = RGBtoHCV(c);
            float S = HCV.Y / (HCV.Z + Epsilon);
            return new Vector3(HCV.X, S, HCV.Z);
        }

        public static Vector3 RGBtoHCV(Color c)
        {
            const float Epsilon = 1e-10F;

            Vector3 rgb = c.ToVector3();
            Vector4 P = (rgb.Y < rgb.Z) ? new Vector4(rgb.Z, rgb.Y, -1.0f, 2 / 3f) : new Vector4(rgb.Y, rgb.Z, 0f, -1 / 3f);
            Vector4 Q = (rgb.X < P.X) ? new Vector4(P.X, P.Y, P.W, rgb.X) : new Vector4(rgb.X, P.Y, P.Z, P.X);
            float C = Q.X - Math.Min(Q.W, Q.Y);
            float H = Math.Abs((Q.W - Q.Y) / ((6 * C) + Epsilon + Q.Z));

            return new Vector3(H, C, Q.X);
        }

        public static Vector3 Vec3(this Vector2 vector) => new(vector.X, vector.Y, 0);

        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            float num = Angle(from, to);
            float num2 = Math.Sign((from.X * to.Y) - (from.Y * to.X));
            return num * num2;
        }

        public static float SqrMagnitude(this Vector2 vector2)
        {
            return (vector2.X * vector2.X) + vector2.Y + vector2.Y;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            float num = (float)Math.Sqrt(from.SqrMagnitude() * to.SqrMagnitude());
            if (num < 1E-15f)
            {
                return 0f;
            }

            float num2 = Clamp(Dot(from, to) / num, -0.9999f, 0.9999f);
            return (float)Math.Acos(num2) * 57.29578f;
        }

        /// <summary>
        /// 计算两个向量的弧度夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float AngleRad(Vector2 from, Vector2 to)
        {
            float num = (float)Math.Sqrt(from.SqrMagnitude() * to.SqrMagnitude());
            if (num < 1E-15f)
            {
                return 0f;
            }

            float num2 = Clamp(Dot(from, to) / num, -0.9999f, 0.9999f);
            return (float)Math.Acos(num2);
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
            return (lhs.X * rhs.X) + (lhs.Y * rhs.Y);
        }

        /// <summary>
        /// 随机一个角度的方向向量
        /// </summary>
        /// <returns></returns>
        public static Vector2 NextVec2Dir()
        {
            return Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
        }

        /// <summary>
        /// 随机一个角度的方向向量，并随机长度
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector2 NextVec2Dir(float min, float max)
        {
            return Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(min, max);
        }

        /// <summary>
        /// 把一个向量随机旋转
        /// </summary>
        /// <param name="baseVec2"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector2 RotateByRandom(this Vector2 baseVec2, float min, float max)
        {
            return baseVec2.RotatedBy(Main.rand.NextFloat(min, max));
        }

        public static bool PointInTile(Vector2 point)
        {
            var startCoords = new Point16((int)point.X / 16, (int)point.Y / 16);
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point16 thisPoint = startCoords + new Point16(x, y);

                    if (!WorldGen.InWorld(thisPoint.X, thisPoint.Y))
                        return false;

                    Tile tile = Framing.GetTileSafely(thisPoint);

                    if (Main.tileSolid[tile.TileType] && tile.HasUnactuatedTile && !Main.tileSolidTop[tile.TileType])
                    {
                        var rect = new Rectangle(thisPoint.X * 16, thisPoint.Y * 16, 16, 16);

                        if (rect.Contains(point.ToPoint()))
                            return true;
                    }
                }
            }

            return false;
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

            var denominator = ((point4.Y - point3.Y) * (point2.X - point1.X)) - ((point4.X - point3.X) * (point2.Y - point1.Y));

            var a = ((point4.X - point3.X) * (point1.Y - point3.Y)) - ((point4.Y - point3.Y) * (point1.X - point3.X));
            var b = ((point2.X - point1.X) * (point1.Y - point3.Y)) - ((point2.Y - point1.Y) * (point1.X - point3.X));

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
                intersectPoint = new Vector2(point1.X + (ua * (point2.X - point1.X)), point1.Y + (ua * (point2.Y - point1.Y)));
                return true;
            }

            return false;
        }

        #region 各种插值函数

        /// <summary>
        /// 两端平滑的贝塞尔曲线
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float BezierEase(float time)
        {
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

        #endregion

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

        public static SlotId PlayPitched(SoundStyle style, Vector2? position = null, float? volume = null, float? pitch = null, float volumeAdjust = 0, float pitchAdjust = 0)
        {
            if (Main.netMode == NetmodeID.Server)
                return SlotId.Invalid;

            if (volume.HasValue)
                style.Volume = volume.Value;

            if (pitch.HasValue)
                style.Pitch = pitch.Value;

            style.Volume += volumeAdjust;
            style.Pitch += pitchAdjust;

            return SoundEngine.PlaySound(style, position);
        }

        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

        public static bool OnScreen(Rectangle rect) => rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));

        public static bool OnScreen(Vector2 pos, Vector2 size) => OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));

        public static void NotOnServer(Action method)
        {
            if (Main.netMode != NetmodeID.Server)
                method();
        }

        public static Rectangle QuickMouseRectangle()
           => new((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 2, 2);

        /// <summary>
        /// 将你的值根据不同模式来改变
        /// </summary>
        /// <param name="normalModeValue">普通模式</param>
        /// <param name="expertModeValue">专家模式</param>
        /// <param name="masterModeValue">大师模式</param>
        /// <param name="FTWModeValue">For The Worthy模式（传奇难度）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ScaleValueForDiffMode<T>(T normalModeValue, T expertModeValue, T masterModeValue, T FTWModeValue)
        {
            T value = normalModeValue;
            if (Main.expertMode)
                value = expertModeValue;

            if (Main.masterMode)
                value = masterModeValue;

            if (Main.getGoodWorld)
                value = FTWModeValue;

            return value;
        }


        public static EntitySource_FairyCatch GetSource_FairyCatch(this Player player, Fairy catchedFairy)
        {
            return new EntitySource_FairyCatch() { player = player, fairy = catchedFairy };
        }
    }
}