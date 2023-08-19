using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareTentacle
    {
        public Vector2 pos;
        public float rotation;
        private readonly int pointCount;

        private readonly Asset<Texture2D> _sampleTexture;
        private readonly Asset<Texture2D> _extraTexture;

        public Func<float, Color> colorFunc;
        public Func<float, float> widthFunc;

        private Vector2[] points;

        public NightmareTentacle(int pointCount,Func<float,Color> colorFunc, Func<float, float> widthFunc, Asset<Texture2D> sampleTexture, Asset<Texture2D> extraTexture)
        {
            this.pointCount = pointCount;
            points = new Vector2[pointCount];
            this.colorFunc = colorFunc;
            this.widthFunc = widthFunc;
            _sampleTexture = sampleTexture;
            _extraTexture = extraTexture;
        }


        /// <summary>
        /// 更新这个触手，触手位置和触手的旋转请在这之前设置好！
        /// </summary>
        /// <param name="tentaclePerLength"></param>
        /// <param name="curve"></param>
        public void UpdateTentacle(float tentaclePerLength, Func<int, float> curve)
        {
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            //由原点加上旋转方向
            for (int i = 0; i < pointCount; i++)
            {
                points[i] = pos + dir * i * tentaclePerLength + normal * curve.Invoke(i);
            }
        }

        public void DrawTentacle()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - i / (float)pointCount;
                float width = widthFunc.Invoke(factor);
                Vector2 Center = points[i];
                Vector2 Top = Center + normal * width;
                Vector2 Bottom = Center - normal * width;

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["NightmareTentacle"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly/2);
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

        }
    }
}
