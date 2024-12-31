using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.Trails
{
    public interface IMeshTrailGenerator
    {
        int AdditionalVertexCount { get; }

        int AdditionalIndexCount { get; }

        void CreateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailThicknessCalculator trailWidthFunction, TrailColorEvaluator trailColorFunction);
    }
}
