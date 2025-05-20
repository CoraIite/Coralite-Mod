using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using static Terraria.Main;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarsBackSky : CustomSky
    {
        private bool _isActive;

        public int OwnerIndex;
        public int State;
        public int Timer;
        public int Timeleft = 0; //弄一个计时器，让天空能自己消失

        Vector2 scale = Vector2.Zero;

        //int frameCounter;
        //int frameX;
        //int frameY;

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused)//游戏暂停时不执行
                return;

            if (Timeleft > 0)
                Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
            else if (SkyManager.Instance["StarsBackSky"].IsActive())
                SkyManager.Instance.Deactivate("StarsBackSky");//消失

            if (!GetOwner(out NPC owner))
            {
                return;
            }

            //if (++frameCounter > 2)
            //{
            //    frameCounter = 0;
            //frameX++;
            //    if (frameX > 3)
            //    {
            //        frameX = 0;
            //    frameY++;
            //        if (frameY > 3)
            //            frameY = 0;
            //    }
            //}

            if (State == 0)//刚生成时
            {
                //if (Timer < 40)
                //{
                //    float factor = Timer / 40;
                //    factor = Helper.SqrtEase(factor);
                //    scale = Vector2.Lerp(Vector2.Zero, new Vector2(0.5f, 0.5f), factor);
                //}
                //else if (Timer < 70)
                //{
                //    float factor = (Timer-40) / 30;
                //    factor = Helper.SqrtEase(factor);

                //    scale = Vector2.Lerp(new Vector2(0.5f, 0.5f)
                //        , new Vector2(0.05f, 0.05f), factor);
                //}
                if (Timer < 30)
                {
                    float factor = Timer / 30f;
                    factor = Helper.BezierEase(factor);

                    scale = Vector2.Lerp(Vector2.Zero/*new Vector2(0.5f, 0.5f)*/, Vector2.One * 1.3f, factor/*(Timer - 70) / 15*/);
                }
                else
                    State = 1;

                Timer++;
                return;
            }

            //常态，最后会随着大球血量减少而减少
            if (owner.ai[0] == (int)ShadowBall.AIPhases.BigBallSmash)
            {

            }
        }

        public override float GetCloudAlpha() => 0.5f;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < 9 && maxDepth > 9)//绘制在最前的背景
            {
                Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "ShadowBallSky").Value;

                Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(sky, screen, Color.White * (Timeleft / 100f));

                int num13 = screenWidth;
                int num14 = screenHeight;
                Vector2 zero = Vector2.Zero;
                if (num13 < 800)
                {
                    int num15 = 800 - num13;
                    zero.X -= num15 * 0.5f;
                    num13 = 800;
                }

                if (num14 < 600)
                {
                    int num16 = 600 - num14;
                    zero.Y -= num16 * 0.5f;
                    num14 = 600;
                }

                SceneArea sceneArea2 = default;
                sceneArea2.bgTopY = 0;
                sceneArea2.totalWidth = num13;
                sceneArea2.totalHeight = num14;
                sceneArea2.SceneLocalScreenPositionOffset = zero;
                SceneArea sceneArea3 = sceneArea2;
                DrawSunAndMoon(sceneArea3);
                return;
            }

            if (!(minDepth < 0 && maxDepth > 2))
                return;

            Effect effect = Filters.Scene["ShadowStars"].GetShader().Shader;

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "BallBack").Value;
            var pos = CoraliteWorld.shadowBallsFightArea.Center.ToVector2() - Main.screenPosition;
            var frameBox = mainTex.Frame();
            var origin = frameBox.Size() / 2;

            //effect.Parameters["uColor"].SetValue(Color.Purple.ToVector4());
            //effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            // effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["exTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "BallBack").Value);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 3);
            effect.Parameters["uSourceRect"].SetValue(new Vector4(frameBox.X, frameBox.Y, frameBox.Width, frameBox.Height));
            effect.Parameters["uExchange"].SetValue(0.15f + (0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly)));
            effect.Parameters["uImageSize0"].SetValue(mainTex.Size());
            effect.Parameters["uLerp"].SetValue(0.1f + (0.02f * MathF.Sin(Main.GlobalTimeWrappedHourly)));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend
                , SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);

            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
            //{
            //    pass.Apply();
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.Purple * (Timeleft / 100f), 0, origin, scale * 6f * (Timeleft / 100f), 0, 0);
            //}

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void DrawSunAndMoon(SceneArea sceneArea)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSparkle").Value;
            var frameBox = mainTex.Frame(1, 2, 0, 1);

            int num2 = sceneArea.bgTopY;
            int num3 = (int)(Main.time / 54000.0 * (double)(sceneArea.totalWidth + (mainTex.Width * 2))) - mainTex.Width;
            int num4 = 0;
            float scale = 1f;
            float rotation = ((float)(Main.time / 54000.0) * 2f) - 7.3f;
            if (dayTime)
            {
                double num10;
                if (Main.time < 27000.0)
                {
                    num10 = Math.Pow(1.0 - (Main.time / 54000.0 * 2.0), 2.0);
                    num4 = (int)(num2 + (num10 * 250.0) + 180.0);
                }
                else
                {
                    num10 = Math.Pow(((Main.time / 54000.0) - 0.5) * 2.0, 2.0);
                    num4 = (int)(num2 + (num10 * 250.0) + 180.0);
                }

                scale = (float)(1.2 - (num10 * 0.4));
            }

            scale *= ForcedMinimumZoom;
            starsHit = 0;

            if (dayTime)
            {
                if ((remixWorld && !gameMenu) || WorldGen.remixWorldGen)
                    return;

                scale *= 1.1f;

                Vector2 origin = frameBox.Size() / 2f;
                Vector2 position = new Vector2(num3, num4 + sunModY) + sceneArea.SceneLocalScreenPositionOffset;

                Color c = Color.White * (Timeleft / 100f);
                //c.A = 0;
                spriteBatch.Draw(mainTex, position, frameBox, c, rotation + 1f, origin, scale * 0.5f * Main.rand.NextFloat(0.97f, 1.03f), SpriteEffects.None, 0f);
                //spriteBatch.Draw(mainTex, position, frameBox, c, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange(OwnerIndex))
            {
                //if (SkyManager.Instance["StarsBackSky"].IsActive())
                //    SkyManager.Instance.Deactivate("StarsBackSky");//消失
                owner = null;
                return false;
            }

            NPC npc = Main.npc[OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<ShadowBall>())
            {
                //if (SkyManager.Instance["StarsBackSky"].IsActive())
                //    SkyManager.Instance.Deactivate("StarsBackSky");//消失
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }


        public override bool IsActive()
        {
            return _isActive;//GetOwner(out _);
        }

        public override void Reset()
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            OwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<ShadowBall>());
            State = 0;
            Timer = 0;
            scale = Vector2.Zero;
            _isActive = true;
            Timeleft = 4;
        }

        public override void Deactivate(params object[] args)
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }
    }

    public class StarsBackSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (SkyManager.Instance["StarsBackSky"].IsActive())
            {
                StarsBackSky sky = (StarsBackSky)SkyManager.Instance["StarsBackSky"];
                backgroundColor = Color.Lerp(backgroundColor, new Color(68, 0, 96), 0.7f * sky.Timeleft / 100f);
                tileColor = Color.Lerp(tileColor, new Color(88, 20, 146), 0.5f * sky.Timeleft / 100f);
            }
        }
    }
}
