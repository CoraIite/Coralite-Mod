using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarsBackSky : CustomSky
    {
        private bool _isActive;

        public int OwnerIndex;
        public float State;
        public float Timer;

        Vector2 scale = Vector2.Zero;

        int frameCounter;
        int frameX;
        int frameY;

        public override void Update(GameTime gameTime)
        {
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
                //    factor = Coralite.Instance.SqrtSmoother.Smoother(factor);
                //    scale = Vector2.Lerp(Vector2.Zero, new Vector2(0.5f, 0.5f), factor);
                //}
                //else if (Timer < 70)
                //{
                //    float factor = (Timer-40) / 30;
                //    factor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                //    scale = Vector2.Lerp(new Vector2(0.5f, 0.5f)
                //        , new Vector2(0.05f, 0.05f), factor);
                //}
                if (Timer < 45)
                {
                    float factor = Timer / 45;
                    factor = Coralite.Instance.BezierEaseSmoother.Smoother(factor);

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

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (!(minDepth < 0 && maxDepth > 2))//绘制在背景景物后面，防止遮挡，当然你想的话，也可以去掉这个条件
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
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 7);
            effect.Parameters["uSourceRect"].SetValue(new Vector4(frameBox.X, frameBox.Y, frameBox.Width, frameBox.Height));
            effect.Parameters["uExchange"].SetValue(0.15f + 0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly));
            effect.Parameters["uImageSize0"].SetValue(mainTex.Size());
            effect.Parameters["uLerp"].SetValue(0.1f + 0.02f * MathF.Sin(Main.GlobalTimeWrappedHourly));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend
                , SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);

            //Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
            //{
            //    pass.Apply();
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.Purple, 0, origin, scale*6f, 0, 0);
            //}

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                if (SkyManager.Instance["StarsBackSky"].IsActive())
                    SkyManager.Instance.Deactivate("StarsBackSky");//消失
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<ShadowBall>())
            {
                if (SkyManager.Instance["StarsBackSky"].IsActive())
                    SkyManager.Instance.Deactivate("StarsBackSky");//消失
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
            _isActive = false;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            OwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<ShadowBall>());
            State = 0;
            Timer = 0;
            scale = Vector2.Zero;
            _isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            OwnerIndex = -1;
            _isActive = false;
        }
    }
}
