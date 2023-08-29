using Coralite.Core;
using Coralite.Core.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSky : CustomSky
    {
        public int Timeleft = 100; //弄一个计时器，让天空能自己消失
        public Color color = new Color(204, 170, 242);

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡，当然你想的话，也可以去掉这个条件
            {
                Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSky").Value;

                Rectangle screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                if (VisualEffectSystem.UseNightmareSky)
                {
                    Effect e = Filters.Scene["GlowingMarblingBlack2"].GetShader().Shader;
                    e.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
                    e.Parameters["viewRange"].SetValue(0.7f + MathF.Sin(Main.GlobalTimeWrappedHourly / 3) * 0.2f);
                    e.Parameters["uC1"].SetValue((color*(Timeleft / 100f)).ToVector3());

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, e);

                    spriteBatch.Draw(sky, screen, Color.White);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
                else
                    spriteBatch.Draw(sky, screen, color * (Timeleft / 100f));
                //把一条带状的图片填满屏幕
            }
        }

        public override bool IsActive()
        {
            return Timeleft > 0;
        }


        public override void Update(GameTime gameTime)
        {
            if (!Main.gamePaused)//游戏暂停时不执行
            {
                if (Timeleft > 0) 
                    Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
                else
                {
                    if (SkyManager.Instance["NeonSky"].IsActive())
                    {
                        SkyManager.Instance.Deactivate("NeonSky");//消失
                    }
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args) { }
        public override void Deactivate(params object[] args) { }
        public override void Reset() { }
        public override float GetCloudAlpha() => 0f;
    }
}
