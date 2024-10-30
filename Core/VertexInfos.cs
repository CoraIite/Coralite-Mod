using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Core
{
    public struct CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord) : IVertexType
    {
        public Vector2 Position = position;
        public Color Color = color;
        public Vector3 TexCoord = texCoord;

        private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
        {
                new(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
                new(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
                new(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }

    public struct VertexInfo(Vector2 position, Vector3 texCoord) : IVertexType
    {
        public Vector2 Position = position;
        public Vector3 TexCoord = texCoord;

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        private static VertexDeclaration _vertexDeclaration = new(new VertexElement[]
        {
                new(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
                new(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
        });
    }
}
