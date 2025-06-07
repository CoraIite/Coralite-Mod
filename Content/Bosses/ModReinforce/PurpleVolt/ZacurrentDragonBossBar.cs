using Coralite.Core;
using Coralite.Core.Prefabs.Misc;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.BigProgressBar;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public class ZacurrentDragonBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + Name;

        public override void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale, NPC npc)
        {
            base.DrawBar(spriteBatch, barTexture, barTopLeft, barFrame, barColor, stretchScale, npc);

            if (npc.ModNPC is ZacurrentDragon zd && zd.PurpleVolt)
            {
                var r = barFrame;
                r.X += 8;
                r.Width = HealthBarFrameWidth;
                r.Height = BarSize.Y;
                Vector2 scale = new(BarSize.X * (float)zd.PurpleVoltCount / zd.GetPurpleVoltMax() / barFrame.Width, 1f);
                spriteBatch.Draw(barTexture, barTopLeft + offset, r, barColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public override void DrawNumber(SpriteBatch spriteBatch, BossBarDrawParams drawParams, float life, float lifeMax, Rectangle barPosition, NPC npc)
        {
            if (npc.ModNPC is ZacurrentDragon zd && zd.PurpleVolt)
                BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, drawParams.TextOffset, zd.PurpleVoltCount, zd.GetPurpleVoltMax());
            else
                base.DrawNumber(spriteBatch, drawParams, life, lifeMax, barPosition, npc);
        }
    }
}
