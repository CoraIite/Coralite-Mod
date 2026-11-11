using Coralite.Core;
using Coralite.Core.Prefabs.Misc;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.BigProgressBar;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public override void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale, NPC npc)
        {
            base.DrawBar(spriteBatch, barTexture, barTopLeft, barFrame, barColor, stretchScale, npc);

            if (npc.ModNPC is ShadowBall ball && ball.Phase == ShadowBall.AIPhases.P1_WithSmallBalls && ball.smallBalls.Count > 0)
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
    }
}
