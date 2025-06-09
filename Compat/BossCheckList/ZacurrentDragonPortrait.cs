using Coralite.Content.Bosses.ModReinforce.PurpleVolt;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Compat.BossCheckList
{
    public static class ZacurrentDragonPortrait
    {
        public static int frameCounter;
        public static int frame;

        public static void DrawPortrait(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Vector2 center = rect.Center()+new Vector2(0,-30);

            float scale = 1.2f;

            Texture2D mainTex = TextureAssets.Npc[ModContent.NPCType<ZacurrentDragon>()].Value;
            SpriteEffects effects = SpriteEffects.None;

            if (Main.zenithWorld)
                color *= 0.2f;

            if (++frameCounter > 4)
            {
                frameCounter = 0;
                if (++frame > 7)
                    frame = 0;
            }

            DrawBackWing(spriteBatch, mainTex, frame, center, color, 0, scale, effects);
            DrawBody(spriteBatch, mainTex, 0, center, color, 0, scale, effects);
            DrawHead(spriteBatch, mainTex, 0, center, color, 0, scale, effects);
            DrawFrontWing(spriteBatch, mainTex, frame, center, color, 0, scale, effects);
        }

        /// <summary>
        /// 绘制背后的翅膀
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="frameX"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public static void DrawBackWing(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(0, frameY, 4, 9);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
        }

        /// <summary>
        /// 绘制身体
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="frameX"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public static void DrawBody(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(1, frameY, 4, 9);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
        }

        /// <summary>
        /// 绘制头
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public static void DrawHead(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(2, frameY, 4, 9);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
        }

        /// <summary>
        /// 绘制前面的翅膀
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public static void DrawFrontWing(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(3, frameY, 4, 9);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
        }

    }
}
