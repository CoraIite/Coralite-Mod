using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderTrail(Asset<Texture2D> thunderTex, Func<float, float> widthFunc, Func<float, Color> colorFunc)
    {
        /// <summary>
        /// 数组元素必须得给我大于2喽
        /// </summary>
        public Vector2[] BasePositions { get; set; }
        public Vector2[] RandomlyPositions { get; set; }
        public bool CanDraw { get; set; }

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
                RandomlyPositions[i] = BasePositions[i] + normal * length + Main.rand.NextVector2Circular(randomExpandWidth, randomExpandWidth);
            }

            RandomlyPositions[^1] = BasePositions[^1];
        }

        public void DrawThunder(GraphicsDevice graphicsDevice)
        {
            if (!CanDraw)
                return;

            Texture2D Texture = ThunderTex.Value;
            List<CustomVertexInfo> bars = new();
            List<CustomVertexInfo> bars2 = new();

            int trailCachesLength = RandomlyPositions.Length;

            //是否在末端绘制遮盖物
            bool drawInTip = false;
            bool drawInBack = false;

            //先添加0的
            Vector2 Center = RandomlyPositions[0] - Main.screenPosition;

            Vector2 normal = (RandomlyPositions[0] - RandomlyPositions[1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            float tipRotaion = normal.ToRotation() + 1.57f;
            Color thunderColor = colorFunc(0);
            float tipWidth = thunderWidthFunc(0);
            drawInTip = tipWidth > 10;
            Vector2 lengthVec2 = normal * tipWidth;
            bars.Add(new(Center + lengthVec2, thunderColor, new Vector3(0, 0, 0)));
            bars.Add(new(Center - lengthVec2, thunderColor, new Vector3(0, 1, 0)));
            bars2.Add(new(Center + lengthVec2, thunderColor, new Vector3(0, 0, 0)));
            bars2.Add(new(Center - lengthVec2, thunderColor, new Vector3(0, 1, 0)));

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

                thunderColor = colorFunc(factor);
                float width = thunderWidthFunc(factor);

                Vector2 dirToBack = RandomlyPositions[i - 1] - RandomlyPositions[i];
                Vector2 dirToTront = RandomlyPositions[i + 1] - RandomlyPositions[i];

                float y = (RandomlyPositions[i].X - RandomlyPositions[i - 1].X)
                    / (RandomlyPositions[i + 1].X - RandomlyPositions[i - 1].X)
                    * (RandomlyPositions[i + 1].Y - RandomlyPositions[i - 1].Y)
                    + RandomlyPositions[i - 1].Y;


                float angle = Helpers.Helper.AngleRad(dirToBack, dirToTront);

                normal = (RandomlyPositions[i - 1] - RandomlyPositions[i + 1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);

                if (angle > 0.5f && angle < 1.2f)
                {
                    if (RandomlyPositions[i].Y > y)
                    {
                        /*
                         * 
                         *      C      -
                         *                      B
                         *          A      -
                         * 角ABC过小
                         * 这时把B点右边拆分成两个点
                         * 
                         */

                        Vector2 exNormal1 = normal.RotatedBy(angle / 2);
                        Vector2 exNormal2 = normal.RotatedBy(-angle / 2);

                        Vector2 Top = Center + normal * width;
                        Vector2 Bottom2 = Center - exNormal1 * width;
                        Vector2 Bottom1 = Center - exNormal2 * width;
                        //Vector2 exBottom1 = Center - exNormal1 * width / 3;
                        //Vector2 exBottom2 = Center - exNormal2 * width / 3;

                        bars.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                        bars.Add(new(Bottom1, thunderColor, new Vector3(factor, 1, 0)));

                        bars.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                        bars.Add(new(Bottom2, thunderColor, new Vector3(factor, 1, 0)));

                        bars2.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                        bars2.Add(new(Bottom1, thunderColor, new Vector3(factor, 1, 0)));

                        bars2.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                        bars2.Add(new(Bottom2, thunderColor, new Vector3(factor, 1, 0)));
                    }
                    else
                    {
                        /*
                         * 
                         *                -      C
                         *        B
                         *               -    A
                         * 角ABC过小，和上面那个反过来的
                         * 这时把B点右边拆分成两个点
                         * 
                         */

                        Vector2 exNormal1 = normal.RotatedBy(-angle / 2);
                        Vector2 exNormal2 = normal.RotatedBy(angle / 2);

                        Vector2 Top2 = Center + exNormal1 * width;
                        Vector2 Top1 = Center + exNormal2 * width;
                        //Vector2 exTop1 = Center + exNormal2 * width / 3;
                        //Vector2 exTop2 = Center + exNormal2 * width / 3;
                        Vector2 Bottom = Center - normal * width;

                        bars.Add(new(Top1, thunderColor, new Vector3(factor, 0, 0)));
                        bars.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                        bars.Add(new(Top2, thunderColor, new Vector3(factor, 0, 0)));
                        bars.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                        bars2.Add(new(Top1, thunderColor, new Vector3(factor, 0, 0)));
                        bars2.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));

                        bars2.Add(new(Top2, thunderColor, new Vector3(factor, 0, 0)));
                        bars2.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));
                    }
                }
                else//正常的
                {
                    Vector2 Top = Center + normal * width;
                    Vector2 Bottom = Center - normal * width;

                    bars.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                    bars.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));
                    bars2.Add(new(Center + normal * width / 3, thunderColor, new Vector3(factor, 0, 0)));
                    bars2.Add(new(Center - normal * width / 3, thunderColor, new Vector3(factor, 1, 0)));
                }
            }

            Center = RandomlyPositions[^1] - Main.screenPosition;
            normal = (RandomlyPositions[^2] - RandomlyPositions[^1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            thunderColor = colorFunc(1);
            float bottomWidth = thunderWidthFunc(1);
            drawInBack = bottomWidth > 10;
            lengthVec2 = normal * bottomWidth;
            bars.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            bars.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));
            bars2.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            bars2.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));

            graphicsDevice.Textures[0] = Texture;
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

            if (drawInTip)
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
                var pos = RandomlyPositions[0] - Main.screenPosition;
                var origin = mainTex.Size() / 2;
                Color c = new(255, 202, 101, 0);

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
                Color c = new(255, 202, 101, 0);

                Vector2 scale = new(thunderWidthFunc(1) / 170, thunderWidthFunc(1) / 200);
                float rot = normal.ToRotation() + 1.57f;
                Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale, 0, 0);
                Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale * 0.75f, 0, 0);
                //Main.spriteBatch.Draw(mainTex, pos, null, c, rot, origin, scale * 0.5f, 0, 0);
            }
        }
    }
}
