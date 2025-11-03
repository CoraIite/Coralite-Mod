using Terraria.DataStructures;

namespace Coralite.Core
{
    public struct Point8
    {
        public readonly sbyte X;
        public readonly sbyte Y;
        public static Point8 Zero = new Point8(0, 0);

        public Point8(Point point)
        {
            X = (sbyte)point.X;
            Y = (sbyte)point.Y;
        }

        public Point8(int X, int Y)
        {
            this.X = (sbyte)X;
            this.Y = (sbyte)Y;
        }

        public Point8(byte X, byte Y)
        {
            this.X = (sbyte)X;
            this.Y = (sbyte)Y;
        }

        public static bool operator ==(Point8 first, Point8 second)
        {
            if (first.X == second.X)
                return first.Y == second.Y;

            return false;
        }

        public static Point16 operator +(Point16 first, Point8 second)
        {
            return new Point16(first.X + second.X, first.Y + second.Y);
        }

        public static bool operator !=(Point8 first, Point8 second)
        {
            if (first.X == second.X)
                return first.Y != second.Y;

            return true;
        }

        public override bool Equals(object obj)
        {
            Point8 point = (Point8)obj;
            if (X != point.X || Y != point.Y)
                return false;

            return true;
        }

        public override int GetHashCode() => (X << 16) | (byte)Y;
        public override string ToString() => $"{{{X}, {Y}}}";

    }
}
