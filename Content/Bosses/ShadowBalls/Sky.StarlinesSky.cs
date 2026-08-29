using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarlinesSky : CustomSky
    {
        private bool _isActive;

        public int OwnerIndex;
        public int State;
        public int Timer;
        public int Timeleft = 0; //弄一个计时器，让天空能自己消失

        public float alpha;

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused)//游戏暂停时不执行
                return;

            if (Timeleft > 0)
                Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
            else if (SkyManager.Instance[nameof(StarlinesSky)].IsActive())
                SkyManager.Instance.Deactivate(nameof(StarlinesSky));//消失

            if (!OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner))
            {
                return;
            }

            if (Timeleft < 100)
                Timeleft += 2;

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
                if (Timer < 90)
                {
                    float factor = Timer / 90f;
                    alpha = Helper.BezierEase(factor)*0.7f;
                }
                else
                    State = 1;

                Timer++;
                return;
            }

            // 二阶段及以后（ai[0] 现为 StateId，勿与旧 Phase 枚举混读）
            if (owner.ModNPC is ShadowBall boss
                && boss.CurrentStateId >= (int)ShadowBallStateId.P1ToP2Exchange)
            {

            }
        }

        public override float GetCloudAlpha() => 0f;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            //if (minDepth < 9 && maxDepth > 9)//绘制在最前的背景
            //{
            //    Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "ShadowBallSky").Value;

            //    Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);
            //    spriteBatch.Draw(sky, screen, Color.White * (Timeleft / 100f));

            //    int num13 = screenWidth;
            //    int num14 = screenHeight;
            //    Vector2 zero = Vector2.Zero;
            //    if (num13 < 800)
            //    {
            //        int num15 = 800 - num13;
            //        zero.X -= num15 * 0.5f;
            //        num13 = 800;
            //    }

            //    if (num14 < 600)
            //    {
            //        int num16 = 600 - num14;
            //        zero.Y -= num16 * 0.5f;
            //        num14 = 600;
            //    }

            //    SceneArea sceneArea2 = default;
            //    sceneArea2.bgTopY = 0;
            //    sceneArea2.totalWidth = num13;
            //    sceneArea2.totalHeight = num14;
            //    sceneArea2.SceneLocalScreenPositionOffset = zero;
            //    SceneArea sceneArea3 = sceneArea2;
            //    DrawSunAndMoon(sceneArea3);
            //    return;
            //}

            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡
            {
                Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);

                Effect e = ShaderLoader.GetShader("StarlineBack");

                e.Parameters["Time"].SetValue(Main.GlobalTimeWrappedHourly);
                e.Parameters["alpha"].SetValue(alpha);
                e.Parameters["Resolution"].SetValue(Main.ScreenSize.ToVector2());
                e.Parameters["offset"].SetValue(new Vector2(0, -0.8f));

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, e);

                spriteBatch.Draw(CoraliteAssets.Misc.White32x32.Value, screen, Color.White);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.BackgroundViewMatrix.EffectMatrix);
            }
        }
        //private void DrawSunAndMoon(SceneArea sceneArea)
        //{
        //    Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSparkle").Value;
        //    var frameBox = mainTex.Frame(1, 2, 0, 1);

        //    int num2 = sceneArea.bgTopY;
        //    int num3 = (int)(Main.time / 54000.0 * (double)(sceneArea.totalWidth + (mainTex.Width * 2))) - mainTex.Width;
        //    int num4 = 0;
        //    float scale = 1f;
        //    float rotation = ((float)(Main.time / 54000.0) * 2f) - 7.3f;
        //    if (dayTime)
        //    {
        //        double num10;
        //        if (Main.time < 27000.0)
        //        {
        //            num10 = Math.Pow(1.0 - (Main.time / 54000.0 * 2.0), 2.0);
        //            num4 = (int)(num2 + (num10 * 250.0) + 180.0);
        //        }
        //        else
        //        {
        //            num10 = Math.Pow(((Main.time / 54000.0) - 0.5) * 2.0, 2.0);
        //            num4 = (int)(num2 + (num10 * 250.0) + 180.0);
        //        }

        //        scale = (float)(1.2 - (num10 * 0.4));
        //    }

        //    scale *= ForcedMinimumZoom;
        //    starsHit = 0;

        //    if (dayTime)
        //    {
        //        if ((remixWorld && !gameMenu) || WorldGen.remixWorldGen)
        //            return;

        //        scale *= 1.1f;

        //        Vector2 origin = frameBox.Size() / 2f;
        //        Vector2 position = new Vector2(num3, num4 + sunModY) + sceneArea.SceneLocalScreenPositionOffset;

        //        Color c = Color.White * (Timeleft / 100f);
        //        //c.A = 0;
        //        spriteBatch.Draw(mainTex, position, frameBox, c, rotation + 1f, origin, scale * 0.5f * Main.rand.NextFloat(0.97f, 1.03f), SpriteEffects.None, 0f);
        //        //spriteBatch.Draw(mainTex, position, frameBox, c, rotation, origin, scale, SpriteEffects.None, 0f);
        //    }
        //}

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

    public class StarlinesSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (SkyManager.Instance[nameof(StarlinesSky)].IsActive())
            {
                StarlinesSky sky = (StarlinesSky)SkyManager.Instance[nameof(StarlinesSky)];
                backgroundColor = Color.Lerp(backgroundColor, new Color(68, 0, 96), 0.7f * sky.Timeleft / 100f);
                tileColor = Color.Lerp(tileColor, new Color(88, 20, 146), 0.5f * sky.Timeleft / 100f);
            }
        }
    }
}
