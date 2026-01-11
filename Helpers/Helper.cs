using Coralite.Core.Systems.FairyCatcherSystem;
using ReLogic.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

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

        public static SlotId PlayPitched(string path, float volume, float pitch, Vector2? position = null, Action<SoundStyle> soundAdjust = null)
        {
            if (VaultUtils.isServer)
                return SlotId.Invalid;

            var style = new SoundStyle($"{nameof(Coralite)}/Sounds/{path}")
            {
                Volume = volume,
                Pitch = pitch,
                MaxInstances = 0
            };

            soundAdjust?.Invoke(style);

            return SoundEngine.PlaySound(style, position);
        }

        public static SlotId PlayPitched(SoundStyle style, Vector2? position = null, float? volume = null, float? pitch = null, float volumeAdjust = 0, float pitchAdjust = 0)
        {
            if (VaultUtils.isServer)
                return SlotId.Invalid;

            if (volume.HasValue)
                style.Volume = volume.Value;

            if (pitch.HasValue)
                style.Pitch = pitch.Value;

            style.Volume += volumeAdjust;
            style.Pitch += pitchAdjust;

            return SoundEngine.PlaySound(style, position);
        }

        public static bool IsPointOnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

        public static bool IsRectangleOnScreen(Rectangle rect) => rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));

        public static bool IsAreaOnScreen(Vector2 pos, Vector2 size) => IsRectangleOnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));

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

        public static Point NextInRectangleEdge(this UnifiedRandom rand, Point topLeft, Point size)
        {
            return rand.Next(4) switch
            {
                0 => new Point(topLeft.X + rand.Next(size.X), topLeft.Y),//顶部一条
                1 => new Point(topLeft.X, topLeft.Y + rand.Next(size.Y)),//左边一条
                2 => new Point(topLeft.X + size.X, topLeft.Y + rand.Next(size.Y)),//右边一条
                _ => new Point(topLeft.X + rand.Next(size.X), topLeft.Y + size.Y),//底部一条
            };
        }


        public static EntitySource_FairyCatch GetSource_FairyCatch(this Player player, Fairy catchedFairy)
        {
            return new EntitySource_FairyCatch() { player = player, fairy = catchedFairy };
        }

        /// <summary>
        /// 获得一个物品，需要指定数量，还可以指定类型<br></br>
        /// 如果数量不足则会只拿出该拿的
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Item GetItem(this Chest chest, int stack, int? type = null)
        {
            Item[] items = chest.item;
            for (int i = 0; i < items.Length; i++)
            {
                Item item = items[i];
                if (item == null || item.IsAir)
                    continue;

                if (type != null)//有指定的类型
                {
                    if (item.type == type.Value)//正好对上了
                        return NewItem(stack, item);

                    continue;
                }

                return NewItem(stack, item);
            }

            return null;

            static Item NewItem(int stack, Item item)
            {
                if (item.stack > stack)//数量多，直接减少
                {
                    item.stack -= stack;

                    Item item1 = item.Clone();
                    item1.stack = stack;
                    return item1;
                }
                else//数量不够，全部返回，自身重置
                {
                    Item item1 = item.Clone();
                    item.TurnToAir();
                    return item1;
                }
            }
        }

        /// <summary>
        /// 能否放入一个物品
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemType"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static bool CanAddItem(this Chest chest, int itemType, int stack)
        {
            for (int i = 0; i < chest.item.Length; i++)
            {
                Item item = chest.item[i];//有空物品或者容量足够就放入
                if (item == null)
                    continue;

                if (item.IsAir || item.type == itemType && item.stack < item.maxStack - stack)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 向箱子内加入一个物品，会按顺序遍历箱子之后添加在最后一个，如果箱子满了就会掉落出来
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        public static void AddItem(this Chest chest, Item itemIn)
        {
            int type = itemIn.type;
            int stack = itemIn.stack;

            foreach (var i in chest.item.Where(i => !i.IsAir && i.type == type && i.stack < i.maxStack))
            {
                int maxCanInsert = Math.Min(i.maxStack - i.stack, stack);
                i.stack += maxCanInsert;
                stack -= maxCanInsert;
                if (stack < 1)
                {
                    itemIn.TurnToAir();
                    return;
                }
            }

            for (int i = 0; i < chest.item.Length; i++)
                if (chest.item[i].IsAir)
                {
                    chest.item[i] = itemIn.Clone();
                    itemIn.TurnToAir();
                    return;
                }

            Item.NewItem(itemIn.GetSource_DropAsItem(), GetTileCenter(new Point(chest.x, chest.y))
                , itemIn.Clone());
            itemIn.TurnToAir();
        }

        public static SetFactory.NamedSetKey CreateCoraliteSet(this SetFactory f, string name)
            => f.CreateNamedSet(nameof(Coralite), name);

        public static void SaveBools(this TagCompound tag, bool[] bools, string name)
        {
            int length = bools.Length;
            int count = length / 8 + 1;

            int k = 0;

            for (int i = 0; i < count; i++)
            {
                BitsByte b = new BitsByte();
                for (int j = 0; j < 8; j++)
                {
                    b[j] = bools[k];
                    k++;
                    if (k > length - 1)
                    {
                        tag.Add(name + i.ToString(), (byte)b);
                        goto over;
                    }
                }

                tag.Add(name + i.ToString(), (byte)b);
            }

        over:
            ;
        }

        /// <summary>
        /// 需要提前设置好bools
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="bools"></param>
        /// <param name="name"></param>
        public static void LoadBools(this TagCompound tag, bool[] bools, string name)
        {
            int length = bools.Length;
            int count = length / 8 + 1;

            int k = 0;

            for (int i = 0; i < count; i++)
            {
                if (!tag.TryGet(name + i.ToString(), out byte b1))
                    continue;

                BitsByte b = b1;

                for (int j = 0; j < 8; j++)
                {
                    bools[k] = b[j];
                    k++;
                    if (k > length - 1)
                        return;
                }
            }
        }

        /// <summary>
        /// 一个bool数组内是否全部为true
        /// </summary>
        /// <param name="bools"></param>
        /// <returns></returns>
        public static bool AllTrue(this bool[] bools)
        {
            foreach (var c in bools)
                if (!c)
                    return false;

            return true;
        }
    }
}