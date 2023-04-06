using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Coralite.Core.Systems.Trails
{
    public class Trail : IDisposable
    {
        private readonly Primitives primitives;
        private readonly int maxPointCount;
        private readonly ITrailTip tip;
        private readonly TrailWidthFunction trailWidthFunction;
        private readonly TrailColorFunction trailColorFunction;

        private bool filpVertical;

        /// <summary>
        /// Array of positions that define the trail. NOTE: Positions[Positions.Length - 1] is assumed to be the start (e.g. Projectile.Center) and Positions[0] is assumed to be the end.
        /// <para></para>定义跟踪的位置数组。注意：位置[Positions.Length - 1]假定为起点（例如Projectile.Center），Positions[0]假定为末尾。
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
        /// <para>用于从最前面的位置计算法线，因为在原始列表中它后面没有点。</para>
        /// </summary>
        public Vector2 NextPosition { get; set; }

        private const float defaultWidth = 16;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device">一般是Main.graphics.GraphicsDevice</param>
        /// <param name="maxPointCount">最大顶点数</param>
        /// <param name="tip">尖端如何处理</param>
        /// <param name="trailWidthFunction">宽度委托</param>
        /// <param name="trailColorFunction">颜色委托</param>
        public Trail(GraphicsDevice device, int maxPointCount, ITrailTip tip, TrailWidthFunction trailWidthFunction, TrailColorFunction trailColorFunction, bool flipVertical = false)
        {
            this.tip = tip ?? new NoTip();
            this.maxPointCount = maxPointCount;
            this.trailWidthFunction = trailWidthFunction;
            this.trailColorFunction = trailColorFunction;
            this.filpVertical = flipVertical;

            /* A---B---C
             * |    / |    / |
             * D /  E  /  F
             * | /   |  /    |
             * G---H--- I
             * 
             * Let D, E, F, etc. be the set of n points that define the trail.
             * Since each point generates 2 vertices, there are 2n vertices, plus the tip's count.
             * 
             * As for indices - in the region between 2 defining points there are 2 triangles.
             * The amount of regions in the whole trail are given by n - 1, so there are 2(n - 1) triangles for n points.
             * Finally, since each triangle is defined by 3 indices, there are 6(n - 1) indices, plus the tip's count.
             * 
             * 让 D、E、F 等。是定义轨迹的 n 个点的集合。
             * 由于每个点生成 2 个顶点，因此有 2n 个顶点，加上尖端的计数。
             * 
             * 至于索引 - 在 2 个定义点之间的区域中有 2 个三角形。
             * 整个轨迹中的三角形数量有 n - 1 个，因此 n 个点有 2（n - 1） 个三角形。
             * 最后，由于每个三角形由 3 个索引定义，因此有 6（n - 1） 个索引，加上尖端的计数。
             * 
             * （最基础的顶点绘制了）
             */

            primitives = new Primitives(device, (maxPointCount * 2) + this.tip.ExtraVertices, (6 * (maxPointCount - 1)) + this.tip.ExtraIndices);
        }

        /// <summary>
        /// 计算所有顶点信息
        /// </summary>
        /// <param name="vertices">顶点数组</param>
        /// <param name="indices">索引数组</param>
        /// <param name="nextAvailableIndex"></param>
        private void GenerateMesh(out VertexPositionColorTexture[] vertices, out short[] indices, out int nextAvailableIndex)
        {
            VertexPositionColorTexture[] verticesTemp = new VertexPositionColorTexture[maxPointCount * 2];

            short[] indicesTemp = new short[maxPointCount * 6 - 6];

            // k = 0 indicates starting at the end of the trail (furthest from the origin of it).
            //k = 0 表示从路径的末端开始（离原点最远）。
            int texCoordA_Y = filpVertical ? 1 : 0;
            int texCoordC_Y = filpVertical ? 0 : 1;
            for (int k = 0; k < Positions.Length; k++)
            {
                // 1 at k = Positions.Length - 1 (start) and 0 at k = 0 (end).
                //1 在 Positions.Length - 1（开始），0 在 k = 0（结束）。
                float factorAlongTrail = (float)k / (Positions.Length - 1);

                // Uses the trail width function to decide the width of the trail at this point (if no function, use 
                //使用轨迹宽度函数确定此时轨迹的宽度（如果没有函数，使用默认宽度）
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
                 * A and C are calculated based on the perpendicular vector to the normal from B to D, scaled by the desired width calculated earlier.
                 * 设 B 为当前点，D 为下一个点。
                 * A 和 C 是根据从 B 到 D 的法向量计算的，并按之前计算的提供的宽度进行缩放。
                 */

                Vector2 a = current + (normalPerp * width);
                Vector2 c = current - (normalPerp * width);

                /* Texture coordinates are calculated such that the top-left is (0, 0) and the bottom-right is (1, 1).
                 * To achieve this, we consider the Y-coordinate of A to be 0 and that of C to be 1, while the X-coordinate is just the factor along the trail.
                 * This results in the point last in the trail having an X-coordinate of 0, and the first one having a Y-coordinate of 1.
                 * 计算纹理坐标时，左上角为 (0, 0)，右下角为 (1, 1)。（Coralite注：这个和泰拉坐标系一模一样，记住就行）
                 * 为了实现这一点，我们令 A 的 Y 坐标为 0，C 的 Y 坐标为 1，而 X 坐标只是沿轨迹的因子。
                 * 这会导致跟踪中最后一个点的 X 坐标为 0，第一个点的 Y 坐标为 1。
                 */
                Vector2 texCoordA = new Vector2(factorAlongTrail, texCoordA_Y);
                Vector2 texCoordC = new Vector2(factorAlongTrail, texCoordC_Y);

                // Calculates the color for each vertex based on its texture coordinates. This acts like a very simple shader (for more complex effects you can use the actual shader).
                //根据每个顶点的纹理坐标计算其颜色。这就像一个非常简单的shader（对于更复杂的效果，建议使用真正的shader）。
                Color colorA = trailColorFunction?.Invoke(texCoordA) ?? Color.White;
                Color colorC = trailColorFunction?.Invoke(texCoordC) ?? Color.White;

                /* 0---1---2
                 * |     /|    /|
                 * A  / B / C
                 * | /    |/   |
                 * 3---4---5
                 * 
                 * Assuming we want vertices to be indexed in this format, where A, B, C, etc. are defining points and numbers are indices of mesh points:
                 * For a given point that is k positions along the chain, we want to find its indices.
                 * These indices are given by k for the above point and k + n for the below point.
                 * 
                 * 假设我们希望以这种格式索引顶点，其中 A、B、C 等。是定义点，数字是网格点的索引：
                 * 对于沿链 k 个位置的给定点，我们希望找到它的索引。
                 * 这些指数由 k 表示上面的那个点，k + n 表示下面的那个点。
                 */

                verticesTemp[k] = new VertexPositionColorTexture(a.Vec3(), colorA, texCoordA);
                verticesTemp[k + maxPointCount] = new VertexPositionColorTexture(c.Vec3(), colorC, texCoordC);
            }

            /* Now, we have to loop through the indices to generate triangles.
             * Looping to maxPointCount - 1 brings us halfway to the end; it covers the top row (excluding the last point on the top row).
             * 现在，我们必须遍历索引以生成三角形。
             * 循环到 maxPointCount - 1 使我们走到了一半;它覆盖顶行（不包括顶行的最后一点）。
             */
            for (short k = 0; k < maxPointCount - 1; k++)
            {
                /* 0---1
                 * |     /|
                 * A  / B
                 * | /    |
                 * 2---3
                 * 
                 * This illustration is the most basic set of points (where n = 2).
                 * In this, we want to make triangles (2, 3, 1) and (1, 0, 2).
                 * Generalising this, if we consider A to be k = 0 and B to be k = 1, then the indices we want are going to be (k + n, k + n + 1, k + 1) and (k + 1, k, k + n)
                 * 此图是最基本的点集（当 n = 2时）。
                 * 在这里，我们要制作三角形 (2, 3, 1)  和 (1, 0, 2)。
                 * 概括这一点，如果我们认为 A 是 k = 0，B 是 k = 1，那么我们想要的索引将是 (k + n, k + n + 1, k + 1) 和 (k + 1, k, k + n)
                 * 
                 * （Coralite注：就是计算一下顶点的索引喽，主要理解了就好了）
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
            //也许我们可以使用数组而不是列表来索引，如果有人弄清楚如何正确地将索引添加到数组中。
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

        /// <summary>
        /// 竖直方向上进行翻转，仅需调用一次
        /// </summary>
        public void FilpVertical()
        {
            filpVertical = !filpVertical;
        }

        /// <summary>
        /// 竖直方向上进行翻转
        /// </summary>
        public void SetVertical(bool filpVertical)
        {
            this.filpVertical = filpVertical;
        }

        public void Dispose()
        {
            primitives?.Dispose();
        }
    }
}
