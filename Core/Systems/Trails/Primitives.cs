using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.Trails
{
    public class Primitives : IDisposable
    {
        public bool IsDisposed { get; private set; }

        //暂存在显存中的顶点数据
        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;

        private readonly GraphicsDevice device;

        public Primitives(GraphicsDevice device, int maxVertices, int maxIndices)
        {
            this.device = device;

            if (device != null && !Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    vertexBuffer = new DynamicVertexBuffer(device, typeof(VertexPositionColorTexture), maxVertices, BufferUsage.None);
                    indexBuffer = new DynamicIndexBuffer(device, IndexElementSize.SixteenBits, maxIndices, BufferUsage.None);
                });
            }
        }

        public void Render(Effect effect)
        {
            if (vertexBuffer is null || indexBuffer is null)
                return;

            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }
        }

        public void SetVertices(VertexPositionColorTexture[] vertices)
        {
            //SetDataOptions.Discard 这个的作用是表示不再需要之前的顶点信息，将在显存的一个新的位置存储数据，
            //此时显卡还仍然可以使用老数据，一旦写入过程完成，显卡将使用新数据而抛弃老数据
            vertexBuffer?.SetData(0, vertices, 0, vertices.Length, VertexPositionColorTexture.VertexDeclaration.VertexStride, SetDataOptions.Discard);
        }

        public void SetIndices(short[] indices)
        {
            indexBuffer?.SetData(0, indices, 0, indices.Length, SetDataOptions.Discard);
        }

        public void Dispose()
        {
            IsDisposed = true;

            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
        }
    }

}
