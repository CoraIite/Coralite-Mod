using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.Trails
{
    public class NoTip : ITrailTip
    {
        public int ExtraVertices => 0;

        public int ExtraIndices => 0;

        public void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            vertices = new VertexPositionColorTexture[0];
            indices = new short[0];
        }
    }
}
