using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Items.AlchorthentSeries
{
    /// <summary>
    /// 线段绘制器
    /// </summary>
    public class LineDrawer
    {
        public Line[] lines;

        public LineDrawer(Line[] lines)
        {
            this.lines = lines;
        }

        public void SetLineWidth(int width)
        {
            foreach (var line in lines)
            {
                line.lineWidth = width;
            }
        }

        public void SetScale(float scale)
        {
            foreach (var line in lines)
            {
                line.scale = scale;
            }
        }

        /// <summary>
        /// 线段：控制绘制，如需使用shader需要提前开启
        /// </summary>
        public abstract class Line(Vector2 startPos)
        {
           public ATex baseTex;

            /// <summary>
            /// 起始点，相对于传入的挤出点的偏移，不是世界坐标！！
            /// </summary>
            public Vector2 StartPos = startPos;

            /// <summary> 对起始点相对原始点的缩放 </summary>
            public float scale = 1;
            /// <summary> 线段宽度 </summary>
            public float lineWidth = 12;

            /// <summary>
            /// 绘制线段
            /// </summary>
            /// <param name="factor"></param>
            /// <param name="basePos"></param>
            public virtual void Render(Vector2 basePos) { }
        }

        /// <summary>
        /// 直线，只需要传入两个点
        /// </summary>
        public class StraightLine : Line
        {
            public Vector2 EndPos;

            /// <param name="startPos"></param>
            /// <param name="endPos"></param>
            public StraightLine(Vector2 startPos, Vector2 endPos, ATex baseTex = null) : base(startPos)
            {
                EndPos = endPos;
                Vector2 dir = (endPos - startPos).SafeNormalize(Vector2.Zero);
                float percent = (endPos - startPos).Length();

                StartPos -= dir * percent / 100;
                EndPos += dir * percent / 100;

                if (baseTex == null)
                    this.baseTex = CoraliteAssets.Sparkle.ShotLineSPA2;
                else
                    this.baseTex = baseTex;
            }

            public override void Render(Vector2 basePos)
            {
                if (baseTex==null)
                    return;
                //从起始点绘制到结束点
                Texture2D tex = baseTex.Value;

                Main.spriteBatch.Draw(tex, basePos + StartPos * scale, null, Color.White, (EndPos - StartPos).ToRotation(), new Vector2(0, tex.Height / 2), new Vector2((EndPos - StartPos).Length() / tex.Width*scale, lineWidth / tex.Height), 0, 0);
            }
        }

        /// <summary>
        /// 曲线，需要配传入末尾点的轨迹委托
        /// </summary>
        public class WarpLine : Line
        {
           static List<ColoredVertex> bars = new();

            public readonly int LinePointCount;

            public Func<float, Vector2> GetEndPos;

            /// <param name="startPos"></param>
            /// <param name="getEndPos"></param>
            public WarpLine(Vector2 startPos, int linePointCount, Func<float,Vector2> getEndPos, ATex baseTex = null) : base(startPos)
            {
                LinePointCount = linePointCount;
                GetEndPos = getEndPos;

                if (baseTex == null)
                    this.baseTex = CoraliteAssets.Sparkle.ShotLineSPA2;
                else
                    this.baseTex = baseTex;
            }

            public override void Render(Vector2 basePos)
            {
                if (baseTex == null)
                    return;

                if (bars == null)
                    bars = new List<ColoredVertex>(LinePointCount);
                else
                    bars.Clear();

                Texture2D Texture = baseTex.Value;

                Vector2 Dir = (GetEndPos(0.001f) - StartPos).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

                bars.Add(new(basePos + StartPos*scale + Dir * lineWidth / 2, Color.White, new Vector3(0, 0, 0)));
                bars.Add(new(basePos + StartPos * scale - Dir * lineWidth / 2, Color.White, new Vector3(0, 1, 0)));

                for (int i = 1; i <= LinePointCount; i++)
                {
                    float factor = (float)i / LinePointCount;

                    Vector2 Center = basePos + GetEndPos(factor) * scale;
                    Vector2 normal = (GetEndPos(factor)- GetEndPos(factor-0.01f)).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                    Vector2 Top = Center + (normal * lineWidth/2);
                    Vector2 Bottom = Center - (normal * lineWidth / 2);

                    bars.Add(new(Top, Color.White, new Vector3(factor, 0, 0)));
                    bars.Add(new(Bottom, Color.White, new Vector3(factor, 1, 0)));
                }

                Main.graphics.GraphicsDevice.Textures[0] = Texture;
                Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            }
        }

        public void Draw(Vector2 basePos)
        {
            if (lines == null)
                return;
            foreach (var line in lines)
            {
                line.Render(basePos);
            }
        }
    }
}
