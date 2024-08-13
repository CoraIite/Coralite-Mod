using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;

namespace Coralite.Core.Prefabs.Misc
{
    public abstract class BaseBossHealthBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        private Vector2 offset;
        private int oldLife;

        public virtual int FrameCount => 4;
        /// <summary>
        /// 血条背景的尺寸
        /// </summary>
        public virtual Point BarSize => new(406, 20);
        /// <summary>
        /// 血条背景的左上角位置<br></br>
        /// 为血条背景这一帧对应的血条左上角到帧左上角的距离
        /// </summary>
        public virtual Point BackgroundTopLeftOffset => new(66, 20);
        public virtual Point IconOffset => new(34, 14);
        /// <summary>
        /// 血条背景的框线宽度
        /// </summary>
        public virtual int BackgroundEdgeWidth => 2;

        public virtual Color DontTakeDamageColor => Color.DarkGray * 0.5f;

        private List<LifeLostData> _lifeLostDatas;

        /// <summary>
        /// 失去的生命多少
        /// </summary>
        private class LifeLostData(Vector2 topLeft, float length)
        {
            public bool active = true;
            public Vector2 topLeft = topLeft;
            public readonly float length = length;

            private float _alpha = 1;
            private float _yExpand = 2f;
            private int _timer = 0;

            public void Update()
            {
                //先聚集之后上升
                const float expandTime = 8;
                const float fadeTime = 10;

                if (_timer < expandTime)
                {
                    _yExpand = Helper.Lerp(2f, 1, _timer / expandTime);
                }
                else
                {
                    _alpha -= 1 / fadeTime;
                    topLeft.Y -= 1f;
                    if (_alpha < 0.1f)
                        active = false;
                }

                _timer++;
            }

            public void Draw(SpriteBatch spriteBatch, Texture2D barTexture, Rectangle frame, Vector2 offset)
            {
                Color c = Color.White;
                c.A = 0;
                c *= _alpha;
                Vector2 stretchScale = new(length / frame.Width, 1f);
                Vector2 pos = topLeft + offset;
                Vector2 origin = new(0, frame.Height / 2);

                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(barTexture, pos + origin, frame, c, 0f, origin, stretchScale, SpriteEffects.None, 0f);
                    c *= 0.8f;
                    stretchScale.Y *= _yExpand;
                }
            }
        }

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
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

            int currentBarLength = BarSize.X * npc.life / npc.lifeMax;
            currentBarLength -= currentBarLength % 2;

            Rectangle barPosition = Utils.CenteredRectangle(barCenter, (BarSize + new Point(4, 0)).ToVector2());
            Vector2 barTopLeft = barPosition.TopLeft();
            Vector2 topLeft = barTopLeft - BackgroundTopLeftOffset.ToVector2();

            #region 背景
            DrawBackground(spriteBatch, barTexture, topLeft, barTexture.Frame(verticalFrames: FrameCount, frameY: 3));
            #endregion

            Point topLeftOffset = BackgroundTopLeftOffset + new Point(BackgroundEdgeWidth, 0);

            barPosition = Utils.CenteredRectangle(barCenter, BarSize.ToVector2());
            barTopLeft = barPosition.TopLeft();
            topLeft = barTopLeft - topLeftOffset.ToVector2();

            #region 条条
            Rectangle barFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 1);   //这里和原版的不一样 抄的时候注意点
            barFrame.X += topLeftOffset.X;
            barFrame.Y += topLeftOffset.Y;
            barFrame.Width = 2;
            barFrame.Height = BarSize.Y;
            Vector2 stretchScale = new(currentBarLength / barFrame.Width, 1f);
            Color barColor = npc.dontTakeDamage ? DontTakeDamageColor : Color.White;

            DrawBar(spriteBatch, barTexture, barTopLeft, barFrame, barColor, stretchScale);
            #endregion

            #region 顶端的绘制
            Rectangle tipFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 2);  //这里和原版的不一样 抄的时候注意点
            tipFrame.X += topLeftOffset.X;
            tipFrame.Y += topLeftOffset.Y;
            tipFrame.Width = 2;
            tipFrame.Height = BarSize.Y;

            DrawTip(spriteBatch, barTexture, barTopLeft, tipFrame, barColor, currentBarLength);
            #endregion

            #region 框框的绘制
            Rectangle frameFrame = barTexture.Frame(verticalFrames: FrameCount, frameY: 0);
            DrawFrame(spriteBatch, barTexture, topLeft, frameFrame);
            #endregion

            #region 绘制额外失去的血量

            _lifeLostDatas ??= new List<LifeLostData>();

            if (npc.life != oldLife && oldLife != -1)
            {
                int lostLife = oldLife - npc.life;
                float lostLength = (float)BarSize.X * lostLife / npc.lifeMax;
                lostLength -= currentBarLength % 2;

                _lifeLostDatas.Add(new LifeLostData(barTopLeft + new Vector2(barFrame.Width * stretchScale.X, 0)
                    , lostLength < 0.5f ? 0.5f : lostLength));

                //设置随机移动
                SetRandomOffset(lostLife, npc.lifeMax);
            }

            _lifeLostDatas.RemoveAll(d => !d.active);

            for (int i = 0; i < _lifeLostDatas.Count; i++)
            {
                _lifeLostDatas[i].Update();
                _lifeLostDatas[i].Draw(spriteBatch, barTexture, barFrame, offset);
            }

            #endregion

            #region 图标的绘制
            Vector2 iconOffset = new(34f, 14f);     //应该在哪绘制图标
            Vector2 iconSize = iconTexture.Size();   //这个要跟着贴图变化
            Vector2 iconPos = iconOffset + iconSize / 2f;
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
                    BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, drawParams.TextOffset, life, lifeMax);
            }

            oldLife = npc.life;
            offset = Vector2.SmoothStep(offset, Vector2.Zero, 0.2f);

            return false;
        }

        public virtual void DrawBackground(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 topLeft, Rectangle backFrame)
        {
            spriteBatch.Draw(barTexture, topLeft + offset, backFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawBar(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, Color barColor, Vector2 stretchScale)
        {
            spriteBatch.Draw(barTexture, barTopLeft + offset, barFrame, barColor, 0f, Vector2.Zero, stretchScale, SpriteEffects.None, 0f);
        }

        public virtual void DrawTip(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 barTopLeft, Rectangle tipFrame, Color barColor, float currentBarLength)
        {
            spriteBatch.Draw(barTexture, barTopLeft + offset + new Vector2(currentBarLength - 2, 0f), tipFrame, barColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawFrame(SpriteBatch spriteBatch, Texture2D barTexture, Vector2 topLeft, Rectangle frameFrame)
        {
            spriteBatch.Draw(barTexture, topLeft + offset, frameFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawIcon(SpriteBatch spriteBatch, Texture2D iconTexture, Vector2 iconCenter, Rectangle iconFrame, Color iconColor, float iconScale)
        {
            spriteBatch.Draw(iconTexture, iconCenter + offset, iconFrame, iconColor, 0f, iconFrame.Size() / 2f, iconScale, SpriteEffects.None, 0f);
        }

        public virtual void SetRandomOffset(int lostLife, int lifeMax)
        {
            float lengthMax = 10;

            float length = lostLife / (float)lifeMax * 550;
            if (length > lengthMax)
                length = lengthMax;
            offset = Helper.NextVec2Dir(length, length * 1.1f);
        }

        public virtual void Reset(NPC npc)
        {
            _lifeLostDatas?.Clear();
            offset = Vector2.Zero;
            oldLife = -1;
        }
    }
}
