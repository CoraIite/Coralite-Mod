using Microsoft.Xna.Framework;

namespace Coralite.Core.Systems.Trails
{
    public class DelegateFunctions
    {
        public delegate float TrailWidthFunction(float factorAlongTrail);

        public delegate Color TrailColorFunction(Vector2 textureCoordinates);
    }
}
