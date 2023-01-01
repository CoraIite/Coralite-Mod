using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core
{
    /*
    * 复制黏贴自星光之河mod
    *稍微能看懂一点
    *但是我自己肯定是写不出来这些代码的
    *TAT
    */
    public class Primitives : IDisposable
    {
        public bool IsDisposed { get; private set; }

        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;

        private readonly GraphicsDevice device;

        public Primitives(GraphicsDevice device, int maxVertices, int maxIndices)
        {
            this.device = device;

            if (device != null)
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

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            }
        }

        public void SetVertices(VertexPositionColorTexture[] vertices)
        {
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

    public interface ITrailTip
    {
        int ExtraVertices { get; }

        int ExtraIndices { get; }

        void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction);
    }

    public delegate float TrailWidthFunction(float factorAlongTrail);

    public delegate Color TrailColorFunction(Vector2 textureCoordinates);

    public class Trail : IDisposable
    {
        private readonly Primitives primitives;

        private readonly int maxPointCount;

        private readonly ITrailTip tip;

        private readonly TrailWidthFunction trailWidthFunction;

        private readonly TrailColorFunction trailColorFunction;

        /// <summary>
        /// Array of positions that define the trail. NOTE: Positions[Positions.Length - 1] is assumed to be the start (e.g. Projectile.Center) and Positions[0] is assumed to be the end.
        /// 决定拖尾的位置的数组
        /// </summary>
        public Vector2[] Positions
        {
            get => positions;
            set
            {
                if (value.Length != maxPointCount)
                {
                    throw new ArgumentException("Array of positions was a different length than the expected result!");
                }

                positions = value;
            }
        }

        private Vector2[] positions;

        /// <summary>
        /// Used in order to calculate the normal from the frontmost position, because there isn't a point after it in the original list.
        /// 用于计算最前面的点，因为原本列表中没有
        /// </summary>
        public Vector2 NextPosition { get; set; }

        private const float defaultWidth = 16;

        public Trail(GraphicsDevice device, int maxPointCount, ITrailTip tip, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            this.tip = tip ?? new NoTip();

            this.maxPointCount = maxPointCount;

            this.trailWidthFunction = trailWidthFunction;

            this.trailColorFunction = trailColorFunction;

            /* A---B---C
             * |  /|  /|
             * D / E / F
             * |/  |/  |
             * G---H---I
             * 
             * Let D, E, F, etc. be the set of n points that define the trail.
             * 让D,E,F等是定义轨迹的n个点的集合
             * Since each point generates 2 vertices, there are 2n vertices, plus the tip's count.
             * 每个点生成2个矩阵，所以有2n个矩阵，再加上尖端的
             * As for indices - in the region between 2 defining points there are 2 triangles.
             * 作为目录，在2个定义点之间的区域中有2个三角形
             * The amount of regions in the whole trail are given by n - 1, so there are 2(n - 1) triangles for n points.
             * 整个路径中的区域数量由 n - 1 给出，因此 n 个点有 2（n - 1） 个三角形。
             * Finally, since each triangle is defined by 3 indices, there are 6(n - 1) indices, plus the tip's count.
             * 最后，由于每个三角形由 3 个索引定义，因此有 6（n - 1） 个索引，再加上尖端的计数。
             */

            primitives = new Primitives(device, (maxPointCount * 2) + this.tip.ExtraVertices, (6 * (maxPointCount - 1)) + this.tip.ExtraIndices);
        }

        private void GenerateMesh(out VertexPositionColorTexture[] vertices, out short[] indices, out int nextAvailableIndex)
        {
            VertexPositionColorTexture[] verticesTemp = new VertexPositionColorTexture[maxPointCount * 2];

            short[] indicesTemp = new short[maxPointCount * 6 - 6];

            // k = 0 indicates starting at the end of the trail (furthest from the origin of it).
            //表示从尾迹的最后开始（距离其原点最远）。
            for (int k = 0; k < Positions.Length; k++)
            {
                // 1 at k = Positions.Length - 1 (start) and 0 at k = 0 (end).
                //插值，k=0是为0，k=坐标数组长度-1时为1
                float factorAlongTrail = (float)k / (Positions.Length - 1);

                // Uses the trail width function to decide the width of the trail at this point (if no function, use 
                //使用尾迹宽度函数去决定这个点的尾迹宽度（如果没有函数，使用）（？？）
                float width = trailWidthFunction?.Invoke(factorAlongTrail) ?? defaultWidth;

                Vector2 current = Positions[k];
                Vector2 next = (k == Positions.Length - 1 ? Positions[Positions.Length - 1] + (Positions[Positions.Length - 1] - Positions[Positions.Length - 2]) : Positions[k + 1]);

                Vector2 normalToNext = (next - current).SafeNormalize(Vector2.Zero);
                Vector2 normalPerp = normalToNext.RotatedBy(MathHelper.PiOver2);

                /* A
                 * |
                 * B---D
                 * |
                 * C
                 * 
                 * Let B be the current point and D be the next one.
                 * 让B点是当前的点，D点是下一个点
                 * A and C are calculated based on the perpendicular vector to the normal from B to D, scaled by the desired width calculated earlier.
                 * A 和 C 基于从 B 到 D 的法线垂直矢量计算，并按先前计算的所需宽度进行缩放
                 */

                Vector2 a = current + (normalPerp * width);
                Vector2 c = current - (normalPerp * width);

                /* Texture coordinates are calculated such that the top-left is (0, 0) and the bottom-right is (1, 1).
                 * 纹理坐标的计算使得左上角为 （0， 0），右下角为 （1， 1）
                 * To achieve this, we consider the Y-coordinate of A to be 0 and that of C to be 1, while the X-coordinate is just the factor along the trail.
                 * 为了实现这一点，我们认定 A 的 Y 坐标为 0，C 的 Y 坐标为 1，而 X 坐标只是沿轨迹的插值。
                 * This results in the point last in the trail having an X-coordinate of 0, and the first one having a Y-coordinate of 1.
                 * 这导致尾迹中最后一个点的 X 坐标为 0，而第一个点的 Y 坐标为 1。
                 */
                Vector2 texCoordA = new Vector2(factorAlongTrail, 0);
                Vector2 texCoordC = new Vector2(factorAlongTrail, 1);

                // Calculates the color for each vertex based on its texture coordinates. This acts like a very simple shader (for more complex effects you can use the actual shader).
                //根据每个顶点的纹理坐标计算其颜色。这就像一个非常简单的着色器（对于更复杂的效果，你可以使用实际的着色器）
                Color colorA = trailColorFunction?.Invoke(texCoordA) ?? Color.White;
                Color colorC = trailColorFunction?.Invoke(texCoordC) ?? Color.White;

                /* 0---1---2
                 * |  /|  /|
                 * A / B / C
                 * |/  |/  |
                 * 3---4---5
                 * 
                 * Assuming we want vertices to be indexed in this format, where A, B, C, etc. are defining points and numbers are indices of mesh points:
                 * 假设我们希望以这种格式对顶点进行索引，其中A，B，C等是定义点，数字是网格点的索引
                 * For a given point that is k positions along the chain, we want to find its indices.
                 * 对于沿链的 k 个位置的给定点，我们想要找到它的索引。
                 * These indices are given by k for the above point and k + n for the below point.
                 * 这些索引由 k 表示上述点，k + n 表示以下点。
                 */

                verticesTemp[k] = new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA);
                verticesTemp[k + maxPointCount] = new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC);
            }

            /* Now, we have to loop through the indices to generate triangles.
             * 现在，我们必须遍历索引以生成三角形。
             * Looping to maxPointCount - 1 brings us halfway to the end; it covers the top row (excluding the last point on the top row).
             * 循环到最大点计数 - 1 将我们带到终点的一半;它覆盖了顶行（不包括顶行的最后一个点）。
             */
            for (short k = 0; k < maxPointCount - 1; k++)
            {
                /* 0---1
                 * |  /|
                 * A / B
                 * |/  |
                 * 2---3
                 * 
                 * This illustration is the most basic set of points (where n = 2).
                 * 此图是最基本的一组点
                 * In this, we want to make triangles (2, 3, 1) and (1, 0, 2).
                 * 在这里，我们想要生成三角形（2，3，1）和（1，0，2）。
                 * Generalising this, if we consider A to be k = 0 and B to be k = 1, then the indices we want are going to be (k + n, k + n + 1, k + 1) and (k + 1, k, k + n)
                 * 概括一下，如果我们认为A是k = 0，B是k = 1，那么我们想要的索引将是（k + n，k + n + 1，k + 1）和（k + 1，k，k + n）
                 */

                indicesTemp[k * 6] = (short)(k + maxPointCount);
                indicesTemp[k * 6 + 1] = (short)(k + maxPointCount + 1);
                indicesTemp[k * 6 + 2] = (short)(k + 1);
                indicesTemp[k * 6 + 3] = (short)(k + 1);
                indicesTemp[k * 6 + 4] = k;
                indicesTemp[k * 6 + 5] = (short)(k + maxPointCount);
            }

            // The next available index will be the next value after the count of points (starting at 0).
            //下一个可用索引将是点计数（从 0 开始）之后的下一个值。
            nextAvailableIndex = verticesTemp.Length;

            vertices = verticesTemp;

            // Maybe we could use an array instead of a list for the indices, if someone figures out how to add indices to an array properly.
            //也许我们可以使用数组而不是索引列表，如果有人弄清楚如何正确地向数组添加索引。
            indices = indicesTemp;
        }

        private void SetupMeshes()
        {
            GenerateMesh(out VertexPositionColorTexture[] mainVertices, out short[] mainIndices, out int nextAvailableIndex);

            Vector2 toNext = (NextPosition - Positions[Positions.Length - 1]).SafeNormalize(Vector2.Zero);

            tip.GenerateMesh(Positions[Positions.Length - 1], toNext, nextAvailableIndex, out VertexPositionColorTexture[] tipVertices, out short[] tipIndices, trailWidthFunction, trailColorFunction);

            primitives.SetVertices(mainVertices.FastUnion(tipVertices));
            primitives.SetIndices(mainIndices.FastUnion(tipIndices));
        }

        public void Render(Effect effect)
        {
            if (Positions == null && !(primitives?.IsDisposed ?? true))
            {
                return;
            }

            SetupMeshes();

            primitives.Render(effect);
        }

        public void Dispose()
        {
            primitives?.Dispose();
        }
    }

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

    public class TriangularTip : ITrailTip
    {
        public int ExtraVertices => 3;

        public int ExtraIndices => 3;

        private readonly float length;

        public TriangularTip(float length)
        {
            this.length = length;
        }

        public void GenerateMesh(Vector2 trailTipPosition, Vector2 trailTipNormal, int startFromIndex, out VertexPositionColorTexture[] vertices, out short[] indices, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction)
        {
            /*     C
             *    / \
             *   /   \
             *  /     \
             * A-------B
             * 
             * This tip is arranged as the above shows.
             * 这个尖端的安排方式如上图
             * Consists of a single triangle with indices (0, 1, 2) offset by the next available index.
             * 由单个三角形组成，其索引 （0， 1， 2） 被下一个可用索引偏移
             */

            Vector2 normalPerp = trailTipNormal.RotatedBy(MathHelper.PiOver2);

            float width = trailWidthFunction?.Invoke(1) ?? 1;
            Vector2 a = trailTipPosition + (normalPerp * width);
            Vector2 b = trailTipPosition - (normalPerp * width);
            Vector2 c = trailTipPosition + (trailTipNormal * length);

            Vector2 texCoordA = Vector2.UnitX;
            Vector2 texCoordB = Vector2.One;
            Vector2 texCoordC = new Vector2(1, 0.5f);
            //this fixes the texture being skewed off to the side
            //这修复了纹理向侧面倾斜的问题

            Color colorA = trailColorFunction?.Invoke(texCoordA) ?? Color.White;
            Color colorB = trailColorFunction?.Invoke(texCoordB) ?? Color.White;
            Color colorC = trailColorFunction?.Invoke(texCoordC) ?? Color.White;

            vertices = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA),
                new VertexPositionColorTexture(b.Vec3(), colorB, texCoordB),
                new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC)
            };

            indices = new short[]
            {
                (short)startFromIndex,
                (short)(startFromIndex + 1),
                (short)(startFromIndex + 2)
            };
        }
    }

}
