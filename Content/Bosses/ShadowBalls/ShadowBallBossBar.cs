using Coralite.Core;
using Coralite.Core.Prefabs.Misc;
using Coralite.Core.Systems.BossSystems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale, NPC npc)
        {
            if (npc.ModNPC is ShadowBall ball)
            {
                barColor = GetShadowBallDontTakeDamage(npc, ball) ? DontTakeDamageColor : BarColor;

                base.DrawBar(spriteBatch, barTexture, barTopLeft, barFrame, barColor, stretchScale, npc);

                if (ball.Phase == ShadowBall.AIPhases.P1_WithSmallBalls && ball.smallBalls.Count > 0)
                {
                    var r = barFrame;
                    r.X += 8;
                    r.Width = HealthBarFrameWidth;
                    r.Height = BarSize.Y;

                    int maxBall = ball.HowmanyPerSummon();
                    int maxLife = maxBall * ball.smallBalls[0].lifeMax;
                    int currLife = 0;
                    foreach (var smallball in ball.smallBalls)
                        currLife += smallball.life;

                    Vector2 scale = new(BarSize.X * (float)currLife / maxLife / barFrame.Width, 1f);
                    spriteBatch.Draw(barTexture, barTopLeft + offset, r, barColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void DrawNumber_Total(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams, bool showText, float life, float lifeMax, Rectangle barPosition)
        {
            if (BigProgressBarSystem.ShowText && showText && npc.ModNPC is ShadowBall ball)
            {
                barPosition.Y += 2;

                if (GetShadowBallDontTakeDamage(npc,ball))
                {
                    DynamicSpriteFont font = FontAssets.ItemStack.Value;
                    Vector2 center = barPosition.Center.ToVector2() + drawParams.TextOffset;
                    center.Y += 1f;
                    string text = BossBarSystem.DontTakeDamage.Value;
                    Vector2 textSize = font.MeasureString(text);
                    Utils.DrawBorderStringFourWay(spriteBatch, font, text, center.X, center.Y, Color.White, Color.Black, textSize / 2);
                }
                else
                    DrawNumber(spriteBatch, drawParams, life, lifeMax, barPosition, npc);
            }
        }

        public override void DrawNumber(SpriteBatch spriteBatch, BossBarDrawParams drawParams, float life, float lifeMax, Rectangle barPosition, NPC npc)
        {
            if (npc.ModNPC is ShadowBall ball && ball.Phase == ShadowBall.AIPhases.P1_WithSmallBalls && ball.smallBalls.Count > 0)
            {
                int maxBall = ball.HowmanyPerSummon();
                int maxLife = maxBall * ball.smallBalls[0].lifeMax;
                int currLife = 0;
                foreach (var smallball in ball.smallBalls)
                    currLife += smallball.life;

                BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, drawParams.TextOffset, currLife, maxLife);
            }
            else
                base.DrawNumber(spriteBatch, drawParams, life, lifeMax, barPosition, npc);
        }

        public bool GetShadowBallDontTakeDamage(NPC npc, ShadowBall ball)
        {
            if (ball.Phase == ShadowBall.AIPhases.P1_WithSmallBalls)
                return false;

            return npc.dontTakeDamage;
        }
    }
}
