using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderTrail
    {
        /// <summary>
        /// 数组元素必须得给我大于2喽
        /// </summary>
        public Vector2[] BasePositions { get; set; }
        public Vector2[] RandomlyPositions { get; set; }
        public bool CanDraw { get; set; }

        private Func<float, float> thunderWidthFunc;
        private Func<float, Color> thunderColorFunc;
        private (float, float) thunderRandomOffsetRange;

        public Asset<Texture2D> ThunderTex { get; private set; }

        private float randomExpandWidth;

        public ThunderTrail(Asset<Texture2D> thunderTex, Func<float, float> widthFunc, Func<float, Color> colorFunc)
        {
            ThunderTex = thunderTex;
            thunderWidthFunc = widthFunc;
            thunderColorFunc = colorFunc;
        }

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
            randomExpandWidth= width;
        }

        public void ExchangeTexture(Asset<Texture2D> asset)
        {
            ThunderTex = asset;
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
                RandomlyPositions[i] = BasePositions[i] + normal * length+Main.rand.NextVector2Circular(randomExpandWidth,randomExpandWidth);
            }

            RandomlyPositions[^1] = BasePositions[^1];
        }

        public void DrawThunder(SpriteBatch spriteBatch)
        {
            if (!CanDraw)
                return;

            Texture2D Texture = ThunderTex.Value;
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bars2 = new List<CustomVertexInfo>();

            int trailCachesLength = RandomlyPositions.Length;

            //是否在末端绘制遮盖物
            bool drawInTip = false;
            bool drawInBack = false;

            //先添加0的
            Vector2 Center = RandomlyPositions[0] - Main.screenPosition;

            Vector2 normal = (RandomlyPositions[1] - RandomlyPositions[0]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            Color thunderColor = thunderColorFunc(0);
            float tipWidth = thunderWidthFunc(0);
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
                normal = (RandomlyPositions[i - 1] - RandomlyPositions[i + 1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
                float width = thunderWidthFunc(factor);

                Vector2 Top = Center + normal * width;
                Vector2 Bottom = Center - normal * width;

                thunderColor = thunderColorFunc(factor);
                bars.Add(new(Top, thunderColor, new Vector3(factor, 0, 0)));
                bars.Add(new(Bottom, thunderColor, new Vector3(factor, 1, 0)));
                bars2.Add(new(Center + normal * width / 3, thunderColor, new Vector3(factor, 0, 0)));
                bars2.Add(new(Center - normal * width / 3, thunderColor, new Vector3(factor, 1, 0)));
            }

            Center = RandomlyPositions[^1] - Main.screenPosition;
            normal = (RandomlyPositions[^2] - RandomlyPositions[^1]).RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.One);
            thunderColor = thunderColorFunc(1);
            float bottomWidth = thunderWidthFunc(1);
            lengthVec2 = normal * bottomWidth;
            bars.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            bars.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));
            bars2.Add(new(Center + lengthVec2, thunderColor, new Vector3(1, 0, 0)));
            bars2.Add(new(Center - lengthVec2, thunderColor, new Vector3(1, 1, 0)));

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
        }
    }
}
