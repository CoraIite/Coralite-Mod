using Coralite.Core;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 影子球的帮助类，用于绘制一个裂隙
    /// </summary>
    public class ShadowSplit
    {
        public Vector2[] SplitPos;
        public float SplitDistance;

        public int SplitPointCount { get; set; }
        public Vector2 SplitDir { get; set; }

        public static Color CommonSplitColor = new Color(169, 60, 198);

        public ShadowSplit(int pointCount, Vector2 splitDir)
        {
            SplitPointCount = pointCount;
            SplitDir = splitDir;
        }

        public void InitSplit(Vector2 center, int pointDistance, int pointOffset)
        {
            SplitPos = new Vector2[SplitPointCount];

            //初始化裂隙的长度
            Vector2 normal = SplitDir.RotatedBy(MathHelper.PiOver2);

            int half = SplitPointCount / 2;

            Vector2 startPos = center - (half-1) * SplitDir * pointDistance;

            for (int i = 0; i < SplitPointCount; i++)
            {
                startPos += SplitDir * pointDistance;
                if (i != half)
                {
                    int i2 = i + (i %2==0 ? 1 : 0);

                    SplitPos[i] = startPos + MathF.Cos(i2 * MathHelper.Pi) * normal * (pointOffset - MathF.Abs(i / half) * pointOffset + Main.rand.NextFloat(-8, 8));
                }
                else
                    SplitPos[i] = startPos;
            }
        }

        public void DrawSplit(float width,Color color)
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            CoraliteSystem.InitBars2();
            List<ColoredVertex> bars2 = CoraliteSystem.Vertexes2;

            Vector2 normal = SplitDir.RotatedBy(MathHelper.PiOver2);

            for (int i = 0; i < SplitPointCount; i++)
            {
                float factor = i / (float)(SplitPointCount );
                float length = factor < SplitDistance ?
                     width * MathF.Pow(factor / SplitDistance < 0.5f ? factor / SplitDistance * 2 : (1 - factor / SplitDistance) / 0.5f, 1.5f)
                     : 0;
                Vector2 Center = SplitPos[i];
                Vector2 Top = Center + (normal * length);
                Vector2 Bottom = Center - (normal * length);

                bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                bars.Add(new(Center, color, new Vector3(factor, 0.5f, 1)));

                bars2.Add(new(Center, color, new Vector3(factor, 0.5f, 1)));
                bars2.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
            }

            Effect effect = ShaderLoader.GetShader("ShadowStarsDissolveNoFade");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["baseTexture"].SetValue(CoraliteAssets.Laser.MultLines.Value);
            effect.Parameters["exTexture"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
            effect.Parameters["uExchange"].SetValue(0.7f);
            effect.Parameters["baseMult"].SetValue(1.2f);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
        }
    }
}
