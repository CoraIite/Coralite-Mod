namespace Coralite.Core.Systems.Trails
{
    public delegate float TrailThicknessCalculator(float factorAlongTrail);

    public delegate Color TrailColorEvaluator(Vector2 textureCoordinates);
}
