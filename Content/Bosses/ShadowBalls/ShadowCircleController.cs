using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowCircleController
    {
        public readonly Asset<Texture2D> CircleTex;

        /// <summary>
        /// 必须在0-1之间，表示圆环自身旋转
        /// </summary>
        public float selfRotation;

        public float xRotation;
        //public float yRotation;
        public float zRotation;
        public float radius;

        //先画在2维的圆
        public Vector2[] vector2D;
        //转化为3D的坐标
        public readonly Vector3[] vector3s = new Vector3[180];
        //给顶点的2D坐标
        public readonly Vector2[] vector2s = new Vector2[180];
        public Vector3 normal = Vector3.Zero;

        //宽度
        public readonly float circleHeight;

        public ShadowCircleController(Asset<Texture2D> CircleTex)
        {
            this.CircleTex = CircleTex;
            circleHeight = CircleTex.Height() / 2;

            if (vector2D == null)
            {
                vector2D = new Vector2[180];
                for (int i = 0; i < 180; i++)
                {
                    //先画个圆
                    vector2D[i] = new Vector2(1, 0).RotatedBy(i * (MathHelper.TwoPi / 180));
                }
            }

            //圆环垂直于Y轴，在XZ平面上
            xRotation = 0;
            radius = CircleTex.Width() / MathHelper.TwoPi;
        }

        public void Update()
        {
            for (int i = 0; i < 180; i++)
            {
                //再画在3维空间 然后转起来
                vector3s[i] = Vector3.Transform(new Vector3(vector2D[i].X, vector2D[i].Y, 0), Matrix.CreateRotationX(xRotation));
                //vector3s[i] = Vector3.Transform(vector3s[i], Matrix.CreateRotationX(xRotation));
                vector3s[i] = Vector3.Transform(vector3s[i], Matrix.CreateRotationZ(zRotation));
            }

            normal = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationX(xRotation));
            //normal = Vector3.Transform(normal, Matrix.CreateRotationX(xRotation));
            normal = Vector3.Transform(normal, Matrix.CreateRotationZ(zRotation));

            for (int i = 0; i < 180; i++)
            {
                //重新投影到二维
                float k1 = -1000 / (vector3s[i].Z - 1000);
                vector2s[i] = k1 * new Vector2(vector3s[i].X, vector3s[i].Y);
            }
        }

        public void DrawBackCircle(SpriteBatch spriteBatch, Vector2 center, Color lightColor)
        {
            if (xRotation.ToRotationVector2().Y > 0)
                DrawCircle(spriteBatch, center, lightColor, 0, 90);
            else
                DrawCircle(spriteBatch, center, lightColor, 90, vector2s.Length - 1);

            spriteBatch.End();
            spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawFrontCircle(SpriteBatch spriteBatch, Vector2 center, Color lightColor)
        {
            if (xRotation.ToRotationVector2().Y > 0)
                DrawCircle(spriteBatch, center, lightColor, 90, vector2s.Length - 1);
            else
                DrawCircle(spriteBatch, center, lightColor, 0, 90);

            spriteBatch.End();
            spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        public void DrawFrontCircle_NoEndBegin(Vector2 center, Color lightColor)
        {
            if (xRotation.ToRotationVector2().Y > 0)
                DrawCircle_NoEndBegin(center, lightColor, 90, vector2s.Length - 1);
            else
                DrawCircle_NoEndBegin(center, lightColor, 0, 90);
        }

        public void DrawBackCircle_NoEndBegin(Vector2 center, Color lightColor)
        {
            if (xRotation.ToRotationVector2().Y > 0)
                DrawCircle_NoEndBegin(center, lightColor, 0, 90);
            else
                DrawCircle_NoEndBegin(center, lightColor, 90, vector2s.Length - 1);
        }

        public void DrawCircle(SpriteBatch spriteBatch, Vector2 center, Color lightColor, int start, int end)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap/*注意了奥*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            DrawCircle_NoEndBegin(center, lightColor, start, end);
        }

        public void DrawCircle_NoEndBegin(Vector2 center, Color lightColor, int start, int end)
        {
            Texture2D Texture = CircleTex.Value;

            List<CustomVertexInfo> bars = new();
            //对法向量进行一个投影
            float k1 = -1000 / (normal.Z - 1000);
            var normalDir = k1 * new Vector2(normal.X, normal.Y);

            for (int i = start; i <= end; ++i)
            {
                //一些数据
                float factor = ((float)i / vector2s.Length) + selfRotation;
                var w = 1;//暂时无用

                bars.Add(new CustomVertexInfo(center + (vector2s[i] * radius) + (normalDir * circleHeight), lightColor, new Vector3(factor, 1, w)));
                bars.Add(new CustomVertexInfo(center + (vector2s[i] * radius) + (normalDir * -circleHeight), lightColor, new Vector3(factor, 0, w)));
                if (i == vector2s.Length - 1)
                {
                    bars.Add(new CustomVertexInfo(center + (vector2s[0] * radius) + (normalDir * circleHeight), lightColor, new Vector3(factor, 1, w)));
                    bars.Add(new CustomVertexInfo(center + (vector2s[0] * radius) + (normalDir * -circleHeight), lightColor, new Vector3(factor, 0, w)));
                }
            }

            List<CustomVertexInfo> Vx = new();
            if (bars.Count > 2)
            {
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    Vx.Add(bars[i]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 1]);

                    Vx.Add(bars[i + 1]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 3]);
                }
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

        }
    }
}
