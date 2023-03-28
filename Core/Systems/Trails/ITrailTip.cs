using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.Trails
{
    public interface ITrailTip
    {
        int ExtraVertices { get; }

        int ExtraIndices { get; }

        void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction);
    }
}
