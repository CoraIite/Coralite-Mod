using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.Trails
{
    public class MeshRenderer : IDisposable
    {
        public bool CanDisposed { get; private set; }

        //暂存在显存中的顶点数据
        private DynamicVertexBuffer vertexDataBuffer;
        private DynamicIndexBuffer indexDataBuffer;

        private readonly GraphicsDevice device;

        public MeshRenderer(GraphicsDevice device, int maxVertices, int maxIndices)
        {
            this.device = device;

            if (device != null && !Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    vertexDataBuffer = new DynamicVertexBuffer(device, typeof(VertexPositionColorTexture), maxVertices, BufferUsage.None);
                    indexDataBuffer = new DynamicIndexBuffer(device, IndexElementSize.SixteenBits, maxIndices, BufferUsage.None);
                });
            }
        }

        public void Draw(Effect effect)
        {
            if (vertexDataBuffer is null || indexDataBuffer is null)
                return;

            device.SetVertexBuffer(vertexDataBuffer);
            device.Indices = indexDataBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexDataBuffer.VertexCount, 0, indexDataBuffer.IndexCount / 3);
            }
        }

        public void UpdateVertexBuffer(VertexPositionColorTexture[] vertices)
        {
            //SetDataOptions.Discard 这个的作用是表示不再需要之前的顶点信息，将在显存的一个新的位置存储数据，
            //此时显卡还仍然可以使用老数据，一旦写入过程完成，显卡将使用新数据而抛弃老数据
            vertexDataBuffer?.SetData(0, vertices, 0, vertices.Length, VertexPositionColorTexture.VertexDeclaration.VertexStride, SetDataOptions.Discard);
        }

        public void UpdateIndexBuffer(short[] indices)
        {
            indexDataBuffer?.SetData(0, indices, 0, indices.Length, SetDataOptions.Discard);
        }

        public void Dispose()
        {
            CanDisposed = true;

            vertexDataBuffer?.Dispose();
            indexDataBuffer?.Dispose();
        }
    }

}
