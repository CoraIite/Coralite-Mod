using Coralite.Helpers;
using Microsoft.Xna.Framework;

namespace Coralite.Core.Systems.IKSystem
{
    public class Arrow
    {
        public float len;
        public float angle;

        public Vector2 angleLimt = new Vector2(-180f, 180f);
        public Vector2 StartPos { get; private set; }
        public Vector2 EndPos { get; private set; }
        public Vector2 Forward { get => Vector2.Normalize(EndPos - StartPos); }

        public void Follow(Vector2 target, Vector2 end, bool limt)
        {
            angle = (angle + Helper.SignedAngle(end - StartPos, target - StartPos)) % 360;
            if (limt && angleLimt.X != -180 || angleLimt.Y != 180)
            {
                angle = Helper.Clamp(angle, angleLimt.X, angleLimt.Y);
            }
        }

        public void CalculateStartAndEnd(Vector2 origin, Vector2 right)
        {
            StartPos = origin;
            EndPos = StartPos + Rotate(right, angle) * len;
        }

        private Vector2 Rotate(Vector2 v, float a)
        {
            a = a * Helper.Deg2Rad + Helper.Atan2(v.Y, v.X);
            return new Vector2(Helper.Cos(a), Helper.Sin(a));
        }
    }
}
