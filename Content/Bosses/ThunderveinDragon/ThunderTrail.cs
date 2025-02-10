using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderTrail(Asset<Texture2D> thunderTex, Func<float, float> widthFunc, Func<float, Color> colorFunc, Func<float, float> alphaFunc)
    {
        /// <summary>
        /// 数组元素必须得给我大于2喽
        /// </summary>
        public Vector2[] BasePositions { get; set; }
        public Vector2[] RandomlyPositions { get; set; }

        /// <summary>
        /// 在绘制同时使用Non与Add的绘制模式
        /// </summary>
        public bool UseNonOrAdd { get; set; } = false;
        public Color FlowColor = Color.White;

        /// <summary>
        /// 是否能绘制闪电，可以用这个来制作闪烁效果
        /// </summary>
        public bool CanDraw { get; set; }

        /// <summary>
        /// 转角小于多少时会进行圆滑处理
        /// </summary>
        public float PartitionLimit { get; set; } = 1.9f;

        /// <summary>
        /// 对于闪电中锐利的部分分割几次
        /// </summary>
        public int PartitionPointCount { get; set; } = 1;

        private Func<float, float> thunderWidthFunc = widthFunc;
        private (float, float) thunderRandomOffsetRange;

        public Asset<Texture2D> ThunderTex { get; private set; } = thunderTex;

        private float randomExpandWidth;

        public void SetWidth(Func<float, float> widthFunc)
        {
            thunderWidthFunc = widthFunc;
        }

        public void SetRange((float, float) range)
        {
            if (range.Item1 > range.Item2)
                throw new Exception("第一个元素不可以比第二个元素大!");

            thunderRandomOffsetRange = range;
        }

        public void SetExpandWidth(float width)
        {
            randomExpandWidth = width;
        }

        public void ExchangeTexture(Asset<Texture2D> asset)
        {
            ThunderTex = asset;
        }

        public void UpdateTrail(Vector2 velocity)
        {
            if (RandomlyPositions == null)
                return;
            for (int i = 0; i < RandomlyPositions.Length; i++)
            {
                RandomlyPositions[i] += velocity;
            }
        }

        /// <summary>
        /// 随机改变闪电形状
        /// </summary>
        public void RandomThunder()
        {
            RandomlyPositions = new Vector2[BasePositions.Length];
            //首位两端的点不动
            RandomlyPositions[0] = BasePositions[0];
            for (int i = 1; i < BasePositions.Length - 1; i++)
            {
                /*
                 *          B
                 * A   -        -
                 *                  C
                 * AC连线的垂直点作为B的法向量
                 */
                Vector2 normal = (BasePositions[i - 1] - BasePositions[i + 1]).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);

                //长度
                float length = Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(thunderRandomOffsetRange.Item1, thunderRandomOffsetRange.Item2);

                //最后赋值
                RandomlyPositions[i] = BasePositions[i] + (normal * length) + Main.rand.NextVector2Circular(randomExpandWidth, randomExpandWidth);
            }

            RandomlyPositions[^1] = BasePositions[^1];
        }

        public void DrawThunder(GraphicsDevice graphicsDevice)
        {
            if (!CanDraw || RandomlyPositions == null)
                return;

            Texture2D Texture = ThunderTex.Value;

            int texWidth = Texture.Width;
            float length = 0;

            List<ColoredVertex> barsTop = new();
            List<ColoredVertex> barsBottom = new();
            List<ColoredVertex> bars2Top = new();
            List<ColoredVertex> bars2Bottom = new();

            int trailCachesLength = RandomlyPositions.Length;

            //是否在末端绘制遮盖物
            bool drawInTip = false;
            bool drawInBack = false;

            //先添加0的
            Vector2 Center = RandomlyPositions[0] - Main.screenPosition;

            Vector2 normal = (RandomlyPositions[0] - RandomlyPositions[1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            float tipRotaion = normal.ToRotation() + 1.57f;
            Color thunderColor = GetColor(0);
            float tipWidth = thunderWidthFunc(0);
            drawInTip = tipWidth > 10;
            Vector2 lengthVec2 = normal * tipWidth;

            AddVertexInfo2(barsTop, barsBottom, Center, lengthVec2, thunderColor, 0);
            AddVertexInfo2(bars2Top, bars2Bottom, Center, lengthVec2, GetFlowColor(0), 0);

            for (int i = 1; i < trailCachesLength - 1; i++)
            {
                float factor = (float)i / trailCachesLength;
                Center = RandomlyPositions[i] - Main.screenPosition;
                /*
                 *          B
                 * A   -        -
                 *                  C
                 * AC连线的垂直点作为B的法向量
                 */

                thunderColor = GetColor(factor);
                float width = thunderWidthFunc(factor);

                Vector2 dirToBack = RandomlyPositions[i - 1] - RandomlyPositions[i];
                Vector2 dirToTront = RandomlyPositions[i + 1] - RandomlyPositions[i];

                length += dirToBack.Length();
                float lengthFactor = length / texWidth;//当前闪电长度相对于图片长度的值，总之是用于拉伸闪电贴图的

                float y = ((RandomlyPositions[i].X - RandomlyPositions[i - 1].X)
                    / (RandomlyPositions[i + 1].X - RandomlyPositions[i - 1].X)
                    * (RandomlyPositions[i + 1].Y - RandomlyPositions[i - 1].Y))
                    + RandomlyPositions[i - 1].Y;


                float angle = MathF.Acos(Vector2.Dot(dirToBack, dirToTront) / (dirToTront.Length() * dirToBack.Length()));//Helpers.Helper.AngleRad(dirToBack, dirToTront);

                //normal = (RandomlyPositions[i - 1].SafeNormalize(Vector2.One) + RandomlyPositions[i + 1].SafeNormalize(Vector2.One)).SafeNormalize(Vector2.One);
                normal = (RandomlyPositions[i - 1] - RandomlyPositions[i + 1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);

                if (angle < PartitionLimit)
                {
                    bool PartitionBottom = RandomlyPositions[i].Y < y;//分割底部的点
                    if (RandomlyPositions[i - 1].X > RandomlyPositions[i + 1].X)
                        PartitionBottom = !PartitionBottom;

                    int sign = PartitionBottom ? 1 : -1;

                    angle = 3.141f - angle;
                    float perAngle = angle / PartitionPointCount;//分割几次的每次增加的角度
                    Vector2 exNormal = normal.RotatedBy(-sign * angle / 2);

                    for (int j = 0; j < PartitionPointCount + 1; j++)
                    {
                        Vector2 exNormal2;
                        if (j != 0)
                            exNormal2 = exNormal.RotatedBy(sign * perAngle);
                        else
                            exNormal2 = exNormal;

                        Vector2 Top;
                        Vector2 Bottom;
                        if (PartitionBottom)
                        {
                            Top = Center + (normal * width);
                            Bottom = Center - (exNormal2 * width);
                        }
                        else
                        {
                            Top = Center + (exNormal2 * width);
                            Bottom = Center - (normal * width);
                        }

                        AddVertexInfo(barsTop, barsBottom, Top, Bottom, thunderColor, lengthFactor);

                        Vector2 center2 = (Top + Bottom) / 2;
                        Vector2 dir = (Top - center2) / 4;
                        AddVertexInfo2(bars2Top, bars2Bottom, center2, dir, GetFlowColor(factor), lengthFactor);
                        exNormal = exNormal2;
                    }

                    #region 旧算法
                    //if (RandomlyPositions[i].Y > y)
                    //{
                    //    /*
                    //     * 
                    //     *      C      -
                    //     *                      B
                    //     *          A      -
                    //     * 角ABC过小
                    //     * 这时把B点右边拆分成两个点
                    //     * 
                    //     */

                    //    Vector2 exNormal1 = normal.RotatedBy(angle / 2);
                    //    Vector2 exNormal2 = normal.RotatedBy(-angle / 2);

                    //    Vector2 Top = Center + normal * width;
                    //    Vector2 Bottom2 = Center - exNormal1 * width;
                    //    Vector2 Bottom1 = Center - exNormal2 * width;
                    //    //Vector2 exBottom1 = Center - exNormal1 * width / 3;
                    //    //Vector2 exBottom2 = Center - exNormal2 * width / 3;

                    //    AddVertexInfo(barsTop, barsBottom, Top, Bottom1, thunderColor, factor);
                    //    AddVertexInfo(barsTop, barsBottom, Top, Bottom2, thunderColor, factor);

                    //    bars2Top.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                    //    bars2Top.Add(new(Bottom1, thunderColor, new Vector3(factor, 1, 0)));

                    //    bars2Top.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                    //    bars2Top.Add(new(Bottom2, thunderColor, new Vector3(factor, 1, 0)));
                    //}
                    //else
                    //{
                    //    /*
                    //     * 
                    //     *                -      C
                    //     *        B
                    //     *               -    A
                    //     * 角ABC过小，和上面那个反过来的
                    //     * 这时把B点右边拆分成两个点
                    //     * 
                    //     */

                    //    Vector2 exNormal1 = normal.RotatedBy(-angle / 2);
                    //    Vector2 exNormal2 = normal.RotatedBy(angle / 2);

                    //    Vector2 Top2 = Center + exNormal1 * width;
                    //    Vector2 Top1 = Center + exNormal2 * width;
                    //    //Vector2 exTop1 = Center + exNormal2 * width / 3;
                    //    //Vector2 exTop2 = Center + exNormal2 * width / 3;
                    //    Vector2 Bottom = Center - normal * width;

                    //    barsTop.Add(new(Top1, thunderColor, new Vector3(factor, 0, 0)));
                    //    barsTop.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                    //    barsTop.Add(new(Top2, thunderColor, new Vector3(factor, 0, 0)));
                    //    barsTop.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                    //    bars2Top.Add(new(Top1, thunderColor, new Vector3(factor, 0, 0)));
                    //    bars2Top.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                    //    bars2Top.Add(new(Top2, thunderColor, new Vector3(factor, 0, 0)));
                    //    bars2Top.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));
                    //}
                    #endregion
                }
                else//正常的
                {
                    //Vector2 Top = Center + normal * width;
                    //Vector2 Bottom = Center - normal * width;
                    AddVertexInfo2(barsTop, barsBottom, Center, normal * width, thunderColor, lengthFactor);
                    AddVertexInfo2(bars2Top, bars2Bottom, Center, normal * width / 4, GetFlowColor(factor), lengthFactor);

                    /*barsTop.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                    //barsTop.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));
                    //bars2Top.Add(new(Center + normal * width / 3, thunderColor, new Vector3(factor, 0, 0)));
                    bars2Top.Add(new(Center - normal * width / 3, thunderColor, new Vector3(factor, 1, 0))); */
                }
            }

            Center = RandomlyPositions[^1] - Main.screenPosition;
            Vector2 dirToBack2 = RandomlyPositions[^2] - RandomlyPositions[^1];
            normal = dirToBack2.RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            float bottomWidth = thunderWidthFunc(1);
            drawInBack = bottomWidth > 10;
            lengthVec2 = normal * bottomWidth;

            length += dirToBack2.Length();
            float lengthFactor2 = length / texWidth;

            AddVertexInfo2(barsTop, barsBottom, Center, lengthVec2, GetColor(1), lengthFactor2);
            AddVertexInfo2(bars2Top, bars2Bottom, Center, lengthVec2, GetFlowColor(1), lengthFactor2);

            /*barsTop.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            //barsTop.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));
            //bars2Top.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            bars2Top.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));*/

            graphicsDevice.Textures[0] = Texture;
            BlendState state = graphicsDevice.BlendState;

            if (UseNonOrAdd)
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsTop.ToArray(), 0, barsTop.Count - 2);
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsBottom.ToArray(), 0, barsTop.Count - 2);

            if (UseNonOrAdd)
                graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2Top.ToArray(), 0, bars2Top.Count - 2);
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2Bottom.ToArray(), 0, bars2Top.Count - 2);

            graphicsDevice.BlendState = state;

            if (drawInTip)
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
                var pos = RandomlyPositions[0] - Main.screenPosition;
                var origin = mainTex.Size() / 2;
                Color c = colorFunc(0);
                c.A = 0;

                Vector2 scale = new(thunderWidthFunc(0) / 90, thunderWidthFunc(0) / 130);

                Main.spriteBatch.Draw(mainTex, pos, null, c, tipRotaion, origin, scale, 0, 0);
                Main.spriteBatch.Draw(mainTex, pos, null, c, tipRotaion, origin, scale * 0.75f, 0, 0);
                //Main.spriteBatch.Draw(mainTex, pos, null, c, tipRotaion, origin, scale * 0.5f, 0, 0);
            }

            if (drawInBack)
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
                var pos = RandomlyPositions[^1] - Main.screenPosition;
                var origin = mainTex.Size() / 2;
                Color c = colorFunc(1);
                c.A = 0;

                Vector2 scale = new(thunderWidthFunc(1) / 170, thunderWidthFunc(1) / 200);
                float rot = normal.ToRotation() + 1.57f;
                Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale, 0, 0);
                Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale * 0.75f, 0, 0);
                //Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale * 0.5f, 0, 0);
            }
        }

        public void AddVertexInfo(List<ColoredVertex> topList, List<ColoredVertex> bottomList, Vector2 top, Vector2 bottom, Color color, float factor)
        {
            Vector2 center = (top + bottom) / 2;

            topList.Add(new(top, color, new Vector3(factor, 0, 0)));
            topList.Add(new(center, color, new Vector3(factor, 0.5f, 0)));

            bottomList.Add(new(center, color, new Vector3(factor, 0.5f, 0)));
            bottomList.Add(new(bottom, color, new Vector3(factor, 1, 0)));
        }

        public void AddVertexInfo2(List<ColoredVertex> topList, List<ColoredVertex> bottomList, Vector2 center, Vector2 dir, Color color, float factor)
        {
            topList.Add(new(center + dir, color, new Vector3(factor, 0, 0)));
            topList.Add(new(center, color, new Vector3(factor, 0.5f, 0)));

            bottomList.Add(new(center, color, new Vector3(factor, 0.5f, 0)));
            bottomList.Add(new(center - dir, color, new Vector3(factor, 1, 0)));
        }

        /// <summary>
        /// 获取闪电颜色，会根据<see cref="UseNonOrAdd"/>发生变化
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Color GetColor(float factor)
        {
            Color thunderColor = colorFunc(factor);
            float alpha = alphaFunc(factor);

            if (UseNonOrAdd)
                thunderColor.A = (byte)(alpha * 255);
            else
            {
                thunderColor.A = 0;
                thunderColor *= alpha;
            }

            return thunderColor;
        }

        public Color GetFlowColor(float factor)
        {
            Color thunderColor = FlowColor;
            float alpha = alphaFunc(factor);

            if (UseNonOrAdd)
                thunderColor.A = (byte)(alpha * 255);
            else
            {
                thunderColor.A = 0;
                thunderColor *= alpha;
            }

            return thunderColor;
        }
    }
}
