using Coralite.Core;
using Coralite.Core.Prefabs.Misc;
using Coralite.Core.Systems.BossSystems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineSentinelBossBar : ModBossBar
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        private int bossHeadIndex = -1;
        public int FrameCount => 4;
        public Vector2 IconOffset = new(24, 10);
        /// <summary>
        /// 血条背景的尺寸
        /// </summary>
        public Vector2 BarSize => new(410, 20);
        public int HealthBarFrameWidth => 2;
        /// <summary>
        /// 血条背景的框线宽度
        /// </summary>
        public int BackgroundEdgeWidth => 2;
        /// <summary>
        /// 血条背景的左上角位置<br></br>
        /// 为血条背景这一帧对应的血条左上角到帧左上角的距离
        /// </summary>
        public Color DontTakeDamageColor => Color.DarkGray * 0.5f;
        public Point BackgroundTopLeftOffset => new(66, 20);
        public override ATex GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            return null;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            bossHeadIndex = npc.GetBossHeadTextureIndex();
            Texture2D barTexture = drawParams.BarTexture;
            Texture2D iconTexture = drawParams.IconTexture;
            Vector2 barCenter = drawParams.BarCenter;
            Rectangle iconFrame = drawParams.IconFrame;
            Color iconColor = drawParams.IconColor;
            float iconScale = drawParams.IconScale;
            bool showText = drawParams.ShowText;
            float life = drawParams.Life;
            float lifeMax = drawParams.LifeMax;

            int currentBarLength = (int)(BarSize.X * (float)npc.life / npc.lifeMax);
            currentBarLength -= currentBarLength % 2;

            Rectangle barPosition = Utils.CenteredRectangle(barCenter, BarSize);
            Vector2 barTopLeft = barPosition.TopLeft();
            Vector2 topLeft = barTopLeft - BackgroundTopLeftOffset.ToVector2();

            #region 背景
            DrawBackground(spriteBatch, barTexture, topLeft, barTexture.Frame(verticalFrames: FrameCount, frameY: 3));
            #endregion

            Point topLeftOffset = BackgroundTopLeftOffset + new Point(BackgroundEdgeWidth, 0);

            barPosition = Utils.CenteredRectangle(barCenter, BarSize);
            barTopLeft = barPosition.TopLeft();
            topLeft = barTopLeft - topLeftOffset.ToVector2();

            #region 条条
            Rectangle barFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 1);   //这里和原版的不一样 抄的时候注意点
            barFrame.X += topLeftOffset.X;
            barFrame.Y += topLeftOffset.Y;
            barFrame.Width = HealthBarFrameWidth;
            barFrame.Height = (int)BarSize.Y;
            Vector2 stretchScale = new(currentBarLength / (float)barFrame.Width, 1f);
            Color barColor = npc.dontTakeDamage ? DontTakeDamageColor : Color.White;

            DrawBar(spriteBatch, barTexture, barTopLeft, barFrame, barColor, stretchScale, npc);
            #endregion

            #region 顶端的绘制
            Rectangle tipFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 2);  //这里和原版的不一样 抄的时候注意点
            tipFrame.X += topLeftOffset.X;
            tipFrame.Y += topLeftOffset.Y;
            tipFrame.Width = 2;
            tipFrame.Height = (int)BarSize.Y;

            DrawTip(spriteBatch, barTexture, barTopLeft, tipFrame, barColor, currentBarLength);
            #endregion

            #region 框框的绘制
            Rectangle frameFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 0);
            DrawFrame(spriteBatch, barTexture, topLeft, frameFrame);
            #endregion

            #region 图标的绘制
            Vector2 iconOffset = IconOffset;     //应该在哪绘制图标
            Vector2 iconSize = iconTexture.Size();   //这个要跟着贴图变化
            Vector2 iconPos = iconOffset + (iconSize / 2f);
            DrawIcon(spriteBatch, iconTexture, topLeft + iconPos, iconFrame, iconColor, iconScale);
            #endregion

            if (BigProgressBarSystem.ShowText && showText)
            {
                barPosition.Y += 2;
                if (npc.dontTakeDamage)
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

            return false;
        }

        public virtual void DrawNumber(SpriteBatch spriteBatch, BossBarDrawParams drawParams, float life, float lifeMax, Rectangle barPosition, NPC npc)
        {
            BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, drawParams.TextOffset, life, lifeMax);
        }

        public virtual void DrawBackground(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 topLeft, Rectangle backFrame)
        {
            spriteBatch.Draw(barTexture, topLeft, backFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale, NPC npc)
        {
            spriteBatch.Draw(barTexture, barTopLeft, barFrame, barColor, 0f, Vector2.Zero, stretchScale, SpriteEffects.None, 0f);
        }

        public virtual void DrawTip(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle tipFrame, Color barColor, float currentBarLength)
        {
            spriteBatch.Draw(barTexture, barTopLeft + new Vector2(currentBarLength - 2, 0f), tipFrame, barColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawFrame(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 topLeft, Rectangle frameFrame)
        {
            spriteBatch.Draw(barTexture, topLeft, frameFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawIcon(SpriteBatch spriteBatch, Texture2D iconTexture, Vector2 iconCenter, Rectangle iconFrame, Color iconColor, float iconScale)
        {
            spriteBatch.Draw(iconTexture, iconCenter, iconFrame, iconColor, 0f, iconFrame.Size() / 2f, iconScale, SpriteEffects.None, 0f);
        }
    }
}
