using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core.Systems.Trails
{
    public class EmptyMeshGenerator : IMeshTrailGenerator
    {
        public int AdditionalVertexCount => 0;

        public int AdditionalIndexCount => 0;

        public void CreateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailThicknessCalculator trailWidthFunction, TrailColorEvaluator trailColorFunction)
        {
            vertices = new VertexPositionColorTexture[0];
            indices = new short[0];
        }
    }
}
