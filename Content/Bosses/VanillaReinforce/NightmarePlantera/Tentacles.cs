using Coralite.Core.Loaders;
using Coralite.Core.Systems.IKSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareTentacle
    {
        private readonly float startOffset;

        public Vector2 pos;
        public float rotation;
        private readonly int pointCount;
        public float flowAlpha;
        public float perLength;

        private readonly Asset<Texture2D> _sampleTexture;
        private readonly Asset<Texture2D> _extraTexture;

        public Func<float, Color> colorFunc;
        public Func<float, float> widthFunc;

        private Vector2[] points;

        public NightmareTentacle(int pointCount, Func<float, Color> colorFunc, Func<float, float> widthFunc, Asset<Texture2D> sampleTexture, Asset<Texture2D> extraTexture)
        {
            this.pointCount = pointCount;
            points = new Vector2[pointCount];
            this.colorFunc = colorFunc;
            this.widthFunc = widthFunc;
            _sampleTexture = sampleTexture;
            _extraTexture = extraTexture;

            startOffset = Main.rand.NextFloat();
        }


        /// <summary>
        /// 更新这个触手，触手位置和触手的旋转请在这之前设置好！
        /// </summary>
        /// <param name="tentaclePerLength"></param>
        /// <param name="curve"></param>
        public void UpdateTentacle(float tentaclePerLength, Func<int, float> curve, float flowAlpha = 1)
        {
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            //由原点加上旋转方向
            for (int i = 0; i < pointCount; i++)
            {
                points[i] = pos + (dir * i * tentaclePerLength) + (normal * curve(i));
            }

            this.flowAlpha = flowAlpha;
            perLength = tentaclePerLength;
        }

        public void DrawTentacle(float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 Center = points[i];
                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(startOffset + (Main.GlobalTimeWrappedHourly / 2));
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.Transform);
            }

        }

        public void DrawTentacle_NoEndBegin(float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 Center = points[i];
                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(startOffset + (Main.GlobalTimeWrappedHourly / 2));
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
        }

        public void DrawTentacle_NoEndBegin_UI(float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 Center = points[i];
                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue( projection);
                effect.Parameters["uTime"].SetValue(startOffset + (Main.GlobalTimeWrappedHourly / 2));
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
        }
    }

    public class RotateTentacle
    {
        private readonly float startOffset;

        public Vector2 pos;
        public Vector2 targetPos;
        public float rotation;
        public float flowAlpha;
        public float perLength;
        public float RepeatCount = 1;

        private readonly int pointCount;

        private readonly Asset<Texture2D> _sampleTexture;
        private readonly Asset<Texture2D> _extraTexture;

        public Func<float, Color> colorFunc;
        public Func<float, float> widthFunc;

        public Vector2[] points;
        public float[] rotates;

        public RotateTentacle(int pointCount, Func<float, Color> colorFunc, Func<float, float> widthFunc, Asset<Texture2D> sampleTexture, Asset<Texture2D> extraTexture)
        {
            this.pointCount = pointCount;
            points = new Vector2[pointCount];
            rotates = new float[pointCount];
            this.colorFunc = colorFunc;
            this.widthFunc = widthFunc;
            _sampleTexture = sampleTexture;
            _extraTexture = extraTexture;

            startOffset = Main.rand.NextFloat();
        }

        /// <summary>
        /// 设置自身位置，目标位置和初始旋转
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="targetPos"></param>
        /// <param name="rotation"></param>
        public void SetValue(Vector2 pos, Vector2 targetPos, float rotation)
        {
            this.pos = pos;
            this.targetPos = targetPos;
            this.rotation = this.rotation.AngleTowards(rotation % MathHelper.TwoPi, 0.1f);
        }

        /// <summary>
        /// 更新这个触手，触手位置和触手的旋转请在这之前设置好！
        /// </summary>
        /// <param name="tentaclePerLength"></param>
        public void UpdateTentacle(float tentaclePerLength, float flowAlpha = 1)
        {
            Vector2 position = pos;
            float rot = rotation;
            //由原点加上旋转方向

            for (int i = 0; i < pointCount; i++)
            {
                float targetAngle = (targetPos - position).ToRotation();

                float angle = rot.AngleLerp(targetAngle, i / (float)pointCount);
                Vector2 dir = angle.ToRotationVector2();
                points[i] = position + (dir * tentaclePerLength);
                rotates[i] = angle;

                position = points[i];
                rot = rotates[i];
            }

            this.flowAlpha = flowAlpha;
            perLength = tentaclePerLength;
        }

        public void UpdateTentacleSmoothly(float tentaclePerLength, float flowAlpha = 1)
        {
            Vector2 position = pos;
            float rot = rotation;
            //由原点加上旋转方向

            for (int i = 0; i < pointCount; i++)
            {
                float targetAngle = (targetPos - position).ToRotation();

                float angle = rot.AngleLerp(targetAngle, i / (float)pointCount);
                float factor = 0.45f + ((1 - ((float)i / pointCount)) * 0.45f);
                Vector2 dir = angle.ToRotationVector2();
                points[i] = Vector2.Lerp(points[i], position + (dir * tentaclePerLength), factor);
                rotates[i] = Helper.Lerp(rotates[i], angle, factor);

                position = points[i];
                rot = rotates[i];
            }

            this.flowAlpha = flowAlpha;
            perLength = tentaclePerLength;
        }

        public void DrawTentacle(Func<int, float> curve, float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 normal = (rotates[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Center = points[i] + (normal * curve(i));

                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor * RepeatCount, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor * RepeatCount, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(startOffset + (Main.GlobalTimeWrappedHourly / 2));
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.Transform);
            }
        }

        public void DrawTentacle_NoEndBegin(Func<int, float> curve, float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 normal = (rotates[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Center = points[i] + (normal * curve(i));

                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor * RepeatCount, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor * RepeatCount, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(startOffset + (Main.GlobalTimeWrappedHourly / 2));
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }

        }
    }

    /// <summary>
    /// 美梦神的飘带<br></br>
    /// 为本体图颜色叠加一层流动颜色
    /// </summary>
    public class FantasyTentacle
    {
        public Vector2 pos;
        public Vector2 targetPos;
        public float rotation;
        public float flowAlpha;
        public float perLength;

        private readonly int pointCount;

        private readonly Asset<Texture2D> _sampleTexture;
        private readonly Asset<Texture2D> _extraTexture;

        public Func<float, Color> colorFunc;
        public Func<float, float> widthFunc;

        private Vector2[] points;
        private float[] rotates;
        public FantasyTentacle(int pointCount, Func<float, Color> colorFunc, Func<float, float> widthFunc, Asset<Texture2D> sampleTexture, Asset<Texture2D> extraTexture)
        {
            this.pointCount = pointCount;
            points = new Vector2[pointCount];
            rotates = new float[pointCount];
            this.colorFunc = colorFunc;
            this.widthFunc = widthFunc;
            _sampleTexture = sampleTexture;
            _extraTexture = extraTexture;
        }

        /// <summary>
        /// 设置自身位置，目标位置和初始旋转
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="targetPos"></param>
        /// <param name="rotation"></param>
        public void SetValue(Vector2 pos, Vector2 targetPos, float rotation)
        {
            this.pos = pos;
            this.targetPos = targetPos;
            this.rotation = this.rotation.AngleTowards(rotation % MathHelper.TwoPi, 0.1f);
        }

        /// <summary>
        /// 更新这个触手，触手位置和触手的旋转请在这之前设置好！
        /// </summary>
        /// <param name="tentaclePerLength"></param>
        public void UpdateTentacle(float tentaclePerLength, float flowAlpha = 1)
        {
            Vector2 position = pos;
            float rot = rotation;
            //由原点加上旋转方向

            for (int i = 0; i < pointCount; i++)
            {
                float targetAngle = (targetPos - position).ToRotation();

                float angle = rot.AngleLerp(targetAngle, i / (float)pointCount);
                Vector2 dir = angle.ToRotationVector2();
                points[i] = position + (dir * tentaclePerLength);
                rotates[i] = angle;

                position = points[i];
                rot = rotates[i];
            }

            this.flowAlpha = flowAlpha;
            perLength = tentaclePerLength;
        }

        public void DrawTentacle(Func<int, float> curve, float warpAmount = -1)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();

            if (warpAmount == -1)
            {
                int width = _extraTexture.Width();
                if (width == 0)
                    width = 256;
                warpAmount = perLength * pointCount / width;
            }

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / (float)pointCount);
                float width = widthFunc.Invoke(factor);
                Vector2 normal = (rotates[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Center = points[i] + (normal * curve(i));

                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc.Invoke(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("FantasyTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
                effect.Parameters["sampleTexture"].SetValue(_sampleTexture.Value);
                effect.Parameters["extraTexture"].SetValue(_extraTexture.Value);
                effect.Parameters["flowAlpha"].SetValue(flowAlpha);
                effect.Parameters["warpAmount"].SetValue(warpAmount);

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

    /// <summary>
    /// 注：暂时无法使用
    /// </summary>
    public class IKTentacle
    {
        public Arrow[] arrows;
        public IKSolverCCD calculator;

        private readonly int pointCount;
        private readonly Asset<Texture2D> _sampleTexture;
        private readonly Asset<Texture2D> _extraTexture;

        public Func<float, Color> colorFunc;
        public Func<float, float> widthFunc;
        public Func<Vector2> centerFunc;

        public IKTentacle(int pointCount, float perLength, Func<Vector2> target, Func<Vector2> self, Func<float, Color> colorFunc, Func<float, float> widthFunc, Asset<Texture2D> sampleTexture, Asset<Texture2D> extraTexture)
        {
            this.pointCount = pointCount;
            arrows = new Arrow[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                arrows[i] = new Arrow();
                //arrows[i].angleLimt = new Vector2(-80, 80);
            }

            calculator = new IKSolverCCD(arrows, target, false);
            calculator.SetLength(i => perLength);

            centerFunc = self;
            this.colorFunc = colorFunc;
            this.widthFunc = widthFunc;
            _sampleTexture = sampleTexture;
            _extraTexture = extraTexture;
        }

        /// <summary>
        /// 更新这个触手，触手位置和触手的旋转请在这之前设置好！
        /// </summary>
        public void UpdateTentacle()
        {
            calculator.arrows[0].CalculateStartAndEnd(centerFunc(), Vector2.UnitX);
            calculator.UpdatePosition(Vector2.UnitX);
            calculator.FollowTarget();
        }

        public void DrawTentacle(Func<int, float> curve)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();

            float count = pointCount + 1;

            for (int i = 0; i < pointCount; i++)
            {
                float factor = 1f - (i / count);
                float width = widthFunc(factor);
                Vector2 normal = arrows[i].Forward.RotatedBy(MathHelper.PiOver2);
                Vector2 Center = arrows[i].StartPos + (normal * curve(i));

                Vector2 Top = Center + (normal * width);
                Vector2 Bottom = Center - (normal * width);

                var color = colorFunc(1 - factor);
                bars.Add(new(Top.Vec3(), color, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), color, new Vector2(factor, 1)));
            }

            float width2 = widthFunc.Invoke(0);

            Vector2 Center2 = arrows[^1].EndPos;
            Vector2 normal2 = arrows[^1].Forward.RotatedBy(MathHelper.PiOver2);

            Vector2 Top2 = Center2 + (normal2 * width2);
            Vector2 Bottom2 = Center2 - (normal2 * width2);

            var color2 = colorFunc.Invoke(0);
            bars.Add(new(Top2.Vec3(), color2, new Vector2(1, 0)));
            bars.Add(new(Bottom2.Vec3(), color2, new Vector2(1, 1)));

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("NightmareTentacle");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
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
