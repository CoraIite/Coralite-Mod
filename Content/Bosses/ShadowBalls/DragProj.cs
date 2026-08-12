using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class DragProj : ModProjectile, IDrawPrimitive, IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float NpcIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Alpha => ref Projectile.localAI[0];
        public ref float LerpValue => ref Projectile.localAI[1];

        public Vector2[] SplitPos;
        public Vector2[] WarpPos;
        public float SplitDistance;

        private Trail DragEffect;

        public override bool ShouldUpdatePosition() => false;


        public override void AI()
        {
            if (!NpcIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            switch (State)
            {
                default:
                case 0://初始化
                    {
                        if (!VaultUtils.isServer)
                        {
                            InitDrag(owner);
                            InitSplit(owner);
                            InitWarp(owner);
                        }

                        State = 1;
                    }
                    break;
                case 1://裂隙正常打开
                    {
                        //更新吸收点
                        if (Timer <= 18)
                            SplitDistance = Timer / 18f;

                        Vector2 startPos = owner.Center;
                        Vector2 endPos = Vector2.Lerp(owner.Center, Projectile.Center, LerpValue);
                        Vector2 endPos2 = Vector2.Lerp(owner.Center, Projectile.Center, LerpValue / 0.7f);

                        for (int i = 0; i < 30; i++)
                        {
                            Projectile.oldPos[i] = Vector2.Lerp(startPos, endPos, i / 30f);
                            WarpPos[i] = Vector2.Lerp(startPos, endPos2, i / 30f);
                        }

                        if (Timer > 12)
                        {
                            if (LerpValue < 0.7f)
                            {
                                LerpValue += 0.03f;
                            }
                        }

                        if (Alpha < 1)
                        {
                            Alpha += 0.08f;
                            if (Alpha > 1)
                            {
                                Alpha = 1;
                            }
                        }

                        Timer++;
                    }
                    break;
                case 2://裂隙消失
                    {
                        Timer++;
                        SplitDistance = 1 - Timer / 25;

                        if (Timer > 25)
                            Projectile.Kill();
                    }
                    break;
            }





            DragEffect.TrailPositions = Projectile.oldPos;

        }

        private void InitWarp(NPC owner)
        {
            if (!owner.boss)
                return;

            WarpPos = new Vector2[30];
        }

        private void InitSplit(NPC owner)
        {
            SplitPos = new Vector2[9];

            //初始化裂隙的长度
            Vector2 dir = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);

            int width = owner.width / 4;
            int width2 = owner.width / 7;

            for (int i = -4; i < 5; i++)
            {
                Vector2 basePos = Projectile.Center + i * normal * width;
                if (i != 0)
                {
                    int i2 = i + (i > 0 ? 1 : 0);

                    SplitPos[i + 4] = basePos + MathF.Cos(i2 * MathHelper.Pi) * dir * (width2 - MathF.Abs(i / 4f) * width2 + Main.rand.NextFloat(-8, 8));
                }
                else
                    SplitPos[i + 4] = basePos;
            }
        }

        private void InitDrag(NPC owner)
        {
            Projectile.InitOldPosCache(30);
            DragEffect ??= new Trail(Main.instance.GraphicsDevice, 30, new EmptyMeshGenerator(), factor =>
            {
                return Helper.Lerp(owner.width / 2, 20, factor);
            }, factor =>
            {
                return new Color(109, 30, 148) * Alpha;
            });
        }

        public void TurnToFade()
        {
            State = 2;
            Timer = 0;
            Alpha = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            CoraliteSystem.InitBars2();
            List<ColoredVertex> bars2 = CoraliteSystem.Vertexes2;

            var color = new Color(109, 30, 148);
            if (!NpcIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return false;

            Vector2 dir = (Projectile.Center - owner.Center).SafeNormalize(Vector2.Zero);
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);
            float width = owner.width *0.6f;

            for (int i = 0; i < 9; i++)
            {
                float factor = i / 8f;
                float length = factor < SplitDistance ?
                     width * MathF.Pow(factor / SplitDistance < 0.5f ? factor / SplitDistance * 2 : (1 - factor / SplitDistance) / 0.5f,1.5f)
                     : 0;
                Vector2 Center = SplitPos[i];
                Vector2 Top = Center + (dir * length);
                Vector2 Bottom = Center - (dir * length);

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

            return false;
        }

        public void DrawPrimitives()
        {
            if (Alpha == 0 || DragEffect == null)
                return;

            Effect effect = ShaderLoader.GetShader("ShadowStarsDissolve");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["baseTexture"].SetValue(CoraliteAssets.Trail.BoosterASP.Value);
            effect.Parameters["exTexture"].SetValue(CoraliteAssets.Laser.WaterFlow.Value);
            effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly/5);
            effect.Parameters["uExchange"].SetValue(0.9f);
            effect.Parameters["baseMult"].SetValue(0.87f);

            DragEffect.DrawTrail(effect);
        }

        public void DrawWarp()
        {
            if (Alpha==0||WarpPos == null)
                return;

            Texture2D Texture = CoraliteAssets.Laser.WaterFlow.Value;

            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;

            if (!NpcIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Vector2 normal = (owner.Center-Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

            float r = (Projectile.Center - owner.Center).ToRotation() % 6.18f;
            float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;

            for (int i = 0; i < 30; i++)
            {
                float factor = (float)i / 29;
                Color c;
                if (i < 3)
                    c = new Color(dir, 0.55f, 0f, Alpha * (i / 3f));
                else if (i > 23)
                    c = new Color(dir, 0.55f, 0f, Alpha * (1-(i - 24) / 5f));
                else
                    c = new Color(dir, 0.55f, 0f, Alpha);

                Vector2 Center = WarpPos[i];
                Vector2 r2 = normal * Helper.Lerp(owner.width / 2, 20, factor);
                Vector2 Top = Center + r2;
                Vector2 Bottom = Center - r2;

                bars.Add(new(Top, c, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, c, new Vector3(factor, 1, 1)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            Effect effect = ShaderLoader.GetShader("ShadowWarp");

            effect.Parameters["uTransform"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly);

            effect.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
